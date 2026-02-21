using Microsoft.Extensions.Logging;
using MQTTnet;

namespace RemotePTT.Controller
{
    public class Controller(ILogger logger) : IDisposable
    {
        public void Init()
        {
            initTask= Task.Run(() =>
               {                   
                   omniRigEngine = new OmniRig.OmniRigX();
                   logger.LogInformation($"Omnirig InterfaceVersion {omniRigEngine.InterfaceVersion}, SoftwareVersion {omniRigEngine.SoftwareVersion}");
                   SetRig(1);
               });
        }

        public async Task StartMqttAsync(string mqttBrokerHostname)
        {
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
            }
            catch (Exception ex)
            {
                logger.LogError($"MQTT error: {ex.Message}");
            }
        }

        public void SetRig(int rigNumber)
        {
            if (omniRigEngine == null)
            {
                logger.LogWarning("OmniRig engine is not initialized.");
                return;
            }

            if (rigNumber < 0 || rigNumber > 2)
            {
                logger.LogWarning("Only rig numbers 1 and 2 are supported.");
                return;
            }

            rig = rigNumber == 1 ? omniRigEngine.Rig1 : omniRigEngine.Rig2;

            LogRigInfo();
        }

        public void RigConfigure()
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

        private void ReleaseRessources()
        {
            mqttClient?.Dispose();
        }

        //private IRigCore? rig = null;
        private IMqttClient? mqttClient = null;

        private bool disposedValue;

        private OmniRig.OmniRigX? omniRigEngine;
        private OmniRig.RigX? rig;
        private readonly ILogger logger = logger;

        private Task? initTask = null;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    ReleaseRessources();
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
