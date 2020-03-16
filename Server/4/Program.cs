using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace _4 {
    class Program {
        static Listener l;
        static List<Socket> sockets;

        static void Main(string[] args) {
            System.Diagnostics.Debug.WriteLine("{0} : {1}", Properties.Settings.Default.ServerIP, Properties.Settings.Default.Port);
            string serverIP = Properties.Settings.Default.ServerIP;
            string port = Properties.Settings.Default.Port;

            l = new Listener(Convert.ToInt32(port));
            sockets = new List<Socket>();

            l.SocketAccepted += new Listener.SocketAcceptedHandler(l_SocketAccepted);
            l.Start(serverIP);

            Console.ReadLine();
        }

        static void l_SocketAccepted(System.Net.Sockets.Socket e) {
            Console.WriteLine("New Connection: {0}\n{1}\n=============================",
                e.RemoteEndPoint.ToString(),DateTime.Now.ToString());

            if (e != null)
                sockets.Add(e);

            int index = 1;
            /*
            Console.WriteLine("Conneted socket list\n================================");
            foreach(Socket s in sockets) {
                Console.WriteLine("{0} : {1} : socket handle {2}", index, s.RemoteEndPoint.ToString(), s.Handle.ToString());
                index++;
            }
            */
            Console.WriteLine("Size : {0}",sockets.Count);
        }
    }
}
