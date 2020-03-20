// 구조체를 이용한 소켓통신3 - 바이너리 포매터 방법(Server)

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _10 {
    public partial class MainForm : Form {
        TcpListener server = null;

        public MainForm() {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(WindowsFormClosing);
            InitStart();
        }

        private void WindowsFormClosing(object sender, FormClosingEventArgs e) {
            if (server != null)
                server = null;

            Application.Exit();
        }

        private void InitStart() {
            Thread socketworker = new Thread(new ThreadStart(socketThread));
            socketworker.IsBackground = true;
            socketworker.Start();
        }

        private void socketThread() {
            try {
                server = new TcpListener(IPAddress.Parse("192.168.254.1"), 13000);
                server.Start();

                while (true) {
                    TcpClient client = server.AcceptTcpClient();
                    updateStatusInfo("Connected");
                    Thread clientworker = new Thread(new ParameterizedThreadStart(clientThread));
                    clientworker.IsBackground = true;
                    clientworker.Start(client);
                }
            } catch (SocketException se) {
                Debug.WriteLine("SocketException : {0}", se.Message);
            } catch (Exception ex) {
                Debug.WriteLine("Exception : {0}", ex.Message);
            }
        }

        private void clientThread(object sender) {
            TcpClient client = sender as TcpClient;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[8092];
            DataPackt packet = new DataPackt();

            while (stream.Read(buffer, 0, buffer.Length) != 0)
                packet = GetBindAck(buffer);

            stream.Close();
            client.Close();

            string Name = packet.Name;
            string Subject = packet.Subject;
            Int32 Grade = packet.Grade;
            string Memo = packet.Memo;

            Debug.WriteLine("{0} : {1} : {2} : {3}", Name, Subject, Grade, Memo);

            Invoke((MethodInvoker)delegate
            {
                int count = listView1.Items.Count;
                count++;

                ListViewItem i = new ListViewItem();
                i.Text = count.ToString();
                i.SubItems.Add(Name);
                i.SubItems.Add(Subject);
                i.SubItems.Add(Grade.ToString());
                i.SubItems.Add(Memo);
                i.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                listView1.Items.Add(i);

                listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
            });

            updateStatusInfo("Data Accepted");
        }

        private void updateStatusInfo(string content) {
            Action del = delegate () {
                lblStatus.Text = content;
            };
            Invoke(del);
        }

        private void btnDataClear_Click(object sender, EventArgs e) {
            if (MessageBox.Show("데이타를 모두 삭제 하시겠습니까?", "질문", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.OK)
                listView1.Items.Clear();
        }

        private void btnClose_Client(object sender, EventArgs e) {
            Application.Exit();
        }

        private DataPackt GetBindAck(byte[] buffer) {
            DataPackt packet = new DataPackt();

            MemoryStream ms = new MemoryStream(buffer, false);
            BinaryReader br = new BinaryReader(ms);

            packet.Name = ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(20)));
            packet.Subject = ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(20)));
            packet.Grade = IPAddress.NetworkToHostOrder(br.ReadInt32());
            packet.Memo = ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(100)));

            br.Close();
            ms.Close();

            return packet;
        }

        private string ExtendedTrim(string source) {
            string dest = source;
            int index = dest.IndexOf('\0');
            if (index > -1) {
                dest = source.Substring(0, index + 1);
            }

            return dest.TrimEnd('\0').Trim();
        }
    }
}
