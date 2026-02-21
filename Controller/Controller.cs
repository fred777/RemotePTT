using Microsoft.Extensions.Logging;
using MQTTnet;
using System.Text;
namespace RemotePTT.Controller
{
    public class Controller(ILogger logger) : IDisposable
    {        
        public string Topic => "ham/remoteptt";

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

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(Topic).Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                logger.LogInformation($"MQTT subscribed to {Topic}");
            }
            catch (Exception ex)
            {
                logger.LogError($"MQTT error: {ex.Message}");
            }
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

            pttTimer = new System.Timers.Timer(TimeSpan.FromSeconds(seconds));
            pttTimer.AutoReset = false;
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
                pttTimer?.Stop();
                pttTimer?.Dispose();
                pttTimer = null;
            }
        }

        private IMqttClient? mqttClient = null;

        private bool disposedValue;

        private OmniRig.OmniRigX? omniRigEngine;
        private OmniRig.RigX? rig;
        private readonly ILogger logger = logger;

        private Task? initTask = null;

        private System.Timers.Timer? pttTimer = null;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    mqttClient?.Dispose();
                    pttTimer?.Dispose();
                    omniRigEngine?.Rig1.Tx = OmniRig.RigParamX.PM_RX;
                    omniRigEngine?.Rig2.Tx = OmniRig.RigParamX.PM_RX;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
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
