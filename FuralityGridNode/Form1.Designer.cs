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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.helloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridPreviewLTRecording = new System.Windows.Forms.GroupBox();
            this.slow = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.botRecieve = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.setVRChatFocus = new System.Windows.Forms.Button();
            this.botSend = new System.Windows.Forms.CheckBox();
            this.vrchatWindowSelect = new System.Windows.Forms.ComboBox();
            this.setVRChatSize = new System.Windows.Forms.Button();
            this.generateTimecode = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.artNetIpRecieveInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.artNetRecievePortInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.artNetUnicast = new System.Windows.Forms.CheckBox();
            this.artNetConnectButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.testingPlayAnimation = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bigDataCheck = new System.Windows.Forms.CheckBox();
            this.editorCheck = new System.Windows.Forms.CheckBox();
            this.midiStatus = new System.Windows.Forms.Label();
            this.midiConnect = new System.Windows.Forms.Button();
            this.midiDevice = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridPreviewMainStream = new System.Windows.Forms.GroupBox();
            this.res1440p = new System.Windows.Forms.CheckBox();
            this.gridPreviewLTStream = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.artNetIpSendInput = new System.Windows.Forms.TextBox();
            this.artNetSendPortInput = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            // gridPreviewLTRecording
            // 
            this.gridPreviewLTRecording.Location = new System.Drawing.Point(610, 4);
            this.gridPreviewLTRecording.Name = "gridPreviewLTRecording";
            this.gridPreviewLTRecording.Size = new System.Drawing.Size(531, 74);
            this.gridPreviewLTRecording.TabIndex = 17;
            this.gridPreviewLTRecording.TabStop = false;
            this.gridPreviewLTRecording.Text = "LT Recording";
            // 
            // slow
            // 
            this.slow.Location = new System.Drawing.Point(246, 134);
            this.slow.Name = "slow";
            this.slow.Size = new System.Drawing.Size(358, 90);
            this.slow.TabIndex = 17;
            this.slow.Text = "test2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.botRecieve);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.generateTimecode);
            this.groupBox2.Controls.Add(this.res1440p);
            this.groupBox2.Controls.Add(this.botSend);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.setVRChatFocus);
            this.groupBox2.Controls.Add(this.vrchatWindowSelect);
            this.groupBox2.Controls.Add(this.testingPlayAnimation);
            this.groupBox2.Controls.Add(this.setVRChatSize);
            this.groupBox2.Location = new System.Drawing.Point(310, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 127);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "LT Sync";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(97, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 25;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // botRecieve
            // 
            this.botRecieve.AutoSize = true;
            this.botRecieve.Location = new System.Drawing.Point(5, 96);
            this.botRecieve.Name = "botRecieve";
            this.botRecieve.Size = new System.Drawing.Size(166, 17);
            this.botRecieve.TabIndex = 24;
            this.botRecieve.Text = "Generate Timecode and sync";
            this.botRecieve.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(160, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Target VRChat Window";
            // 
            // setVRChatFocus
            // 
            this.setVRChatFocus.Location = new System.Drawing.Point(4, 44);
            this.setVRChatFocus.Name = "setVRChatFocus";
            this.setVRChatFocus.Size = new System.Drawing.Size(92, 23);
            this.setVRChatFocus.TabIndex = 22;
            this.setVRChatFocus.Text = "Focus Window";
            this.setVRChatFocus.UseVisualStyleBackColor = true;
            this.setVRChatFocus.Click += new System.EventHandler(this.setVRChatFocus_Click);
            // 
            // botSend
            // 
            this.botSend.AutoSize = true;
            this.botSend.Location = new System.Drawing.Point(5, 73);
            this.botSend.Name = "botSend";
            this.botSend.Size = new System.Drawing.Size(178, 17);
            this.botSend.TabIndex = 20;
            this.botSend.Text = "Record bot and Send via ArtNet";
            this.botSend.UseVisualStyleBackColor = true;
            // 
            // vrchatWindowSelect
            // 
            this.vrchatWindowSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vrchatWindowSelect.FormattingEnabled = true;
            this.vrchatWindowSelect.Location = new System.Drawing.Point(5, 18);
            this.vrchatWindowSelect.Margin = new System.Windows.Forms.Padding(2);
            this.vrchatWindowSelect.Name = "vrchatWindowSelect";
            this.vrchatWindowSelect.Size = new System.Drawing.Size(90, 21);
            this.vrchatWindowSelect.TabIndex = 14;
            // 
            // setVRChatSize
            // 
            this.setVRChatSize.Location = new System.Drawing.Point(97, 44);
            this.setVRChatSize.Name = "setVRChatSize";
            this.setVRChatSize.Size = new System.Drawing.Size(98, 23);
            this.setVRChatSize.TabIndex = 20;
            this.setVRChatSize.Text = "Set Window Size";
            this.setVRChatSize.UseVisualStyleBackColor = true;
            this.setVRChatSize.Click += new System.EventHandler(this.setVRChatSize_Click);
            // 
            // generateTimecode
            // 
            this.generateTimecode.AutoSize = true;
            this.generateTimecode.Location = new System.Drawing.Point(239, 57);
            this.generateTimecode.Name = "generateTimecode";
            this.generateTimecode.Size = new System.Drawing.Size(120, 17);
            this.generateTimecode.TabIndex = 19;
            this.generateTimecode.Text = "Generate Timecode";
            this.generateTimecode.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.statusLabel);
            this.groupBox1.Controls.Add(this.artNetIpRecieveInput);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.artNetRecievePortInput);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.artNetUnicast);
            this.groupBox1.Controls.Add(this.artNetConnectButton);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(173, 127);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ArtNet Recieve";
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
            // artNetIpRecieveInput
            // 
            this.artNetIpRecieveInput.Location = new System.Drawing.Point(9, 16);
            this.artNetIpRecieveInput.Name = "artNetIpRecieveInput";
            this.artNetIpRecieveInput.Size = new System.Drawing.Size(89, 20);
            this.artNetIpRecieveInput.TabIndex = 0;
            this.artNetIpRecieveInput.Text = "127.0.0.1";
            this.artNetIpRecieveInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
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
            // artNetRecievePortInput
            // 
            this.artNetRecievePortInput.Location = new System.Drawing.Point(9, 42);
            this.artNetRecievePortInput.Name = "artNetRecievePortInput";
            this.artNetRecievePortInput.Size = new System.Drawing.Size(44, 20);
            this.artNetRecievePortInput.TabIndex = 1;
            this.artNetRecievePortInput.Text = "6454";
            this.artNetRecievePortInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
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
            // artNetUnicast
            // 
            this.artNetUnicast.AutoSize = true;
            this.artNetUnicast.Checked = true;
            this.artNetUnicast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.artNetUnicast.Location = new System.Drawing.Point(9, 68);
            this.artNetUnicast.Name = "artNetUnicast";
            this.artNetUnicast.Size = new System.Drawing.Size(62, 17);
            this.artNetUnicast.TabIndex = 11;
            this.artNetUnicast.Text = "Unicast";
            this.artNetUnicast.UseVisualStyleBackColor = true;
            // 
            // artNetConnectButton
            // 
            this.artNetConnectButton.Location = new System.Drawing.Point(6, 91);
            this.artNetConnectButton.Name = "artNetConnectButton";
            this.artNetConnectButton.Size = new System.Drawing.Size(75, 23);
            this.artNetConnectButton.TabIndex = 9;
            this.artNetConnectButton.Text = "Connect";
            this.artNetConnectButton.UseVisualStyleBackColor = true;
            this.artNetConnectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // testingPlayAnimation
            // 
            this.testingPlayAnimation.Location = new System.Drawing.Point(239, 80);
            this.testingPlayAnimation.Name = "testingPlayAnimation";
            this.testingPlayAnimation.Size = new System.Drawing.Size(122, 23);
            this.testingPlayAnimation.TabIndex = 15;
            this.testingPlayAnimation.Text = "Play Test Animation";
            this.testingPlayAnimation.UseVisualStyleBackColor = true;
            this.testingPlayAnimation.Click += new System.EventHandler(this.testAnimation_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bigDataCheck);
            this.groupBox3.Controls.Add(this.editorCheck);
            this.groupBox3.Controls.Add(this.midiStatus);
            this.groupBox3.Controls.Add(this.midiConnect);
            this.groupBox3.Controls.Add(this.midiDevice);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(6, 137);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(234, 87);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MIDIDMX";
            // 
            // bigDataCheck
            // 
            this.bigDataCheck.AutoSize = true;
            this.bigDataCheck.Location = new System.Drawing.Point(74, 49);
            this.bigDataCheck.Margin = new System.Windows.Forms.Padding(2);
            this.bigDataCheck.Name = "bigDataCheck";
            this.bigDataCheck.Size = new System.Drawing.Size(141, 17);
            this.bigDataCheck.TabIndex = 13;
            this.bigDataCheck.Text = "Big Data [client crashes]";
            this.bigDataCheck.UseVisualStyleBackColor = true;
            this.bigDataCheck.Visible = false;
            // 
            // editorCheck
            // 
            this.editorCheck.AutoSize = true;
            this.editorCheck.Location = new System.Drawing.Point(6, 49);
            this.editorCheck.Name = "editorCheck";
            this.editorCheck.Size = new System.Drawing.Size(65, 17);
            this.editorCheck.TabIndex = 12;
            this.editorCheck.Text = "In Editor";
            this.editorCheck.UseVisualStyleBackColor = true;
            this.editorCheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // midiStatus
            // 
            this.midiStatus.AutoSize = true;
            this.midiStatus.Location = new System.Drawing.Point(104, 71);
            this.midiStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.midiStatus.Name = "midiStatus";
            this.midiStatus.Size = new System.Drawing.Size(37, 13);
            this.midiStatus.TabIndex = 11;
            this.midiStatus.Text = "Status";
            // 
            // midiConnect
            // 
            this.midiConnect.Location = new System.Drawing.Point(163, 19);
            this.midiConnect.Name = "midiConnect";
            this.midiConnect.Size = new System.Drawing.Size(59, 23);
            this.midiConnect.TabIndex = 10;
            this.midiConnect.Text = "Refresh";
            this.midiConnect.UseVisualStyleBackColor = true;
            this.midiConnect.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // midiDevice
            // 
            this.midiDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.midiDevice.FormattingEnabled = true;
            this.midiDevice.Location = new System.Drawing.Point(5, 20);
            this.midiDevice.Margin = new System.Windows.Forms.Padding(2);
            this.midiDevice.Name = "midiDevice";
            this.midiDevice.Size = new System.Drawing.Size(156, 21);
            this.midiDevice.TabIndex = 0;
            this.midiDevice.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "version 5";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1155, 255);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.slow);
            this.tabPage1.Controls.Add(this.gridPreviewMainStream);
            this.tabPage1.Controls.Add(this.gridPreviewLTStream);
            this.tabPage1.Controls.Add(this.gridPreviewLTRecording);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1147, 229);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridPreviewMainStream
            // 
            this.gridPreviewMainStream.Location = new System.Drawing.Point(610, 151);
            this.gridPreviewMainStream.Name = "gridPreviewMainStream";
            this.gridPreviewMainStream.Size = new System.Drawing.Size(531, 74);
            this.gridPreviewMainStream.TabIndex = 19;
            this.gridPreviewMainStream.TabStop = false;
            this.gridPreviewMainStream.Text = "Output for Main Stream";
            // 
            // res1440p
            // 
            this.res1440p.AutoSize = true;
            this.res1440p.Location = new System.Drawing.Point(239, 105);
            this.res1440p.Margin = new System.Windows.Forms.Padding(2);
            this.res1440p.Name = "res1440p";
            this.res1440p.Size = new System.Drawing.Size(56, 17);
            this.res1440p.TabIndex = 18;
            this.res1440p.Text = "1440p";
            this.res1440p.UseVisualStyleBackColor = true;
            // 
            // gridPreviewLTStream
            // 
            this.gridPreviewLTStream.Location = new System.Drawing.Point(610, 77);
            this.gridPreviewLTStream.Name = "gridPreviewLTStream";
            this.gridPreviewLTStream.Size = new System.Drawing.Size(531, 74);
            this.gridPreviewLTStream.TabIndex = 18;
            this.gridPreviewLTStream.TabStop = false;
            this.gridPreviewLTStream.Text = "Output for LT";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1147, 229);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "creature??";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(431, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "creature!!";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick_1);
            // 
            // artNetIpSendInput
            // 
            this.artNetIpSendInput.Location = new System.Drawing.Point(6, 19);
            this.artNetIpSendInput.Name = "artNetIpSendInput";
            this.artNetIpSendInput.Size = new System.Drawing.Size(89, 20);
            this.artNetIpSendInput.TabIndex = 20;
            this.artNetIpSendInput.Text = "127.0.0.1";
            // 
            // artNetSendPortInput
            // 
            this.artNetSendPortInput.Location = new System.Drawing.Point(6, 45);
            this.artNetSendPortInput.Name = "artNetSendPortInput";
            this.artNetSendPortInput.Size = new System.Drawing.Size(44, 20);
            this.artNetSendPortInput.TabIndex = 21;
            this.artNetSendPortInput.Text = "6454";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.artNetIpSendInput);
            this.groupBox4.Controls.Add(this.artNetSendPortInput);
            this.groupBox4.Location = new System.Drawing.Point(185, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(119, 127);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ArtNet Send";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1163, 258);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Furality Grid Node - PHC Edition";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helloToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox artNetRecievePortInput;
        private System.Windows.Forms.TextBox artNetIpRecieveInput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button artNetConnectButton;
        private System.Windows.Forms.CheckBox artNetUnicast;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.GroupBox gridPreviewLTRecording;
        private System.Windows.Forms.Button testingPlayAnimation;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label slow;
        private System.Windows.Forms.ComboBox midiDevice;
        private System.Windows.Forms.Label midiStatus;
        private System.Windows.Forms.Button midiConnect;
        private System.Windows.Forms.CheckBox editorCheck;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox bigDataCheck;
        private System.Windows.Forms.CheckBox generateTimecode;
        private System.Windows.Forms.Button setVRChatSize;
        private System.Windows.Forms.ComboBox vrchatWindowSelect;
        private System.Windows.Forms.Button setVRChatFocus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox botSend;
        private System.Windows.Forms.GroupBox gridPreviewLTStream;
        private System.Windows.Forms.CheckBox res1440p;
        private System.Windows.Forms.CheckBox botRecieve;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.GroupBox gridPreviewMainStream;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox artNetIpSendInput;
        private System.Windows.Forms.TextBox artNetSendPortInput;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}

