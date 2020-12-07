using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.DAL;
using TCPServer.Models;

namespace TCPServer
{
    public class JokeSQL
    {

        JokeRepository x;

        public JokeSQL(ServerDatabaseContext context)
        {
            x = new JokeRepository(context);
        }


        public string GetJoke()
        {
            Random rnd = new Random();
            int jokeID = rnd.Next(1, x.GetNum()+1);
            Joke joke = x.GetByID(jokeID);
            return joke.Content;
        }


        public void AddJoke(String txt)
        {
            Joke nowy = new Joke();
            nowy.Content = txt;
            x.Insert(nowy);
            x.Commit();
        }

    }
}
