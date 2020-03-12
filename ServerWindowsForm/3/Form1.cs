// 동기 서버 msdn 예제 - 윈도우 버전
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _3 {
    public partial class MainForm : Form {
        public static string data = null;

        private Socket listener = null;
        private Socket handler = null;

        public MainForm() {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        public void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (handler != null) {
                handler.Close();
                handler.Dispose();
            }

            if (listener != null) {
                listener.Close();
                listener.Dispose();
            }
            Application.Exit();
        }

        public static string TruncateLeft(string value, int maxLength) {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    return localIP;
                }
            }
            return "127.0.0.1";
        }

        private void btnListen_Click(object sender, EventArgs e) {
            byte[] bytes = new byte[1024];

            IPAddress localIPAddress = IPAddress.Parse(LocalIPAddress());
            IPEndPoint localEndPoint = new IPEndPoint(localIPAddress, 12000);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                new Thread(delegate () {
                    while (true) {
                        Invoke((MethodInvoker)delegate {
                            listBox1.Items.Add("Waiting for connections.....");
                        });

                        handler = listener.Accept();
                        Invoke((MethodInvoker)delegate {
                            listBox1.Items.Add("클라이언트 연결.....OK");
                        });

                        try {
                            data = null;

                            while (true) {
                                bytes = new byte[1024];
                                int bytesRec = handler.Receive(bytes);
                                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                                if (data.IndexOf("<eof>") > -1) break;
                            }

                            data = TruncateLeft(data, data.Length - 5);

                            Invoke((MethodInvoker)delegate {
                                listBox1.Items.Add(string.Format("Text received : {0}", data));
                            });

                            data = "[Server Echo 메시지]" + data;
                            byte[] msg = Encoding.UTF8.GetBytes(data);

                            handler.Send(msg);
                        } catch {
                            MessageBox.Show("서버: DISCONNECTION!");
                            handler.Close();
                            handler.Dispose();
                            break;
                        }
                    }
                }).Start();
            }catch(SocketException se) {
                MessageBox.Show("SocketException 에러 : " + se.ToString());
                switch (se.SocketErrorCode) {
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        break;
                }
            }catch(Exception ex) {
                MessageBox.Show("Exception 에러 : " + ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e) {
            listBox1.Items.Clear();
        }
    }
}
