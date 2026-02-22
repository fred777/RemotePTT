using Microsoft.Extensions.Logging;
using MQTTnet;
using System.Text;
namespace RemotePTT.Controller
{
    public class Controller(ILogger logger) : IDisposable
    {
        private static class Topics
        {
            public const string Ptt = "ham/remoteptt/ptt";
            public const string qrg = "ham/remoteptt/qrg";
            public const string trx = "ham/remoteptt/trx";
            public const string rig = "ham/remoteptt/rig";
        }

        public Task InitAsync()
        {
            return Task.Run(() =>
               {
                   omniRigEngine = new OmniRig.OmniRigX();
                   logger.LogInformation($"Omnirig InterfaceVersion {omniRigEngine.InterfaceVersion}, SoftwareVersion {omniRigEngine.SoftwareVersion}");
                   SelectRig(1);
               });
        }

        public async Task StartMqttAsync(string mqttBrokerHostname)
        {
            if (string.IsNullOrWhiteSpace(mqttBrokerHostname))
            {
                logger.LogError("MQTT broker hostname must not be empty.");
                return;
            }

            var mqttFactory = new MqttClientFactory();

            mqttClient?.Dispose();
            mqttClient = mqttFactory.CreateMqttClient();
            // Use builder classes where possible in this project.
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(mqttBrokerHostname).Build();

            try
            {
                // This will throw an exception if the server is not available.
                // The result from this message returns additional data which was sent
                // from the server. Please refer to the MQTT protocol specification for details.
                var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                logger.LogInformation($"MQTT connected: {response.ResultCode}");

                mqttClient.ApplicationMessageReceivedAsync += HandleMqttMessageAsync;

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(Topics.Ptt).Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                logger.LogInformation($"MQTT subscribed to {Topics.Ptt}");

                DisposeTimer(ref infoTimer);

                infoTimer = new System.Timers.Timer(1000)
                {
                    AutoReset = true
                };

                infoTimer.Elapsed += async (_, _) =>
                {
                    if (rig == null || mqttClient == null)
                    {
                        DisposeTimer(ref infoTimer);
                        return;
                    }

                    try
                    {
                        PublishStatus();
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"Failed to publish MQTT message: {ex.Message}");
                        DisposeTimer(ref infoTimer);
                    }
                };

                infoTimer.Start();
            }
            catch (Exception ex)
            {
                logger.LogError($"MQTT error: {ex.Message}");
            }
        }

        private async void PublishStatus()
        {
            if (mqttClient == null) return;

            const string NA = "N/A";
            var isOnline = rig != null && rig.Status == OmniRig.RigStatusX.ST_ONLINE;

            var qrg = isOnline ? $"{rig.GetTxFrequency() / 1e6:F4} MHz" : NA;
            var trx = isOnline ? (rig.Tx == OmniRig.RigParamX.PM_TX ? "TX" : "RX") : NA;
            var rigName = rig != null ? rig.RigType : NA;

            await mqttClient.PublishAsync(new MqttApplicationMessageBuilder().WithTopic(Topics.qrg).WithPayload(qrg).Build());
            await mqttClient.PublishAsync(new MqttApplicationMessageBuilder().WithTopic(Topics.rig).WithPayload(rigName).Build());
            await mqttClient.PublishAsync(new MqttApplicationMessageBuilder().WithTopic(Topics.trx).WithPayload(trx).Build());
        }

        private Task HandleMqttMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                logger.LogInformation($"Received MQTT message \"{payload}\"");
                PTT(int.Parse(payload));
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing payload: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public void SelectRig(int id)
        {
            if (omniRigEngine == null)
            {
                logger.LogWarning("OmniRig engine is not initialized.");
                return;
            }

            if (id < 0 || id > 2)
            {
                logger.LogWarning("Only rig ids 1 and 2 are supported.");
            }

            rig = id == 1 ? omniRigEngine.Rig1 : omniRigEngine.Rig2;

            LogRigInfo();
        }

        public void ShowRigConfigurationDialog()
        {
            omniRigEngine?.DialogVisible = true;
        }

        public void LogRigInfo()
        {
            if (rig == null)
            {
                logger.LogWarning("Rig is not initialized.");
                return;
            }

            logger.LogInformation($"RigType={rig.RigType}, Status={rig.StatusStr}, Freq={rig.Freq}, Mode={rig.Mode}");
        }

        /// <summary>
        /// Push PTT on selected rig for specified number of seconds. If seconds is 0, release PTT immediately. If seconds is negative or greater than 60, log an error and do nothing.
        /// </summary>
        /// <param name="seconds"></param>
        public void PTT(int seconds)
        {
            if (rig == null)
            {
                logger.LogWarning("Rig is not initialized.");
                return;
            }

            if (seconds < 0 || seconds > 60)
            {
                logger.LogError($"Invalid PTT duration: {seconds}");
                return;
            }

            if (seconds == 0)
            {
                HandlePttTimerElapsed();
                return;
            }

            if (pttTimer != null && pttTimer.Enabled)
            {
                logger.LogWarning("PTT is already active.");
                return;
            }

            rig.Mode = OmniRig.RigParamX.PM_CW_L;
            rig.Tx = OmniRig.RigParamX.PM_TX;

            if (rig.Status != OmniRig.RigStatusX.ST_ONLINE || rig.Tx == OmniRig.RigParamX.PM_UNKNOWN)
            {
                logger.LogError($"Failed to set PTT for {rig.RigType}, {rig.StatusStr}");
                return;
            }

            logger.LogInformation($"Pressing PTT on {rig.RigType} for {seconds} seconds, mode {rig.Mode}, qrg {rig.GetTxFrequency()}");

            pttTimer = new System.Timers.Timer(TimeSpan.FromSeconds(seconds))
            {
                AutoReset = false
            };
            pttTimer.Elapsed += (_, _) => HandlePttTimerElapsed();
            pttTimer.Start();
        }

        private void HandlePttTimerElapsed()
        {
            lock (this)
            {
                logger.LogInformation($"Releasing PTT for all rigs.");
                omniRigEngine?.Rig1.Tx = OmniRig.RigParamX.PM_RX;
                omniRigEngine?.Rig2.Tx = OmniRig.RigParamX.PM_RX;
                DisposeTimer(ref pttTimer);
            }
        }

        private void DisposeTimer(ref System.Timers.Timer? timer)
        {
            timer?.Stop();
            timer?.Dispose();
            timer = null;
        }

        private IMqttClient? mqttClient = null;

        private bool isDisposed;

        private OmniRig.OmniRigX? omniRigEngine;
        private OmniRig.RigX? rig;
        private readonly ILogger logger = logger;

        private Task? initTask = null;

        private System.Timers.Timer? pttTimer = null;
        private System.Timers.Timer? infoTimer = null;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    DisposeTimer(ref pttTimer);
                    DisposeTimer(ref infoTimer);
                    omniRigEngine?.Rig1.Tx = OmniRig.RigParamX.PM_RX;
                    omniRigEngine?.Rig2.Tx = OmniRig.RigParamX.PM_RX;
                    rig = null;
                    PublishStatus();
                    mqttClient?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Controller()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
