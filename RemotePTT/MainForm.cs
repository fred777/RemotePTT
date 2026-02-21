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
            await _controller.StartMqttAsync(host);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _controller?.Dispose();
        }

        private void btnRiginfo_Click(object sender, EventArgs e)
        {
            _controller.LogRigInfo();
        }

        private void btnOmnirigConfigure_Click(object sender, EventArgs e)
        {
            _controller.RigConfigure();
        }

        private void rbRig_CheckedChanged(object sender, EventArgs e)
        {
            _controller.SetRig(rbRig1.Checked ? 1 : 2);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _controller.Init();
        }
    }
}
