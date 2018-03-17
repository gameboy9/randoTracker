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
        PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        FontFamily gameFontFamily;
        string gameFile = Path.Combine(GetExecutingDirectory(), "sml2.xml");
        Stopwatch clock = new Stopwatch();
        SimpleLabel[] lblPlayers = new SimpleLabel[4];
        SimpleLabel[] lblFinal = new SimpleLabel[4];
        SimpleLabel[] lblCom = new SimpleLabel[4];
        SimpleLabel[] lblFree = new SimpleLabel[4];
        SimpleLabel lblCommentary = new SimpleLabel();
        Label lblFreeText = new Label();
        Label[] lblSplitNames = new Label[4];
        Label[] lblSplitTimes = new Label[4];

        int sizeRestriction = 100;

        SimpleLabel lblClock2 = new SimpleLabel();
        Image mainImage;
        Image[] playerImages = new Image[4];
        int bgPics = 0;
        string[] bgImages;
        string[] bgNames;
        int xAdjustment = 10;
        int yAdjustment = 10;
        bool cboUpdate = true;
        
        int extraTime = 0;

        private Socket m_sock;                      // Server connection
        private byte[] m_byBuff = new byte[256];    // Recieved data buffer
        private ArrayList m_aryClients = new ArrayList();	// List of Client Connections

        bool client = false;

        string subLayoutName = string.Empty;

        const int LayoutXAdjust = 260;
        const int consoleXAdjust = 0;

        bool initialLoad = true;

        private static class SocketMessages
        {
            public const byte START_CLOCK = 0xf1;
            public const byte STOP_CLOCK = 0xf2;
            public const byte RESET_CLOCK = 0xf3;
            public const byte LOAD_LAYOUT = 0xf4;
        }

        public Form1()
        {
            InitializeComponent();
        }

        public static string GetExecutingDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.AutoScaleMode = AutoScaleMode.None;

            string[] lineArgs = Environment.GetCommandLineArgs();

            if (lineArgs.Count() > 1)
            {
                gameFile = Path.Combine(GetExecutingDirectory(), lineArgs[1]);
            }
            try
            {
                if (File.Exists("randoSettings.txt"))
                {
                    using (TextReader reader = File.OpenText("randoSettings.txt"))
                    {   
                        txtIP.Text = reader.ReadLine();
                        txtPort.Text = reader.ReadLine();

                        if (string.IsNullOrWhiteSpace(gameFile))
                        {
                            gameFile = reader.ReadLine();
                        }

                        cboCompression.SelectedIndex = Convert.ToInt32(reader.ReadLine());
                    }
                }
            }
            catch
            {
                // ignore error
            }

            if (string.IsNullOrWhiteSpace(gameFile))
            {
                // Load something at least
                gameFile = Path.Combine(GetExecutingDirectory(), "sml2.xml");
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

                txtPlayer[i].Top = radAudio[i].Top = txtFinalTime[i].Top = cboState[i].Top = 360 + (25 * i);
                txtPlayer[i].Left = lblSplitNames[i].Left = 5 + consoleXAdjust;
                txtPlayer[i].Width = 70;
                radAudio[i].Left = txtFinalTime[i].Left = cboState[i].Left = 85 + consoleXAdjust;

                lblSplitTimes[i].Left = 95 + consoleXAdjust;
                lblSplitNames[i].Top = lblSplitTimes[i].Top = 246 + (22 * i);
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

            audioMic.Image = Image.FromFile(Path.Combine(GetExecutingDirectory(), "speaker.png"));
            audioMic.SizeMode = PictureBoxSizeMode.StretchImage;
            audioMic.BackColor = Color.Transparent;
            audioMic.Height = 40;
            audioMic.Width = 40;

            comMic.Image = Image.FromFile(Path.Combine(GetExecutingDirectory(), "mic2.png"));
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

            initialLoad = false;
            loadFonts();

            try
            {
                loadGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {gameFile}. {ex.Message}", "Error Loading Game");
                gameFile = string.Empty;
                return;
            }

            if (cboCompression.SelectedIndex < 0) cboCompression.SelectedIndex = 0;
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

            audioMic.Left = radAudio[player].X + xAdjustment + LayoutXAdjust;
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


        private void cboCompression_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadGame();
        }

        private void loadGame(string game)
        {
            string startDir = GetExecutingDirectory();
            string gameName = game + ".XML";
            gameFile = Path.Combine(startDir, gameName);
            loadGame();
        }

        private void loadFonts()
        {
            if (privateFontCollection.Families.Any())
            {
                // We already did load our fonts, so we're good.
                return;
            }

            foreach (var file in Directory.GetFiles(Path.Combine(Path.Combine(GetExecutingDirectory(), "fonts"))))
            {
                privateFontCollection.AddFontFile(file);
            }
        }

        private FontFamily loadFont(string fontName)
        {
            FontFamily fontFamily = privateFontCollection.Families.FirstOrDefault(f => string.Equals(f.Name, fontName, StringComparison.OrdinalIgnoreCase));

            if (fontFamily != null)
            {
                return fontFamily;
            }

            // Let's see if they have it installed
            try
            {
                fontFamily = new FontFamily(fontName);
                return fontFamily;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load font {fontName}.  Defaulting to Arial");
                return gameFontFamily = new FontFamily("Arial");
            }
        }

        private void loadGame()
        {
            if (initialLoad)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(gameFile))
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (lblPlayers[i] != null)
                {
                    lblPlayers[i].Text = "";
                    lblPlayers[i].hasBG = false;
                }
                if (lblFinal[i] != null)
                    lblFinal[i].Text = "";

                if (pictures == null || pictures.GetUpperBound(1) < 0 || pictures.GetUpperBound(0) < i || pictures[i, 0] == null) continue;
                for (int j = 0; j < pics; j++)
                {
                    this.Controls.Remove(pictures[i, j]);
                    this.Controls.Remove(picCovers[i, j]);
                }
            }

            try
            {
                if (neutralPictures != null)
                {
                    for (int i = 0; i < neutralPictures.Count(); i++)
                    {
                        Controls.Remove(neutralPictures[i]);
                        Controls.Remove(NPicCovers[i]);
                    }
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
            
            XElement game = gameXML.Element("game");

            if (game == null)
            {
                MessageBox.Show($"Unable to find root game tag for {gameFile}", "Required Tag");
                return;
            }

            lblGameName.Text = "Game:  " + game.Attribute("name")?.Value;
            players = Convert.ToInt32(game.Attribute("players")?.Value);

            if (players != 4)
            {
                players = 2;
            }

            txtPlayer[2].Enabled = txtPlayer[3].Enabled = txtFinalTime[2].Enabled = txtFinalTime[3].Enabled = radAudio[2].Enabled = radAudio[3].Enabled = cboState[2].Enabled = cboState[3].Enabled = (players == 4);
            
            string gameFont = game.Attribute("Font").Value;

            gameFontFamily = loadFont(gameFont);

            XElement micElement = gameXML.Descendants("mic").FirstOrDefault();

            if (micElement.Attribute("visible").Value == "false")
            {
                lblCommentary.Visible = false;
                comMic.Visible = false;
            }
            else
            {
                lblCommentary.Visible = true;
                comMic.Visible = true;
            }

            playerFontSize = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("fontSize").Value) * sizeRestriction / 100;
            finalFontSize = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalFont").Value) * sizeRestriction / 100;
            
            Font playerFont = new Font(gameFontFamily, playerFontSize);
            Font finalFont = new Font(gameFontFamily, finalFontSize);
            int playerWidth = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("width").Value);
            int playerHeight = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("height").Value); // audioMic.Width = audioMic.Height = comMic.Width = comMic.Height = 

            if (gameXML.Descendants("players").First().Attribute("speakerHeight") != null)
                audioMic.Height = audioMic.Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("speakerHeight").Value) * sizeRestriction / 100;
            else
                audioMic.Height = audioMic.Width = 32 * sizeRestriction / 100;

            if (micElement.Attribute("micHeight") != null)
                comMic.Height = comMic.Width = Convert.ToInt32(micElement.Attribute("micHeight").Value) * sizeRestriction / 100;
            else
                comMic.Height = comMic.Width = 32 * sizeRestriction / 100;

            // Load pictures
            var pictureElements = gameXML.Descendants("pictures");
            XElement pictureElement = null;

            cboSublayout.Visible = false;
            lblSublayout.Visible = false;
            cboSublayout.Items.Clear();

            if (pictureElements.Any())
            {
                if (pictureElements.Count() == 1)
                {
                    pictureElement = pictureElements.FirstOrDefault();
                }
                else
                {
                    cboSublayout.Visible = true;
                    lblSublayout.Visible = true;

                    foreach (var picture in pictureElements)
                    {
                        cboSublayout.Items.Add(picture.Attribute("name")?.Value ?? string.Empty);
                    }

                    if (string.IsNullOrWhiteSpace(subLayoutName) == false)
                    {
                        cboSublayout.SelectedItem = subLayoutName;
                    }
                    else
                    {
                        cboSublayout.SelectedIndex = 0;
                    }

                    // If one is selected, load that.  Otherwise load first
                    pictureElement = pictureElements.Skip(cboSublayout.SelectedIndex).First();
                }
            }
            
            List<XElement> neutralPicsElements = gameXML.Descendants("neutralPics").ToList();

            if (cboSublayout.Visible)
            {
                neutralPicsElements = neutralPicsElements.Where(el => el.Attribute("name")?.Value == (string) cboSublayout.SelectedItem).ToList();
            }
            
            int finalNeutral = 0;
            for (int i = 0; i < neutralPicsElements.Count(); i++)
            {
                int repeat = 1;
                try
                {
                    repeat = Convert.ToInt32(neutralPicsElements.Descendants("neutralPic").Skip(i).First().Attribute("repeat").Value);
                }
                catch
                {
                    // Do nothing and do one repeat.
                }
                finalNeutral += repeat;
            }
            neutralPics = finalNeutral;
            
            if (pictureElement != null)
            {
                pics = pictureElement.Descendants("picture").Count();
            }
            else
            {
                pics = 0;
            }

            int xNumber = pictureElement == null ? 0 : Convert.ToInt32(pictureElement.Attribute("xNumber")?.Value);
            int picXGap = pictureElement == null ? 0 : Convert.ToInt32(pictureElement.Attribute("xGap")?.Value);
            int picYGap = pictureElement == null ? 0 : Convert.ToInt32(pictureElement.Attribute("yGap")?.Value);
            int picXSize = pictureElement == null ? 0 : Convert.ToInt32(pictureElement.Attribute("xSize")?.Value);
            int picYSize = pictureElement == null ? 0 : Convert.ToInt32(pictureElement.Attribute("ySize")?.Value);
            int adjustedXGap = picXGap + picXSize;
            int adjustedYGap = picYGap + picYSize;

            if (cboCompression.SelectedIndex == 0)
            {
                Width = 1574;
                Height = 786;

                int optimalSize = 1574 - LayoutXAdjust;
                int actualSize = optimalSize - (1574 - Width);

                sizeRestriction = Math.Min(actualSize * 100 / optimalSize, Height * 100 / 786);
            }
            else if (cboCompression.SelectedIndex == 1)
            {
                // Ignore for now.
                //this.Width = Math.Max(LayoutXAdjust + 30 + (((xNumber * 2) + 2) * adjustedXGap), picClock.Left + picClock.Width + 20);
                //this.Width = Math.Max(this.Width, (int)lblPlayers[1].X + (int)lblPlayers[1].Width + 20);
                //this.Height = 786;
            }
            else
            {
                Width = LayoutXAdjust + 30;
                Height = 344;
            }

            for (int i = 0; i < players; i++)
            {
                lblPlayers[i] = new SimpleLabel();
                lblPlayers[i].Font = playerFont;

                if (cboCompression.SelectedIndex == 0)
                {
                    lblPlayers[i].X = (Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("locX").Value) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                    lblPlayers[i].Y = (Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("locY").Value) * sizeRestriction / 100) + yAdjustment;
                }
                else if (cboCompression.SelectedIndex == 1)
                {
                    lblPlayers[i].X = 10 + (i % 2 == 1 ? (xNumber + 2) * adjustedXGap : 0) + LayoutXAdjust;
                    lblPlayers[i].Y = (i / 2 == 1 ? 300 : 10);
                }
                else
                {
                    lblPlayers[i].X = -1000;
                    lblPlayers[i].Y = -1000;
                }

                lblPlayers[i].Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("width").Value) * sizeRestriction / 100;
                lblPlayers[i].Height = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("height").Value) * sizeRestriction / 100;

                if (cboCompression.SelectedIndex == 0 && gameXML.Descendants("players").First().Attribute("background") != null)
                    lblPlayers[i].hasBG = (cboCompression.SelectedIndex == 0 && gameXML.Descendants("players").First().Attribute("background").Value.ToLower() == "true");
                else
                    lblPlayers[i].hasBG = false;

                lblPlayers[i].ForeColor = parseColor(gameXML.Descendants("players").First().Attribute("fontColor")?.Value, Color.White);
                lblPlayers[i].ShadowColor = parseColor(gameXML.Descendants("players").First().Attribute("fontShadowColor")?.Value, Color.Black);
                lblPlayers[i].VerticalAlignment = StringAlignment.Center;

                if (gameXML.Descendants("player").Skip(i).First().Attribute("align") != null)
                {
                    string align = gameXML.Descendants("player").Skip(i).First().Attribute("align").Value.ToLower();
                    lblPlayers[i].HorizontalAlignment = (align == "center" ? StringAlignment.Center : align == "left" ? StringAlignment.Near : StringAlignment.Far);
                } else
                    lblPlayers[i].HorizontalAlignment = (i % 2 == 0 ? StringAlignment.Near : StringAlignment.Far);

                lblPlayers[i].Text = txtPlayer[i].Text;

                lblFinal[i] = new SimpleLabel();
                lblFinal[i].Font = finalFont;
                if (cboCompression.SelectedIndex == 0)
                {
                    lblFinal[i].X = (Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("finalX").Value) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                    lblFinal[i].Y = (Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("finalY").Value) * sizeRestriction / 100) + yAdjustment;
                }
                else
                {
                    lblFinal[i].X = -1000;
                    lblFinal[i].Y = -1000;
                }
                lblFinal[i].Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalWidth").Value) * sizeRestriction / 100;
                lblFinal[i].Height = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalHeight").Value) * sizeRestriction / 100;
                lblFinal[i].ForeColor = parseColor(gameXML.Descendants("players").First().Attribute("finalFontColor")?.Value, Color.LightGreen);
                lblFinal[i].ShadowColor = parseColor(gameXML.Descendants("players").First().Attribute("finalFontShadowColor")?.Value, Color.Blue);
                lblFinal[i].hasBG = true;
                lblFinal[i].BackColor = Color.FromArgb(192, 0, 0, 0);
                lblFinal[i].HorizontalAlignment = StringAlignment.Center;
                lblFinal[i].VerticalAlignment = StringAlignment.Center;
                lblFinal[i].Text = txtFinalTime[i].Text;

                try
                {
                    if (cboCompression.SelectedIndex == 0)
                    {
                        radAudio[i].X = (Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("audioX").Value) * sizeRestriction / 100) + xAdjustment;
                        radAudio[i].Y = (Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("audioY").Value) * sizeRestriction / 100) + yAdjustment;
                        if (radAudio[i].Checked)
                        {
                            audioMic.Left = (radAudio[i].X * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                            audioMic.Top = (radAudio[i].Y * sizeRestriction / 100) + yAdjustment;
                        }
                    } else
                    {
                        radAudio[i].X = -100;
                        radAudio[i].Y = -100;
                        audioMic.Left = -100;
                        audioMic.Top = -100;
                    }

                } catch (Exception ex)
                {
                    radAudio[i].X = -100;
                    radAudio[i].Y = -100;
                    audioMic.Left = -100;
                    audioMic.Top = -100;
                }
            }

            if (cboCompression.SelectedIndex == 1)
            {
                this.Width = Math.Max(LayoutXAdjust + 30 + (((xNumber * 2) + 2) * adjustedXGap), picClock.Left + picClock.Width + 20);
                this.Width = Math.Max(this.Width, (int)lblPlayers[1].X + (int)lblPlayers[1].Width + 20);
                this.Height = 786;
            }

            lblClock2.Text = ":00.0";
            lblClock2.Font = new Font(gameFontFamily, Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("fontSize").Value) * sizeRestriction / 100);
            lblClock2.IsMonospaced = true;
            lblClock2.ForeColor = parseColor(gameXML.Descendants("clock").First().Attribute("fontColor")?.Value, Color.White);
            lblClock2.ShadowColor = parseColor(gameXML.Descendants("clock").First().Attribute("fontShadowColor")?.Value, Color.Black);
            lblClock2.Y = 5;
            lblClock2.X = 5;
            lblClock2.Height = 30;
            lblClock2.HasShadow = true;
            lblClock2.Width = 180;
            lblClock2.HorizontalAlignment = StringAlignment.Far;

            if (cboCompression.SelectedIndex == 0)
            {
                picClock.Left = (Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("locX").Value) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                picClock.Top = (Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("locY").Value) * sizeRestriction / 100) + yAdjustment;
            }
            else if (cboCompression.SelectedIndex == 1)
            {
                picClock.Left = LayoutXAdjust + 30;
                picClock.Top = 700;
            }
            else
            {
                picClock.Left = -1000;
                picClock.Top = -1000;
            }
            picClock.Width = (Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("width").Value) * sizeRestriction / 100) + 10;
            lblClock2.Width = (Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("width").Value) * sizeRestriction / 100);
            picClock.Height = (Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("height").Value) * sizeRestriction / 100) + 10;
            lblClock2.Height = (Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("height").Value) * sizeRestriction / 100);

            try
            {
                lblCommentary.Font = new Font(gameFontFamily, Convert.ToInt32(micElement.Attribute("fontSize").Value) * sizeRestriction / 100);
                if (cboCompression.SelectedIndex == 0)
                    lblCommentary.X = ((Convert.ToInt32(micElement.Attribute("locX").Value) + 50) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                else
                    lblCommentary.X = -1000;
                lblCommentary.Y = (Convert.ToInt32(micElement.Attribute("locY").Value) * sizeRestriction / 100) + yAdjustment;
                lblCommentary.Width = (Convert.ToInt32(micElement.Attribute("width").Value) - 50) * sizeRestriction / 100;
                lblCommentary.Height = (Convert.ToInt32(micElement.Attribute("height").Value)) * sizeRestriction / 100;
                lblCommentary.HorizontalAlignment = StringAlignment.Near;
                lblCommentary.VerticalAlignment = StringAlignment.Center;
                lblCommentary.ForeColor = parseColor(micElement.Attribute("fontColor")?.Value, Color.White);
                lblCommentary.ShadowColor = parseColor(micElement.Attribute("fontShadowColor")?.Value, Color.Black);

                if (cboCompression.SelectedIndex == 0)
                {
                    comMic.Left = (Convert.ToInt32(micElement.Attribute("locX").Value) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                    comMic.Top = (Convert.ToInt32(micElement.Attribute("locY").Value) * sizeRestriction / 100) + yAdjustment;
                } else
                {
                    comMic.Left = -1000;
                    comMic.Top = -1000;
                }
            }
            catch
            {
                lblCommentary.X = -1000;
                lblCommentary.Y = -1000;
                lblCommentary.Width = 1;
                lblCommentary.Height = 1;
                comMic.Left = -1000;
                comMic.Top = -1000;
            }

            if (gameXML.Descendants("freetext").Count() > 0)
            {
                lblFreeText.Font = new Font(gameFontFamily, Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("fontSize").Value) * sizeRestriction / 100);
                if (cboCompression.SelectedIndex == 0)
                    lblFreeText.Left = (Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("locX").Value) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                else
                    lblFreeText.Left = -1000;
                lblFreeText.Top = (Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("locY").Value) * sizeRestriction / 100) + yAdjustment;
                lblFreeText.Width = (Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("width").Value) * sizeRestriction / 100);
                lblFreeText.Height = (Convert.ToInt32(gameXML.Descendants("freetext").First().Attribute("height").Value) * sizeRestriction / 100);
                lblFreeText.TextAlign = ContentAlignment.MiddleLeft;
            } else
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
                    bgImages[i] = Path.Combine(GetExecutingDirectory(), gameXML.Descendants("background").Skip(i).First().Attribute("file").Value.Replace("/", "\\"));
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
                string bgImage = Path.Combine(GetExecutingDirectory(), gameXML.Descendants("game").First().Attribute("background").Value.Replace("/", "\\"));
                mainImage = Image.FromFile(bgImage);
            }

            for (int i = 0; i < players; i++)
            {
                int picX = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("picX")?.Value) * sizeRestriction / 100;
                int picY = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("picY")?.Value) * sizeRestriction / 100;

                for (int j = 0; j < pics; j++)
                {
                    var picture = pictureElement.Descendants("picture").Skip(j).First();
                    var mode = pictureElement.Attribute("layout")?.Value;

                    pictures[i, j] = new superPic();

                    if (string.IsNullOrWhiteSpace(mode))
                    {
                        mode = "grid";
                    }

                    string firstPicture = "";
                    int numberOfPics = -1;

                    if (picture.Attribute("src") == null)
                    {
                        firstPicture = Path.Combine(GetExecutingDirectory(), picture.Descendants("state").First().Attribute("src").Value.Replace("/", "\\"));
                        numberOfPics = picture.Descendants("state").Count();
                    }
                    else
                    {
                        firstPicture = Path.Combine(GetExecutingDirectory(), picture.Attribute("src").Value.Replace("/", "\\"));
                    }

                    pictures[i, j].Image = Image.FromFile(firstPicture);

                    pictures[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictures[i, j].BackColor = Color.Transparent;

                    picCovers[i, j] = new picLabel();
                    picCovers[i, j].loadPictures(picture);
                    picCovers[i, j].Parent = pictures[i, j];
                    picCovers[i, j].BackColor = Color.FromArgb(numberOfPics == -1 ? 192 : 0, Color.Black);
                    picCovers[i, j].Click += new EventHandler(picClick);
                    picCovers[i, j].DoubleClick += new EventHandler(picClick);
                    picCovers[i, j].MouseWheel += new MouseEventHandler(picWheel);
                    picCovers[i, j].MouseHover += new EventHandler(picHover);
                    picCovers[i, j].playerNumber = i;
                    picCovers[i, j].labelNumber = j;
                    picCovers[i, j].Left = 0; // picX + (adjustedXGap * (j % xNumber));
                    picCovers[i, j].Top = 0; // picY + (adjustedYGap * (j / xNumber));

                    if (mode == "freeform" && cboCompression.SelectedIndex == 0)
                    {
                        var location = new Point(picX, picY);
                        var size = new Point(picXSize, picYSize);
                        var pictureLocationX = picture.Attribute("locX");
                        var pictureLocationY = picture.Attribute("locY");
                        var pictureSizeX = picture.Attribute("xSize");
                        var pictureSizeY = picture.Attribute("ySize");

                        if (pictureLocationX != null)
                        {
                            location.X = Convert.ToInt32(pictureLocationX.Value);
                        }

                        if (pictureLocationY != null)
                        {
                            location.Y = Convert.ToInt32(pictureLocationY.Value);
                        }

                        if (pictureSizeX != null)
                        {
                            size.X = Convert.ToInt32(pictureSizeX.Value);
                        }

                        if (pictureSizeY != null)
                        {
                            size.Y = Convert.ToInt32(pictureSizeY.Value);
                        }

                        pictures[i, j].Left = (picX + location.X * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                        pictures[i, j].Top = (picY + location.Y * sizeRestriction / 100) + yAdjustment;
                        pictures[i, j].Width = size.X * sizeRestriction / 100;
                        pictures[i, j].Height = size.Y * sizeRestriction / 100;
                    }
                    else
                    {
                        if (cboCompression.SelectedIndex == 0)
                        {
                            pictures[i, j].Left = (picX + (adjustedXGap * (j % xNumber)) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                            pictures[i, j].Top = (picY + (adjustedYGap * (j / xNumber)) * sizeRestriction / 100) + yAdjustment;
                        }
                        else if (cboCompression.SelectedIndex == 1)
                        {
                            pictures[i, j].Left = 10 + (i % 2 == 1 ? (xNumber + 2) * adjustedXGap : 0) + (adjustedXGap * (j % xNumber)) + LayoutXAdjust;
                            pictures[i, j].Top = 10 + (i / 2 == 1 ? 335 : 35) + (adjustedYGap * (j / xNumber));
                        }
                        else
                        {
                            pictures[i, j].Left = -1000;
                            pictures[i, j].Top = -1000;
                        }

                        pictures[i, j].Width = picXSize * sizeRestriction / 100;
                        pictures[i, j].Height = picYSize * sizeRestriction / 100;
                    }

                    picCovers[i, j].Width = pictures[i, j].Width;
                    picCovers[i, j].Height = pictures[i, j].Height;
                    pictures[i, j].Invalidate();
                    Controls.Add(pictures[i, j]);
                    pictures[i, j].Controls.Add(picCovers[i, j]);
                    picCovers[i, j].BringToFront();
                }
            }

            int k = -1;
            foreach (XElement neutralPicsElement in neutralPicsElements)
            {
                xNumber = Convert.ToInt32(neutralPicsElement.Attribute("xNumber").Value);

                int picX = Convert.ToInt32(neutralPicsElement.Attribute("locX").Value);
                int picY = Convert.ToInt32(neutralPicsElement.Attribute("locY").Value);

                picXGap = Convert.ToInt32(neutralPicsElement.Attribute("xGap").Value);
                picYGap = Convert.ToInt32(neutralPicsElement.Attribute("yGap").Value);
                picXSize = Convert.ToInt32(neutralPicsElement.Attribute("xSize").Value);
                picYSize = Convert.ToInt32(neutralPicsElement.Attribute("ySize").Value);

                adjustedXGap = picXGap + picXSize;
                adjustedYGap = picYGap + picYSize;
                
                for (int i = 0; i < neutralPicsElement.Descendants("neutralPic").Count(); i++)
                {
                    int neutralPicIndex = -1;
                    int repeat = 1;

                    try
                    {
                        repeat = Convert.ToInt32(neutralPicsElement.Descendants("neutralPic").Skip(i).First().Attribute("repeat").Value);
                    } catch
                    { 
                        // Do nothing and do one repeat.
                    }

                    for (int j = 0; j < repeat; j++)
                    {
                        neutralPicIndex++;
                        k++;
                        string firstNeutralPic = "";
                        int neutralPics = -1;
                        if (neutralPicsElement.Descendants("neutralPic").Skip(i).First().Attribute("src") == null)
                        {
                            firstNeutralPic = Path.Combine(GetExecutingDirectory(), neutralPicsElement.Descendants("neutralPic").Skip(i).First().Descendants("state").First().Attribute("src").Value.Replace("/", "\\"));
                            neutralPics = neutralPicsElement.Descendants("neutralPic").Skip(i).First().Descendants("state").Count();
                        }
                        else
                        {
                            firstNeutralPic = Path.Combine(GetExecutingDirectory(), neutralPicsElement.Descendants("neutralPic").Skip(i).First().Attribute("src").Value.Replace("/", "\\"));
                        }

                        neutralPictures[k] = new PictureBox();
                        neutralPictures[k].Image = Image.FromFile(firstNeutralPic);

                        if (cboCompression.SelectedIndex == 0)
                        {
                            neutralPictures[k].Left = ((picX + (adjustedXGap * (neutralPicIndex % xNumber))) * sizeRestriction / 100) + xAdjustment + LayoutXAdjust;
                            neutralPictures[k].Top = ((picY + (adjustedYGap * (neutralPicIndex / xNumber))) * sizeRestriction / 100) + yAdjustment;
                        }
                        else if (cboCompression.SelectedIndex == 1)
                        {
                            neutralPictures[k].Left = 10 + (adjustedXGap * (neutralPicIndex % xNumber)) + LayoutXAdjust;
                            neutralPictures[k].Top = 610 + (adjustedYGap * (neutralPicIndex / xNumber));
                        }
                        else
                        {
                            neutralPictures[k].Left = -1000;
                            neutralPictures[k].Top = -1000;
                        }

                        neutralPictures[k].SizeMode = PictureBoxSizeMode.StretchImage;
                        neutralPictures[k].Width = picXSize * sizeRestriction / 100;
                        neutralPictures[k].Height = picYSize * sizeRestriction / 100;
                        neutralPictures[k].BackColor = Color.Transparent;

                        neutralPictures[k].Invalidate();

                        this.Controls.Add(neutralPictures[k]);

                        NPicCovers[k] = new picLabel();
                        NPicCovers[k].loadPictures(neutralPicsElement.Descendants("neutralPic").Skip(i).First());

                        NPicCovers[k].Parent = neutralPictures[k];
                        NPicCovers[k].BackColor = Color.Transparent;
                        NPicCovers[k].Left = 0;
                        NPicCovers[k].Top = 0;
                        NPicCovers[k].Width = picXSize * sizeRestriction / 100;
                        NPicCovers[k].Height = picYSize * sizeRestriction / 100;
                        NPicCovers[k].Click += new EventHandler(picClick);
                        NPicCovers[k].DoubleClick += new EventHandler(picClick);
                        NPicCovers[k].MouseWheel += new MouseEventHandler(picWheel);
                        NPicCovers[k].MouseHover += new EventHandler(picHover);
                        NPicCovers[k].playerNumber = 5;
                        NPicCovers[k].labelNumber = k;

                        neutralPictures[k].Controls.Add(NPicCovers[k]);
                        NPicCovers[k].BringToFront();
                        neutralPictures[k].Controls.Add(NPicCovers[k]);
                    }
                }
            }

            if (gameXML.Descendants("logo").Count() > 0)
            {
                string logoName = gameXML.Descendants("logo").First().Attribute("file").Value;
                logo = new PictureBox();
                logo.Image = Image.FromFile(logoName);

                if (cboCompression.SelectedIndex == 0)
                {
                    logo.Left = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("locX").Value) + LayoutXAdjust;
                    logo.Top = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("locY").Value);
                } else
                {
                    logo.Left = -1000;
                    logo.Top = -1000;
                }

                logo.SizeMode = PictureBoxSizeMode.StretchImage;
                logo.Width = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("width").Value);
                logo.Height = Convert.ToInt32(gameXML.Descendants("logo").First().Attribute("height").Value);
                logo.BackColor = Color.Transparent;

                logo.Invalidate();

                this.Controls.Add(logo);
            }
            comChange();
            freeTextChange();
        }

        private void CboSublayout_SelectionChangeCommitted(object sender, EventArgs e)
        {
            subLayoutName = cboSublayout.SelectedItem as string;
            reloadLayout();
        }

        private void picHover(object sender, EventArgs e)
        {
            picLabel clicked = (picLabel)sender;
            clicked.Focus();
        }
        
        private void picWheel(object sender, MouseEventArgs me)
        {
            picClick(sender, me);
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
                if (clicked.isClicked(me) == false)
                {
                    return;
                }

                if (client == true)
                {
                    sendBytes(new byte[] { (byte)(clicked.playerNumber + (me.Button == MouseButtons.Right ? 0x10 : 0)), (byte)clicked.labelNumber });
                }
                else
                {
                    changePicture(clicked.playerNumber, clicked.labelNumber, (me.Button == MouseButtons.Right));
                    serverSendBytes(new byte[] { (byte)(clicked.playerNumber + (me.Button == MouseButtons.Right ? 0x10 : 0)), (byte)clicked.labelNumber });
                }
            }
        }

        private void showTimes(picLabel clicked)
        {
            // Show times for that state on the right
            lblSplitTitle.Text = "Splits - " + clicked.imageName[clicked.currentState];
            for (int i = 0; i < players; i++)
            {
                TimeSpan ts = picCovers[i, clicked.labelNumber].elapsed[clicked.currentState];
                if (ts.TotalHours >= 1)
                    lblSplitTimes[i].Text = Math.Floor(ts.TotalHours) + ":" + Math.Floor((double)ts.Minutes).ToString("00") + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
                else
                    lblSplitTimes[i].Text = Math.Floor((double)ts.Minutes) + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
            }
        }

        private void cmdSplitReport_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("analysis.txt"))
            {
                if (players == 4)
                {
                    writer.WriteLine("Game".PadRight(30) + txtPlayer[0].Text.PadRight(20) + txtPlayer[1].Text.PadRight(20) + txtPlayer[2].Text.PadRight(20) + txtPlayer[3].Text.PadRight(20));
                    writer.WriteLine("--------------------------------------------------------------------------------------------------------------");
                }
                else
                {
                    writer.WriteLine("Game".PadRight(30) + txtPlayer[0].Text.PadRight(20) + txtPlayer[1].Text.PadRight(20));
                    writer.WriteLine("----------------------------------------------------------------------");
                }

                for (int j = 0; j < picCovers.GetLength(1); j++)
                {
                    for (int k = 0; k < picCovers[0, j].elapsed.Length; k++)
                    {
                        if (picCovers[0, j].elapsed.Length > 1 && k == 0)
                            continue;

                        string line = picCovers[0, j].imageName[k].PadRight(30);
                        for (int i = 0; i < players; i++)
                        {
                            TimeSpan ts = picCovers[i, j].elapsed[k];
                            line += (Math.Floor(ts.TotalHours) + ":" + Math.Floor((double)ts.Minutes).ToString("00") + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100).PadRight(20);
                        }

                        writer.WriteLine(line);
                    }
                    writer.WriteLine("");
                }
            }

            Process.Start("notepad.exe", "analysis.txt");
        }

        private void changePicture(int playerNumber, int labelNumber, bool backwards)
        {
            if (playerNumber != 5)
            {
                if (playerNumber >= picCovers.GetLength(0))
                {
                    MessageBox.Show($"Unable to change picture.  playerNumber is {playerNumber}, limit is {picCovers.GetLength(0)}", "Error");
                    return;
                }

                if (labelNumber >= picCovers.GetLength(1))
                {
                    MessageBox.Show($"Unable to change picture.  labelNumber is {labelNumber}, limit is {picCovers.GetLength(1)}", "Error");
                    return;
                }

                picLabel clicked = picCovers[playerNumber, labelNumber];

                if (clicked.multiState)
                {
                    PictureBox picClicked = pictures[playerNumber, labelNumber];
                    if (!backwards)
                        picClicked.Image = clicked.nextImage(clock.Elapsed.Add(new TimeSpan(0, 0, extraTime)));
                    else
                        picClicked.Image = clicked.prevImage(clock.Elapsed.Add(new TimeSpan(0, 0, extraTime)));
                }
                else
                {
                    if (clicked.BackColor == Color.FromArgb(0, Color.Black))
                    {
                        clicked.BackColor = Color.FromArgb(192, Color.Black);
                        clicked.elapsed[0] = new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        clicked.BackColor = Color.FromArgb(0, Color.Black);
                        clicked.elapsed[0] = clock.Elapsed.Add(new TimeSpan(0, 0, extraTime));
                    }
                }
                showTimes(clicked);
            }
            else
            {
                if (labelNumber >= NPicCovers.Length)
                {
                    MessageBox.Show($"Unable to change picture.  labelNumber is {labelNumber}, limit is {NPicCovers.Length}", "Error");
                    return;
                }

                picLabel clicked = NPicCovers[labelNumber];
                PictureBox picClicked = neutralPictures[labelNumber];

                if (!backwards)
                    picClicked.Image = clicked.nextImage(clock.Elapsed.Add(new TimeSpan(0, 0, extraTime)));
                else
                    picClicked.Image = clicked.prevImage(clock.Elapsed.Add(new TimeSpan(0, 0, extraTime)));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter writer = File.CreateText("randoSettings.txt"))
            {
                writer.WriteLine(txtIP.Text);
                writer.WriteLine(txtPort.Text);
                writer.WriteLine(gameFile);
                writer.WriteLine(cboCompression.SelectedIndex.ToString());
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
        
        private static Color parseColor(string colorValue, Color defaultColor)
        {
            if (string.IsNullOrWhiteSpace(colorValue))
            {
                return defaultColor;
            }

            try
            {
                return ColorTranslator.FromHtml(colorValue);
            }
            catch (Exception)
            {
                return defaultColor;
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
                lblCom[i] = new SimpleLabel(comText, lblCommentary.X, lblCommentary.Y + (i * lblCommentary.Height), lblCommentary.Font, new SolidBrush(lblCommentary.ForeColor), lblCommentary.Width, lblCommentary.Height);
                lblCom[i].HasShadow = true;
                lblCom[i].ShadowColor = lblCommentary.ShadowColor;
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
                sendBytes(new byte[] { SocketMessages.START_CLOCK, extraTime1, extraTime2 });
            else
            {
                startClocks();
                serverSendBytes(new byte[] { SocketMessages.START_CLOCK, extraTime1, extraTime2 });
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (client == true)
                sendBytes(new byte[] { SocketMessages.STOP_CLOCK });
            else
            {
                stopClocks();
                serverSendBytes(new byte[] { SocketMessages.STOP_CLOCK });
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "DualSplit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (client == true)
                    sendBytes(new byte[] { SocketMessages.RESET_CLOCK });
                else
                {
                    resetClocks();
                    serverSendBytes(new byte[] { SocketMessages.RESET_CLOCK });
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

        private void SendLoadLayout()
        {
            var data = Path.GetFileNameWithoutExtension(gameFile);

            if (string.IsNullOrWhiteSpace(subLayoutName) == false)
            {
                data += "|" + subLayoutName;
            }

            var bytes = Encoding.UTF8.GetBytes(data);

            if (client == true)
            {
                sendBytes(SocketMessages.LOAD_LAYOUT, bytes);
            }
            else
            {
                serverSendBytes(SocketMessages.LOAD_LAYOUT, bytes);
            }
        }

        private void ReceiveLoadLayout(byte[] bytes)
        {
            var data = Encoding.UTF8.GetString(bytes).Replace("\0", "");
            var splitData = data.Split('|');

            if (splitData.Length > 1)
            {
                subLayoutName = splitData[1];
            }
            else
            {
                subLayoutName = string.Empty;
            }

            loadGame(splitData[0]);
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

            // SendLoadLayout
            var data = Path.GetFileNameWithoutExtension(gameFile);

            if (string.IsNullOrWhiteSpace(subLayoutName) == false)
            {
                data += "|" + subLayoutName;
            }

            var array = Encoding.UTF8.GetBytes(data).ToList();
            
            array.Insert(0, SocketMessages.LOAD_LAYOUT); 
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

                if (aryRet[0] == SocketMessages.START_CLOCK)
                {
                    extraTime = aryRet[1] + (aryRet[2] * 256);
                    txtStartClock.Text = Math.Floor((double)extraTime / 3600) + ":" + Math.Floor((double)(extraTime / 60) % 60).ToString("00") + ":" + Math.Floor((double)extraTime % 60).ToString("00");
                    startClocks();
                }
                else if (aryRet[0] == SocketMessages.STOP_CLOCK) stopClocks();
                else if (aryRet[0] == SocketMessages.RESET_CLOCK) resetClocks();
                else if (aryRet[0] == SocketMessages.LOAD_LAYOUT)
                {
                    List<byte> byteArray = aryRet.ToList();
                    byteArray.RemoveAt(0);
                    ReceiveLoadLayout(byteArray.ToArray());
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
                    changePicture(aryRet[0], aryRet[1], false);
                else if (aryRet[0] <= 0x15 && aryRet[0] >= 0x10)
                    changePicture(aryRet[0] - 0x10, aryRet[1], true);
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

                        if (m_byBuff[0] == SocketMessages.START_CLOCK)
                        {
                            extraTime = m_byBuff[1] + (m_byBuff[2] * 256);
                            txtStartClock.Text = Math.Floor((double)extraTime / 3600) + ":" + Math.Floor((double)(extraTime / 60) % 60).ToString("00") + ":" + Math.Floor((double)extraTime % 60).ToString("00");
                            startClocks();
                        }
                        else if (m_byBuff[0] == SocketMessages.STOP_CLOCK) stopClocks();
                        else if (m_byBuff[0] == SocketMessages.RESET_CLOCK) resetClocks();
                        else if (m_byBuff[0] == SocketMessages.LOAD_LAYOUT)
                        {
                            List<byte> byteArray = m_byBuff.ToList();
                            byteArray.RemoveAt(0);
                            ReceiveLoadLayout(byteArray.ToArray());
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
                            changePicture(m_byBuff[0], m_byBuff[1], false);
                        else if (m_byBuff[0] <= 0x15 && m_byBuff[0] >= 0x10)
                            changePicture(m_byBuff[0] - 0x10, m_byBuff[1], true);
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

        private void reloadLayout(bool forceReload = false)
        {
            if (clock.IsRunning && forceReload == false)
            {
                var result = MessageBox.Show("Your clock is currently running.  Are you sure you want to reset?", "Confirmation", MessageBoxButtons.YesNo);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            // Mandatory reset clock
            resetClocks();
            
            if (client == false)
            {
                try
                {
                    loadGame();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to reload {gameFile}. {ex.Message}", "Layout Load Error");
                    gameFile = string.Empty;
                    return;
                }
            }

            SendLoadLayout();
        }

        private void btnChooseGame_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = GetExecutingDirectory();
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gameFile = openFileDialog1.FileName;
                subLayoutName = string.Empty;
                reloadLayout(true);
            }
        }

        private void picClock_Paint(object sender, PaintEventArgs e)
        {
            lblClock2.Draw(e.Graphics);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                //MessageBox.Show(mainImage.ToString());
                if (cboCompression.SelectedIndex == 0)
                    e.Graphics.DrawImage(mainImage, new Rectangle(10 + LayoutXAdjust, 10, 1280 * sizeRestriction / 100, 720 * sizeRestriction / 100));

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
            } catch (Exception ex)
            {
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

        private void btnReloadLayout_Click(object sender, EventArgs e)
        {
            reloadLayout();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                reloadLayout();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
        private bool transparentClickable = true;

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
                    images[lnI] = Image.FromFile(Path.Combine(Form1.GetExecutingDirectory(), fileName));
                    elapsed[lnI] = new TimeSpan();
                    imageName[lnI] = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    lnI++;
                }
                multiState = true;
            }
            else
            {
                elapsed = new TimeSpan[1];
                imageName = new string[1];
                string fileName = xPic.Attribute("src").Value.Replace("/", "\\");
                imageName[0] = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                elapsed[0] = new TimeSpan();
                multiState = false;
            }

            XAttribute transparentClickableAttribute = xPic.Attribute("transparentClickable");

            if (transparentClickableAttribute != null)
            {
                bool.TryParse(transparentClickableAttribute.Value, out transparentClickable);
            }
        }

        public bool isClicked(MouseEventArgs me)
        {
            if (transparentClickable)
            {
                return true;
            }

            if (images == null)
            {
                return false;
            }

            Image image = images[currentState];

            if (image == null)
            {
                return false;
            }

            int pixelX = (int)((me.X * 1.0f / Width) * image.Width);
            int pixelY = (int)((me.Y * 1.0f / Height) * image.Height);

            if (pixelX < 0 || pixelY < 0 || pixelX > image.Width || pixelY > image.Height)
            {
                return false;
            }

            using (Bitmap bitmap = new Bitmap(image))
            {
                Color pixel = bitmap.GetPixel(pixelX, pixelY);

                if (pixel.A == 0)
                {
                    // Transparent, ignore click event
                    return false;
                }
            }

            return true;
        }
        
        public Image nextImage(TimeSpan clock)
        {
            currentState++;
            if (currentState == numberOfStates) currentState = 0;
            elapsed[currentState] = clock;
            return images[currentState];
        }

        public Image prevImage(TimeSpan clock)
        {
            elapsed[currentState] = new TimeSpan(0);
            currentState--;
            if (currentState == -1) currentState = (numberOfStates - 1);
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
        public bool Visible { get; set; } = true;

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
            if (Visible == false)
            {
                return;
            }

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
            if (Visible == false)
            {
                return;
            }

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
