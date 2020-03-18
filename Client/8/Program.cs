// 구조체를 이용한 소켓통신3 - 바이너리 포매터 방법(Client)
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _8 {
    class Program {
        static void Main(string[] args) {
            try {
                string Name = string.Empty;
                string Subject = string.Empty;
                Int32 Grade = 0;
                string Memo = string.Empty;

                do {
                    // 1. 데이타 입력
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

                    byte[] buffer = GetBytes_Bind(packet);

                    TcpClient client = new TcpClient();
                    client.Connect("192.168.254.1", 13000);
                    Console.WriteLine("Connected...");

                    NetworkStream stream = client.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("{0} data sent\n", buffer.Length);

                    stream.Close();
                    client.Close();
                } while (Name != "" && Subject != "");
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("Exception : {0} ", ex.Message.ToString());
            }

            Console.WriteLine("press the ENTER to continue...");
            Console.ReadLine();
        }

        public const int BODY_BIND_SIZE = 20 + 20 + 4 + 100;

        public static byte[] GetBytes_Bind(DataPacket packet) {
            byte[] btBuffer = new byte[BODY_BIND_SIZE];

            MemoryStream ms = new MemoryStream(btBuffer, true);
            BinaryWriter bw = new BinaryWriter(ms);

            try {
                byte[] btName = new byte[20];
                Encoding.UTF8.GetBytes(packet.Name, 0, packet.Name.Length, btName, 0);
                bw.Write(btName);
            }catch(Exception ex) {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }
            try {
                byte[] btSubject = new byte[20];
                Encoding.UTF8.GetBytes(packet.Subject, 0, packet.Subject.Length, btSubject, 0);
                bw.Write(btSubject);
            } catch (Exception ex) {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            bw.Write(IPAddress.HostToNetworkOrder(packet.Grade));

            try {
                byte[] btMemo = new byte[100];
                Encoding.UTF8.GetBytes(packet.Memo, 0, packet.Memo.Length, btMemo, 0);
                bw.Write(btMemo);
            } catch (Exception ex) {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }

            bw.Close();
            ms.Close();

            return btBuffer;
        }
    }
}
