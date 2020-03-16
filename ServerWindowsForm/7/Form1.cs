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

        private void btnStart_Click(object sender, EventArgs e) {
            Thread tcpServerRunThread = new Thread(new ThreadStart(TcpServerRun));
            tcpServerRunThread.IsBackground = true;
            tcpServerRunThread.Start();
        }

        private void TcpServerRun() {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 5004);
            tcpListener.Start();
            updateUI("Listening");
            updateUI("==========================================");

            while (true) {
                TcpClient client = tcpListener.AcceptTcpClient();
                updateUI("Connected");
                Thread tcpHandlerThread = new Thread(new ParameterizedThreadStart(tcpHandler));
                tcpHandlerThread.Start(client);
            }
        }

        private void tcpHandler(object client) {
            TcpClient mClient = client as TcpClient;
            NetworkStream stream = mClient.GetStream();
            byte[] message = new byte[1024];
            int byteRead = stream.Read(message, 0, message.Length);
            updateUI("New Message = " + Encoding.UTF8.GetString(message, 0, byteRead));
            updateUI("");

            stream.Close();
            mClient.Close();
        }

        private void updateUI(string s) {
            Action del = delegate () {
                textBox1.AppendText(s + System.Environment.NewLine);
            };
            Invoke(del);
        }
    }
}
