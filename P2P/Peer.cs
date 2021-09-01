using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace P2P
{
    class Peer
    {

        private List<IPEndPoint> _peers;
        private const string DiscoveryServer = "http://192.168.0.16:25796/";

        private readonly HttpClient _discoveryClient = new HttpClient();

        private static PeerListener _listener = new PeerListener();

        public string version
        {
            get; private set;
        }

        public Peer(string version)
        {
            this.version = version;
            _peers = new List<IPEndPoint>();
        }

        // Ask for peers on the discovery server and add yourself to the list in the process
        public async Task Register()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(DiscoveryServer),
                    Content = new StringContent(version, Encoding.UTF8)
                };
                var response = await _discoveryClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                string peerList = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (peerList.Length > 0)
                {
                    string[] peerStrings = peerList.Split('\n');
                    Console.WriteLine(peerStrings[1]);
                    for (int i = 0; i < peerStrings.Length - 1; i++) // don't include last element because it will just be a '/n'
                    {
                        IPEndPoint temp = new IPEndPoint(IPAddress.Parse(peerStrings[i].Trim()), 4278);
                        _peers.Add(temp);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Error in meetPeers");
                Console.WriteLine(e.Message);
            }

        }

        //Add check to make sure there are peers in the list
        public void PingFirst ()
        {
            IPEndPoint target = _peers[0];
            Console.Write("Pinging {0}", target.ToString());
            Ping ping = new Ping(target.Address.ToString(), 4278);
            ping.Start();
        }

        public static void Listen()
        {
            _listener.Listen();
        }

        public void printPeers ()
        {
            Console.WriteLine("Current Peers: ");
            foreach (IPEndPoint peer in _peers)
            {
                Console.WriteLine(peer.ToString());
            }
        }

    }
}
