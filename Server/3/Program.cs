// 비동기 서버 msdn예제 - 콘솔 버전
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _3 {
    public class StateObject {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    class Program {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static string TruncateLeft(string value, int maxLength) {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    return localIP;
                }
            }
            return "127.0.0.1";
        }

        private static void StartListening() {
            byte[] bytes = new byte[1024];

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress()), 11000);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true) {
                    allDone.Reset();

                    Console.WriteLine("\n Waiting for a connections...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }catch(SocketException se) {
                Console.WriteLine("StartListening[SocketException] Error : {0} ", se.Message.ToString());
            }catch(Exception ex) {
                Console.WriteLine("StartListening[Exception] Error : {0} ", ex.Message.ToString());
            }
            Console.WriteLine("\n Press ENTER to continue.....\n");
            Console.ReadLine();
        }

        public static void AcceptCallback(IAsyncResult ar) {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar) {
            string content = string.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<eof>") > -1) {
                    content = TruncateLeft(content, content.Length - 5);
                    Console.WriteLine("Read {0} bytes from socket \nData : {1}", content.Length, content);

                    Send(handler, content);
                } else {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, string data) {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = ar.AsyncState as Socket;

                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }catch(SocketException se) {
                Console.WriteLine("SendCallback[SocetException] Error : {0}", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("SendCallback[Exception] Error : {0} ", ex.Message.ToString());
            }
        }
        static void Main(string[] args) {
            StartListening();
        }
    }
}
