// 동기 클라이언트 msdn 예제 - 콘솔 버전

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _2 {
    class Program {
        public static string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private static void StartClient() {
            byte[] bytes = new byte[1024];

            try {
                IPAddress remoteIPAddress = IPAddress.Parse(LocalIPAddress());
                IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, 11000);

                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try {
                    sender.Connect(remoteEndPoint);

                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint);

                    Console.Write("Client 메시지 :");
                    byte[] msg = Encoding.UTF8.GetBytes(Console.ReadLine());

                    int bytesSent = sender.Send(msg);

                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}", Encoding.UTF8.GetString(bytes, 0, bytesRec));

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }catch(ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }catch(SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }catch(Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args) {
            StartClient();
            Console.ReadLine();
        }
    }
}
