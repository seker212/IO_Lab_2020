using System;
using System.Net.Sockets;
using System.Text;

namespace TCPServer
{
    public class Server
    {
        TcpListener Listener;
        byte[] Buffer;

        public Server(string IPAddress = "127.0.0.1", int port = 8001, int bufferSize = 1024)
        {
            Listener = new TcpListener(System.Net.IPAddress.Parse(IPAddress), port);
            Buffer = new byte[bufferSize];
        }

        public void Run()
        {
            Listener.Start();

            var Client = Listener.AcceptTcpClient();
            var NStream = Client.GetStream();
            bool even = true;
            while (Client.Connected)
            {
                try
                {
                    NStream.Read(Buffer, 0, Buffer.Length);
                    if (even)
                    {
                        try
                        {
                            var tab = ToNumber();
                            NStream.Write(tab, 0, tab.Length);
                        }
                        catch (Exception e) when (e.Message == "Empty Buffer") {}
                    }
                    else
                        NStream.Write(Buffer, 0, Buffer.Length);
                    ClearBuffer();
                    even = !even;
                }
                catch (System.IO.IOException e) when (e.Message == "Unable to read data from the transport connection: An established connection was aborted by the software in your host machine.") {}
            }
            Client.Close();
            Listener.Stop();
        }

        void ClearBuffer()
        {
            Buffer = new byte[Buffer.Length];
        }

        byte[] ToNumber()
        {
            string numbers = "";
            for (int i = 0; i < Buffer.Length; i++)
            {
                if (Buffer[i] != 0)
                    numbers += Buffer[i].ToString() + " ";
            }
            if (string.IsNullOrEmpty(numbers))
                throw new Exception("Empty Buffer");
            numbers.Remove(numbers.Length - 1);
            return Encoding.ASCII.GetBytes(numbers);
        }
    }
}
