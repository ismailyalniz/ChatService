using Helper;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        private static readonly Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly string ip = "127.0.0.1";
        private static readonly int port = 4242;
        private static readonly int bufferSize = 4096;

        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
                InitializeClient();
                SendAndReceiveUntilExit();
                Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// It starts the client for connecto to server.
        /// </summary>
        private static void InitializeClient()
        {
            Console.WriteLine("Client is connecting...");
            Console.Title = "Chat Client";
            int connectionTryCount = 0;

            while (!socketClient.Connected)
            {
                try
                {
                    connectionTryCount++;
                    Console.WriteLine("trying to connect (" + connectionTryCount + ")");
                    socketClient.Connect(ip, port);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            Console.WriteLine("Client is connected.");
        }

        /// <summary>
        /// It sends message to server.
        /// </summary>
        private static void SendMessage()
        {
            Console.Write("Send a request: ");
            string text = Console.ReadLine();
            SendString(text);

            if (text.ToLower() == "exit")
                Exit();
        }

        /// <summary>
        /// It converts message to byte array and sends this array to server.
        /// </summary>
        /// <param name="text"></param>
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            socketClient.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// It receives message from server.
        /// </summary>
        private async static void ReceiveMessage()
        {
            var buffer = new byte[bufferSize];
            int receivedLength = await socketClient.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            if (receivedLength == 0)
            {
                var data = new byte[receivedLength];
                Array.Copy(buffer, data, receivedLength);
                Console.WriteLine(Encoding.ASCII.GetString(data));
            }
        }

        /// <summary>
        /// Exists from application and sends exit message to server.
        /// </summary>
        private static void Exit()
        {
            SendString("exit");
            Console.WriteLine("Exiting...");
            ReceiveMessage();
            Environment.Exit(0);
        }

        /// <summary>
        /// It provides continuous chatting (send message and receive message)
        /// </summary>
        private static void SendAndReceiveUntilExit()
        {
            try
            {
                Console.WriteLine(@"Type ""exit"" for exiting from server and auto close the client.");
                while (socketClient.Connected)
                {
                    SendMessage();
                    ReceiveMessage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.Log(ex);
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Exit();
        }


    }
}
