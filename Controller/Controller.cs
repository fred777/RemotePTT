using Microsoft.Extensions.Logging;
using MQTTnet;

namespace RemotePTT.Controller
{
    public class Controller(ILogger logger) : IDisposable
    {
        public async Task StartAsync(string mqttBrokerHostname)
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

        //private IRigCore? rig = null;
        private IMqttClient? mqttClient=null;

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    mqttClient?.Dispose();                    
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
