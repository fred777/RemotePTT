using Microsoft.Extensions.Logging;

namespace RemotePTT.GUI
{
    public partial class MainForm : Form
    {
        private readonly Controller.Controller _controller;

        public MainForm()
        {
            InitializeComponent();

            // Create a logger factory that writes to the RichTextBox on the form
            using var _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new RichTextBoxLoggerProvider(rtbInfo));
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var _logger = _loggerFactory.CreateLogger("Controller");

            // instantiate controller with the logger
            _controller = new Controller.Controller(_logger);
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            var host = tbMqttBroker.Text?.Trim();
            if (string.IsNullOrEmpty(host))
            {
                MessageBox.Show("Please enter a valid MQTT broker hostname.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            await _controller.StartAsync(host);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _controller?.Dispose();
        }
    }
}
