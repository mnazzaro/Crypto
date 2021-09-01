using System;
using System.Threading.Tasks;

namespace Discovery_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            DiscoveryServer server = new DiscoveryServer();
            //bool quit = false;
            //string command;

            //Task.Factory.StartNew(() => { server.start(); });
            //Console.WriteLine("Discovery Server started");
            //while (!quit)
            //{
            //    Console.Write("$ ");
            //    command = Console.ReadLine();

            //    switch (command)
            //    {
            //        case ("quit"):
            //            quit = true;
            //            break;
            //        default:
            //            Console.WriteLine(command + " is not a valid command");
            //            break;
            //    }
            //}
            server.start();
        }
    }
}
