using Microsoft.Extensions.Logging;

namespace RemotePTT.GUI
{
    public partial class MainForm : Form
    {
        private readonly Controller.Controller controller;

        public MainForm()
        {
            InitializeComponent();

            // Create a logger factory that writes to the RichTextBox on the form
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new RichTextBoxLoggerProvider(rtbInfo));
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var logger = loggerFactory.CreateLogger("Controller");
            
            controller = new Controller.Controller(logger);
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            await controller.StartMqttAsync(tbMqttBroker.Text.Trim());
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller?.Dispose();
        }

        private void btnRiginfo_Click(object sender, EventArgs e)
        {
            controller.LogRigInfo();
        }

        private void btnOmnirigConfigure_Click(object sender, EventArgs e)
        {
            controller.ShowRigConfigurationDialog();
        }

        private void rbRig_CheckedChanged(object sender, EventArgs e)
        {
            controller.SelectRig(rbRig1.Checked ? 1 : 2);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await controller.InitAsync();
            var hostname = tbMqttBroker.Text.Trim();
            if (!string.IsNullOrWhiteSpace(hostname)) await controller.StartMqttAsync(hostname);
        }

        private void btnTestPTT_Click(object sender, EventArgs e)
        {
            controller.PTT(3);
        }
    }
}
