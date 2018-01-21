﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RandoTracker
{
    public partial class Form1 : Form
    {
        int players = 2;
        int pics = 0;
        picLabel[,] picCovers;
        PictureBox[,] pictures;
        Label[] finalTime;
        int playerFontSize = 0;
        int finalFontSize = 0;
        string gameFont = "";
        string gameFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "sml2.xml");
        Stopwatch clock = new Stopwatch();
        Label lblPlayerA = new Label();
        Label lblPlayerB = new Label();
        Label lblPlayerC = new Label();
        Label lblPlayerD = new Label();
        Label lblCommentary = new Label();
        Label lblClock = new Label();

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

            loadGame();

            this.Left = 200;
            this.Top = 200;

            lblPlayerA.Parent = pictureBox1;
            lblPlayerA.BackColor = Color.Transparent;
            lblPlayerA.ForeColor = Color.White;
            pictureBox1.Controls.Add(lblPlayerA);

            lblPlayerB.Parent = pictureBox1;
            lblPlayerB.BackColor = Color.Transparent;
            lblPlayerB.ForeColor = Color.White;
            pictureBox1.Controls.Add(lblPlayerB);

            lblPlayerC.Parent = pictureBox1;
            lblPlayerC.BackColor = Color.Transparent;
            lblPlayerC.ForeColor = Color.White;
            pictureBox1.Controls.Add(lblPlayerC);

            lblPlayerD.Parent = pictureBox1;
            lblPlayerD.BackColor = Color.Transparent;
            lblPlayerD.ForeColor = Color.White;
            pictureBox1.Controls.Add(lblPlayerD);

            lblCommentary.Parent = pictureBox1;
            lblCommentary.BackColor = Color.Transparent;
            lblCommentary.ForeColor = Color.White;
            pictureBox1.Controls.Add(lblCommentary);

            lblClock.Parent = pictureBox1;
            lblClock.BackColor = Color.Transparent;
            lblClock.ForeColor = Color.White;
            lblClock.Text = "0:00:00.0";
            pictureBox1.Controls.Add(lblClock);

            //label1.Parent = pictureBox1;
            //label1.BackColor = Color.FromArgb(127, Color.Black);
        }

        private void loadGame()
        {
            for (int i = 0; i < pics; i++)
            {
                for (int j = 0; j < players; j++)
                {
                    Controls.Remove(pictures[j, i]);
                    Controls.Remove(picCovers[j, i]);
                }
            }

            XDocument gameXML = XDocument.Load(gameFile);
            XElement game = gameXML.Root.Element("game");

            lblGameName.Text = "Game:  " + gameXML.Element("game").Attribute("name").Value;
            players = Convert.ToInt32(gameXML.Element("game").Attribute("players").Value);
            if (players != 4) players = 2;

            gameFont = gameXML.Element("game").Attribute("Font").Value;
            pics = gameXML.Descendants("picture").Count();

            lblPlayerA.Visible = (gameXML.Descendants("player").Skip(0).First().Attribute("visible").Value.ToUpper().Substring(0, 1) == "T");
            lblPlayerB.Visible = (gameXML.Descendants("player").Skip(1).First().Attribute("visible").Value.ToUpper().Substring(0, 1) == "T");
            lblPlayerC.Visible = (players == 4 && gameXML.Descendants("player").Skip(2).First().Attribute("visible").Value.ToUpper().Substring(0, 1) == "T");
            lblPlayerD.Visible = (players == 4 && gameXML.Descendants("player").Skip(3).First().Attribute("visible").Value.ToUpper().Substring(0, 1) == "T");

            if (gameXML.Descendants("mic").First().Attribute("visible").Value == "false")
                lblCommentary.Visible = false;
            else
                lblCommentary.Visible = true;

            playerFontSize = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("fontSize").Value);
            finalFontSize = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalFont").Value);

            Font playerFont = new Font(gameFont, playerFontSize);
            lblPlayerA.Left = Convert.ToInt32(gameXML.Descendants("player").Skip(0).First().Attribute("locX").Value);
            lblPlayerA.Top = Convert.ToInt32(gameXML.Descendants("player").Skip(0).First().Attribute("locY").Value);
            lblPlayerB.Left = Convert.ToInt32(gameXML.Descendants("player").Skip(1).First().Attribute("locX").Value);
            lblPlayerB.Top = Convert.ToInt32(gameXML.Descendants("player").Skip(1).First().Attribute("locY").Value);
            if (players == 4)
            {
                lblPlayerC.Left = Convert.ToInt32(gameXML.Descendants("player").Skip(2).First().Attribute("locX").Value);
                lblPlayerC.Top = Convert.ToInt32(gameXML.Descendants("player").Skip(2).First().Attribute("locY").Value);
                lblPlayerD.Left = Convert.ToInt32(gameXML.Descendants("player").Skip(3).First().Attribute("locX").Value);
                lblPlayerD.Top = Convert.ToInt32(gameXML.Descendants("player").Skip(3).First().Attribute("locY").Value);
            }

            lblPlayerA.Width = lblPlayerB.Width = lblPlayerC.Width = lblPlayerD.Width = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("width").Value);
            lblPlayerA.Height = lblPlayerB.Height = lblPlayerC.Height = lblPlayerD.Height = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("height").Value);

            lblPlayerA.Font = playerFont;
            lblPlayerB.Font = playerFont;
            lblPlayerC.Font = playerFont;
            lblPlayerD.Font = playerFont;
            lblPlayerA.TextAlign = ContentAlignment.MiddleLeft;
            lblPlayerB.TextAlign = ContentAlignment.MiddleRight;
            lblPlayerC.TextAlign = ContentAlignment.MiddleLeft;
            lblPlayerD.TextAlign = ContentAlignment.MiddleRight;

            lblClock.AutoSize = false;
            lblClock.Font = new Font(gameFont, Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("fontSize").Value));
            lblClock.Left = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("locX").Value);
            lblClock.Top = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("locY").Value);
            lblClock.Width = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("width").Value);
            lblClock.Height = Convert.ToInt32(gameXML.Descendants("clock").First().Attribute("height").Value);
            lblClock.TextAlign = ContentAlignment.MiddleCenter;

            lblCommentary.AutoSize = false;
            lblCommentary.Font = new Font(gameFont, Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("fontSize").Value));
            lblCommentary.Left = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("locX").Value);
            lblCommentary.Top = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("locY").Value);
            lblCommentary.Width = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("width").Value);
            lblCommentary.Height = Convert.ToInt32(gameXML.Descendants("mic").First().Attribute("height").Value);
            lblCommentary.TextAlign = ContentAlignment.MiddleLeft;

            picCovers = new picLabel[players, pics];
            pictures = new PictureBox[players, pics];
            finalTime = new Label[players];

            //this.BackgroundImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("game").First().Attribute("background").Value.Replace("/", "\\")));
            //this.BackgroundImageLayout = ImageLayout.Tile; // .Stretch

            pictureBox1.BackgroundImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), gameXML.Descendants("game").First().Attribute("background").Value.Replace("/", "\\")));
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch; // .Stretch

            int picXGap = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("xGap").Value);
            int picYGap = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("yGap").Value);
            int picXSize = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("xSize").Value);
            int picYSize = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("ySize").Value);
            int xNumber = Convert.ToInt32(gameXML.Descendants("pictures").First().Attribute("xNumber").Value);
            int finalWidth = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalWidth").Value);
            int finalHeight = Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalHeight").Value);
            Font finalFont = new Font(gameFont, Convert.ToInt32(gameXML.Descendants("players").First().Attribute("finalFont").Value));

            for (int i = 0; i < players; i++)
            {
                int picX = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("picX").Value);
                int picY = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("picY").Value);

                finalTime[i] = new Label();
                finalTime[i].Left = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("finalX").Value);
                finalTime[i].Top = Convert.ToInt32(gameXML.Descendants("player").Skip(i).First().Attribute("finalY").Value);
                finalTime[i].Width = finalWidth;
                finalTime[i].Height = finalHeight;
                finalTime[i].BackColor = Color.FromArgb(192, Color.Black);
                finalTime[i].ForeColor = Color.LightGreen;
                finalTime[i].Visible = false;
                finalTime[i].Font = finalFont;
                finalTime[i].TextAlign = ContentAlignment.MiddleCenter;
                pictureBox1.Controls.Add(finalTime[i]);

                for (int j = 0; j < pics; j++)
                {
                    pictures[i, j] = new PictureBox();

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

                    pictures[i, j].Left = picX + (picXGap * (j % xNumber));
                    pictures[i, j].Top = picY + (picYGap * (j / xNumber));

                    pictures[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictures[i, j].Width = picXSize;
                    pictures[i, j].Height = picYSize;

                    pictures[i, j].Invalidate();

                    pictureBox1.Controls.Add(pictures[i, j]);

                    picCovers[i, j] = new picLabel();
                    picCovers[i, j].loadPictures(gameXML.Descendants("picture").Skip(j).First());

                    picCovers[i, j].Parent = pictures[i, j];
                    picCovers[i, j].BackColor = Color.FromArgb(numberOfPics == -1 ? 128 : 0, Color.Black);
                    picCovers[i, j].Left = 0; // picX + (picXGap * (j % xNumber));
                    picCovers[i, j].Top = 0; // picY + (picYGap * (j / xNumber));
                    picCovers[i, j].Width = picXSize;
                    picCovers[i, j].Height = picYSize;
                    picCovers[i, j].Click += new EventHandler(picClick);
                    picCovers[i, j].playerNumber = i;
                    picCovers[i, j].labelNumber = j;

                    pictures[i, j].Controls.Add(picCovers[i, j]);
                    picCovers[i, j].BringToFront();
                    //Controls.Add(picCovers[i, j]);
                }
            }
        }

        private void picClick(object sender, EventArgs e)
        {
            picLabel clicked = (picLabel)sender;
            if (client == true)
                sendBytes(new byte[] { (byte)clicked.playerNumber, (byte)clicked.labelNumber });
            else
            {
                changePicture(clicked.playerNumber, clicked.labelNumber);
                serverSendBytes(new byte[] { (byte)clicked.playerNumber, (byte)clicked.labelNumber });
            }
        }

        private void changePicture(int playerNumber, int labelNumber)
        {
            picLabel clicked = picCovers[playerNumber, labelNumber];

            if (clicked.multiState)
            {
                PictureBox picClicked = pictures[playerNumber, labelNumber];
                picClicked.Image = clicked.nextImage();
            } else
            {
                if (clicked.BackColor == Color.FromArgb(0, Color.Black))
                    clicked.BackColor = Color.FromArgb(128, Color.Black);
                else
                    clicked.BackColor = Color.FromArgb(0, Color.Black);
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

        private void txtCommentary_TextChanged(object sender, EventArgs e)
        {
            lblCommentary.Text = txtCommentary.Text;
        }

        private void txtPlayerA_TextChanged(object sender, EventArgs e)
        {
            lblPlayerA.Text = txtPlayerA.Text;
        }

        private void txtPlayerB_TextChanged(object sender, EventArgs e)
        {
            lblPlayerB.Text = txtPlayerB.Text;
        }

        private void txtPlayerC_TextChanged(object sender, EventArgs e)
        {
            lblPlayerC.Text = txtPlayerC.Text;
        }

        private void txtPlayerD_TextChanged(object sender, EventArgs e)
        {
            lblPlayerD.Text = txtPlayerD.Text;
        }

        private void txtTimeA_Leave(object sender, EventArgs e)
        {
            lblPlayerA.Font = new Font(gameFont, txtTimeA.Text != "" ? finalFontSize : playerFontSize);
            finalTime[0].Text = txtTimeA.Text;
            finalTime[0].Visible = (txtTimeA.Text != "");
            finalTime[0].BringToFront();
        }

        private void txtTimeB_Leave(object sender, EventArgs e)
        {
            lblPlayerB.Font = new Font(gameFont, txtTimeB.Text != "" ? finalFontSize : playerFontSize);
            finalTime[1].Text = txtTimeB.Text;
            finalTime[1].Visible = (txtTimeB.Text != "");
            finalTime[1].BringToFront();
        }

        private void txtTimeC_Leave(object sender, EventArgs e)
        {
            lblPlayerC.Font = new Font(gameFont, txtTimeC.Text != "" ? finalFontSize : playerFontSize);
            finalTime[2].Text = txtTimeC.Text;
            finalTime[2].Visible = (txtTimeC.Text != "");
            finalTime[2].BringToFront();
        }

        private void txtTimeD_Leave(object sender, EventArgs e)
        {
            lblPlayerD.Font = new Font(gameFont, txtTimeD.Text != "" ? finalFontSize : playerFontSize);
            finalTime[3].Text = txtTimeD.Text;
            finalTime[3].Visible = (txtTimeD.Text != "");
            finalTime[3].BringToFront();
        }

        // *****************************************************************************************************
        // *****************************************************************************************************
        // *****************************************************************************************************

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < players; i++)
            {
                TimeSpan ts = clock.Elapsed;
                string timeElapsed = Math.Floor(ts.TotalHours) + ":" + Math.Floor((double)ts.Minutes).ToString("00") + ":" + Math.Floor((double)ts.Seconds).ToString("00") + "." + ts.Milliseconds / 100;
                lblClock.Text = timeElapsed;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (client == true)
                sendBytes(new byte[] { 0xf1 });
            else
            {
                startClocks();
                serverSendBytes(new byte[] { 0xf1 });
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
            clock.Reset();
            timer1.Enabled = false;
            lblClock.Text = "0:00:00.0";
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

            // Get current date and time.
            DateTime now = DateTime.Now;
            String strDateLine = "Welcome " + now.ToString("G") + "\n\r";

            // Convert to byte array and send.
            Byte[] byteDateLine = System.Text.Encoding.ASCII.GetBytes(strDateLine.ToCharArray());
            client.Sock.Send(byteDateLine, byteDateLine.Length, 0);

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

                if (aryRet[0] == 0xf1) startClocks();
                else if (aryRet[0] == 0xf2) stopClocks();
                else if (aryRet[0] == 0xf3) resetClocks();
                else if (aryRet[0] <= 0x03)
                    changePicture(aryRet[0], aryRet[1]);
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

                        if (m_byBuff[0] == 0xf1) startClocks();
                        else if (m_byBuff[0] == 0xf2) stopClocks();
                        else if (m_byBuff[0] == 0xf3) resetClocks();
                        else if (m_byBuff[0] <= 0x03)
                            changePicture(m_byBuff[0], m_byBuff[1]);
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
    }

    /// <summary>
    /// Class holding information and buffers for the Client socket connection
    /// </summary>
    internal class SocketChatClient
    {
        private Socket m_sock;                      // Connection to the client
        private byte[] m_byBuff = new byte[50];     // Receive data buffer
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
        public void SetupRecieveCallback(RandoTracker.Form1 app)
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
        private int numberOfStates = -1;
        private int currentState = 0;

        public void loadPictures(XElement xPic)
        {
            numberOfStates = xPic.Descendants("state").Count();
            if (numberOfStates > 0)
            {
                images = new Image[numberOfStates];
                int lnI = 0;
                foreach (XElement pic in xPic.Descendants("state"))
                {
                    images[lnI] = Image.FromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), pic.Attribute("src").Value.Replace("/", "\\")));
                    lnI++;
                }
                multiState = true;
            } else
            {
                multiState = false;
            }
        }

        public Image nextImage()
        {
            currentState++;
            if (currentState == numberOfStates) currentState = 0;
            return images[currentState];
        }
    }
}
