using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;



namespace server_client_app
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        public IPAddress userIP;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in localIP)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPtextbox.Text = address.ToString();
                    userIP = address;
                }
            }


        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(ServerPorttextbox.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
            OnlineTextbox.Text = "Server awaiting connection...";
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IPEnd = new IPEndPoint(IPAddress.Parse(ClientIPtextbox.Text), int.Parse(ClientPorttextbox.Text));

            try
            {
                client.Connect(IPEnd);
                if (client.Connected)
                {
                    OnlineTextbox.Text = (userIP.ToString() + " is online now" + "\n");
                    ChatScreenTB.AppendText("Connected to Server " + "\n");
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.ChatScreenTB.Invoke(new MethodInvoker(delegate ()
                    {
                        ChatScreenTB.AppendText(userIP.ToString() + ": " + recieve + "\n");
                    }));
                    recieve = "";
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                
                STW.WriteLine(TextToSend);
                this.ChatScreenTB.Invoke(new MethodInvoker(delegate ()
                {
                    OnlineTextbox.AppendText(userIP.ToString() + " is just connected.\n");
                    ChatScreenTB.AppendText("Me: " + TextToSend + "\n");
                    OnlineTextbox.Text = (userIP.ToString() + " is online now" + "\n");
                }));

            }
            else
            {
                OnlineTextbox.Text = "";
                MessageBox.Show("Cannot send.");
            }
            backgroundWorker2.CancelAsync();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(SendTB.Text != "")
            {
                TextToSend = SendTB.Text + "  [" + DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "]";
                SendTB.Text = "";
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            client.Close();
            ChatScreenTB.Text = "** Connection terminated by " + userIP.ToString() + " **\n";
            OnlineTextbox.Text = "";
        }
    }
}
