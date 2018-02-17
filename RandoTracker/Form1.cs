using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.TextRenderer;

namespace RandoTracker
{
    public partial class Form1 : Form
    {
        int players = 2;
        int pics = 0;
        int neutralPics = 0;
        superText[] txtPlayer = new superText[4];
        superCheck[] radAudio = new superCheck[4];
        superText[] txtFinalTime = new superText[4];
        superCombo[] cboState = new superCombo[4];
        PictureBox comMic = new PictureBox();
        PictureBox audioMic = new PictureBox(); 
        picLabel[,] picCovers;
        superPic[,] pictures;
        PictureBox[] neutralPictures;
        picLabel[] NPicCovers;
        PictureBox logo = new PictureBox();
        PictureBox picClock = new PictureBox();
        Label[] finalTime;
        int playerFontSize = 0;
        int finalFontSize = 0;
        string gameFont = "";
        string gameFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "sml2.xml");
        Stopwatch clock = new Stopwatch();
        SimpleLabel[] lblPlayers = new SimpleLabel[4];
        SimpleLabel[] lblFinal = new SimpleLabel[4];
        SimpleLabel[] lblCom = new SimpleLabel[4];
        SimpleLabel[] lblFree = new SimpleLabel[4];
        Label lblCommentary = new Label();
        Label lblFreeText = new Label();
        Label[] lblSplitNames = new Label[4];
        Label[] lblSplitTimes = new Label[4];

        SimpleLabel lblClock2 = new SimpleLabel();
        Image mainImage;
        Image[] playerImages = new Image[4];
        int bgPics = 0;
        string[] bgImages;
        string[] bgNames;
        int xAdjustment = 10;
        int yAdjustment = 10;
        bool cboUpdate = true;

        string shortName = "";

        int extraTime = 0;

        private Socket m_sock;                      // Server connection
        private byte[] m_byBuff = new byte[256];    // Recieved data buffer
        private ArrayList m_aryClients = new ArrayList();	// List of Client Connections

        bool client = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.AutoScaleMode = AutoScaleMode.None;

            try
            {
                using (TextReader reader = File.OpenText("randoSettings.txt"))
                {
                    txtIP.Text = reader.ReadLine();
                    txtPort.Text = reader.ReadLine(); 

                    gameFile = reader.ReadLine();
                }
            }
            catch
            {
                // ignore error
            }

            this.Left = 200;
            this.Top = 200;

            picClock.Paint += picClock_Paint;
            picClock.BackColor = Color.Transparent;
            this.Controls.Add(picClock);
              
            for (int i = 0; i < 4; i++)
            {
                lblSplitNames[i] = new Label();
                lblSplitTimes[i] = new Label();

                txtPlayer[i] = new superText();
                radAudio[i] = new superCheck();
                if (i == 0) radAudio[i].Checked = true;

                txtFinalTime[i] = new superText();
                txtFinalTime[i].Text = "";
                txtFinalTime[i].Width = 50;

                cboState[i] = new superCombo();
                cboState[i].DropDownStyle = ComboBoxStyle.DropDownList;

                txtPlayer[i].player = radAudio[i].player = txtFinalTime[i].player = cboState[i].player = i;

                txtPlayer[i].Top = radAudio[i].Top = txtFinalTime[i].Top = cboState[i].Top = 75 + (25 * i);
                txtPlayer[i].Left = lblSplitNames[i].Left = 1305;
                txtPlayer[i].Width = 70;
                radAudio[i].Left = txtFinalTime[i].Left = cboState[i].Left = 1390;

                lblSplitTimes[i].Left = 1400;
                lblSplitNames[i].Top = lblSplitTimes[i].Top = 365 + (25 * i);
                lblSplitNames[i].AutoSize = false;
                lblSplitNames[i].Width = 80;
                lblSplitTimes[i].Text = lblSplitNames[i].Text = "";
                lblSplitNames[i].BackColor = lblSplitTimes[i].BackColor = Color.Transparent;
                lblSplitNames[i].ForeColor = lblSplitTimes[i].ForeColor = Color.White;

                txtPlayer[i].Leave += playerChange;
                radAudio[i].Click += audioChange;
                txtFinalTime[i].Leave += finalTimeChange;
                cboState[i].SelectedIndexChanged += stateChange;

                this.Controls.Add(txtPlayer[i]);
                this.Controls.Add(radAudio[i]);
                this.Controls.Add(txtFinalTime[i]);
                this.Controls.Add(cboState[i]);
                this.Controls.Add(lblSplitNames[i]);
                this.Controls.Add(lblSplitTimes[i]);
            }

            audioMic.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "speaker.png"));
            audioMic.SizeMode = PictureBoxSizeMode.StretchImage;
            audioMic.BackColor = Color.Transparent;
            audioMic.Height = 40;
            audioMic.Width = 40;

            comMic.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "mic2.png"));
            comMic.SizeMode = PictureBoxSizeMode.StretchImage;
            comMic.BackColor = Color.Transparent;
            comMic.Height = 40;
            comMic.Width = 40;

            this.Controls.Add(comMic);
            this.Controls.Add(audioMic);

            radVisAudio.CheckedChanged += visibleStateChanged;
            radVisFinal.CheckedChanged += visibleStateChanged;
            radVisState.CheckedChanged += visibleStateChanged;

            radVisAudio.Checked = true;
            radVisFinal.Checked = false;
            radVisState.Checked = false;

            loadGame();
        }

        private void playerChange(object sender, EventArgs e)
        {
            superText txtPlayer = (superText)sender;
            int player = txtPlayer.player;

            if (client == true)
                sendBytes((byte)(0xf8 + player), Encoding.UTF8.GetBytes(txtPlayer.Text));
            else
            {
                playerChange2(player, txtPlayer.Text);
                serverSendBytes((byte)(0xf8 + player), Encoding.UTF8.GetBytes(txtPlayer.Text));
            }
        }

        private void audioChange(object sender, EventArgs e)
        {
            superCheck chkPlayer = (superCheck)sender;
            int player = chkPlayer.player;

            if (client == true)
                sendBytes(new byte[] { (byte)(0xe0 + player) });
            else
            {
                audioChange2(player);
                serverSendBytes(new byte[] { (byte)(0xe0 + player) });
            }
        }

        private void finalTimeChange(object sender, EventArgs e)
        {
            superText txtPlayer = (superText)sender;
            int player = txtPlayer.player;

            if (client == true)
                sendBytes((byte)(0xe4 + player), Encoding.UTF8.GetBytes(txtPlayer.Text));
            else
            {
                finalTimeChange2(player, txtPlayer.Text);
                serverSendBytes((byte)(0xe4 + player), Encoding.UTF8.GetBytes(txtPlayer.Text));
            }
        }

        private void stateChange(object sender, EventArgs e)
        {
            superCombo txtPlayer = (superCombo)sender;
            int player = txtPlayer.player;

            if (client == true)
                sendBytes((byte)(0xe4 + player), Encoding.UTF8.GetBytes(txtPlayer.Text));
            else
            {
                stateChange2(player, txtPlayer.SelectedIndex);
                serverSendBytes(new byte[] { (byte)(0xe8 + player), (byte)txtPlayer.SelectedIndex });
            }
        }

        private void playerChange2(int player, string playerName)
        {
            if (lblPlayers[player] == null) return;
            txtPlayer[player].Text = playerName;
            lblPlayers[player].Text = playerName;
            lblSplitNames[player].Text = playerName;
            this.Invalidate();
        }

        private void audioChange2(int player)
        {
            for (int i = 0; i < 4; i++)
                radAudio[i].Checked = (i == player);

            audioMic.Left = radAudio[player].X + xAdjustment;
            audioMic.Top = radAudio[player].Y + yAdjustment;
        }

        private void finalTimeChange2(int player, string finalTime)
        {
            if (lblFinal[player] == null) return;
            lblFinal[player].Text = finalTime;
            this.Invalidate();
        }

        private void stateChange2(int player, int state)
        {
            // This is where the challenge starts...
        }

        private void visibleStateChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                radAudio[i].Visible = radVisAudio.Checked;
                txtFinalTime[i].Visible = radVisFinal.Checked;
                cboState[i].Visible = radVisState.Checked;
            }
        }

        private void loadGame(byte[] gameBytes)
        {
            string startDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string gameName = (Encoding.UTF8.GetString(gameBytes)).Replace("\0", "") + ".XML";
            gameFile = Path.Combine(startDir, gameName);
            loadGame();
        }

        private void loadGame()
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    lblPlayers[i].Text = "";
                    lblFinal[i].Text = "";
                    for (int j = 0; j < pics; j++)
                    {
                        this.Controls.Remove(pictures[i, j]);
                        this.Controls.Remove(picCovers[i, j]);
                    }
                }
            } catch { }

            try
            {
                for (int i = 0; i < neutralPics; i++)
                {
                    this.Controls.Remove(neutralPictures[i]);
                    this.Controls.Remove(NPicCovers[i]);
                }
            } catch { }
            try
            {
                this.Controls.Remove(logo);
            } catch { }

            XDocument gameXML = new XDocument();
            try
            {
                gameXML = XDocument.Load(gameFile);
            } catch (Exception ex)
            {
                var asdf = 1234;
            }
            XElement game = gameXML.Root.Element("game");

            lblGameName.Text = "Game:  " + gameXML.Element("game").Attribute("name").Value;
            shortName = gameXML.Element("game").Attribute("shortname").Value;
            players = Convert.ToInt32(gameXML.Element("game").Attribute("players").Value);
            if (players != 4) players = 2;
            txtPlayer[2].Enabled = txtPlayer[3].Enabled = txtFinalTime[2].Enabled = txtFinalTime[3].Enabled = radAudio[2].Enabled = radAudio[3].Enabled = cboState[2].Enabled = cboState[3].Enabled = (players == 4);

            gameFont = gameXML.Element("game").Attribute("Font").Value;
            pics = gameXML.Descendants("picture").Count();
            neutralPics = gameXML.Descendants("neutralPic").Count();

            if (gameXML.Descendants("mic").First().Attribute("visible").Value == "false")
                lblCommentary.Visible = false;
            else
                lblCommentary.Visible = true;

            playerFontSize = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("fontSize").Value);
            finalFontSize = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalFont").Value);

            Font playerFont = new Font(gameFont, playerFontSize);
            Font finalFont = new Font(gameFont, finalFontSize);
            int playerWidth = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("width").Value);
            int playerHeight = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("height").Value); // audioMic.Width = audioMic.Height = comMic.Width = comMic.Height = 
            try
            {
                audioMic.Height = audioMic.Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("speakerHeight").Value);
                comMic.Height = comMic.Width = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("micHeight").Value);
            }
            catch
            {
                audioMic.Height = audioMic.Width = 32;
                comMic.Height = comMic.Width = 32;
            }

            for (int i = 0; i < players; i++)
            {
                lblPlayers[i] = new SimpleLabel();
                lblPlayers[i].Font = playerFont;
                lblPlayers[i].X = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("locX").Value) + xAdjustment;
                lblPlayers[i].Y = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("locY").Value) + yAdjustment;
                lblPlayers[i].Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("width").Value);
                lblPlayers[i].Height = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("height").Value);
                try
                {
                    lblPlayers[i].hasBG = gameXML.Descendants("players").First().Attribute("background").Value.ToLower() == "true";
                }
                catch
                {
                    lblPlayers[i].hasBG = false;
                }
                lblPlayers[i].ForeColor = Color.White;
                lblPlayers[i].ShadowColor = Color.Black;
                lblPlayers[i].VerticalAlignment = StringAlignment.Center;
                try
                {
                    string align = gameXML.Descendants("player").Skip(i).First().Attribute("align").Value.ToLower();
                    lblPlayers[i].HorizontalAlignment = (align == "center" ? StringAlignment.Center : align == "left" ? StringAlignment.Near : StringAlignment.Far);
                } catch
                {
                    lblPlayers[i].HorizontalAlignment = (i % 2 == 0 ? StringAlignment.Near : StringAlignment.Far);
                }
                lblPlayers[i].Text = txtPlayer[i].Text;

                lblFinal[i] = new SimpleLabel();
                lblFinal[i].Font = finalFont;
                lblFinal[i].X = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("finalX").Value) + xAdjustment;
                lblFinal[i].Y = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("finalY").Value) + yAdjustment;
                lblFinal[i].Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalWidth").Value);
                lblFinal[i].Height = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalHeight").Value);
                lblFinal[i].ForeColor = Color.LightGreen;
                lblFinal[i].ShadowColor = Color.Blue;
                lblFinal[i].hasBG = true;
                lblFinal[i].BackColor = Color.FromArgb(192, 0, 0, 0);
                lblFinal[i].HorizontalAlignment = StringAlignment.Center;
                lblFinal[i].VerticalAlignment = StringAlignment.Center;
                lblFinal[i].Text = txtFinalTime[i].Text;

                try
                {
                    radAudio[i].X = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("audioX").Value) + xAdjustment;
                    radAudio[i].Y = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("audioY").Value) + yAdjustment;
                    if (radAudio[i].Checked)
                    {
                        audioMic.Left = radAudio[i].X + xAdjustment;
                        audioMic.Top = radAudio[i].Y + yAdjustment;
                    }

                } catch (Exception ex)
                {
                    radAudio[i].X = -100;
                    radAudio[i].Y = -100;
                    audioMic.Left = -100;
                    audioMic.Top = -100;
                    //MessageBox.Show("AudioX - " + ex.Message);
                }
            }

            lblClock2.Text = ":00.0";
            lblClock2.Font = new Font(gameFont, Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("fontSize").Value));
            lblClock2.IsMonospaced = true;
            lblClock2.ForeColor = Color.White;
            lblClock2.ShadowColor = Color.Black;
            lblClock2.Y = 5;
            lblClock2.X = 5;
            lblClock2.Height = 30;
            lblClock2.HasShadow = true;
            lblClock2.Width = 180;
            lblClock2.HorizontalAlignment = StringAlignment.Far;

            picClock.Left = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("locX").Value) + xAdjustment;
            picClock.Top = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("locY").Value) + yAdjustment;
            picClock.Width = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("width").Value) + 10;
            lblClock2.Width = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("width").Value);
            picClock.Height = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("height").Value) + 10;
            lblClock2.Height = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("height").Value);

            try
            {
                lblCommentary.Font = new Font(gameFont, Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("fontSize").Value));
                lblCommentary.Left = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("locX").Value) + 50 + xAdjustment;
                lblCommentary.Top = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("locY").Value) + yAdjustment;
                lblCommentary.Width = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("width").Value) - 50;
                lblCommentary.Height = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("height").Value);
                lblCommentary.TextAlign = ContentAlignment.MiddleLeft;
                comMic.Left = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("locX").Value) + xAdjustment;
                comMic.Top = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("locY").Value) + yAdjustment;
            }
            catch
            {
                lblCommentary.Left = -1000;
                lblCommentary.Top = -1000;
                lblCommentary.Width = 1;
                lblCommentary.Height = 1;
            }

            try
            {
                lblFreeText.Font = new Font(gameFont, Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("fontSize").Value));
                lblFreeText.Left = Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("locX").Value) + xAdjustment;
                lblFreeText.Top = Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("locY").Value) + yAdjustment;
                lblFreeText.Width = Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("width").Value);
                lblFreeText.Height = Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("height").Value);
                lblFreeText.TextAlign = ContentAlignment.MiddleLeft;
            } catch
            {
                lblFreeText.Left = -1000;
                lblFreeText.Top = -1000;
                lblFreeText.Width = 1;
                lblFreeText.Height = 1;
            }

            picCovers = new picLabel[players, pics];
            pictures = new superPic[players, pics];
            finalTime = new Label[players];
            neutralPictures = new PictureBox[neutralPics];
            NPicCovers = new picLabel[neutralPics];

            bgPics = gameXML.Descendants("background").Count();
            if (bgPics > 0)
            {
                bgImages = new string[bgPics];
                bgNames = new string[bgPics];
                cboBackground.Items.Clear();
                for (int i = 0; i < bgPics; i++)
                {
                    bgImages[i] = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("background").Skip(i).First().Attribute("file").Value.Replace("/", "\\"));
                    bgNames[i] = gameXML.Descendants("background").Skip(i).First().Attribute("name").Value;
                    cboBackground.Items.Add(bgNames[i]);
                }
                cboBackground.SelectedIndex = 0;
                cboBackground.Enabled = true;
                mainImage = Image.FromFile(bgImages[0]);
            }
            else
            {
                cboBackground.Enabled = false;
                string bgImage = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("game").First().Attribute("background").Value.Replace("/", "\\"));
                mainImage = Image.FromFile(bgImage);
            }

            int picXGap = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("xGap").Value);
            int picYGap = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("yGap").Value);
            int picXSize = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("xSize").Value);
            int picYSize = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("ySize").Value);
            int xNumber = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("xNumber").Value);
            int finalWidth = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalWidth").Value);
            int finalHeight = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalHeight").Value);

            for (int i = 0; i < players; i++)
            {
                int picX = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("picX").Value);
                int picY = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("picY").Value);

                for (int j = 0; j < pics; j++)
                {
                    pictures[i, j] = new superPic();

                    string firstPicture = "";
                    int numberOfPics = -1;
                    if (gameXML.Descendants("picture").Skip(j).First().Attribute("src") == null)
                    {
                        firstPicture = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("picture").Skip(j).First().Descendants("state").First().Attribute("src").Value.Replace("/", "\\"));
                        numberOfPics = gameXML.Descendants("picture").Skip(j).First().Descendants("state").Count();
                    }
                    else
                    {
                        firstPicture = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("picture").Skip(j).First().Attribute("src").Value.Replace("/", "\\"));
                    }
                    pictures[i, j].Image = Image.FromFile(firstPicture);

                    pictures[i, j].Left = picX + (picXGap * (j % xNumber)) + xAdjustment;
                    pictures[i, j].Top = picY + (picYGap * (j / xNumber)) + yAdjustment;

                    pictures[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictures[i, j].Width = picXSize;
                    pictures[i, j].Height = picYSize;
                    pictures[i, j].BackColor = Color.Transparent;

                    pictures[i, j].Invalidate();

                    this.Controls.Add(pictures[i, j]);

                    picCovers[i, j] = new picLabel();
                    picCovers[i, j].loadPictures(gameXML.Descendants("picture").Skip(j).First());

                    picCovers[i, j].Parent = pictures[i, j];
                    picCovers[i, j].BackColor = Color.FromArgb(numberOfPics == -1 ? 192 : 0, Color.Black);
                    picCovers[i, j].Left = 0; // picX + (picXGap * (j % xNumber));
                    picCovers[i, j].Top = 0; // picY + (picYGap * (j / xNumber));
                    picCovers[i, j].Width = picXSize;
                    picCovers[i, j].Height = picYSize;
                    picCovers[i, j].Click += new EventHandler(picClick);
                    picCovers[i, j].playerNumber = i;
                    picCovers[i, j].labelNumber = j;

                    pictures[i, j].Controls.Add(picCovers[i, j]);
                    picCovers[i, j].BringToFront();
                }
            }

            if (gameXML.Descendants("neutralPics").Count() > 0)
            {
                int picX = Convert.ToInt32(gameXML.Descendants("neutralPics").First().Attribute("locX").Value);
                int picY = Convert.ToInt32(gameXML.Descendants("neutralPics").First().Attribute("locY").Value);

                for (int i = 0; i < neutralPics; i++)
                {
                    string firstNeutralPic = "";
                    int neutralPics = -1;
                    if (gameXML.Descendants("neutralPic").Skip(i).First().Attribute("src") == null)
                    {
                        firstNeutralPic = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("neutralPic").Skip(i).First().Descendants("state").First().Attribute("src").Value.Replace("/", "\\"));
                        neutralPics = gameXML.Descendants("neutralPic").Skip(i).First().Descendants("state").Count();
                    }
                    else
                    {
                        firstNeutralPic = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("neutralPic").Skip(i).First().Attribute("src").Value.Replace("/", "\\"));
                    }

                    neutralPictures[i] = new PictureBox();
                    neutralPictures[i].Image = Image.FromFile(firstNeutralPic);

                    neutralPictures[i].Left = picX + (picXGap * (i % xNumber)) + xAdjustment;
                    neutralPictures[i].Top = picY + (picYGap * (i / xNumber)) + yAdjustment;

                    neutralPictures[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    neutralPictures[i].Width = picXSize;
                    neutralPictures[i].Height = picYSize;
                    neutralPictures[i].BackColor = Color.Transparent;

                    neutralPictures[i].Invalidate();

                    this.Controls.Add(neutralPictures[i]);

                    NPicCovers[i] = new picLabel();
                    NPicCovers[i].loadPictures(gameXML.Descendants("neutralPic").Skip(i).First());

                    NPicCovers[i].Parent = neutralPictures[i];
                    NPicCovers[i].BackColor = Color.Transparent;
                    NPicCovers[i].Left = 0;
                    NPicCovers[i].Top = 0;
                    NPicCovers[i].Width = picXSize;
                    NPicCovers[i].Height = picYSize;
                    NPicCovers[i].Click += new EventHandler(picClick);
                    NPicCovers[i].playerNumber = 5;
                    NPicCovers[i].labelNumber = i;

                    neutralPictures[i].Controls.Add(NPicCovers[i]);
                    NPicCovers[i].BringToFront();
                    neutralPictures[i].Controls.Add(NPicCovers[i]);
                }
            }

            try
            {
                string logoName = gameXML.Descendants("logo").First().Attribute("file").Value;
                logo = new PictureBox();
                logo.Image = Image.FromFile(logoName);

                logo.Left = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("locX").Value);
                logo.Top = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("locY").Value);

                logo.SizeMode = PictureBoxSizeMode.StretchImage;
                logo.Width = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("width").Value);
                logo.Height = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("height").Value);
                logo.BackColor = Color.Transparent;

                logo.Invalidate();

                this.Controls.Add(logo);
            }
            catch
            {
                // Ignore; no logo
            }
            comChange();
            freeTextChange();
        }

        private void picClick(object sender, EventArgs e)
        {
            picLabel clicked = (picLabel)sender;

            MouseEventArgs me = (MouseEventArgs)e;
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                showTimes(clicked);
            }
            else // Proceed as normal
            {
                if (client == true)
                    sendBytes(new byte[] { (byte)clicked.playerNumber, (byte)clicked.labelNumber });
                else
                {
                    changePicture(clicked.playerNumber, clicked.labelNumber);
                    serverSendBytes(new byte[] { (byte)clicked.playerNumber, (byte)clicked.labelNumber });
                }
            }
        }

        private void showTimes(picLabel clicked)
        {
            // Show times for that state on the right
            lblSplitTitle.Text = "Splits - " + (clicked.multiState ? clicked.imageName[clicked.currentState] : "");
            for (int i = 0; i < players; i++)
            {
                TimeSpan ts = picCovers[i, clicked.labelNumber].elapsed[clicked.currentState];
                if (ts.TotalHours >= 1)
                    lblSplitTimes[i].Text = Math.Floor(ts.TotalHours) + ":" + Math.Floor((double)ts.Minutes).ToString("00") + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
                else
                    lblSplitTimes[i].Text = Math.Floor((double)ts.Minutes) + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
            }
        }

        private void changePicture(int playerNumber, int labelNumber)
        {
            if (playerNumber != 5)
            {
                picLabel clicked = picCovers[playerNumber, labelNumber];

                if (clicked.multiState)
                {
                    PictureBox picClicked = pictures[playerNumber, labelNumber];
                    picClicked.Image = clicked.nextImage(clock.Elapsed.Add(new TimeSpan(0, 0, extraTime)));
                }
                else
                {
                    if (clicked.BackColor == Color.FromArgb(0, Color.Black))
                    {
                        clicked.BackColor = Color.FromArgb(192, Color.Black);
                        picCovers[playerNumber, labelNumber].elapsed[0] = new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        clicked.BackColor = Color.FromArgb(0, Color.Black);
                        picCovers[playerNumber, labelNumber].elapsed[0] = clock.Elapsed.Add(new TimeSpan(0, 0, extraTime));
                    }
                }
                showTimes(clicked);
            } else
            {
                picLabel clicked = NPicCovers[labelNumber];

                PictureBox picClicked = neutralPictures[labelNumber];
                picClicked.Image = clicked.nextImage(clock.Elapsed.Add(new TimeSpan(0, 0, extraTime)));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter writer = File.CreateText("randoSettings.txt"))
            {
                writer.WriteLine(txtIP.Text);
                writer.WriteLine(txtPort.Text);
                writer.WriteLine(gameFile);
            }
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Close the socket if it is still open
                if (m_sock != null && m_sock.Connected)
                {
                    m_sock.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    m_sock.Close();
                }

                // Create the socket object
                m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Define the Server address and port
                IPEndPoint epServer = new IPEndPoint(IPAddress.Parse(txtIP.Text), Convert.ToInt32(txtPort.Text));

                // Connect to server non-Blocking method
                m_sock.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                m_sock.BeginConnect(epServer, onconnect, m_sock);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Server Connect failed!");
            }
        }

        // *****************************************************************************************************
        // *****************************************************************************************************
        // *****************************************************************************************************

        private void comChange()
        {
            string[] comLines = txtCommentary.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < 4; i++)
            {
                string comText = (i >= comLines.Length ? "" : comLines[i]);
                lblCom[i] = new SimpleLabel(comText, lblCommentary.Left, lblCommentary.Top + (i * lblCommentary.Height), lblCommentary.Font, new SolidBrush(Color.White), lblCommentary.Width, lblCommentary.Height);
                lblCom[i].HasShadow = true;
                lblCom[i].ShadowColor = Color.Black;
            }
            this.Invalidate();
        }

        private void freeTextChange()
        {
            string[] comLines = txtFreeText.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < 4; i++)
            {
                string comText = (i >= comLines.Length ? "" : comLines[i]);
                lblFree[i] = new SimpleLabel(comText, lblFreeText.Left, lblFreeText.Top + (i * lblFreeText.Height), lblFreeText.Font, new SolidBrush(Color.White), lblFreeText.Width, lblFreeText.Height);
                lblFree[i].HasShadow = true;
                lblFree[i].ShadowColor = Color.Black;
            }
            this.Invalidate();
        }

        private void txtCommentary_Leave(object sender, EventArgs e)
        {
            if (client == true)
                sendBytes(0xf5, Encoding.UTF8.GetBytes(txtCommentary.Text));
            else
            {
                comChange();
                serverSendBytes(0xf5, Encoding.UTF8.GetBytes(txtCommentary.Text));
            }
        }

        private void txtFreeText_Leave(object sender, EventArgs e)
        {
            if (client == true)
                sendBytes(0xf6, Encoding.UTF8.GetBytes(txtFreeText.Text));
            else
            {
                freeTextChange();
                serverSendBytes(0xf6, Encoding.UTF8.GetBytes(txtFreeText.Text));
            }
        }

        // *****************************************************************************************************
        // *****************************************************************************************************
        // *****************************************************************************************************

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = clock.Elapsed.Add(new TimeSpan(0, 0, extraTime));
            string timeElapsed;
            if (ts.TotalHours >= 1)
                timeElapsed = Math.Floor(ts.TotalHours) + ":" + Math.Floor((double)ts.Minutes).ToString("00") + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
            else if (ts.Minutes >= 1)
                timeElapsed = Math.Floor((double)ts.Minutes) + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
            else
                timeElapsed = ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
            lblClock2.Text = timeElapsed;
            picClock.Invalidate();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtStartClock.Enabled)
            {
                try
                {
                    string[] timeStart = txtStartClock.Text.Split(new[] { ":" }, StringSplitOptions.None);
                    if (timeStart.Length > 3 || timeStart.Length <= 0)
                        MessageBox.Show("Clock must be in a #:##:##, ##:##, or ## format.  Starting from 0:00:00.");
                    else if (timeStart.Length == 3)
                        extraTime = (Convert.ToInt32(timeStart[0]) * 3600) + (Convert.ToInt32(timeStart[1]) * 60) + Convert.ToInt32(timeStart[2]);
                    else if (timeStart.Length == 2)
                        extraTime = (Convert.ToInt32(timeStart[0]) * 60) + Convert.ToInt32(timeStart[1]);
                    else
                        extraTime = Convert.ToInt32(timeStart[0]);
                }
                catch
                {
                    MessageBox.Show("Clock is in an invalid format.  It must be in a #:##:##, ##:##, or ## format.  Starting from 0:00:00.");
                }
            }

            byte extraTime1 = (byte)(extraTime % 256);
            byte extraTime2 = (byte)(extraTime / 256);

            if (client == true)
                sendBytes(new byte[] { 0xf1, extraTime1, extraTime2 });
            else
            {
                startClocks();
                serverSendBytes(new byte[] { 0xf1, extraTime1, extraTime2 });
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (client == true)
                sendBytes(new byte[] { 0xf2 });
            else
            {
                stopClocks();
                serverSendBytes(new byte[] { 0xf2 });
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "DualSplit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (client == true)
                    sendBytes(new byte[] { 0xf3 });
                else
                {
                    resetClocks();
                    serverSendBytes(new byte[] { 0xf3 });
                }
            }
        }

        private void startClocks()
        {
            txtStartClock.Enabled = false;
            clock.Start();
            timer1.Enabled = true;
        }

        private void stopClocks()
        {
            clock.Stop();
            timer1.Enabled = false;
        }

        private void resetClocks()
        {
            txtStartClock.Enabled = true;
            clock.Reset();
            timer1.Enabled = false;
            lblClock2.Text = ":00.0";
            picClock.Invalidate();
        }

        // *****************************************************************************************************
        // *****************************************************************************************************
        // *****************************************************************************************************

        private void cmdStartServer_Click(object sender, EventArgs e)
        {
            IPAddress[] aryLocalAddr = null;
            String strHostName = "";
            try
            {
                // NOTE: DNS lookups are nice and all but quite time consuming.
                strHostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                aryLocalAddr = ipEntry.AddressList;
            }
            catch (Exception ex)
            {
                listBox1.Items.Insert(0, "Error trying to get local address " + ex.Message);
            }

            // Verify we got an IP address. Tell the user if we did
            if (aryLocalAddr == null || aryLocalAddr.Length < 1)
            {
                listBox1.Items.Insert(0, "Unable to get local address");
                return;
            }

            // Create the listener socket in this machines IP address
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress actual = null;
            int lnI = -1;
            while (actual == null || actual.AddressFamily != AddressFamily.InterNetwork)
            {
                lnI++;
                actual = aryLocalAddr[lnI];
            }
            listener.Bind(new IPEndPoint(aryLocalAddr[lnI], Convert.ToInt32(txtPort.Text)));
            listener.Listen(10);

            // Setup a callback to be notified of connection requests
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
            listBox1.Items.Insert(0, "Accepting connections");
        }

        /// <summary>
        /// Callback used when a client requests a connection. 
        /// Accpet the connection, adding it to our list and setup to 
        /// accept more connections.
        /// </summary>
        /// <param name="ar"></param>
        public void OnConnectRequest(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            NewConnection(listener.EndAccept(ar));
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
        }

        /// <summary>
        /// Add the given connection to our list of clients
        /// Note we have a new friend
        /// Send a welcome to the new client
        /// Setup a callback to recieve data
        /// </summary>
        /// <param name="sockClient">Connection to keep</param>
        public void NewConnection(Socket sockClient)
        {
            // Program blocks on Accept() until a client connects.
            SocketChatClient client = new SocketChatClient(sockClient);
            m_aryClients.Add(client);
            this.BeginInvoke(new MethodInvoker(delegate
            {
                listBox1.Items.Insert(0, "Client " + client.Sock.RemoteEndPoint + " joined");
            }));

            List<byte> array = Encoding.ASCII.GetBytes(shortName).ToList();
            array.Insert(0, 0xf4);
            byte[] gameArray = array.ToArray();
            client.Sock.Send(gameArray, gameArray.Length, 0);

            client.SetupRecieveCallback(this);
        }

        /// <summary>
        /// Get the new data and send it out to all other connections. 
        /// Note: If not data was recieved the connection has probably 
        /// died.
        /// </summary>
        /// <param name="ar"></param>
        public void OnRecievedDataServer(IAsyncResult ar)
        {
            SocketChatClient client = (SocketChatClient)ar.AsyncState;
            byte[] aryRet = client.GetRecievedData(ar);

            // If no data was recieved then the connection is probably dead
            if (aryRet.Length < 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        listBox1.Items.Insert(0, "Client " + client.Sock.RemoteEndPoint + " disconnected");
                    }
                    catch
                    {

                    }
                }));
                client.Sock.Close();
                m_aryClients.Remove(client);
                return;
            }

            // Do something with the received data
            this.BeginInvoke(new MethodInvoker(delegate
            {
                listBox1.Items.Insert(0, "Server Data Get:  " + aryRet[0]);
                cleanListBox();

                if (aryRet[0] == 0xf1)
                {
                    extraTime = aryRet[1] + (aryRet[2] * 256);
                    txtStartClock.Text = Math.Floor((double)extraTime / 3600) + ":" + Math.Floor((double)(extraTime / 60) % 60).ToString("00") + ":" + Math.Floor((double)extraTime % 60).ToString("00");
                    startClocks();
                }
                else if (aryRet[0] == 0xf2) stopClocks();
                else if (aryRet[0] == 0xf3) resetClocks();
                else if (aryRet[0] == 0xf4)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    loadGame(byteArray.ToArray());
                }
                else if (aryRet[0] == 0xf5)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    txtCommentary.Text = Encoding.UTF8.GetString(byteArray.ToArray());
                    comChange();
                }
                else if (aryRet[0] == 0xf6)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    txtFreeText.Text = Encoding.UTF8.GetString(byteArray.ToArray());
                    freeTextChange();
                }
                else if (aryRet[0] == 0xf8)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    playerChange2(0, Encoding.UTF8.GetString(byteArray.ToArray()));
                }
                else if (aryRet[0] == 0xf9)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    playerChange2(1, Encoding.UTF8.GetString(byteArray.ToArray()));
                }
                else if (aryRet[0] == 0xfa)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    playerChange2(2, Encoding.UTF8.GetString(byteArray.ToArray()));
                }
                else if (aryRet[0] == 0xfb)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    playerChange2(3, Encoding.UTF8.GetString(byteArray.ToArray()));
                }
                else if (aryRet[0] <= 0x05)
                    changePicture(aryRet[0], aryRet[1]);
                else if (aryRet[0] == 0x0f)
                    newBackground(aryRet[1]);
            }));

            //Send the recieved data to all clients(including sender for echo)
            foreach (SocketChatClient clientSend in m_aryClients)
            {
                try
                {
                    clientSend.Sock.Send(aryRet);
                }
                catch
                {
                    // If the send fails the close the connection
                    Console.WriteLine("Send to client {0} failed", client.Sock.RemoteEndPoint);
                    clientSend.Sock.Close();
                    m_aryClients.Remove(client);
                    return;
                }
            }
            client.SetupRecieveCallback(this);
        }

        // **************************************************************************************

        public void OnConnect(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we were sucessfull
            try
            {
                //sock.EndConnect( ar );
                if (sock.Connected)
                    SetupRecieveCallback(sock);
                else
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        MessageBox.Show(this, "Unable to connect to remote machine", "Connect Failed!");
                    }));
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(this, ex.Message, "Unusual error during Connect!");
                }));
            }
        }

        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        public void SetupRecieveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, sock);
                //this.BeginInvoke(new MethodInvoker(delegate
                //{
                //    listBox1.Items.Insert(0, "Connection successful");
                //}));
                client = true;
            }
            catch (Exception ex)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(this, ex.Message, "Setup Recieve Callback failed!");
                }));
            }
        }

        /// <summary>
        /// Get the new data and send it out to all other connections. 
        /// Note: If not data was recieved the connection has probably 
        /// died.
        /// </summary>
        /// <param name="ar"></param>
        public void OnRecievedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Do something with the data passed.

                    // Wrote the data to the List
                    string sRecieved = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);

                    // If in server mode, receive commands from client... 01 = Start clock, 02 = Reset Clock, 03 = Split A, 04 = Split B, 05 = Reverse A, 06 = Reverse B, 10 = Split A -1 sec, 11 = Split A +1 sec, 12 = Split A -10 sec, 13 = Split A +10 sec,
                    // 20 = Split B -1 sec, 21 = Split B +1 sec, 22 = Split B -10 sec, 23 = Split B +10 sec, 30 = Clock A +1 sec, 31 = Clock A -1 sec, 32 = Clock B +1 sec, 33 = Clock B -1 sec
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        listBox1.Items.Insert(0, "Client Data Get:  " + m_byBuff[0]);
                        cleanListBox();

                        if (m_byBuff[0] == 0xf1)
                        {
                            extraTime = m_byBuff[1] + (m_byBuff[2] * 256);
                            txtStartClock.Text = Math.Floor((double)extraTime / 3600) + ":" + Math.Floor((double)(extraTime / 60) % 60).ToString("00") + ":" + Math.Floor((double)extraTime % 60).ToString("00");
                            startClocks();
                        }
                        else if (m_byBuff[0] == 0xf2) stopClocks();
                        else if (m_byBuff[0] == 0xf3) resetClocks();
                        else if (m_byBuff[0] == 0xf4)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            loadGame(byteArray.ToArray());
                        }
                        else if (m_byBuff[0] == 0xf5)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            txtCommentary.Text = Encoding.UTF8.GetString(byteArray.ToArray());
                            comChange();
                        }
                        else if (m_byBuff[0] == 0xf6)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            txtFreeText.Text = Encoding.UTF8.GetString(byteArray.ToArray());
                            freeTextChange();
                        }
                        else if (m_byBuff[0] == 0xf8)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            playerChange2(0, Encoding.UTF8.GetString(byteArray.ToArray()));
                        }
                        else if (m_byBuff[0] == 0xf9)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            playerChange2(1, Encoding.UTF8.GetString(byteArray.ToArray()));
                        }
                        else if (m_byBuff[0] == 0xfa)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            playerChange2(2, Encoding.UTF8.GetString(byteArray.ToArray()));
                        }
                        else if (m_byBuff[0] == 0xfb)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            playerChange2(3, Encoding.UTF8.GetString(byteArray.ToArray()));
                        }
                        else if (m_byBuff[0] <= 0x05)
                            changePicture(m_byBuff[0], m_byBuff[1]);
                        else if (m_byBuff[0] == 0x0f)
                            newBackground(m_byBuff[1]);
                    }));

                    // If the connection is still usable restablish the callback
                    SetupRecieveCallback(sock);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        listBox1.Items.Insert(0, "Client " + sock.RemoteEndPoint + ", disconnected");
                    }));

                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(this, ex.Message, "Unusual error during Recieve!");
                }));
            }
        }

        private void sendBytes(byte initial, byte[] detail)
        {
            List<byte> byteArray = detail.ToList();
            byteArray.Insert(0, initial);
            for (int i = byteArray.Count; i < 190; i++)
                byteArray.Add(0x00);
            sendBytes(byteArray.ToArray());
        }

        /// <summary>
        /// Send the Message in the Message area. Only do this if we are connected
        /// </summary>
        private void sendBytes(byte[] bytesToSend)
        {
            // Check we are connected
            if (m_sock == null || !m_sock.Connected)
            {
                MessageBox.Show(this, "Must be connected to Send a message");
                return;
            }

            // Read the message from the text box and send it
            try
            {
                // Convert to byte array and send.
                m_sock.Send(bytesToSend, bytesToSend.Length, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Send Message Failed!");
            }
        }

        private void serverSendBytes(byte initial, byte[] detail)
        {
            List<byte> byteArray = detail.ToList();
            byteArray.Insert(0, initial);
            for (int i = byteArray.Count; i < 190; i++)
                byteArray.Add(0x00);
            serverSendBytes(byteArray.ToArray());
        }

        private void serverSendBytes(byte[] bytesToSend)
        {
            //Send the recieved data to all clients(including sender for echo)
            foreach (SocketChatClient clientSend in m_aryClients)
            {
                try
                {
                    clientSend.Sock.Send(bytesToSend);
                }
                catch
                {
                    // If the send fails the close the connection
                    Console.WriteLine("Send to client failed; closing client");
                    clientSend.Sock.Close();
                    m_aryClients.Remove(client);
                    return;
                }
            }
        }

        private void cleanListBox()
        {
            while (listBox1.Items.Count > 20)
                listBox1.Items.RemoveAt(20);
        }

        private void btnChooseGame_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gameFile = openFileDialog1.FileName;
                // Mandatory reset clock
                resetClocks();

                if (client == true)
                    sendBytes(0xf4, Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(gameFile)));
                else
                {
                    loadGame();
                    serverSendBytes(0xf4, Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(gameFile)));
                }

                //loadGame();
            }
        }

        private void picClock_Paint(object sender, PaintEventArgs e)
        {
            lblClock2.Draw(e.Graphics);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(mainImage, new Rectangle(10, 10, 1280, 720));

            for (int i = 0; i < 4; i++)
            {
                if (lblPlayers[i] == null) continue;
                lblPlayers[i].Draw(e.Graphics);
            }
            for (int i = 0; i < 4; i++)
            {
                if (lblCom[i] == null) continue;
                lblCom[i].Draw(e.Graphics);
            }
            for (int i = 0; i < 4; i++)
            {
                if (lblFree[i] == null) continue;
                lblFree[i].Draw(e.Graphics);
            }

            for (int i = 0; i < 4; i++)
            {
                if (lblFinal[i] == null || lblFinal[i].Text == "") continue;
                lblFinal[i].Draw(e.Graphics);
            }
        }

        private void newBackground(int background, int player = 0)
        {
            if (player == 0)
            {
                mainImage = Image.FromFile(bgImages[background]);
                cboUpdate = false;
                cboBackground.SelectedIndex = background;
                cboUpdate = true;
            }
            else
                playerImages[player] = Image.FromFile(bgImages[background]);
            this.Invalidate();
        }

        private void cboBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboUpdate == false)
                return;
            if (client == true)
                sendBytes(new byte[] { 0x0f, (byte)cboBackground.SelectedIndex });
            else
            {
                newBackground(cboBackground.SelectedIndex);
                serverSendBytes(new byte[] { 0x0f, (byte)cboBackground.SelectedIndex });
            }

            mainImage = Image.FromFile(bgImages[cboBackground.SelectedIndex]);
            this.Invalidate();
        }
    }

    /// <summary>
    /// Class holding information and buffers for the Client socket connection
    /// </summary>
    internal class SocketChatClient
    {
        private Socket m_sock;                      // Connection to the client
        private byte[] m_byBuff = new byte[200];    // Receive data buffer
                                                    /// <summary>
                                                    /// Constructor
                                                    /// </summary>
                                                    /// <param name="sock">client socket conneciton this object represents</param>
        public SocketChatClient(Socket sock)
        {
            m_sock = sock;
        }

        // Readonly access
        public Socket Sock
        {
            get { return m_sock; }
        }

        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        /// <param name="app"></param>
        public void SetupRecieveCallback(Form1 app)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(app.OnRecievedDataServer);
                m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Data has been recieved so we shall put it in an array and
        /// return it.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Array of bytes containing the received data</returns>
        public byte[] GetRecievedData(IAsyncResult ar)
        {
            int nBytesRec = 0;
            try
            {
                nBytesRec = m_sock.EndReceive(ar);
            }
            catch { }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(m_byBuff, byReturn, nBytesRec);

            return byReturn;
        }
    }

    public class picLabel : Label
    {
        public int playerNumber = 0;
        public int labelNumber = 0;
        public bool multiState = false;
        private Image[] images;
        public string[] imageName;
        public TimeSpan[] elapsed;
        private int numberOfStates = -1;
        public int currentState = 0;

        public void loadPictures(XElement xPic)
        {
            numberOfStates = xPic.Descendants("state").Count();
            if (numberOfStates > 0)
            {
                images = new Image[numberOfStates];
                elapsed = new TimeSpan[numberOfStates];
                imageName = new string[numberOfStates];
                int lnI = 0;
                foreach (XElement pic in xPic.Descendants("state"))
                {
                    string fileName = pic.Attribute("src").Value.Replace("/", "\\");
                    images[lnI] = Image.FromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), fileName));
                    elapsed[lnI] = new TimeSpan();
                    imageName[lnI] = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    lnI++;
                }
                multiState = true;
            } else
            {
                elapsed = new TimeSpan[1];
                elapsed[0] = new TimeSpan();
                multiState = false;
            }
        }

        public Image nextImage(TimeSpan clock)
        {
            currentState++;
            if (currentState == numberOfStates) currentState = 0;
            elapsed[currentState] = clock;
            return images[currentState];
        }
    }

    public class SimpleLabel
    {
        public string Text { get; set; }
        public ICollection<string> AlternateText { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Font Font { get; set; }
        public Brush Brush { get; set; }
        public Pen Pen { get; set; }
        public StringAlignment HorizontalAlignment { get; set; }
        public StringAlignment VerticalAlignment { get; set; }
        public Color ShadowColor { get; set; }
        public Color OutlineColor { get; set; }
        public Color BackColor { get; set; }

        public bool hasBG { get; set; }
        public bool HasShadow { get; set; }
        public bool IsMonospaced { get; set; }

        private StringFormat Format { get; set; }

        public float ActualWidth { get; set; }

        public Color ForeColor
        {
            get
            {
                return ((SolidBrush)Brush).Color;
            }
            set
            {
                try
                {
                    if (Brush is SolidBrush)
                    {
                        ((SolidBrush)Brush).Color = value;
                    }
                    else
                    {
                        Brush = new SolidBrush(value);
                    }
                    Pen = new Pen(Brush);
                }
                catch (Exception ex)
                {
                    //Log.Error(ex);
                }
            }
        }

        public SimpleLabel(
            string text = "",
            float x = 0.0f, float y = 0.0f,
            Font font = null, Brush brush = null,
            float width = float.MaxValue, float height = float.MaxValue,
            StringAlignment horizontalAlignment = StringAlignment.Near,
            StringAlignment verticalAlignment = StringAlignment.Near,
            IEnumerable<string> alternateText = null)
        {
            Text = text;
            X = x;
            Y = y;
            Font = font ?? new Font("Arial", 1.0f);
            Brush = brush ?? new SolidBrush(Color.Black);
            Width = width;
            Height = height;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            IsMonospaced = false;
            HasShadow = true;
            ShadowColor = Color.FromArgb(128, 0, 0, 0);
            OutlineColor = Color.FromArgb(0, 0, 0, 0);
            hasBG = false;
            BackColor = Color.FromArgb(192, 0, 0, 0);
            ((List<string>)(AlternateText = new List<string>())).AddRange(alternateText ?? new string[0]);
            Format = new StringFormat
            {
                Alignment = HorizontalAlignment,
                LineAlignment = VerticalAlignment,
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisCharacter
            };
        }

        public void Draw(Graphics g)
        {
            Format.Alignment = HorizontalAlignment;
            Format.LineAlignment = VerticalAlignment;

            if (!IsMonospaced)
            {
                var actualText = CalculateAlternateText(g, Width);
                DrawText(actualText, g, X, Y, Width, Height, Format);
            }
            else
            {
                var monoFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = VerticalAlignment
                };

                var measurement = MeasureText(g, "0", Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
                var offset = Width;
                var charIndex = 0;
                SetActualWidth(g);
                var cutOffText = CutOff(g);

                offset = Width - MeasureActualWidth(cutOffText, g);
                if (HorizontalAlignment != StringAlignment.Far)
                    offset = 0f;


                while (charIndex < cutOffText.Length)
                {
                    var curOffset = 0f;
                    var curChar = cutOffText[charIndex];

                    if (char.IsDigit(curChar))
                        curOffset = measurement;
                    else
                        curOffset = MeasureText(g, curChar.ToString(), Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;

                    DrawText(curChar.ToString(), g, X + offset - curOffset / 2f, Y, curOffset * 2f, Height, monoFormat);

                    charIndex++;
                    offset += curOffset;
                }
            }
        }

        private void DrawText(string text, Graphics g, float x, float y, float width, float height, StringFormat format)
        {
            if (text != null)
            {
                if (g.TextRenderingHint == TextRenderingHint.AntiAlias && OutlineColor.A > 0)
                {
                    var fontSize = GetFontSize(g);
                    using (var shadowBrush = new SolidBrush(ShadowColor))
                    using (var gp = new GraphicsPath())
                    using (var outline = new Pen(OutlineColor, GetOutlineSize(fontSize)) { LineJoin = LineJoin.Round })
                    {
                        if (HasShadow)
                        {
                            gp.AddString(text, Font.FontFamily, (int)Font.Style, fontSize, new RectangleF(x + 1f, y + 1f, width, height), format);
                            g.FillPath(shadowBrush, gp);
                            gp.Reset();
                            gp.AddString(text, Font.FontFamily, (int)Font.Style, fontSize, new RectangleF(x + 2f, y + 2f, width, height), format);
                            g.FillPath(shadowBrush, gp);
                            gp.Reset();
                        }
                        gp.AddString(text, Font.FontFamily, (int)Font.Style, fontSize, new RectangleF(x, y, width, height), format);
                        g.DrawPath(outline, gp);
                        g.FillPath(Brush, gp);
                    }
                }
                else
                {
                    if (hasBG)
                    {
                        using (var backBrush = new SolidBrush(BackColor))
                        {
                            g.FillRectangle(backBrush, X, Y, Width, Height);
                            g.DrawRectangle(Pen, X + 2, Y + 2, width - 4, Height - 4);
                            g.DrawRectangle(Pen, X + 1, Y + 1, width - 2, Height - 2);
                        }
                    }
                    if (HasShadow)
                    {
                        using (var shadowBrush = new SolidBrush(ShadowColor))
                        {
                            g.DrawString(text, Font, shadowBrush, new RectangleF(x + 1f, y + 1f, width, height), format);
                            g.DrawString(text, Font, shadowBrush, new RectangleF(x + 2f, y + 2f, width, height), format);
                        }
                    }
                    g.DrawString(text, Font, Brush, new RectangleF(x, y, width, height), format);
                }
            }
        }

        private float GetOutlineSize(float fontSize)
        {
            return 2.1f + fontSize * 0.055f;
        }

        private float GetFontSize(Graphics g)
        {
            if (Font.Unit == GraphicsUnit.Point)
                return Font.Size * g.DpiY / 72;
            return Font.Size;
        }

        public void SetActualWidth(Graphics g)
        {
            Format.Alignment = HorizontalAlignment;
            Format.LineAlignment = VerticalAlignment;

            if (!IsMonospaced)
                ActualWidth = g.MeasureString(Text, Font, 9999, Format).Width;
            else
                ActualWidth = MeasureActualWidth(Text, g);
        }

        public string CalculateAlternateText(Graphics g, float width)
        {
            var actualText = Text;
            ActualWidth = g.MeasureString(Text, Font, 9999, Format).Width;
            foreach (var curText in AlternateText.OrderByDescending(x => x.Length))
            {
                if (width < ActualWidth)
                {
                    actualText = curText;
                    ActualWidth = g.MeasureString(actualText, Font, 9999, Format).Width;
                }
                else
                {
                    break;
                }
            }
            return actualText;
        }

        private float MeasureActualWidth(string text, Graphics g)
        {
            var charIndex = 0;
            var measurement = MeasureText(g, "0", Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
            var offset = 0;

            while (charIndex < text.Length)
            {
                var curChar = text[charIndex];

                if (char.IsDigit(curChar))
                    offset += measurement;
                else
                    offset += MeasureText(g, curChar.ToString(), Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;

                charIndex++;
            }
            return offset;
        }

        private string CutOff(Graphics g)
        {
            if (ActualWidth < Width)
                return Text;
            var cutOffText = Text;
            while (ActualWidth >= Width && !string.IsNullOrEmpty(cutOffText))
            {
                cutOffText = cutOffText.Remove(cutOffText.Length - 1, 1);
                ActualWidth = MeasureActualWidth(cutOffText + "...", g);
            }
            if (ActualWidth >= Width)
                return "";
            return cutOffText + "...";
        }
    }

    public class superText : TextBox
    {
        public int player = 0;
    }

    public class superCombo: ComboBox
    {
        public int player = 0;
    }

    public class superCheck : CheckBox
    {
        public int player = 0;
        public int X = 0;
        public int Y = 0;
    }

    public class superPic : PictureBox
    {
        public TimeSpan elapsed = new TimeSpan();
    }
}
