using System;
using System.Threading.Tasks;

namespace P2P
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Peer me = new Peer("1.0.0");
            Peer.Listen();
            bool quit = false;
            string command = null;
            while (!quit)
            {
                Console.Write("$ ");
                command = Console.ReadLine();

                switch (command)
                {
                    case ("quit"):
                        quit = true;
                        break;
                    case ("connect"):
                        await me.Register();
                        me.printPeers();
                        break;
                    case ("ping"):
                        me.PingFirst();
                        break;
                    default:
                        Console.WriteLine(command + " is not a valid command");
                        break;
                }
            }
        }
    }
}
