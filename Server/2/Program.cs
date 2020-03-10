// 동기 서버 msdn 예제 - 콘솔 버전

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _2 {
    class Program {
        public static string data = null;

        public static string TruncateLeft(string value, int maxLength) {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public static void StartListening() {
            Socket listener = null;
            Socket handler = null;

            byte[] bytes = new byte[1024];

            IPAddress localIPAddress = IPAddress.Parse(LocalIPAddress());
            IPEndPoint localEndPoint = new IPEndPoint(localIPAddress, 11000);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true) {
                    Console.WriteLine("Waiting for connections.....");

                    handler = listener.Accept();
                    data = null;

                    while (true) {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<eof>") > -1) break;
                    }
                    data = TruncateLeft(data, data.Length - 5);

                    Console.WriteLine("Text received : {0}", data);

                    data = "[Server Echo 메시지]" + data;
                    byte[] msg = Encoding.UTF8.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }catch(SocketException se) {
                Console.WriteLine("Socket 에러 : {0}", se.ToString());
                switch (se.SocketErrorCode) {
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                        handler.Close();
                        break;
                }
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args) {
            StartListening();
            Console.ReadLine();
        }
    }
}
