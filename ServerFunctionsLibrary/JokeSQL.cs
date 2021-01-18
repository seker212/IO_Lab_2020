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

        public struct suchar
        {
            
            public suchar(int Id, int CreatorID, string Content)
            {
                id = Id;
                creatorID = CreatorID;
                content = Content;
            }

            public int id { get; set;  }
            public int creatorID { get; set; }
            public string content { get; set; }

            public string Stringify()
            {
                string s = "|" + this.id.ToString() + "|" + this.creatorID.ToString() + "|" + this.content + "|\r\n";
                return s;
            }
        }

        JokeRepository x;
        UserRepository y;

        public JokeSQL(ServerDatabaseContext context)
        {
            x = new JokeRepository(context);
            y = new UserRepository(context);
        }


        public string GetJoke()
        {
            Random rnd = new Random();
            //int jokeID = rnd.Next(1, x.GetNum()+1);
            //Joke joke = x.GetByID(jokeID);
            var list = x.getIDList();
            int index = rnd.Next(list.Count);
            Joke choosen = x.GetByID(list[index]);

            string jk = (string)choosen.Content;
            jk = jk.Replace("\\r\\n", "\r\n");
            return jk;
        }


        public bool AddJoke(String txt, ref string[] actualUser)
        {
            string login = actualUser[0];
            User usr = y.getUser(login);
            int userID = usr.ID;
            

            if (string.IsNullOrEmpty(txt))
            {
                return false;
            }
            else
            {
                txt = txt + "\\r\\n";
                Joke nowy = new Joke();
                nowy.Content = txt;
                nowy.CreatorID = userID;
                x.Insert(nowy);
                x.Commit();
                return true;
            }
        }

        public bool delJoke(string usuw)
        {
            
            if (usuw == null)
            {
                return false;
            }
            else
            {
                int usuwID = int.Parse(usuw);
                Joke usuwany = this.x.GetByID(usuwID);
                this.x.Delete(usuwany);
                x.Commit();
                return true;
            }
            
        }

        public string listJokes(ref string[] actualUser)
        {
            string login = actualUser[0];
            User usr = y.getUser(login);
            int userID = usr.ID;
            bool admin = usr.isAdmin;
            //Joke joke;
            var ilosc = x.getIDList();

            suchar[] lista = new suchar[ilosc.Count()+1];
            String output = "\n\r";

            /*int maxJokes = x.GetNum() + 1;
            for (int i = 1; i < maxJokes; i++)
            {
                joke = x.GetByID(i);
                suchar z = new suchar(i, joke.CreatorID, joke.Content);
                lista[i] = z;
            }*/

            var list = x.getJokeList();
            int l = 1;
            foreach (Joke elem in list)
            {
                suchar z = new suchar(elem.ID, elem.CreatorID, elem.Content);
                lista[l] = z;
                l++;
            }

            if (admin == true)
            {
                for(int i = 1; i < lista.Length; i++)
                {
                    suchar y = lista[i];
                    output = output + y.Stringify();
                }
                return output;
            }
            else
            {
                for (int i = 1; i < lista.Length; i++)
                {
                    suchar y = lista[i];
                    if(userID == y.creatorID)
                    {
                        output = output + y.Stringify();
                    }
                }
                return output;
            }
        }

    }
}
