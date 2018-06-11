using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpChat1
{
    class User
    {
        const int TimeBetweenConnectionAttempts = 1000;  // In Miliseconds
        const int DefaultBufferSize = 1024;              // In Bytes

        private string username;
        private string friendIP;
        private int port;
        private int friendPort;
        private TcpListener tcpListener;
        private TcpClient outputTcpClient;
        private TcpClient inputTcpClient;
        public event EventHandler Connected;


        protected virtual void OnConnected()
        {
            if (this.Connected != null)
                Connected(this, EventArgs.Empty);
        }


        public User(string username, string friendIP, int port, int friendPort)
        {
            this.username = username;
            this.friendIP = friendIP;
            this.port = port;
            this.friendPort = friendPort;
            this.tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, this.port));
            this.outputTcpClient = new TcpClient();
        }


        public string Username { get { return this.username; } }
        public string FriendIP { get { return this.friendIP; } }
        public int Port { get { return this.port; } }
        public int FriendPort { get { return this.friendPort; } }
        private NetworkStream OutputStream { get { return this.outputTcpClient.GetStream(); } }
        private NetworkStream InputStream { get { return this.inputTcpClient.GetStream(); } }


        /// <summary>
        /// Connect to the other client
        /// </summary>
        public void Connect()
        {
            this.tcpListener.Start();
            bool isConnected = false;
            while (!isConnected)
            {
                isConnected = this.TryToConnect();
                if (!isConnected) Thread.Sleep(TimeBetweenConnectionAttempts);
            }
            this.OnConnected();
        }


        /// <summary>
        /// Try to connect to the other user
        /// </summary>
        /// <returns> Whether connected to the other user or not </returns>
        private bool TryToConnect()
        {
            try
            {
                this.outputTcpClient.Connect(new IPEndPoint(IPAddress.Parse(this.FriendIP), this.FriendPort));

                const int attempts = 3;
                for (int i = 0; i < attempts; i++)
                {
                    if (this.tcpListener.Pending())
                    {
                        this.inputTcpClient = tcpListener.AcceptTcpClient();
                        // In case connected successfully without throwing an exception
                        return true;
                    }
                    Thread.Sleep(TimeBetweenConnectionAttempts);
                }
                throw new Exception("Something went wrong!");   // In case all attempts failed
            }
            catch (SocketException)
            {
                return false;
            }

        }


        /// <summary>
        /// Send a message to the other user
        /// </summary>
        /// <param name="message">message to send</param>
        public void Send(string messageContent)
        {
            var message = new Message(messageContent, this);
            byte[] buffer = Encoding.UTF8.GetBytes(message.ToString());
            this.OutputStream.Write(buffer, 0, buffer.Length);
        }


        /// <summary>
        /// Receives a message from the other user
        /// </summary>
        /// <returns> The message from the other user </returns>
        public Message Receive()
        {
            return new Message(ReadFromStream(this.InputStream));
        }


        /// <summary>
        /// Closes the connection with the other user
        /// </summary>
        public void CloseConnection()
        {
            this.tcpListener.Stop();
            this.inputTcpClient.Close();
            this.inputTcpClient.Close();
        }


        /// <summary>
        /// Read data from given network stream
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>The stream data</returns>
        private static string ReadFromStream(NetworkStream stream)
        {
            var streamData = new StringBuilder();
            var buffer = new byte[DefaultBufferSize];

            do
            {
                int dataLength = stream.Read(buffer, 0, buffer.Length);
                streamData.Append(Encoding.UTF8.GetString(buffer, 0, dataLength));
            } while (stream.DataAvailable);

            return streamData.ToString();
        }


        public override string ToString()
        {
            return string.Format("{0}: My port -> {1}, Friend's port -> {2}, Friend's IP Address -> {3}", this.Username, this.Port, this.FriendPort, this.FriendIP);
        }
    }
}
