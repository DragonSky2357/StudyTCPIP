using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _4 {
    class Client {
        public delegate void OnConnectEventHandler(Client sender, bool connected);
        public event OnConnectEventHandler OnConnect;

        public delegate void OnSendEventHandler(Client sender, int bytesSent);
        public event OnSendEventHandler OnSend;

        public delegate void OnReceiveEventHandler(string receiveData);
        public event OnReceiveEventHandler OnReceive;

        public delegate void OnDisconnectEventHandler(Client sender);
        public event OnDisconnectEventHandler OnDisconnect;

        private const int port = 11000;
        private string tcpServerIP = string.Empty;

        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        private string response = string.Empty;

        Socket client = null;

        public bool Connected {
            get {
                if (client != null) return client.Connected;
                return false;
            }
        }

        public Client(string serverIP) {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServerIP = serverIP;
        }

        public void Close() {
            if (client.Connected) {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                client = null;
                if (OnDisconnect != null)
                    OnDisconnect(this);
            }
        }

        public void StartClient(string sendData) {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(tcpServerIP), port);

            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();

            sendData += "<EOF>";
            Send(sendData);
            sendDone.WaitOne();

            Receive(client);
            receiveDone.WaitOne();

            Close();
        }

        public void Receive(Socket client) {
            try {
                StateObject state = new StateObject();
                state.workSocket = client;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                } else {
                    if (state.sb.Length > 1) {
                        response = state.sb.ToString();

                        if (OnReceive != null)
                            OnReceive(response);
                    }
                    receiveDone.Set();
                }
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(string data) {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);

                if (OnSend != null)
                    OnSend(this, bytesSent);

                sendDone.Set();
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public void ConnectCallback(IAsyncResult ar) {
            try {
                Socket client = ar.AsyncState as Socket;

                client.EndConnect(ar);

                if (OnConnect != null)
                    OnConnect(this, Connected);

                connectDone.Set();
            } catch (SocketException se) {
                Console.WriteLine("ConnectCallback [SocketException] Error : {0} ", se.Message.ToString());
            } catch (Exception ex) {
                Console.Write("ConnectCallback [Exception] Error : {0} ", ex.Message.ToString());
            }
        }
    }

    public class StateObject {
        // Client socket
        public Socket workSocket = null;
        // Size of receive buffer
        public const int BufferSize = 256;
        // Receive buffer
        public byte[] buffer = new byte[BufferSize];
        // Receiving data string
        public StringBuilder sb = new StringBuilder();
    }
}
