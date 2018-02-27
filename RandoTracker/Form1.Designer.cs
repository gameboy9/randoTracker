namespace RandoTracker
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.cmdConnect = new System.Windows.Forms.Button();
            this.cmdStartServer = new System.Windows.Forms.Button();
            this.btnChooseGame = new System.Windows.Forms.Button();
            this.lblGameName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCommentary = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFreeText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStartClock = new System.Windows.Forms.TextBox();
            this.radVisAudio = new System.Windows.Forms.RadioButton();
            this.radVisFinal = new System.Windows.Forms.RadioButton();
            this.radVisState = new System.Windows.Forms.RadioButton();
            this.cboBackground = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSplitTitle = new System.Windows.Forms.Label();
            this.cboCompression = new System.Windows.Forms.ComboBox();
            this.cmdSplitReport = new System.Windows.Forms.Button();
            this.btnReloadLayout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(13, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 91;
            this.label2.Text = "IP/Port:";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(172, 69);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(55, 20);
            this.txtPort.TabIndex = 108;
            // 
            // txtIP
            // 
            this.txtIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIP.Location = new System.Drawing.Point(67, 69);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(89, 20);
            this.txtIP.TabIndex = 107;
            // 
            // cmdConnect
            // 
            this.cmdConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdConnect.Location = new System.Drawing.Point(104, 96);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(87, 23);
            this.cmdConnect.TabIndex = 110;
            this.cmdConnect.Text = "Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // cmdStartServer
            // 
            this.cmdStartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdStartServer.Location = new System.Drawing.Point(13, 96);
            this.cmdStartServer.Name = "cmdStartServer";
            this.cmdStartServer.Size = new System.Drawing.Size(87, 23);
            this.cmdStartServer.TabIndex = 109;
            this.cmdStartServer.Text = "Start Server";
            this.cmdStartServer.UseVisualStyleBackColor = true;
            this.cmdStartServer.Click += new System.EventHandler(this.cmdStartServer_Click);
            // 
            // btnChooseGame
            // 
            this.btnChooseGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseGame.Location = new System.Drawing.Point(13, 12);
            this.btnChooseGame.Name = "btnChooseGame";
            this.btnChooseGame.Size = new System.Drawing.Size(87, 23);
            this.btnChooseGame.TabIndex = 1;
            this.btnChooseGame.Text = "Choose Game";
            this.btnChooseGame.UseVisualStyleBackColor = true;
            this.btnChooseGame.Click += new System.EventHandler(this.btnChooseGame_Click);
            // 
            // lblGameName
            // 
            this.lblGameName.BackColor = System.Drawing.Color.Transparent;
            this.lblGameName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameName.ForeColor = System.Drawing.Color.White;
            this.lblGameName.Location = new System.Drawing.Point(105, 16);
            this.lblGameName.Name = "lblGameName";
            this.lblGameName.Size = new System.Drawing.Size(142, 38);
            this.lblGameName.TabIndex = 93;
            this.lblGameName.Text = "Game:  Dragon Quest 1 SFC";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 455);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 103;
            this.label1.Text = "Comm:";
            // 
            // txtCommentary
            // 
            this.txtCommentary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommentary.Location = new System.Drawing.Point(82, 443);
            this.txtCommentary.Multiline = true;
            this.txtCommentary.Name = "txtCommentary";
            this.txtCommentary.Size = new System.Drawing.Size(164, 40);
            this.txtCommentary.TabIndex = 100;
            this.txtCommentary.Leave += new System.EventHandler(this.txtCommentary_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(13, 306);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Players";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(10, 627);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 23);
            this.btnStart.TabIndex = 104;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(97, 627);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(64, 23);
            this.btnStop.TabIndex = 105;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(183, 627);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(64, 23);
            this.btnReset.TabIndex = 106;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 90;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(13, 123);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(235, 56);
            this.listBox1.TabIndex = 118;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(13, 513);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 121;
            this.label7.Text = "Free Text";
            // 
            // txtFreeText
            // 
            this.txtFreeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFreeText.Location = new System.Drawing.Point(82, 495);
            this.txtFreeText.Multiline = true;
            this.txtFreeText.Name = "txtFreeText";
            this.txtFreeText.Size = new System.Drawing.Size(164, 52);
            this.txtFreeText.TabIndex = 101;
            this.txtFreeText.WordWrap = false;
            this.txtFreeText.Leave += new System.EventHandler(this.txtFreeText_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(10, 600);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 123;
            this.label8.Text = "Start clock at ";
            // 
            // txtStartClock
            // 
            this.txtStartClock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStartClock.Location = new System.Drawing.Point(177, 597);
            this.txtStartClock.Name = "txtStartClock";
            this.txtStartClock.Size = new System.Drawing.Size(69, 20);
            this.txtStartClock.TabIndex = 103;
            this.txtStartClock.Text = "0:00:00";
            this.txtStartClock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radVisAudio
            // 
            this.radVisAudio.AutoSize = true;
            this.radVisAudio.ForeColor = System.Drawing.Color.White;
            this.radVisAudio.Location = new System.Drawing.Point(91, 306);
            this.radVisAudio.Name = "radVisAudio";
            this.radVisAudio.Size = new System.Drawing.Size(52, 17);
            this.radVisAudio.TabIndex = 124;
            this.radVisAudio.TabStop = true;
            this.radVisAudio.Text = "Audio";
            this.radVisAudio.UseVisualStyleBackColor = true;
            // 
            // radVisFinal
            // 
            this.radVisFinal.AutoSize = true;
            this.radVisFinal.ForeColor = System.Drawing.Color.White;
            this.radVisFinal.Location = new System.Drawing.Point(150, 306);
            this.radVisFinal.Name = "radVisFinal";
            this.radVisFinal.Size = new System.Drawing.Size(47, 17);
            this.radVisFinal.TabIndex = 125;
            this.radVisFinal.TabStop = true;
            this.radVisFinal.Text = "Final";
            this.radVisFinal.UseVisualStyleBackColor = true;
            // 
            // radVisState
            // 
            this.radVisState.AutoSize = true;
            this.radVisState.ForeColor = System.Drawing.Color.White;
            this.radVisState.Location = new System.Drawing.Point(203, 306);
            this.radVisState.Name = "radVisState";
            this.radVisState.Size = new System.Drawing.Size(40, 17);
            this.radVisState.TabIndex = 126;
            this.radVisState.TabStop = true;
            this.radVisState.Text = "BG";
            this.radVisState.UseVisualStyleBackColor = true;
            // 
            // cboBackground
            // 
            this.cboBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBackground.FormattingEnabled = true;
            this.cboBackground.Location = new System.Drawing.Point(82, 564);
            this.cboBackground.Name = "cboBackground";
            this.cboBackground.Size = new System.Drawing.Size(164, 21);
            this.cboBackground.TabIndex = 102;
            this.cboBackground.SelectedIndexChanged += new System.EventHandler(this.cboBackground_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(13, 567);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 128;
            this.label5.Text = "BG:";
            // 
            // lblSplitTitle
            // 
            this.lblSplitTitle.AutoSize = true;
            this.lblSplitTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSplitTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSplitTitle.ForeColor = System.Drawing.Color.White;
            this.lblSplitTitle.Location = new System.Drawing.Point(12, 194);
            this.lblSplitTitle.Name = "lblSplitTitle";
            this.lblSplitTitle.Size = new System.Drawing.Size(41, 13);
            this.lblSplitTitle.TabIndex = 129;
            this.lblSplitTitle.Text = "Splits - ";
            // 
            // cboCompression
            // 
            this.cboCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCompression.FormattingEnabled = true;
            this.cboCompression.Items.AddRange(new object[] {
            "Restreamer view",
            "Tracker view",
            "Commentator view"});
            this.cboCompression.Location = new System.Drawing.Point(14, 41);
            this.cboCompression.Name = "cboCompression";
            this.cboCompression.Size = new System.Drawing.Size(234, 21);
            this.cboCompression.TabIndex = 130;
            this.cboCompression.SelectedIndexChanged += new System.EventHandler(this.cboCompression_SelectedIndexChanged);
            // 
            // cmdSplitReport
            // 
            this.cmdSplitReport.Location = new System.Drawing.Point(195, 190);
            this.cmdSplitReport.Name = "cmdSplitReport";
            this.cmdSplitReport.Size = new System.Drawing.Size(50, 23);
            this.cmdSplitReport.TabIndex = 131;
            this.cmdSplitReport.Text = "Report";
            this.cmdSplitReport.UseVisualStyleBackColor = true;
            this.cmdSplitReport.Click += new System.EventHandler(this.cmdSplitReport_Click);
            // 
            // btnReloadLayout
            // 
            this.btnReloadLayout.Location = new System.Drawing.Point(10, 713);
            this.btnReloadLayout.Name = "btnReloadLayout";
            this.btnReloadLayout.Size = new System.Drawing.Size(90, 23);
            this.btnReloadLayout.TabIndex = 132;
            this.btnReloadLayout.Text = "Reload Layout";
            this.btnReloadLayout.UseVisualStyleBackColor = true;
            this.btnReloadLayout.Click += new System.EventHandler(this.btnReloadLayout_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1558, 748);
            this.Controls.Add(this.btnReloadLayout);
            this.Controls.Add(this.cmdSplitReport);
            this.Controls.Add(this.cboCompression);
            this.Controls.Add(this.lblSplitTitle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboBackground);
            this.Controls.Add(this.radVisState);
            this.Controls.Add(this.radVisFinal);
            this.Controls.Add(this.radVisAudio);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtStartClock);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtFreeText);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCommentary);
            this.Controls.Add(this.btnChooseGame);
            this.Controls.Add(this.lblGameName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.cmdConnect);
            this.Controls.Add(this.cmdStartServer);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "RandoTracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button cmdConnect;
        private System.Windows.Forms.Button cmdStartServer;
        private System.Windows.Forms.Button btnChooseGame;
        private System.Windows.Forms.Label lblGameName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCommentary;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFreeText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtStartClock;
        private System.Windows.Forms.RadioButton radVisAudio;
        private System.Windows.Forms.RadioButton radVisFinal;
        private System.Windows.Forms.RadioButton radVisState;
        private System.Windows.Forms.ComboBox cboBackground;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSplitTitle;
        private System.Windows.Forms.ComboBox cboCompression;
        private System.Windows.Forms.Button cmdSplitReport;
        private System.Windows.Forms.Button btnReloadLayout;
    }
}

