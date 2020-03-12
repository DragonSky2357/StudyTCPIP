// 비동기 클라이언트 msdn예제 - 콘솔 버전
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _3 {
    public class StateObject {
        public Socket workSocket = null;
        public const int BufferSize = 256;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
    class Program {
        private const int port = 11000;

        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private static String response = String.Empty;

        private static void StartClient() {
            try {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.254.1"), port);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Console.Write("전송할 메시지를 입력해 주세요 : ");
                string sendData = Console.ReadLine() + "<eof>";
                Send(client, sendData);
                sendDone.WaitOne();

                Receive(client);
                receiveDone.WaitOne();

                Console.WriteLine("Response received : {0}", response);

                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }catch(SocketException se) {
                Console.WriteLine("StartClient SocketException Error : {0}", se.Message.ToString());
            }catch(Exception ex) {
                Console.WriteLine("StartClient Exceptino Error : {0}", ex.Message.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar) {
            try {
                Socket client = (Socket)ar.AsyncState;

                Console.WriteLine(client.LocalEndPoint);
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                connectDone.Set();
            } catch (SocketException se) {
                Console.WriteLine("ConnectCallback SocketException Error : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("ConnectCallback Exception Error : {0} ", ex.Message.ToString());
            }
        }

        private static void Receive(Socket client) {
            try {
                // Create the state object
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            } catch (SocketException se) {
                Console.WriteLine("Receive SocketException Error : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("Receive Exception Error : {0} ", ex.Message.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar) {
            try {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                } else {
                    if (state.sb.Length > 1)
                        response = state.sb.ToString();

                    receiveDone.Set();
                }
            } catch (SocketException se) {
                Console.WriteLine("ReceiveCallback SocketException Error : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("ReceiveCallback Exception Error : {0} ", ex.Message.ToString());
            }
        }

        private static void Send(Socket client, string data) {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                Socket client = ar.AsyncState as Socket;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server", bytesSent);

                sendDone.Set();
            } catch (SocketException se) {
                Console.WriteLine("SendCallback SocketException Error : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine("SendCallback Exception Error : {0} ", ex.Message.ToString());
            }
        }
        static void Main(string[] args) {
            StartClient();
            Console.ReadLine();
        }
    }
}
