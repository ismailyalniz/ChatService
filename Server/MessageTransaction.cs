using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    /// <summary>
    /// Keeps for message transaction for warn client when one client try to send more than 1 message per second the chat server 
    /// and close client connection when it happen again
    /// </summary>
    public class MessageTransaction
    {
        /// <summary>
        /// Current socket object
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// Message receieve date and time
        /// </summary>
        public DateTime ReceiveTime { get; set; }
    }
}
