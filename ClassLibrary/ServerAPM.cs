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
        static bool SHUTDOWN = false;
        ServerDatabaseContext context;
        UserManager um;

        public delegate void TransmissionDataDelegate(NetworkStream stream);
        public ServerAPM(string IPAddress = "127.0.0.1", int port = 8001, int bufferSize = 1024) : base(System.Net.IPAddress.Parse(IPAddress), port)
        {
            Buffer_size = bufferSize;
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
                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
            }
        }
        
        private void TransmissionCallback(IAsyncResult ar)
        {
            if (SHUTDOWN)
                throw new Exception("SHUTDOWN");
        }
        
        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[Buffer_size];
            UserType userType = SignIn(stream, buffer);
            try
            { 
                if (userType == UserType.NotValid)
                {
                    var wrongPass = new ASCIIEncoding().GetBytes("Zly login lub haslo");
                    stream.Write(wrongPass, 0, wrongPass.Length);
                }
                else if (userType == UserType.Standard || userType == UserType.Admin)
                {
                    CommandHandler(stream, buffer, userType);
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

        protected UserType SignIn(NetworkStream stream, byte[] buffer)
        {
            var cridentials = GetCridentials(buffer, stream, false);
            var user = um.verifyUser(cridentials[0], cridentials[1]);
            if (user == null)
                return UserType.NotValid;
            else
            {
                if (user.isAdmin == true)
                    return UserType.Admin;
                else
                    return UserType.Standard;
            }
        }

        private string[] GetCridentials(byte[] buffer, NetworkStream stream, bool messagesForUpdating)
        {
            string[] result = new string[2];

            if (messagesForUpdating)
            {
                result[0] = GetStringFromUser("Podaj nowy login: ", stream, buffer);
                result[1] = GetStringFromUser("Podaj nowe haslo: ", stream, buffer);
            }
            else
            {
                result[0] = GetStringFromUser("Podaj login: ", stream, buffer);
                result[1] = GetStringFromUser("Podaj haslo: ", stream, buffer);
            }

            return result;
        }

        protected void CommandHandler(NetworkStream stream, byte[] buffer, UserType userType)
        {
            string negatyw = "Ja tylko serwuje suchary\r\n";
            string helpString = "POLECENIA\r\n\t\"suchar\" wysyla suchara, \r\n\t\"nowy\" pozwala dodac suchara, \r\n\t\"quit\" rozlacza klienta,\r\nPOLECENIA ADMINA \r\n\t\"shutdown\" zamyka serwer, \r\n\t\"addUser\" dodaje uzytkownika, \r\n\t\"deleteUser\" usuwa uzytkownika, \r\n\t\"updateUser\" zmienia wlasnosci uzytkownika\r\n";
            string addJokeString = "\r\nNapisz tutaj suchara, enter wysyla.\r\n";
            string response = "Zmiany zostaly wprowadzone pomyslnie\r\n";
            byte[] helpByte = new ASCIIEncoding().GetBytes(helpString);
            byte[] responseByte = new ASCIIEncoding().GetBytes(response);

            bool quit = false;
            JokeSQL generator = new JokeSQL(context);
            
            //Wypisanie help'a
            stream.Write(helpByte, 0, helpByte.Length);
            while (quit == false)
            {
                string command = GetStringFromUser(null, stream, buffer);
                Console.WriteLine(command);

                //Rozpoznawanie otrzymanego komunikatu i odpowiedzi
                if (command == "shutdown" && userType == UserType.Admin)
                {
                    Console.WriteLine("Zamykam serwer\r\n");
                    stream.Close();
                    quit = true;
                    SHUTDOWN = true;
                }
                else if (command == "addUser" && userType == UserType.Admin)
                {
                    Console.WriteLine("addUser Invoked\r\n");
                    var cridentials = GetCridentials(buffer, stream, false);
                    um.addUser(cridentials[0], cridentials[1]);
                    stream.Write(responseByte, 0, responseByte.Length);
                }
                else if (command == "deleteUser" && userType == UserType.Admin)
                {
                    Console.WriteLine("deleteUser Invoked\r\n");
                    var cridentials = GetCridentials(buffer, stream, false);
                    um.deleteUser(cridentials[0], cridentials[1]);
                    stream.Write(responseByte, 0, responseByte.Length);
                }
                else if (command == "updateUser" && userType == UserType.Admin)
                {
                    Console.WriteLine("updateUser Invoked\r\n");
                    var cridentials = GetCridentials(buffer, stream, false);
                    var newCridentials = GetCridentials(buffer, stream, false);
                    um.updateUser(cridentials[0], cridentials[1], newCridentials[0], newCridentials[1]);
                    stream.Write(responseByte, 0, responseByte.Length);
                }
                else if (command == "nowy")
                {
                    Console.WriteLine("Dodawanie suchara\n");
                    string nowy = GetStringFromUser(addJokeString, stream, buffer);
                    generator.AddJoke(nowy);
                }
                else if (command == "quit")
                {
                    Console.WriteLine("Rozłączam\n");
                    quit = true;
                    stream.Close();
                }
                else if (command == "suchar")
                {
                    Console.WriteLine("Potwierdzam\n");
                    String sucharek = generator.GetJoke();
                    byte[] pozytyw = Encoding.ASCII.GetBytes(sucharek);
                    stream.Write(pozytyw, 0, pozytyw.Length);
                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(negatyw);
                    Console.WriteLine("Odrzucam\n");
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        string GetStringFromUser(string message, NetworkStream stream, byte[] buffer)
        {
            char[] trim = { (char)0x0 };

            if (!string.IsNullOrWhiteSpace(message))
            {
                byte[] byteMessage = new ASCIIEncoding().GetBytes(message);
                stream.Write(byteMessage, 0, byteMessage.Length);
            }
            int dlugosc = stream.Read(buffer, 0, buffer.Length);
            if (Encoding.ASCII.GetString(buffer, 0, dlugosc) == "\r\n")
            {
                stream.Read(buffer, 0, buffer.Length);
            }
            string resultText = Encoding.ASCII.GetString(buffer).Trim(trim);
            Array.Clear(buffer, 0, buffer.Length);

            return resultText;
        }
    }
}
