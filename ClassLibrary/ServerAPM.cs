using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCPServer.Models;

namespace TCPServer
{
    public class ServerAPM : AbstractServer
    {
        readonly byte[] msgLogin;
        readonly byte[] msgPass;
        readonly byte[] wrongPass;
        static bool SHUTDOWN = false;
        ServerDatabaseContext context;
        UserManager um;

        public delegate void TransmissionDataDelegate(NetworkStream stream);
        public ServerAPM(string IPAddress = "127.0.0.1", int port = 8001, int bufferSize = 1024) : base(System.Net.IPAddress.Parse(IPAddress), port)
        {
            Buffer_size = bufferSize;
            this.msgLogin = new ASCIIEncoding().GetBytes("Podaj login: ");
            this.msgPass = new ASCIIEncoding().GetBytes("Podaj haslo: ");
            this.wrongPass = new ASCIIEncoding().GetBytes("Zly login lub haslo");
            var opBuilder = new DbContextOptionsBuilder<ServerDatabaseContext>();
            var conStringBuilder = new SqliteConnectionStringBuilder();
            conStringBuilder.DataSource = @"..\..\..\DBIO.db";
            opBuilder.UseSqlite(conStringBuilder.ConnectionString);
            this.context = new ServerDatabaseContext(opBuilder.Options);
            this.um = new UserManager(this.context);
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
            if (SHUTDOWN)
                throw new Exception("SHUTDOWN");
            // sprzątanie
        }

        /// <summary>
        /// This function will sign in user to server.
        /// </summary>
        /// <returns>
        /// An information about type of user:
        /// - 0 - user doesn't exist
        /// - 1 - administrator
        /// - 2 - normal user
        /// </returns>
        protected int signIn(NetworkStream stream, byte[] buffer)
        {
            char[] trim = { (char)0x0 };
            User u = new User();
            stream.Write(msgLogin, 0, msgLogin.Length);
            int dlugosc = stream.Read(buffer, 0, buffer.Length);
            if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
            {
                stream.Read(buffer, 0, buffer.Length);
            }
            string login = Encoding.ASCII.GetString(buffer).Trim(trim);
            Array.Clear(buffer, 0, buffer.Length);

            stream.Write(msgPass, 0, msgPass.Length);
            dlugosc = stream.Read(buffer, 0, buffer.Length);
            if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
            {
                stream.Read(buffer, 0, buffer.Length);
            }
            string password = Encoding.ASCII.GetString(buffer).Trim(trim);
            Array.Clear(buffer, 0, buffer.Length);

            u = um.verifyUser(login, password);
            if (u == null)
            {
                return 0;
            }
            else
            {
                if (u.isAdmin == true)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }

        protected void userHandler(string command, byte[] buffer, NetworkStream stream)
        {
            byte[] newLoginMsg = new ASCIIEncoding().GetBytes("Podaj nowy login: ");
            byte[] newPassMsg = new ASCIIEncoding().GetBytes("Podaj nowe haslo: ");
            char[] trim = { (char)0x0 };
            if (command == "addUser")
            {
                stream.Write(msgLogin, 0, msgLogin.Length);
                int dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string login = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);

                stream.Write(msgPass, 0, msgPass.Length);
                dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string password = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);
                um.addUser(login, password);
            }
            else if(command == "deleteUser")
            {
                stream.Write(msgLogin, 0, msgLogin.Length);
                int dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string login = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);

                stream.Write(msgPass, 0, msgPass.Length);
                dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string password = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);
                um.deleteUser(login, password);
            }
            else if(command == "updateUser")
            {
                stream.Write(msgLogin, 0, msgLogin.Length);
                int dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string login = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);

                stream.Write(msgPass, 0, msgPass.Length);
                dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string password = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);
                stream.Write(newLoginMsg, 0, newLoginMsg.Length);
                dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string newlogin = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);

                stream.Write(newPassMsg, 0, newPassMsg.Length);
                dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string newpassword = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);
                um.updateUser(login, password, newlogin, newpassword);
            }
        }

        protected void HandlarzSucharow(NetworkStream stream, int UserType)
        {
            string negatyw = "Ja tylko serwuje suchary\n";
            string instrukcja = "\r\n\"suchar\" wysyla suchara, \"nowy\" pozwala dodac suchara, \"quit\" rozlacza klienta,\r\nFUNKCJE ADMINA \r\n\"shutdown\" zamyka serwer, \"addUser\" dodaje uzytkownika, \"deleteUser\" usuwa uzytkownika, \"updateUser\" zmienia wlasnosci uzytkownika\r\n";
            string dodaj = "\nNapisz tutaj suchara, enter wysyla.\n";
            byte[] instr = Encoding.ASCII.GetBytes(instrukcja);
            byte[] bytes = Encoding.ASCII.GetBytes(negatyw);
            byte[] dod = Encoding.ASCII.GetBytes(dodaj);

            bool quit = false;

            byte[] buffer = new byte[Buffer_size];
            char[] trim = { (char)0x0 };
            JokeSQL generator = new JokeSQL(context);

            while (quit == false)
            {
                try
                {
                    stream.Write(instr, 0, instr.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //Odbieranie wiadomości

                int dlugosc = stream.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                string text = Encoding.ASCII.GetString(buffer).Trim(trim);
                Array.Clear(buffer, 0, buffer.Length);
                Console.WriteLine(text);

                //Rozpoznawanie otrzymanego komunikatu i odpowiedzi
                if (text == "shutdown" && UserType == 1)
                {
                    Console.WriteLine("Zamykam serwer\n");
                    stream.Close();
                    quit = true;
                    SHUTDOWN = true;
                }
                else if (text == "addUser" && UserType == 1)
                {
                    userHandler(text, buffer, stream);
                }
                else if (text == "deleteUser" && UserType == 1)
                {
                    userHandler(text, buffer, stream);
                }
                else if (text == "updateUser" && UserType == 1)
                {
                    userHandler(text, buffer, stream);
                }
                else if (text == "nowy")
                {
                    Console.WriteLine("Dodawanie suchara\n");
                    stream.Write(dod, 0, dod.Length);
                    dlugosc = stream.Read(buffer, 0, buffer.Length);
                    if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
                    {
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    string nowy = Encoding.ASCII.GetString(buffer).Trim(trim);
                    Array.Clear(buffer, 0, buffer.Length);
                    Console.WriteLine(nowy);
                    generator.AddJoke(nowy);
                }
                else if (text == "quit")
                {
                    Console.WriteLine("Rozłączam\n");
                    quit = true;
                    stream.Close();
                    //throw new ObjectDisposedException("Disconnect");
                }
                else if (text == "suchar")
                {
                    Console.WriteLine("Potwierdzam\n");
                    String sucharek = generator.GetJoke();
                    byte[] pozytyw = Encoding.ASCII.GetBytes(sucharek);
                    stream.Write(pozytyw, 0, pozytyw.Length);
                }
                else
                {
                    Console.WriteLine("Odrzucam\n");
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[Buffer_size];
            int userType = signIn(stream, buffer);
            try
            { 
                if (userType == 0)
                {
                    stream.Write(wrongPass, 0, wrongPass.Length);
                }
                else if (userType == 1)
                {
                    //Admin works
                    HandlarzSucharow(stream, userType);
                    //TODO: Admin query
                }
                else if (userType == 2)
                {
                    //Normal user works
                    HandlarzSucharow(stream, userType);
                    //TODO: Normal user query
                }
                else
                {
                    throw new Exception("Cos poszlo nie tak");
                }
            }
            catch (IOException e)
            {
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
