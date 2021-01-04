using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientFunctionsLibrary
{
    public class ClientConnection
    {
        protected int port;
        protected IPAddress address;
        protected int Buffer_size;
        public ClientConnection(IPAddress address ,int port, int bufferSize = 1024)
        {
            this.address = address;
            this.port = port;
            Buffer_size = bufferSize;
        }

        public TcpClient createClient()
        {
            TcpClient c = new TcpClient();
            c.Connect(address, port);
            return c;
        }

        public void sendMessage(NetworkStream stream, string msg)
        {
            byte[] msgByte = new ASCIIEncoding().GetBytes(msg);
            stream.Write(msgByte, 0, msgByte.Length);
        }

        public string reciveMessage(NetworkStream stream, byte[] buffer)
        {
            char[] trim = { (char)0x0 };
            int len = stream.Read(buffer, 0, buffer.Length);
            if (Encoding.ASCII.GetString(buffer, 0, len) == "\r\n")
            {
                stream.Read(buffer, 0, buffer.Length);
            }
            string rec = Encoding.ASCII.GetString(buffer).Trim(trim);
            Array.Clear(buffer, 0, buffer.Length);
            return rec;
        }

        public void clientLoop(ref NetworkStream stream, byte[] buffer, ref TcpClient client)
        {
            Console.Write(reciveMessage(stream, buffer));
            string msg = Console.ReadLine();
            sendMessage(stream, msg);
            if (msg == "quit")
            {
                closeClient(ref client, ref stream);
                Console.WriteLine("Czy chcesz sie polaczyc ponownie? (y/n)");
                string res = Console.ReadLine();
                if (res == "y")
                {
                    client = createClient();
                    stream = client.GetStream();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        public void closeClient(ref TcpClient client, ref NetworkStream stream)
        {
            stream.Close();
            client.Close();
        }
    }
}
