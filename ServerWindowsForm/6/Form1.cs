using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _6 {
    public partial class MainForm : Form {
        TcpListener server = null;

        public MainForm() {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form_Closing);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e) {
            if (server != null)
                server = null;

            Application.Exit();
        }
        private void btnListen_Click(object sender, EventArgs e) {
            new Thread(delegate () {
                try {
                    Int32 port = 14000;
                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                    server = new TcpListener(localAddr, port);
                    server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    server.Start();

                    Byte[] bytes = new Byte[256];
                    String data = null;

                    while (true) {
                        Invoke((MethodInvoker)delegate {
                            listBox1.Items.Add("Waiting for a conncetion.....");
                        });

                        TcpClient client = server.AcceptTcpClient();
                        Invoke((MethodInvoker)delegate {
                            listBox1.Items.Add("Connected!");
                        });

                        data = null;

                        NetworkStream stream = client.GetStream();

                        int i;

                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
                            data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                            Invoke((MethodInvoker)delegate {
                                listBox1.Items.Add(string.Format("Received: {0}", data));
                            });

                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

                            stream.Write(msg, 0, msg.Length);
                            Invoke((MethodInvoker)delegate {
                                listBox1.Items.Add(string.Format("Sent: {0}", data));
                                listBox1.Items.Add("");
                            });
                        }
                        client.Close();
                    }
                } catch (SocketException se) {
                    MessageBox.Show(string.Format("SocketException:{0}", se.Message.ToString()));
                } catch (Exception ex) {
                    MessageBox.Show(string.Format("Exception: {0}", ex.Message.ToString()));
                } finally {
                    server.Stop();
                }
            }).Start();
        }
    }
}
