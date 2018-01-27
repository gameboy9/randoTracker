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
            this.label3 = new System.Windows.Forms.Label();
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
            this.txtPlayerA = new System.Windows.Forms.TextBox();
            this.txtPlayerC = new System.Windows.Forms.TextBox();
            this.txtPlayerB = new System.Windows.Forms.TextBox();
            this.txtPlayerD = new System.Windows.Forms.TextBox();
            this.txtTimeD = new System.Windows.Forms.TextBox();
            this.txtTimeB = new System.Windows.Forms.TextBox();
            this.txtTimeC = new System.Windows.Forms.TextBox();
            this.txtTimeA = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAudioA = new System.Windows.Forms.RadioButton();
            this.btnAudioB = new System.Windows.Forms.RadioButton();
            this.btnAudioC = new System.Windows.Forms.RadioButton();
            this.btnAudioD = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFreeText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStartClock = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(1305, 598);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 92;
            this.label3.Text = "Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(1305, 573);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 91;
            this.label2.Text = "IP Address:";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(1372, 595);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(67, 20);
            this.txtPort.TabIndex = 2;
            // 
            // txtIP
            // 
            this.txtIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIP.Location = new System.Drawing.Point(1372, 570);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(135, 20);
            this.txtIP.TabIndex = 1;
            // 
            // cmdConnect
            // 
            this.cmdConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdConnect.Location = new System.Drawing.Point(1400, 621);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(87, 23);
            this.cmdConnect.TabIndex = 4;
            this.cmdConnect.Text = "Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // cmdStartServer
            // 
            this.cmdStartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdStartServer.Location = new System.Drawing.Point(1307, 621);
            this.cmdStartServer.Name = "cmdStartServer";
            this.cmdStartServer.Size = new System.Drawing.Size(87, 23);
            this.cmdStartServer.TabIndex = 3;
            this.cmdStartServer.Text = "Start Server";
            this.cmdStartServer.UseVisualStyleBackColor = true;
            this.cmdStartServer.Click += new System.EventHandler(this.cmdStartServer_Click);
            // 
            // btnChooseGame
            // 
            this.btnChooseGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseGame.Location = new System.Drawing.Point(1308, 22);
            this.btnChooseGame.Name = "btnChooseGame";
            this.btnChooseGame.Size = new System.Drawing.Size(87, 23);
            this.btnChooseGame.TabIndex = 20;
            this.btnChooseGame.Text = "Choose Game";
            this.btnChooseGame.UseVisualStyleBackColor = true;
            this.btnChooseGame.Click += new System.EventHandler(this.btnChooseGame_Click);
            // 
            // lblGameName
            // 
            this.lblGameName.BackColor = System.Drawing.Color.Transparent;
            this.lblGameName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameName.ForeColor = System.Drawing.Color.White;
            this.lblGameName.Location = new System.Drawing.Point(1401, 26);
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
            this.label1.Location = new System.Drawing.Point(1304, 200);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 103;
            this.label1.Text = "Commentary:";
            // 
            // txtCommentary
            // 
            this.txtCommentary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommentary.Location = new System.Drawing.Point(1378, 188);
            this.txtCommentary.Multiline = true;
            this.txtCommentary.Name = "txtCommentary";
            this.txtCommentary.Size = new System.Drawing.Size(164, 40);
            this.txtCommentary.TabIndex = 21;
            this.txtCommentary.Leave += new System.EventHandler(this.txtCommentary_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(1312, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Players";
            // 
            // txtPlayerA
            // 
            this.txtPlayerA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayerA.Location = new System.Drawing.Point(1315, 75);
            this.txtPlayerA.Name = "txtPlayerA";
            this.txtPlayerA.Size = new System.Drawing.Size(67, 20);
            this.txtPlayerA.TabIndex = 5;
            this.txtPlayerA.Leave += new System.EventHandler(this.txtPlayerA_Leave);
            // 
            // txtPlayerC
            // 
            this.txtPlayerC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayerC.Location = new System.Drawing.Point(1315, 126);
            this.txtPlayerC.Name = "txtPlayerC";
            this.txtPlayerC.Size = new System.Drawing.Size(67, 20);
            this.txtPlayerC.TabIndex = 7;
            this.txtPlayerC.Leave += new System.EventHandler(this.txtPlayerC_Leave);
            // 
            // txtPlayerB
            // 
            this.txtPlayerB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayerB.Location = new System.Drawing.Point(1315, 101);
            this.txtPlayerB.Name = "txtPlayerB";
            this.txtPlayerB.Size = new System.Drawing.Size(67, 20);
            this.txtPlayerB.TabIndex = 6;
            this.txtPlayerB.Leave += new System.EventHandler(this.txtPlayerB_Leave);
            // 
            // txtPlayerD
            // 
            this.txtPlayerD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayerD.Location = new System.Drawing.Point(1315, 152);
            this.txtPlayerD.Name = "txtPlayerD";
            this.txtPlayerD.Size = new System.Drawing.Size(67, 20);
            this.txtPlayerD.TabIndex = 8;
            this.txtPlayerD.Leave += new System.EventHandler(this.txtPlayerD_Leave);
            // 
            // txtTimeD
            // 
            this.txtTimeD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeD.Location = new System.Drawing.Point(1455, 152);
            this.txtTimeD.Name = "txtTimeD";
            this.txtTimeD.Size = new System.Drawing.Size(67, 20);
            this.txtTimeD.TabIndex = 16;
            this.txtTimeD.Leave += new System.EventHandler(this.txtTimeD_Leave);
            // 
            // txtTimeB
            // 
            this.txtTimeB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeB.Location = new System.Drawing.Point(1455, 101);
            this.txtTimeB.Name = "txtTimeB";
            this.txtTimeB.Size = new System.Drawing.Size(67, 20);
            this.txtTimeB.TabIndex = 14;
            this.txtTimeB.Leave += new System.EventHandler(this.txtTimeB_Leave);
            // 
            // txtTimeC
            // 
            this.txtTimeC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeC.Location = new System.Drawing.Point(1455, 126);
            this.txtTimeC.Name = "txtTimeC";
            this.txtTimeC.Size = new System.Drawing.Size(67, 20);
            this.txtTimeC.TabIndex = 15;
            this.txtTimeC.Leave += new System.EventHandler(this.txtTimeC_Leave);
            // 
            // txtTimeA
            // 
            this.txtTimeA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeA.Location = new System.Drawing.Point(1455, 75);
            this.txtTimeA.Name = "txtTimeA";
            this.txtTimeA.Size = new System.Drawing.Size(67, 20);
            this.txtTimeA.TabIndex = 13;
            this.txtTimeA.Leave += new System.EventHandler(this.txtTimeA_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(1452, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 112;
            this.label5.Text = "Final Time";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1398, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 117;
            this.label6.Text = "Audio";
            // 
            // btnAudioA
            // 
            this.btnAudioA.AutoSize = true;
            this.btnAudioA.Location = new System.Drawing.Point(1408, 79);
            this.btnAudioA.Name = "btnAudioA";
            this.btnAudioA.Size = new System.Drawing.Size(14, 13);
            this.btnAudioA.TabIndex = 9;
            this.btnAudioA.TabStop = true;
            this.btnAudioA.UseVisualStyleBackColor = true;
            // 
            // btnAudioB
            // 
            this.btnAudioB.AutoSize = true;
            this.btnAudioB.Location = new System.Drawing.Point(1408, 104);
            this.btnAudioB.Name = "btnAudioB";
            this.btnAudioB.Size = new System.Drawing.Size(14, 13);
            this.btnAudioB.TabIndex = 10;
            this.btnAudioB.TabStop = true;
            this.btnAudioB.UseVisualStyleBackColor = true;
            // 
            // btnAudioC
            // 
            this.btnAudioC.AutoSize = true;
            this.btnAudioC.Location = new System.Drawing.Point(1408, 129);
            this.btnAudioC.Name = "btnAudioC";
            this.btnAudioC.Size = new System.Drawing.Size(14, 13);
            this.btnAudioC.TabIndex = 11;
            this.btnAudioC.TabStop = true;
            this.btnAudioC.UseVisualStyleBackColor = true;
            // 
            // btnAudioD
            // 
            this.btnAudioD.AutoSize = true;
            this.btnAudioD.Location = new System.Drawing.Point(1408, 155);
            this.btnAudioD.Name = "btnAudioD";
            this.btnAudioD.Size = new System.Drawing.Size(14, 13);
            this.btnAudioD.TabIndex = 12;
            this.btnAudioD.TabStop = true;
            this.btnAudioD.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(1308, 427);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 23);
            this.btnStart.TabIndex = 17;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(1396, 427);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(64, 23);
            this.btnStop.TabIndex = 18;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(1482, 427);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(64, 23);
            this.btnReset.TabIndex = 19;
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
            this.listBox1.Location = new System.Drawing.Point(1308, 648);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(235, 56);
            this.listBox1.TabIndex = 118;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Navy;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(10, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1280, 720);
            this.pictureBox1.TabIndex = 119;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(1304, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 121;
            this.label7.Text = "Free Text";
            // 
            // txtFreeText
            // 
            this.txtFreeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFreeText.Location = new System.Drawing.Point(1378, 240);
            this.txtFreeText.Multiline = true;
            this.txtFreeText.Name = "txtFreeText";
            this.txtFreeText.Size = new System.Drawing.Size(164, 52);
            this.txtFreeText.TabIndex = 120;
            this.txtFreeText.WordWrap = false;
            this.txtFreeText.Leave += new System.EventHandler(this.txtFreeText_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(1305, 400);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 123;
            this.label8.Text = "Start clock at ";
            // 
            // txtStartClock
            // 
            this.txtStartClock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStartClock.Location = new System.Drawing.Point(1396, 397);
            this.txtStartClock.Name = "txtStartClock";
            this.txtStartClock.Size = new System.Drawing.Size(69, 20);
            this.txtStartClock.TabIndex = 122;
            this.txtStartClock.Text = "0:00:00";
            this.txtStartClock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1558, 748);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtStartClock);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtFreeText);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnAudioD);
            this.Controls.Add(this.btnAudioC);
            this.Controls.Add(this.btnAudioB);
            this.Controls.Add(this.btnAudioA);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTimeD);
            this.Controls.Add(this.txtTimeB);
            this.Controls.Add(this.txtTimeC);
            this.Controls.Add(this.txtTimeA);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPlayerD);
            this.Controls.Add(this.txtPlayerB);
            this.Controls.Add(this.txtPlayerC);
            this.Controls.Add(this.txtPlayerA);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCommentary);
            this.Controls.Add(this.btnChooseGame);
            this.Controls.Add(this.lblGameName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.cmdConnect);
            this.Controls.Add(this.cmdStartServer);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "RandoTracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
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
        private System.Windows.Forms.TextBox txtPlayerA;
        private System.Windows.Forms.TextBox txtPlayerC;
        private System.Windows.Forms.TextBox txtPlayerB;
        private System.Windows.Forms.TextBox txtPlayerD;
        private System.Windows.Forms.TextBox txtTimeD;
        private System.Windows.Forms.TextBox txtTimeB;
        private System.Windows.Forms.TextBox txtTimeC;
        private System.Windows.Forms.TextBox txtTimeA;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton btnAudioA;
        private System.Windows.Forms.RadioButton btnAudioB;
        private System.Windows.Forms.RadioButton btnAudioC;
        private System.Windows.Forms.RadioButton btnAudioD;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFreeText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtStartClock;
    }
}

