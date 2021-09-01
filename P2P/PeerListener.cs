using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace P2P
{
    class PeerListener : TcpListener
    {

        private static readonly byte[] s_endTransmission = Encoding.UTF8.GetBytes("<EOT>");
        public PeerListener() : base(IPAddress.Any, 4278) { }

        public void Listen ()
        {
            if (!Active)
            {
                Start();
            }
            Console.WriteLine("Listening");
            BeginAcceptTcpClient(new AsyncCallback(onNewClient), null);
        }

        private void onNewClient (IAsyncResult asyncClient)
        {
            try
            {
                TcpClient client = EndAcceptTcpClient(asyncClient);
                ParameterizedThreadStart ts = new ParameterizedThreadStart(handleClient);
                Thread t = new Thread(ts);
                t.Start(client);
            } catch (Exception e)
            {
                Console.WriteLine("Error in onNewClient: {0}", e.Message);
            }
        }
     
        private static void handleClient (object clientObj)
        {
            TcpClient client = (TcpClient) clientObj;
            NetworkStream stream = client.GetStream();

            StringBuilder data = new StringBuilder();
            byte[] dataBuffer = new byte[256];
            int i;

            int count = 1;
            do
            {
                i = stream.Read(dataBuffer, 0, dataBuffer.Length);
                Console.WriteLine("Loop {0}", count);
                data.Append(Encoding.UTF8.GetString(dataBuffer, 0, i));
                Console.WriteLine("Received {0}", data.ToString());
                Console.WriteLine("Last Five: {0}", Encoding.UTF8.GetString(dataBuffer[(i-5)..i]));
                Console.WriteLine("i: {0}", i);
                count += 1;
            } while (i != 0 && !Enumerable.SequenceEqual(dataBuffer[(i-5)..i], s_endTransmission));

            byte[] response = Encoding.UTF8.GetBytes("Message Received");
            stream.Write(response, 0, response.Length);
            Console.WriteLine("Sent: {0}", response);

            stream.Close();
            client.Close();
        }
    }
}
