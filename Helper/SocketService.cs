using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Helper
{
    public class SocketService : ISocketService
    {
        private readonly Socket _socket;

        public SocketService()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketService(Socket socket)
        {
            _socket = socket;
        }

        public bool Connected()
        {
            return _socket.Connected;
        }

        public void Bind(IPEndPoint ipEndPoint)
        {
            _socket.Bind(ipEndPoint);
        }

        public void Close()
        {
            _socket.Close();
        }

        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        public int Receive(byte[] buffer)
        {
            return _socket.Receive(buffer);
        }

        public int Send(byte[] buffer)
        {
            return _socket.Send(buffer);
        }

        public void Shutdown(SocketShutdown how)
        {
            _socket.Shutdown(how);
        }
    }
}
