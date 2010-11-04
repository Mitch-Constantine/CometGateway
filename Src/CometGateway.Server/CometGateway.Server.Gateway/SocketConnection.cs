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
        private byte[] inputBuffer = new byte[1000];

        public void StartConnect(string server, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(server, port, OnConnectionCompleted, null);
        }

        public void StartDisconnect()
        {
            socket.BeginDisconnect(false, OnDisconnectCompleted, null);
        }

        public void Send(byte[] data)
        {
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSendCompleted, null);
        }

        public bool Connected
        {
            get { return socket != null && socket.Connected; }
        }

        public event Action ConnectionSucceeded;
        public event Action<string> ErrorOccurred;
        public event Action ServerDisconnected;
        public event Action<byte[]> DataReceived;

        private void OnConnectionCompleted(IAsyncResult ar)
        {
            OnOperationCompleted(
                ar,
                () => socket.EndConnect(ar),
                () =>
                    {
                        OnConnectionSucceeded();
                        BeginRead();
                    }
            );
        }

        private void BeginRead()
        {
            socket.BeginReceive(inputBuffer, 0, inputBuffer.Length, SocketFlags.None, OnReceiveCompleted, null);
        }

        private void OnSendCompleted(IAsyncResult ar)
        {
            OnOperationCompleted(ar, () => socket.EndSend(ar), null);
        }

        private void OnReceiveCompleted(IAsyncResult ar)
        {
            int bytesReceived = 0;
            OnOperationCompleted(
                ar,
                () => bytesReceived = socket.EndReceive(ar),
                () =>
                {
                    if (bytesReceived > 0)
                    {
                        byte[] outputBytes = new byte[bytesReceived];
                        Array.Copy(inputBuffer, outputBytes, bytesReceived);
                        if (DataReceived != null)
                            DataReceived(outputBytes);
                    }
                    BeginRead();
                }
            );
        }

        private void OnOperationCompleted(IAsyncResult ar, Action toComplete, Action onSuccess)
        {
            bool successful = true;

            try
            {
                toComplete();
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception) { }
                successful = false;
            }

            if (successful && onSuccess != null)
                onSuccess();
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
