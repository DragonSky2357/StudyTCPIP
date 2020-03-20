// 구조체를 이용한 소켓통신3 - 바이너리 포매터 방법(Client)

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _10 {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();

            btnSend.MouseEnter += new EventHandler(btnSend_MouseEnter);
            btnSend.MouseLeave += new EventHandler(btnSend_MouseLeave);
        }

        void btnSend_MouseEnter(object sender, EventArgs e) {
            btnSend.UseVisualStyleBackColor = false;
            btnSend.BackColor = Color.FromArgb(255, 255, 165, 00);  // 배경색을 오렌지 색으로 변경함...
            btnSend.ForeColor = Color.White;                        // 글자색을 흰색으로 변경함
        }

        void btnSend_MouseLeave(object sender, EventArgs e) {
            btnSend.UseVisualStyleBackColor = true;
            btnSend.BackColor = SystemColors.Control;               // 배경색을 시스템 기본색으로 변경함...
            btnSend.ForeColor = SystemColors.ControlText;           // 글자색을 시스템 기본색으로 변경함...
        }

        private void btnSend_Click(object sender, EventArgs e) {
            Thread socketworker = new Thread(new ThreadStart(socketThread));
            socketworker.IsBackground = true;
            socketworker.Start();
        }

        private void socketThread() {
            // 1. 데이타패킷 조합
            Debug.WriteLine("{0} : {1} : {2} : {3}", txtName.Text, txtSubject.Text, txtGrade.Text, txtMemo.Text);

            DataPacket packet = new DataPacket();

            packet.Name = txtName.Text;
            packet.Subject = txtSubject.Text;

            Int32 outNum;
            if (Int32.TryParse(txtGrade.Text, out outNum)) {
                packet.Grade = Convert.ToInt32(txtGrade.Text);
            } else {
                packet.Grade = 0;
            }

            packet.Memo = txtMemo.Text;

            if (string.IsNullOrEmpty(packet.Name)) {
                MessageBox.Show("[이 름]을 입력하시기 바랍니다", "경고", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)delegate
                {
                    txtName.Focus();

                });
                return;
            }

            if (string.IsNullOrEmpty(packet.Subject)) {
                MessageBox.Show("[과 목]을 입력하시기 바랍니다", "경고", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)delegate
                {
                    txtSubject.Focus();

                });
                return;
            }

            if (packet.Grade == 0) {
                MessageBox.Show("[점 수]을 입력하시기 바랍니다", "경고", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)delegate
                {
                    txtGrade.Focus();

                });
                return;
            }

            if (string.IsNullOrEmpty(packet.Memo)) {
                packet.Memo = " ";
            }

            // 2. TcpClient 생성 및 설정
            TcpClient client = new TcpClient(txtServerIP.Text, Convert.ToInt32(txtServerPort.Text));
            NetworkStream stream = client.GetStream();
            updateStatusInfo("Connected");

            // 3. 전송하기
            byte[] buffer = GetBytes_Bind(packet);

            stream.Write(buffer, 0, buffer.Length);
            updateStatusInfo(string.Format("{0} data sent", buffer.Length));

            // 4. 스트림과 소켓 닫기
            stream.Close();
            client.Close();

            // 5. listview 에 추가하기
            Invoke((MethodInvoker)delegate
            {
                int count = listView1.Items.Count;
                count++;

                ListViewItem i = new ListViewItem();
                i.Text = count.ToString();
                i.SubItems.Add(txtName.Text);
                i.SubItems.Add(txtSubject.Text);
                i.SubItems.Add(txtGrade.Text);
                i.SubItems.Add(txtMemo.Text);
                i.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                listView1.Items.Add(i);

                listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
            });
        }

        private void updateStatusInfo(string content) {
            Action del = delegate ()
            {
                lblStatus.Text = content;
            };
            Invoke(del);
        }

        // 패킷 사이즈
        private const int BODY_BIND_SIZE = 20 + 20 + 4 + 100;

        // 인증 패킷 구조체를 바이트 배열로 변환하는 함수
        private byte[] GetBytes_Bind(DataPacket packet) {
            byte[] btBuffer = new byte[BODY_BIND_SIZE];

            MemoryStream ms = new MemoryStream(btBuffer, true);
            BinaryWriter bw = new BinaryWriter(ms);

            // Name - string
            try {
                byte[] btName = new byte[20];
                Encoding.UTF8.GetBytes(packet.Name, 0, packet.Name.Length, btName, 0);
                bw.Write(btName);
            } catch (Exception ex) {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            // Subject - string
            try {
                byte[] btSubject = new byte[20];
                Encoding.UTF8.GetBytes(packet.Subject, 0, packet.Subject.Length, btSubject, 0);
                bw.Write(btSubject);
            } catch (Exception ex) {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            // Grade - long
            bw.Write(IPAddress.HostToNetworkOrder(packet.Grade));

            // Memo - string
            try {
                byte[] btMemo = new byte[100];
                Encoding.UTF8.GetBytes(packet.Memo, 0, packet.Memo.Length, btMemo, 0);
                bw.Write(btMemo);
            } catch (Exception ex) {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            bw.Close();
            ms.Close();

            return btBuffer;
        }

    }
}
