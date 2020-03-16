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

namespace _5 {
    public partial class MainForm : Form {
        Socket sck;

        public MainForm() {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            string ServerIP = Properties.Settings.Default.ServerIP;
            string Port = Properties.Settings.Default.Port;

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ServerIP), Convert.ToInt32(Port));
            sck.Connect(remoteEP);
            lblInfo.Text = "Connected";
        }

        private void btnSend_Click(object sender, EventArgs e) {
            int s = sck.Send(Encoding.UTF8.GetBytes(txtData.Text));
            if (s > 0)
                lblInfo.Text = string.Format("{0} bytes data sent", s.ToString());
        }

        private void btnClose_Click(object sender, EventArgs e) {
            lblInfo.Text = "Not Connected.....";
            sck.Close();
            sck.Dispose();
            Close();
        }
    }
}
