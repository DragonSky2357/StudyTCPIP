// 비동기 서버 msdn 예제 - 윈도우 버전

using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace _4 {
    public partial class MainForm : Form {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public Socket g_listener = null;

        public MainForm() {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        public void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (g_listener != null) {
                g_listener.Close();
                g_listener.Dispose();
            }
            Application.Exit();
        }
        private void btnListen_Click(object sender, EventArgs e) {
            btnListen.Enabled = false;

            byte[] bytes = new byte[10240];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress()), 11000);
            g_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                g_listener.Bind(localEndPoint);
                g_listener.Listen(100);

                new Thread(delegate () {
                    while (true) {
                        allDone.Reset();

                        Invoke((MethodInvoker)delegate {
                            listBox1.Items.Add("Waiting for a connection.....");
                        });
                        g_listener.BeginAccept(new AsyncCallback(AcceptCallback), g_listener);

                        allDone.WaitOne();
                    }
                }).Start();
            }catch(SocketException se) {
                MessageBox.Show(string.Format("StartListening [SocketException] Error : {0}", se.Message.ToString()));
            }catch(Exception ex) {
                MessageBox.Show(string.Format("StartListening [Exception] Error : {0} ", ex.Message.ToString()));
            }
        }
        public void AcceptCallback(IAsyncResult ar) {
            allDone.Set();

            Socket listener = ar.AsyncState as Socket;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar) {
            string content = string.Empty;

            StateObject state = ar.AsyncState as StateObject;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1) {
                    Invoke((MethodInvoker)delegate {
                        content = TruncateLeft(content, content.Length - 5);
                        listBox1.Items.Add(string.Format("Read {0} bytes from socket. Data : {1}", content.Length, content));
                    });
                    Send(handler, content);
                } else {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        public void Send(Socket handler, string data) {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), handler);
        }

        public void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = ar.AsyncState as Socket;

                int bytesSent = handler.EndSend(ar);
                Invoke((MethodInvoker)delegate {
                    listBox1.Items.Add(string.Format("Sent {0} bytes to client ", bytesSent));
                    listBox1.Items.Add("");
                });

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } catch (SocketException se) {
                MessageBox.Show(string.Format("SendCallback [SocketException] Error : {0} ", se.Message.ToString()));
            } catch (Exception ex) {
                MessageBox.Show(string.Format("SendCallback [Exception] Error : {0} ", ex.Message.ToString()));
            }
        }

        public string TruncateLeft(string value, int maxLength) {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        // Get local IP
        public string LocalIPAddress() {
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

        private void btnClear_Click(object sender, EventArgs e) {
            listBox1.Items.Clear();
        }
    }
}

public class StateObject {
    // Client socket
    public Socket workSocket = null;
    // Size of receive buffer
    public const int BufferSize = 1024;
    // Receive buffer
    public byte[] buffer = new byte[BufferSize];
    // Received data string
    public StringBuilder sb = new StringBuilder();
}
