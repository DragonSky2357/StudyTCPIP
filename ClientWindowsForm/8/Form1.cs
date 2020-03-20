// 구조체를 이용한 소켓통신2 - 마샬링 방법(Client)
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace _8 {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();

            btnSend.MouseEnter += new EventHandler(btnSend_MouseEnter);
            btnSend.MouseLeave += new EventHandler(btnSend_MouseLeave);
        }

        void btnSend_MouseLeave(object sender, EventArgs e) {
            btnSend.UseVisualStyleBackColor = false;
            btnSend.BackColor = Color.FromArgb(255, 255, 165, 00);
            btnSend.ForeColor = Color.White;
        }

        void btnSend_MouseEnter(object sender, EventArgs e) {
            btnSend.UseVisualStyleBackColor = true;
            btnSend.BackColor = SystemColors.Control;
            btnSend.ForeColor = SystemColors.ControlText;
        }

        private void btnSend_Click(object sender, EventArgs e) {
            Thread socketworker = new Thread(new ThreadStart(socketThread));
            socketworker.IsBackground = true;
            socketworker.Start();
        }

        private void socketThread() {
            Debug.WriteLine("{0} : {1} : {2} : {3}", txtName.Text, txtSubject.Text, txtGrade.Text, txtMemo.Text);

            DataPacket packet = new DataPacket();

            packet.Name = txtName.Text;
            packet.Subject = txtSubject.Text;

            Int32 outNum;
            if (Int32.TryParse(txtGrade.Text, out outNum))
                packet.Grade = Convert.ToInt32(txtGrade.Text);
            else
                packet.Grade = 0;

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

            TcpClient client = new TcpClient(txtServerIP.Text, Convert.ToInt32(txtServerPort.Text));
            NetworkStream stream = client.GetStream();
            updateStatusInfo("Connected");

            byte[] buffer = new byte[Marshal.SizeOf(packet)];

            unsafe {
                fixed (byte* fixed_buffer = buffer) {
                    Marshal.StructureToPtr(packet, (IntPtr)fixed_buffer, false);
                }
            }

            stream.Write(buffer, 0, Marshal.SizeOf(packet));
            updateStatusInfo(string.Format("{0} data sent", Marshal.SizeOf(packet)));

            stream.Close();
            stream.Close();

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
