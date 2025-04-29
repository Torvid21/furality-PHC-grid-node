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
            this.configPanel = new System.Windows.Forms.Panel();
            this.selectRig = new System.Windows.Forms.Button();
            this.connect = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.colorTypeDropdown = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.portInput = new System.Windows.Forms.TextBox();
            this.ipInput = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.configPanel.SuspendLayout();
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
            this.helloToolStripMenuItem.Click += new System.EventHandler(this.Config_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // configPanel
            // 
            this.configPanel.Controls.Add(this.selectRig);
            this.configPanel.Controls.Add(this.connect);
            this.configPanel.Controls.Add(this.statusLabel);
            this.configPanel.Controls.Add(this.colorTypeDropdown);
            this.configPanel.Controls.Add(this.label3);
            this.configPanel.Controls.Add(this.label2);
            this.configPanel.Controls.Add(this.label1);
            this.configPanel.Controls.Add(this.portInput);
            this.configPanel.Controls.Add(this.ipInput);
            this.configPanel.Location = new System.Drawing.Point(12, 12);
            this.configPanel.Name = "configPanel";
            this.configPanel.Size = new System.Drawing.Size(165, 184);
            this.configPanel.TabIndex = 1;
            // 
            // selectRig
            // 
            this.selectRig.Location = new System.Drawing.Point(6, 151);
            this.selectRig.Name = "selectRig";
            this.selectRig.Size = new System.Drawing.Size(75, 23);
            this.selectRig.TabIndex = 10;
            this.selectRig.Text = "Select FRig";
            this.selectRig.UseVisualStyleBackColor = true;
            this.selectRig.Click += new System.EventHandler(this.selectRig_Click);
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(3, 121);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 9;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(3, 105);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(37, 13);
            this.statusLabel.TabIndex = 8;
            this.statusLabel.Text = "Status";
            // 
            // colorTypeDropdown
            // 
            this.colorTypeDropdown.FormattingEnabled = true;
            this.colorTypeDropdown.Items.AddRange(new object[] {
            "VRSL",
            "Packed",
            "FRig"});
            this.colorTypeDropdown.Location = new System.Drawing.Point(3, 81);
            this.colorTypeDropdown.Name = "colorTypeDropdown";
            this.colorTypeDropdown.Size = new System.Drawing.Size(100, 21);
            this.colorTypeDropdown.TabIndex = 7;
            this.colorTypeDropdown.Text = "FRig";
            this.colorTypeDropdown.SelectedIndexChanged += new System.EventHandler(this.colorTypeDropdown_SelectedIndexChanged);
            this.colorTypeDropdown.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "DMX Grid Config";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(109, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP";
            // 
            // portInput
            // 
            this.portInput.Location = new System.Drawing.Point(3, 55);
            this.portInput.Name = "portInput";
            this.portInput.Size = new System.Drawing.Size(100, 20);
            this.portInput.TabIndex = 1;
            this.portInput.Text = "6454";
            this.portInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
            // 
            // ipInput
            // 
            this.ipInput.Location = new System.Drawing.Point(3, 29);
            this.ipInput.Name = "ipInput";
            this.ipInput.Size = new System.Drawing.Size(100, 20);
            this.ipInput.TabIndex = 0;
            this.ipInput.Text = "127.0.0.1";
            this.ipInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 208);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.ControlBox = false;
            this.Controls.Add(this.configPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Furality Grid Node";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.configPanel.ResumeLayout(false);
            this.configPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helloToolStripMenuItem;
        private System.Windows.Forms.Panel configPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox portInput;
        private System.Windows.Forms.TextBox ipInput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox colorTypeDropdown;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button selectRig;
    }
}

