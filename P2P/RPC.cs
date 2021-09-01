using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace P2P
{
    abstract class RPC
    {

        public enum RPCState
        {
            Unconnected,
            Connected,
            Interrupted,
            Finished
        }

        protected String ip;
        protected int port;
        protected TcpClient client;
        public String Error { get; protected set; }
        protected delegate void ListenHandler(NetworkStream stream);
        public RPCState CurrentState { get; protected set; }

        protected static readonly byte[] s_endTransmission = Encoding.UTF8.GetBytes("<EOT>");

        public RPC (String ip, int port)
        {
            this.ip = ip;
            this.port = port;
            CurrentState = RPCState.Unconnected;
        }

        protected void transmit (byte[] message)
        {
            if (message != null && message.Length > 0)
            {
                if (CurrentState == RPCState.Connected)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(message, 0, message.Length);
                    } 
                    catch (SocketException e)
                    {
                        Console.WriteLine("Error in transmit: {0}", e.Message);
                        Error = e.Message;
                        if (!client.Connected)
                        {
                            CurrentState = RPCState.Interrupted;
                        }
                    }
                } 
            }
        }

        protected void receive (ListenHandler handler)
        {
            if (CurrentState == RPCState.Connected)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    handler(stream);
                } catch (SocketException e)
                {
                    Console.WriteLine("Error in connect: {0}", e.Message);
                    Error = e.Message;
                    if (!client.Connected)
                    {
                        CurrentState = RPCState.Interrupted;
                    }
                }
            }
        }

        protected void connect ()
        {
            if (CurrentState == RPCState.Unconnected)
            {
                try
                {
                    client = new TcpClient(ip, port);
                    if (client.Connected)
                    {
                        CurrentState = RPCState.Connected;
                    }
                } catch (SocketException e)
                {
                    Console.WriteLine("Error in connect: {0}", e.Message);
                    Error = e.Message;
                }
            } 
            else
            {
                throw new InvalidOperationException("Cannot connect unless in Unconnected state");
            }
        }

        public void disconnect ()
        {
            NetworkStream stream = client.GetStream();
            stream.Write(s_endTransmission);
            stream.Close();
            client.Close();
            CurrentState = RPCState.Finished;
        }

        public abstract void Start();
    }
}
