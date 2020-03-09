using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClientWindowsForm {
    public partial class MainForm : System.Windows.Forms.Form {
        Socket sock;
        public MainForm() {
            InitializeComponent();
            sock = socket();
            FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            sock.Close();
        }

        Socket socket() {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        void read() {
            while (true) {
                try {
                    byte[] buffer = new byte[255];
                    int rec = sock.Receive(buffer, 0, buffer.Length, SocketFlags.None);

                    if (rec <= 0) throw new SocketException();

                    Array.Resize(ref buffer, rec);

                    Invoke((MethodInvoker)delegate {
                        listBox1.Items.Add(Encoding.UTF8.GetString(buffer));
                    });
                } catch {
                    MessageBox.Show("클라이언트: DISCONNECTION");
                    sock.Close();
                    break;
                }
            }
            Application.Exit();
        }

        private void btn_connect_Click(object sender, EventArgs e) {
            try {
                sock.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 3));
                new Thread(() => {
                    read();
                }).Start();
            } catch {
                MessageBox.Show("클라이언트 : CONNECTION FAILED!");
            }
        }

        private void btn_send_Click(object sender, EventArgs e) {
            byte[] data = Encoding.UTF8.GetBytes(textBox2.Text);
            sock.Send(data, 0, data.Length, SocketFlags.None);
        }
    }
}
