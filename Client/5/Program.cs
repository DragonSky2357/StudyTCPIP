using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _5 {
    class Program {
        static void Main(string[] args) {
            string sendData = string.Empty;
            do {
                Console.WriteLine("\n전송할 데이터를 입력해 주세요 : ");
                sendData = Console.ReadLine();

                if (!string.IsNullOrEmpty(sendData))
                    Connect("127.0.0.1", sendData);
            } while (sendData != "");

            Console.WriteLine("\nPress Enter to continue...");
            Console.Read();
        }

        static void Connect(string server, string message) {
            try {
                Int32 port = 14000;
                TcpClient client = new TcpClient(server, port);

                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                data = new byte[256];

                string responseData = string.Empty;

                Int32 bytesRead = stream.Read(data, 0, data.Length);
                responseData = Encoding.UTF8.GetString(data, 0, bytesRead);
                Console.WriteLine("Received: {0}", responseData);

                stream.Close();
                client.Close();
            }catch(ArgumentException e) {
                Console.WriteLine("ArgumentNullException: {0}", e.Message.ToString());
            }catch(SocketException e) {
                Console.WriteLine("SocketException: {0}", e.Message.ToString());
            }
        }
    }
}
