using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _9 {
    class Program {
        static void Main(string[] args) {
            TcpListener server = null;
            try {
                server = new TcpListener(IPAddress.Parse("192.168.254.1"), 13000);
                server.Start();

                byte[] buffer = new byte[256];

                while (true) {
                    Console.WriteLine("Waiting for a connection.....");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("\nConnected!!");

                    NetworkStream stream = client.GetStream();

                    while (stream.Read(buffer, 0, buffer.Length) != 0) {
                        DataPacket packet = GetBindAck(buffer);

                        string Name = packet.Name;
                        string Subject = packet.Subject;
                        Int32 Grade = packet.Grade;
                        string Memo = packet.Memo;

                        Console.WriteLine("이 름 : {0}", Name);
                        Console.WriteLine("과 목 : {0}", Subject);
                        Console.WriteLine("점 수 : {0}", Grade);
                        Console.WriteLine("메 모 : {0}", Memo);
                        Console.WriteLine("");
                        Console.WriteLine("===========================================");
                        Console.WriteLine("");
                    }

                    stream.Close();
                    client.Close();
                }
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0}", se.Message);
            } catch (Exception ex) {
                Console.WriteLine("Exception : {0}", ex.Message);
            }
        }

        private static DataPacket GetBindAck(byte[] btfuffer) {
            DataPacket packet = new DataPacket();

            MemoryStream ms = new MemoryStream(btfuffer, false);
            BinaryReader br = new BinaryReader(ms);

            packet.Name = ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(20)));
            packet.Subject = ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(20)));
            packet.Grade = IPAddress.NetworkToHostOrder(br.ReadInt32());
            packet.Memo = ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(100)));

            br.Close();
            ms.Close();

            return packet;
        }

        private static string ExtendedTrim(string source) {
            string dest = source;
            int index = dest.IndexOf('\0');
            if (index > -1)
                dest = source.Substring(0, index + 1);

            return dest.TrimEnd('\0').Trim();
        }
    }
}
