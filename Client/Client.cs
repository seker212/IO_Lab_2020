using ClientFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            ClientConnection cc = new ClientConnection(IPAddress.Parse("127.0.0.1"), 8001);
            TcpClient client = cc.createClient();
            byte[] buffer = new byte[1024];
            NetworkStream stream = client.GetStream();
            while(true)
            {
                try
                {
                    cc.clientLoop(stream, buffer);
                }
                catch(Exception e) { System.Console.Write(e.StackTrace); }
            }
        }
    }
}
