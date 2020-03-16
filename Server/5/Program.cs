// TcpListener - TcpClient msdn 예제(Server)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _5 {
    class Program {
        static void Main(string[] args) {
            TcpListener server = null;

            try {
                Int32 port = 14000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                byte[] bytes = new byte[256];
                string data = string.Empty;

                while (true) {
                    Console.WriteLine("Waiting for a connection...");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = string.Empty;

                    NetworkStream stream = client.GetStream();

                    int bytesRead = 0;

                    while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0) {
                        data = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                        Console.WriteLine("Received: {0}", data);

                        data = data.ToUpper();

                        byte[] msg = Encoding.UTF8.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}\n", data);
                    }
                    client.Close();
                }
            }catch(SocketException se) {
                Console.WriteLine("SocketException : {0}", se.Message.ToString());
            } finally {
                server.Stop();
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
