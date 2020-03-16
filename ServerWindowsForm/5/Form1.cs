using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;

namespace _5 {
    public partial class MainForm : Form {
        Listener listener;
        public MainForm() {
            InitializeComponent();

            int port = Convert.ToInt32(Properties.Settings.Default.Port);
            listener = new Listener(port);
            listener.SocketAccepted += new Listener.SocketAcceptedHandler(listener_SocketAccepted);
            Load += new EventHandler(Main_Load);
        }

        void Main_Load(object sender, EventArgs e) {
            string tcpServerIP = Properties.Settings.Default.ServerIP;
            listener.Start(tcpServerIP);
        }

        void listener_SocketAccepted(System.Net.Sockets.Socket e) {
            Client client = new Client(e);
            client.Received += new Client.ClientReceivedHandler(client_Received);
            client.Disconnected += new Client.ClientDisconnectedHandler(client_Disconnected);

            Invoke((MethodInvoker)delegate {
                ListViewItem i = new ListViewItem();
                i.Text = client.EndPoint.ToString();
                i.SubItems.Add(client.ID);
                i.SubItems.Add("XX");
                i.SubItems.Add("YY");
                i.Tag = client;
                lstClient.Items.Add(i);
            });
        }

        void client_Disconnected(Client sender) {
            Invoke((MethodInvoker)delegate {
                for (int i = 0; i < lstClient.Items.Count; i++) {
                    Client client = lstClient.Items[i].Tag as Client;

                    if (client.ID == sender.ID) {
                        lstClient.Items.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void client_Received(Client sender, byte[] data) {
            Invoke((MethodInvoker)delegate {
                for (int i = 0; i < lstClient.Items.Count; i++) {
                    Client client = lstClient.Items[i].Tag as Client;

                    if (client.ID == sender.ID) {
                        lstClient.Items[i].SubItems[2].Text = Encoding.UTF8.GetString(data);
                        lstClient.Items[i].SubItems[3].Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    }
                }
            });
        }
    }
}
