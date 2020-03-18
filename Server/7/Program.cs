// 구조체를 이용한 소켓통신2 - 마샬링 방법(Server)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _7 {
    class Program {
        static void Main(string[] args) {
            TcpListener server = null;

            try {
                server = new TcpListener(IPAddress.Parse("192.168.254.1"), 13000);
                server.Start();

                byte[] buffer = new byte[1024];

                while (true) {
                    Console.WriteLine("Waiting for a connection.....");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("\nConnected!!");

                    NetworkStream stream = client.GetStream();

                    while (stream.Read(buffer, 0, buffer.Length) != 0) {
                        DataPacket packet = new DataPacket();
                        packet.Deserialize(ref buffer);

                        string Name = packet.Name;
                        string Subject = packet.Subject;
                        Int32 Grade = packet.Grade;
                        string Memo = packet.Memo;

                        Console.WriteLine("이 름 : {0}", Name);
                        Console.WriteLine("과 목 : {0}", Subject);
                        Console.WriteLine("점 수 : {0}", Grade);
                        Console.WriteLine("메 모 : {0}", Memo);
                        Console.WriteLine("");
                        Console.WriteLine("============================================");
                        Console.WriteLine("");
                    }
                    stream.Close();
                    client.Close();
                }
            }catch(SocketException se) {
                Console.WriteLine(se.Message.ToString());
            }catch(Exception ex) {
                Console.WriteLine(ex.Message.ToString());
            }
            Console.ReadLine();
        }
    }
}
