using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _6 {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e) {
            string serverIP = textServerIP.Text;
            string sendData = textMessage.Text;

            SendMessage(serverIP, sendData);
        }

        private void SendMessage(string server, string message) {
            try {
                Int32 port = 14000;
                TcpClient client = new TcpClient(server, port);

                Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Invoke((MethodInvoker)delegate {
                    listBox1.Items.Add(string.Format("Sent: {0}", message));
                });

                data = new Byte[256];

                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                Invoke((MethodInvoker)delegate {
                    listBox1.Items.Add(string.Format("Received: {0}", responseData));
                    listBox1.Items.Add("");
                });

                stream.Close();
                client.Close();
            } catch (ArgumentNullException e) {
                MessageBox.Show(string.Format("ArgumentNullException: {0}", e.Message.ToString()));
            } catch (SocketException e) {
                MessageBox.Show(string.Format("SocketException: {0}", e.Message.ToString()));
            }
        }
    }
}
