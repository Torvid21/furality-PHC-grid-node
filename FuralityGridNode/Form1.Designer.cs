using System.Media;

namespace FuralityGridNode
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.helloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridPreview = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.largeCRC = new System.Windows.Forms.CheckBox();
            this.layoutStatus = new System.Windows.Forms.Label();
            this.rigTypeDropdown = new System.Windows.Forms.ComboBox();
            this.UnloadLayout = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.selectRig = new System.Windows.Forms.Button();
            this.LoadLayout = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.ipInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.portInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.unicast = new System.Windows.Forms.CheckBox();
            this.connect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.testAnimation = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helloToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(111, 48);
            // 
            // helloToolStripMenuItem
            // 
            this.helloToolStripMenuItem.Name = "helloToolStripMenuItem";
            this.helloToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.helloToolStripMenuItem.Text = "Config";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // gridPreview
            // 
            this.gridPreview.Location = new System.Drawing.Point(480, 7);
            this.gridPreview.Name = "gridPreview";
            this.gridPreview.Size = new System.Drawing.Size(499, 180);
            this.gridPreview.TabIndex = 17;
            this.gridPreview.TabStop = false;
            this.gridPreview.Text = "Preview";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.largeCRC);
            this.groupBox2.Controls.Add(this.layoutStatus);
            this.groupBox2.Controls.Add(this.rigTypeDropdown);
            this.groupBox2.Controls.Add(this.UnloadLayout);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.selectRig);
            this.groupBox2.Controls.Add(this.LoadLayout);
            this.groupBox2.Location = new System.Drawing.Point(244, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 209);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rig";
            // 
            // largeCRC
            // 
            this.largeCRC.AutoSize = true;
            this.largeCRC.Location = new System.Drawing.Point(153, 15);
            this.largeCRC.Name = "largeCRC";
            this.largeCRC.Size = new System.Drawing.Size(71, 17);
            this.largeCRC.TabIndex = 15;
            this.largeCRC.Text = "8-bit CRC";
            this.largeCRC.UseVisualStyleBackColor = true;
            // 
            // layoutStatus
            // 
            this.layoutStatus.Location = new System.Drawing.Point(7, 130);
            this.layoutStatus.Name = "layoutStatus";
            this.layoutStatus.Size = new System.Drawing.Size(217, 55);
            this.layoutStatus.TabIndex = 13;
            this.layoutStatus.Text = "VRSL\r\nsize: 1920x208\r\nchannels: 1560";
            // 
            // rigTypeDropdown
            // 
            this.rigTypeDropdown.FormattingEnabled = true;
            this.rigTypeDropdown.Items.AddRange(new object[] {
            "VRSL",
            "Packed",
            "FRig",
            "Binary"});
            this.rigTypeDropdown.Location = new System.Drawing.Point(10, 19);
            this.rigTypeDropdown.Name = "rigTypeDropdown";
            this.rigTypeDropdown.Size = new System.Drawing.Size(64, 21);
            this.rigTypeDropdown.TabIndex = 7;
            this.rigTypeDropdown.Text = "FRig";
            this.rigTypeDropdown.SelectedIndexChanged += new System.EventHandler(this.colorTypeDropdown_SelectedIndexChanged);
            this.rigTypeDropdown.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
            // 
            // UnloadLayout
            // 
            this.UnloadLayout.Location = new System.Drawing.Point(10, 104);
            this.UnloadLayout.Name = "UnloadLayout";
            this.UnloadLayout.Size = new System.Drawing.Size(104, 23);
            this.UnloadLayout.TabIndex = 13;
            this.UnloadLayout.Text = "Unload Layout";
            this.UnloadLayout.UseVisualStyleBackColor = true;
            this.UnloadLayout.Click += new System.EventHandler(this.UnloadLayout_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Type";
            // 
            // selectRig
            // 
            this.selectRig.Location = new System.Drawing.Point(10, 46);
            this.selectRig.Name = "selectRig";
            this.selectRig.Size = new System.Drawing.Size(104, 23);
            this.selectRig.TabIndex = 10;
            this.selectRig.Text = "Select FRig";
            this.selectRig.UseVisualStyleBackColor = true;
            this.selectRig.Click += new System.EventHandler(this.selectRig_Click);
            // 
            // LoadLayout
            // 
            this.LoadLayout.Location = new System.Drawing.Point(10, 75);
            this.LoadLayout.Name = "LoadLayout";
            this.LoadLayout.Size = new System.Drawing.Size(104, 23);
            this.LoadLayout.TabIndex = 12;
            this.LoadLayout.Text = "Load Layout";
            this.LoadLayout.UseVisualStyleBackColor = true;
            this.LoadLayout.Click += new System.EventHandler(this.LoadLayout_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.statusLabel);
            this.groupBox1.Controls.Add(this.ipInput);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.portInput);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.unicast);
            this.groupBox1.Controls.Add(this.connect);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 127);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ArtNet";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(87, 96);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(37, 13);
            this.statusLabel.TabIndex = 12;
            this.statusLabel.Text = "Status";
            // 
            // ipInput
            // 
            this.ipInput.Location = new System.Drawing.Point(9, 16);
            this.ipInput.Name = "ipInput";
            this.ipInput.Size = new System.Drawing.Size(89, 20);
            this.ipInput.TabIndex = 0;
            this.ipInput.Text = "127.0.0.1";
            this.ipInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Address";
            // 
            // portInput
            // 
            this.portInput.Location = new System.Drawing.Point(9, 42);
            this.portInput.Name = "portInput";
            this.portInput.Size = new System.Drawing.Size(44, 20);
            this.portInput.TabIndex = 1;
            this.portInput.Text = "6454";
            this.portInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // unicast
            // 
            this.unicast.AutoSize = true;
            this.unicast.Checked = true;
            this.unicast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unicast.Location = new System.Drawing.Point(9, 68);
            this.unicast.Name = "unicast";
            this.unicast.Size = new System.Drawing.Size(62, 17);
            this.unicast.TabIndex = 11;
            this.unicast.Text = "Unicast";
            this.unicast.UseVisualStyleBackColor = true;
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(6, 91);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 9;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // testAnimation
            // 
            this.testAnimation.Location = new System.Drawing.Point(480, 191);
            this.testAnimation.Name = "testAnimation";
            this.testAnimation.Size = new System.Drawing.Size(122, 23);
            this.testAnimation.TabIndex = 15;
            this.testAnimation.Text = "Play Test Animation";
            this.testAnimation.UseVisualStyleBackColor = true;
            this.testAnimation.Click += new System.EventHandler(this.testAnimation_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Generate Mask Image";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Location = new System.Drawing.Point(12, 140);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 76);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tools";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(187, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Mask";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 51);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(175, 20);
            this.textBox1.TabIndex = 13;
            this.textBox1.Text = "5, 20-45, 32, 800-900";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 224);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.testAnimation);
            this.Controls.Add(this.gridPreview);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Furality Grid Node";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helloToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox portInput;
        private System.Windows.Forms.TextBox ipInput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox rigTypeDropdown;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button selectRig;
        private System.Windows.Forms.Button LoadLayout;
        private System.Windows.Forms.Button UnloadLayout;
        private System.Windows.Forms.CheckBox unicast;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label layoutStatus;
        private System.Windows.Forms.GroupBox gridPreview;
        private System.Windows.Forms.Button testAnimation;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox largeCRC;
    }
}

