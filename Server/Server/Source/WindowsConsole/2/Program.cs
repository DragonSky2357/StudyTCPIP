using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Source.WindowsConsole._2 {
    class Program {
        static void Main(string[] args) {
            Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sck.Bind(new IPEndPoint(0, 1994));
            sck.Listen(0);

            Socket acc = sck.Accept();

            byte[] buffer = Encoding.UTF8.GetBytes("Hello Client");
            acc.Send(buffer, 0, buffer.Length, SocketFlags.None);

            buffer = new byte[acc.SendBufferSize];
            int rec = acc.Receive(buffer, 0, buffer.Length, SocketFlags.None);

            Array.Resize(ref buffer, rec);

            Console.WriteLine("Received: {0}", Encoding.UTF8.GetString(buffer));

            sck.Close();
            acc.Close();

            Console.Read();
        }
        
    }
}
