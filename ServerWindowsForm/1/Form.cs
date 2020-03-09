using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServerWindowsForm {
    public partial class MainForm : System.Windows.Forms.Form {
        Socket sock;
        Socket acc;

        public MainForm() {
            InitializeComponent();
        }

        Socket socket() {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        private void btn_Listen_Click(object sender, EventArgs e) {
            sock = socket();

            sock.Bind(new IPEndPoint(0, 3));
            sock.Listen(0);

            new Thread(delegate () {
                acc = sock.Accept();
                MessageBox.Show("서버 : CONNECTION ACCEPTED!");
                sock.Close();

                while (true) {
                    try {
                        byte[] buffer = new byte[255];
                        int rec = acc.Receive(buffer, 0, buffer.Length, SocketFlags.None);

                        if (rec <= 0) throw new SocketException();

                        Array.Resize(ref buffer, rec);

                        Invoke((MethodInvoker)delegate {
                            listBox1.Items.Add(Encoding.UTF8.GetString(buffer));
                        });
                    } catch {
                        MessageBox.Show("서버: DISCONNECTION!");
                        acc.Close();
                        acc.Dispose();
                        break;
                    }
                }
                Application.Exit();
            }).Start();
        }

        private void btn_Send_Click(object sender, EventArgs e) {
            try {
                byte[] data = Encoding.UTF8.GetBytes(textBox1.Text);
                acc.Send(data, 0, data.Length, 0);
            }catch(SocketException ex) {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
