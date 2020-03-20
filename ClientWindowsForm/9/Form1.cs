using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace _9 {
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
            byte[] buffer = packet.Serialize();

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
    }
}
