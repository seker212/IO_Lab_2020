using System;
using System.Net.Sockets;
using System.Text;

namespace TCPServer
{
    public class ServerSync : AbstractServer
    {

        public ServerSync(string IPAddress = "127.0.0.1", int port = 8001, int bufferSize = 1024) : base (System.Net.IPAddress.Parse(IPAddress), port)
        {
            Buffer_size = bufferSize;
        }

        protected override void AcceptClient()

        {

            TcpClient = TcpListener.AcceptTcpClient();

            byte[] buffer = new byte[Buffer_size];

            Stream = TcpClient.GetStream();

            BeginDataTransmission(Stream);

        }

        protected override void BeginDataTransmission(NetworkStream NStream)
        {
            byte[] Buffer = new byte[Buffer_size];
            bool even = true;
            while (true)
            {
                try
                {
                    NStream.Read(Buffer, 0, Buffer.Length);
                    if (even)
                    {
                        try
                        {
                            var tab = ToNumber(Buffer);
                            NStream.Write(tab, 0, tab.Length);
                        }
                        catch (Exception e) when (e.Message == "Empty Buffer") {}
                    }
                    else
                        NStream.Write(Buffer, 0, Buffer.Length);
                    even = !even;
                }
                catch (System.IO.IOException e) when (e.Message == "Unable to read data from the transport connection: An established connection was aborted by the software in your host machine.") {}
            }
        }

        /// <summary>
        /// Overrided comment.
        /// </summary>
        public override void Start()
        {
            StartListening();
            //transmission starts within the accept function
            AcceptClient();
        }

        byte[] ToNumber(byte[] buffer)
        {
            string numbers = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != 0)
                    numbers += buffer[i].ToString() + " ";
            }
            if (string.IsNullOrEmpty(numbers))
                throw new Exception("Empty Buffer");
            numbers.Remove(numbers.Length - 1);
            return Encoding.ASCII.GetBytes(numbers);
        }
    }
}
