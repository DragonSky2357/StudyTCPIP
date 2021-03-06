﻿// Socket 통신 - Multiple Connection/Accepting Connections(Server)

using System;
using System.Net;
using System.Net.Sockets;

namespace _4 {
    class Listener {
        Socket s;
        public bool Listening { get; private set; }
        public int Port { get; private set; }
        public Listener(int port) {
            Port = port;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start(string tcpServerIP) {
            if (Listening) return;

            s.Bind(new IPEndPoint(IPAddress.Parse(tcpServerIP), Port));
            s.Listen(0);

            s.BeginAccept(callback, null);
            Listening = true;
        }
        public void Stop() {
            if (!Listening) return;

            s.Close();
            s.Dispose();

            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        void callback(IAsyncResult ar) {
            try {
                Socket socket = s.EndAccept(ar);

                if (SocketAccepted != null) {
                    SocketAccepted(socket);
                }
                
                s.BeginAccept(callback, null);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
        public delegate void SocketAcceptedHandler(Socket e);
        public event SocketAcceptedHandler SocketAccepted;
    }
}
