using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Discovery_Server
{
    class DiscoveryServer
    {

        private List<IPAddress> _peerList;
        private HttpListener _listener;
        private volatile bool quit;
        private readonly Regex _versionRx = new Regex(@"^(\d+)\.(\d+)\.(\d+)$");

        public static readonly int MIN_MAJOR = 1;
        public static readonly int MIN_MINOR = 0;
        public static readonly int MIN_PATCH = 0;


        public DiscoveryServer ()
        {
            quit = true;
            _peerList = new List<IPAddress>();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:25796/");
        }

        public void start ()
        {
            quit = false;
            _listener.Start();
            while (!quit)
            {
                HttpListenerContext ctx = _listener.GetContext();
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;

                Console.WriteLine("* Received Request: " + request.RemoteEndPoint.ToString());

                byte[] buffer = processRequest(request);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }

            _listener.Close();
            Console.WriteLine("Discovery Server Stopped");
        }

        private bool versionUpToDate (int major, int minor, int patch)
        {
            return major >= MIN_MAJOR && minor >= MIN_MINOR && patch >= MIN_PATCH;
        }

        private byte[] processRequest (HttpListenerRequest request)
        {

            System.IO.StreamReader reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
            string body = reader.ReadToEnd();

            MatchCollection matches = _versionRx.Matches(body);

            if  (matches.Count == 1)
            {
                Console.WriteLine("  Valid request body");
                GroupCollection groups = matches[0].Groups;
                int major = Int32.Parse(groups[1].Value);
                int minor = Int32.Parse(groups[2].Value);
                int patch = Int32.Parse(groups[3].Value);
                StringBuilder responseBuilder = new StringBuilder();
                if (versionUpToDate (major, minor, patch))
                {
                    Console.WriteLine("  Valid version");
                    foreach (IPAddress ip in _peerList)
                    {
                        if (!ip.Equals(request.RemoteEndPoint.Address))
                        {
                            responseBuilder.Append(ip.ToString() + "\n");
                        }
                    }
                    Console.Write("  Sent Ip's:\n" + responseBuilder.ToString());
                    addToPeerList(request.RemoteEndPoint.Address);
                } else
                {
                    Console.WriteLine("  Invalid version");
                }
                return Encoding.UTF8.GetBytes(responseBuilder.ToString());
            } else
            {
                Console.WriteLine("  Invalid request body");
                return new byte[0];
            }
        }

        private bool addToPeerList (IPAddress ip)
        {
            if (!_peerList.Contains(ip))
            {
                _peerList.Add(ip);
                return true;
            }
            return false;
        }

        public void stop ()
        {
            quit = true;
        }

    }
}
