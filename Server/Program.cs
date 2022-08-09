using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private static readonly Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly string ip = "127.0.0.1";
        private static readonly int port = 4242;
        private static List<Socket> clientList = new List<Socket>();
        private static readonly int bufferSize = 4096;
        private static readonly byte[] buffer = new byte[bufferSize];
        private static List<MessageTransaction> messageTransactions = new List<MessageTransaction>();

        static void Main(string[] args)
        {
            try
            {
                InitializeServer();
                Console.ReadLine();
                CloseAllClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// It starts the server.
        /// </summary>
        private static void InitializeServer()
        {
            Console.WriteLine("Server is starting...");
            Console.Title = "Chat Server";
            socketServer.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            socketServer.Listen(0);
            socketServer.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server is started.");
        }

        /// <summary>
        /// It accepts connection request and it joins the client.
        /// </summary>
        /// <param name="result"></param>
        private static void AcceptCallback(IAsyncResult result)
        {
            Socket socket = socketServer.EndAccept(result);
            clientList.Add(socket);
            socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, ReceiveCallback, socket);
            socketServer.BeginAccept(AcceptCallback, null);
            Console.WriteLine("A client joined.");
        }

        /// <summary>
        /// It receive message from client and do some rules for disconnecting or warning clients
        /// </summary>
        /// <param name="result"></param>
        private static void ReceiveCallback(IAsyncResult result)
        {
            var receiveTime = DateTime.Now;
            receiveTime = new DateTime(receiveTime.Year, receiveTime.Month, receiveTime.Day, receiveTime.Hour, receiveTime.Minute, receiveTime.Second, 0);
            Socket socketCurrent = (Socket)result.AsyncState;
            int receivedLength = socketCurrent.EndReceive(result);
            byte[] receivedData = new byte[receivedLength];
            Array.Copy(buffer, receivedData, receivedLength);
            string text = Encoding.ASCII.GetString(receivedData);
            Console.WriteLine("Received Text: " + text);
            if (messageTransactions.Count(t => t.Socket == socketCurrent && t.ReceiveTime == receiveTime) == 1)
            {
                Console.WriteLine("Text is an invalid request");
                byte[] data = Encoding.ASCII.GetBytes("Warning! You can not more than 1 message per second. If this happen again you will disconnect from chat server.");
                socketCurrent.Send(data);
                Console.WriteLine("Warning sent to client.");
            }
            else if (messageTransactions.Count(t => t.Socket == socketCurrent && t.ReceiveTime == receiveTime) == 2)
            {
                string message = "Client automatically disconnected by server for sending more than 1 message per second.";
                byte[] data = Encoding.ASCII.GetBytes(message);
                socketCurrent.Send(data);
                socketCurrent.Shutdown(SocketShutdown.Both);
                socketCurrent.Close();
                clientList.Remove(socketCurrent);
                Console.WriteLine(message);
            }
            else if (text.ToLower() == "exit")
            {
                socketCurrent.Shutdown(SocketShutdown.Both);
                socketCurrent.Close();
                clientList.Remove(socketCurrent);
                Console.WriteLine("Client disconnected");
                return;
            }

            socketCurrent.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, ReceiveCallback, socketCurrent);
            messageTransactions.Add(new MessageTransaction { Socket = socketCurrent, ReceiveTime = receiveTime });
        }

        /// <summary>
        /// It close all clients from server.
        /// </summary>
        private static void CloseAllClient()
        {
            foreach (Socket socket in clientList)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            socketServer.Close();
        }
    }
}
