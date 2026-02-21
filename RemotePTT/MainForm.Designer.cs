namespace RemotePTT.GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label2 = new Label();
            tbMqttBroker = new TextBox();
            btnConnect = new Button();
            label1 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            rbRig1 = new RadioButton();
            rbRig2 = new RadioButton();
            btnOmnirigConfigure = new Button();
            btnRiginfo = new Button();
            btnTestPTT = new Button();
            rtbInfo = new RichTextBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(rtbInfo, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(569, 425);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(label2, 0, 1);
            tableLayoutPanel2.Controls.Add(tbMqttBroker, 1, 0);
            tableLayoutPanel2.Controls.Add(btnConnect, 2, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(flowLayoutPanel1, 1, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(569, 64);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 35);
            label2.Margin = new Padding(3);
            label2.Name = "label2";
            label2.Size = new Size(66, 26);
            label2.TabIndex = 3;
            label2.Text = "OmniRig";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tbMqttBroker
            // 
            tbMqttBroker.Dock = DockStyle.Fill;
            tbMqttBroker.Location = new Point(75, 3);
            tbMqttBroker.Name = "tbMqttBroker";
            tbMqttBroker.Size = new Size(423, 23);
            tbMqttBroker.TabIndex = 1;
            tbMqttBroker.Text = "s740.fritz.box";
            // 
            // btnConnect
            // 
            btnConnect.AutoSize = true;
            btnConnect.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnConnect.Dock = DockStyle.Fill;
            btnConnect.Location = new Point(504, 3);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(62, 26);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 3);
            label1.Margin = new Padding(3);
            label1.Name = "label1";
            label1.Size = new Size(66, 26);
            label1.TabIndex = 0;
            label1.Text = "MQTT host";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            tableLayoutPanel2.SetColumnSpan(flowLayoutPanel1, 2);
            flowLayoutPanel1.Controls.Add(rbRig1);
            flowLayoutPanel1.Controls.Add(rbRig2);
            flowLayoutPanel1.Controls.Add(btnOmnirigConfigure);
            flowLayoutPanel1.Controls.Add(btnRiginfo);
            flowLayoutPanel1.Controls.Add(btnTestPTT);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(72, 32);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(497, 32);
            flowLayoutPanel1.TabIndex = 4;
            // 
            // rbRig1
            // 
            rbRig1.AutoSize = true;
            rbRig1.Checked = true;
            rbRig1.Dock = DockStyle.Left;
            rbRig1.Location = new Point(3, 3);
            rbRig1.Name = "rbRig1";
            rbRig1.Size = new Size(51, 25);
            rbRig1.TabIndex = 0;
            rbRig1.TabStop = true;
            rbRig1.Text = "Rig 1";
            rbRig1.UseVisualStyleBackColor = true;
            rbRig1.CheckedChanged += rbRig_CheckedChanged;
            // 
            // rbRig2
            // 
            rbRig2.AutoSize = true;
            rbRig2.Dock = DockStyle.Left;
            rbRig2.Location = new Point(60, 3);
            rbRig2.Name = "rbRig2";
            rbRig2.Size = new Size(51, 25);
            rbRig2.TabIndex = 1;
            rbRig2.Text = "Rig 2";
            rbRig2.UseVisualStyleBackColor = true;
            // 
            // btnOmnirigConfigure
            // 
            btnOmnirigConfigure.AutoSize = true;
            btnOmnirigConfigure.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOmnirigConfigure.Dock = DockStyle.Left;
            btnOmnirigConfigure.Location = new Point(117, 3);
            btnOmnirigConfigure.Name = "btnOmnirigConfigure";
            btnOmnirigConfigure.Size = new Size(70, 25);
            btnOmnirigConfigure.TabIndex = 4;
            btnOmnirigConfigure.Text = "Configure";
            btnOmnirigConfigure.UseVisualStyleBackColor = true;
            btnOmnirigConfigure.Click += btnOmnirigConfigure_Click;
            // 
            // btnRiginfo
            // 
            btnRiginfo.AutoSize = true;
            btnRiginfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnRiginfo.Dock = DockStyle.Left;
            btnRiginfo.Location = new Point(193, 3);
            btnRiginfo.Name = "btnRiginfo";
            btnRiginfo.Size = new Size(58, 25);
            btnRiginfo.TabIndex = 3;
            btnRiginfo.Text = "Rig Info";
            btnRiginfo.UseVisualStyleBackColor = true;
            btnRiginfo.Click += btnRiginfo_Click;
            // 
            // btnTestPTT
            // 
            btnTestPTT.AutoSize = true;
            btnTestPTT.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnTestPTT.Dock = DockStyle.Left;
            btnTestPTT.Location = new Point(257, 3);
            btnTestPTT.Name = "btnTestPTT";
            btnTestPTT.Size = new Size(62, 25);
            btnTestPTT.TabIndex = 5;
            btnTestPTT.Text = "Test PTT";
            btnTestPTT.UseVisualStyleBackColor = true;
            // 
            // rtbInfo
            // 
            rtbInfo.Dock = DockStyle.Fill;
            rtbInfo.Location = new Point(3, 67);
            rtbInfo.Name = "rtbInfo";
            rtbInfo.ReadOnly = true;
            rtbInfo.Size = new Size(563, 355);
            rtbInfo.TabIndex = 1;
            rtbInfo.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(569, 425);
            Controls.Add(tableLayoutPanel1);
            Name = "MainForm";
            Text = "RemotePTT";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private TextBox tbMqttBroker;
        private Button btnConnect;
        private RichTextBox rtbInfo;
        private Label label2;
        private FlowLayoutPanel flowLayoutPanel1;
        private RadioButton rbRig1;
        private RadioButton rbRig2;
        private Button btnRiginfo;
        private Button btnOmnirigConfigure;
        private Button btnTestPTT;
    }
}
