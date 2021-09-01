using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace P2P
{
    class Ping : RPC
    {

        byte[] msg;

        public Ping (String ip, int port, uint size) : base (ip, port) 
        {
            int tempSize = (int)size;
            if (tempSize > 65527)
            {
                tempSize = 65527;
            }
            msg = new byte[tempSize];
            for (int i = 0; i < tempSize; i++)
            {
                msg[i] = 0;
            }
        }

        public Ping(String ip, int port) : this(ip, port, 32) { }

        public override void Start ()
        {
            connect();
            transmit(msg);
            disconnect();
        }
    }
}
