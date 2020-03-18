// 구조체를 이용한 소켓통신2 - 마샬링 방법(Client)
using System;
using System.Net.Sockets;

namespace _7 {
   class Program {
        static void Main(string[] args) {
            try {
                string Name = string.Empty;
                string Subject = string.Empty;
                Int32 Grade = 0;
                string Memo = string.Empty;

                do {
                    Console.Write("이름 : ");
                    Name = Console.ReadLine();

                    Console.Write("과목 : ");
                    Subject = Console.ReadLine();

                    Console.Write("점수 : ");
                    string tmpGrage = Console.ReadLine();
                    if (tmpGrage != "") {
                        int outGrade = 0;
                        if (Int32.TryParse(tmpGrage, out outGrade))
                            Grade = Convert.ToInt32(tmpGrage);
                        else
                            Grade = 0;
                    } else
                        Grade = 0;

                    Console.Write("메모 : ");
                    Memo = Console.ReadLine();

                    if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Subject))
                        break;

                    DataPacket packet = new DataPacket();
                    packet.Name = Name;
                    packet.Subject = Subject;
                    packet.Grade = Grade;
                    packet.Memo = Memo;

                    byte[] buffer = packet.Serialize();

                    TcpClient client = new TcpClient();
                    client.Connect("192.168.254.1", 13000);
                    Console.WriteLine("Connected...");

                    NetworkStream stream = client.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("{0} data sent", buffer.Length);
                    Console.WriteLine("===============================\n");

                    stream.Close();
                    client.Close();
                } while (Name != "" && Subject != "");
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("Exception : {0} ", ex.Message.ToString());
            }
            Console.ReadLine();
        }
    }
}
