using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client.Source.WindowsConsole._2 {
    class Program {
        static void Main(string[] args) {
            Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1994);
            sck.Connect(endPoint);

            Console.WriteLine("Enter Message: ");
            string msg = Console.ReadLine();
            byte[] msgBuffer = Encoding.UTF8.GetBytes(msg);
            sck.Send(msgBuffer, 0, msgBuffer.Length, SocketFlags.None);

            byte[] buffer = new byte[255];
            int rec = sck.Receive(buffer, 0, buffer.Length, SocketFlags.None);

            Array.Resize(ref buffer, rec);

            Console.WriteLine("Recevied: {0}", Encoding.UTF8.GetString(buffer));

            Console.Read();
        }
    }
}
