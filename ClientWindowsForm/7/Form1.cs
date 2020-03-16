using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _7 {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e) {
            Thread mThread = new Thread(new ThreadStart(SendAsClient));
            mThread.Start();
        }

        private void SendAsClient() {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(textServerIP.Text), Convert.ToInt32(textServerPort.Text));
            updateUI("Connected");

            NetworkStream stream = client.GetStream();

            string sendData = textMessage.Text;
            byte[] msg = new byte[256];
            msg = Encoding.UTF8.GetBytes(sendData);

            stream.Write(msg, 0, msg.Length);
            updateUI(string.Format("send data : {0}", sendData));
            updateUI(string.Format("{0} bytes sent", msg.Length));

            stream.Close();
            client.Close();
        }

        private void updateUI(string s) {
            Func<int> del = delegate () {
                textBox1.AppendText(s + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }
    }
}
