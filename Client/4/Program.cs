// // Socket 통신 - Multiple Connection/Accepting Connections(Client)
using System;
using System.Net.Sockets;
using System.Net;

namespace _4 {
    class Program {
        static void Main(string[] args) {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string serverIP = Properties.Settings.Default.ServerIP;
            string port = Properties.Settings.Default.Port;

            s.Connect(IPAddress.Parse(serverIP), Convert.ToInt32(port));
            s.Close();
            s.Dispose();
        }
    }
}
