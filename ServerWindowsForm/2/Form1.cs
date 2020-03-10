// 버퍼 사이즈보다 많은 양의 데이터를 받을 시(Server)
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _2 {
    public partial class MainForm : Form {
        Socket sck;
        Socket acc;

        public MainForm() {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e) {
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sck.Bind(new IPEndPoint(0, 8));
            sck.Listen(0);

            acc = sck.Accept();

            sck.Close();

            new Thread(() => {
                while (true) {
                    try {
                        byte[] sizeBuf = new byte[4];

                        acc.Receive(sizeBuf, 0, sizeBuf.Length, 0);

                        int size = BitConverter.ToInt32(sizeBuf, 0);
                        MemoryStream ms = new MemoryStream();

                        while (size > 0) {
                            byte[] buffer;
                            if (size < acc.ReceiveBufferSize)
                                buffer = new byte[size];
                            else
                                buffer = new byte[acc.ReceiveBufferSize];

                            int rec = acc.Receive(buffer, 0, buffer.Length, 0);
                            size -= rec;

                            ms.Write(buffer, 0, buffer.Length);
                        }
                        ms.Close();

                        byte[] data = ms.ToArray();

                        ms.Dispose();

                        Invoke((MethodInvoker)delegate {
                            richTextBox1.Text = Encoding.UTF8.GetString(data);
                        });
                    } catch {
                        MessageBox.Show("서버 : DISCONNECTION!");
                        acc.Close();
                        acc.Dispose();
                        break;
                    }
                }
                Application.Exit();
            }).Start();
        }
    }
}
