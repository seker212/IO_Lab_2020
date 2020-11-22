using System;
using System.IO;
using TCPServer;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write(Directory.GetCurrentDirectory());
            var Server = new ServerAPM();
            Server.Start();
            //Test.test();
        }
    }
}
