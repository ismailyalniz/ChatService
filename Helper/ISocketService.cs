using System.Net;
using System.Net.Sockets;

namespace Helper
{
    public interface ISocketService
    {
        bool Connected();
        void Close();
        void Shutdown(SocketShutdown how);
        int Send(byte[] buffer);
        int Receive(byte[] buffer);
        void Bind(IPEndPoint ipEndPoint);
        void Listen(int backlog);
        //ISocketService Accept();
    }
}
