using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public class SocketConnection : ISocketConnection
    {
        private Socket socket;

        public void StartConnect(string server, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(server, port, OnConnectionCompleted, null);
        }

        public void StartDisconnect()
        {
            socket.BeginDisconnect(false, OnDisconnectCompleted, null);
        }

        public bool Connected
        {
            get { return socket != null && socket.Connected; }
        }

        public event Action ConnectionSucceeded;
        public event Action<string> ErrorOccurred;
        public event Action ServerDisconnected;

        private void OnConnectionCompleted(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
                try
                {
                    socket.Dispose();
                    socket = null;
                }
                catch (Exception) {}
                return;
            }

            OnConnectionSucceeded();
        }

        private void OnDisconnectCompleted(IAsyncResult ar)
        {
            try
            {
                socket.EndDisconnect(ar);
                socket.Dispose();
                socket = null;
            }
            catch (Exception) {}
            
            OnDisconnected();
        }

        private void OnConnectionSucceeded()
        {
            if (ConnectionSucceeded != null)
                ConnectionSucceeded();
        }

        private void OnErrorOccurred(string message)
        {
            if (ErrorOccurred != null)
                ErrorOccurred(message);
        }

        private void OnDisconnected()
        {
            if (ServerDisconnected != null)
                ServerDisconnected();
        }
    }
}
