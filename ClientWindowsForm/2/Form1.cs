using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2 {
    public partial class MainForm : Form {
        Socket sck;
        public MainForm() {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (sck.Connected)
                sck.Close();
        }
        private void btnConnect_Click(object sender, EventArgs e) {
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                sck.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8));
            } catch {
                MessageBox.Show("Unable to connect!");
            }
        }

        private void btnSendText_Click(object sender, EventArgs e) {
            byte[] data = Encoding.UTF8.GetBytes(richTextBox1.Text);
            sck.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            sck.Send(data);
        }
    }
}
