﻿using Microsoft.Data.Sqlite;
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
            //TODO: Add connection parameters
            conStringBuilder.DataSource = "C:\\Users\\stempel\\Documents\\baza\\DBIO.db";
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

        protected void HandlarzSucharow(NetworkStream stream, int UserType)
        {
            string negatyw = "Ja tylko serwuje suchary\n";
            string instrukcja = "\n\n\"suchar\" wysyla suchara, \"add\" pozwala dodac suchara, \"quit\" rozlacza klienta,  \"shutdown\" zamyka serwer (Admin only)\n";
            string dodaj = "\nNapisz tutaj suchara, enter wysyla.\n";
            byte[] instr = Encoding.ASCII.GetBytes(instrukcja);
            byte[] bytes = Encoding.ASCII.GetBytes(negatyw);
            byte[] dod = Encoding.ASCII.GetBytes(dodaj);


            byte[] buffer = new byte[Buffer_size];
            char[] trim = { (char)0x0 };
            JokeSQL generator = new JokeSQL(context);

            while (true)
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
                    //shutdown = true;
                    break;
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
                    stream.Close();
                    break;
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
            while (true)
            {
                try
                { 
                    if (userType == 0)
                    {
                        stream.Write(wrongPass, 0, wrongPass.Length);
                        break;
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
                    break;
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