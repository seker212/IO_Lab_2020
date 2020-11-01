using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace TCPServer
{
    public class ServerAPM : AbstractServer
    {
        public delegate void TransmissionDataDelegate(NetworkStream stream);
        public ServerAPM(string IPAddress = "127.0.0.1", int port = 8001, int bufferSize = 1024) : base(System.Net.IPAddress.Parse(IPAddress), port)
        {
            Buffer_size = bufferSize;
        }

        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = TcpListener.AcceptTcpClient();
                Stream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                //callback style
                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
                // async result style
                //IAsyncResult result = transmissionDelegate.BeginInvoke(Stream, null, null);
                ////operacje......
                //while (!result.IsCompleted) ;
                ////sprzątanie
            }
        }

        private void TransmissionCallback(IAsyncResult ar)
        {
            // sprzątanie
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            using (Database db = new Database())
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[Buffer_size];
                        int message_size = stream.Read(buffer, 0, Buffer_size);
                        string command = Encoding.UTF8.GetString(buffer);
                        string[] cmd = command.Remove(command.IndexOf("\0")).Split(new Char[] {' '});
                        if (cmd[0] == "add")
                            db.Add(cmd[1]);
                        else if (cmd[0] == "list")
                        {
                            byte[] bufferOut = Encoding.ASCII.GetBytes(db.Get());
                            stream.Write(bufferOut, 0, bufferOut.Length);
                        }
                        else if (cmd[0] == "complete")
                            db.Complete(Int32.Parse(cmd[1]));
                    }
                    catch (IOException e)
                    {
                        break;
                    }
                }
            }
        }

        public override void Start()
        {
            StartListening();
            //transmission starts within the accept function
            AcceptClient();
        }
    }
}