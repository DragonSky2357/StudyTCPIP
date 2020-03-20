// 구조체를 이용한 소켓통신2 - 마샬링 방법 - (Server)

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace _9 {
    public partial class MainForm : Form {
        TcpListener server = null;
        public MainForm() {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(WindowsFormClosing);
            InitStart();
        }

        private void WindowsFormClosing(object sender, FormClosingEventArgs s) {
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
                server = new TcpListener(IPAddress.Parse("192.168.0.12"), 13000);
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
            // 1. 데이타 받기
            TcpClient client = sender as TcpClient;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[8092];
            DataPacket packet = new DataPacket();

            while (stream.Read(buffer, 0, buffer.Length) != 0) {
                // deserializing;               
                packet.Deserialize(ref buffer);
            }

            stream.Close();
            client.Close();

            // 2. 데이타 표시하기
            string Name = packet.Name;
            string Subject = packet.Subject;
            Int32 Grade = packet.Grade;
            string Memo = packet.Memo;

            Debug.WriteLine("{0} : {1} : {2} : {3}", Name, Subject, Grade, Memo);

            Invoke((MethodInvoker)delegate {
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


            // 3. 상태값 표시하기
            updateStatusInfo("Data Accepted");
        }

        private void updateStatusInfo(string content) {
            Action del = delegate () {
                lblStatus.Text = content;
            };
            Invoke(del);
        }

        private void btnDataClear_Click(object sender, EventArgs e) {
            if (MessageBox.Show("데이타를 모두 삭제 하시겠습니까?", "질문", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK) {
                listView1.Items.Clear();
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
