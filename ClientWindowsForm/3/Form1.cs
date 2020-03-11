using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _3 {
    public partial class MainForm : Form {
        private Socket sock = null;
        byte[] bytes = new byte[1024];

        public MainForm() {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e) {
            new Thread(() => {
                try {
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 12000));

                    byte[] msg = Encoding.UTF8.GetBytes(textBox2.Text + "<eof>");
                    int bytesSent = sock.Send(msg);

                    int bytesREc = sock.Receive(bytes);

                    if (bytesSent <= 0) throw new SocketException();

                    Invoke((MethodInvoker)delegate {
                        listBox1.Items.Add(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                    });
                } catch (SocketException se) {
                    MessageBox.Show("SocketException = " + se.Message.ToString());
                    sock.Close();
                    sock.Dispose();
                }
            }).Start();
        }

        private void btnDisconnect_Click(object sender, EventArgs e) {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
}
