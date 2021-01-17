using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Models;

namespace TCPServer.DAL
{
    public class JokeRepository : BaseRepository<Joke>
    {
       
        public JokeRepository(ServerDatabaseContext context) : base(context)
        {

        }

        public int GetNum()
        {
            return dbSet.Count();
        }

        public List<int> getIDList()
        {
            var jokelist = dbSet.Select(x => x.ID).ToList();
            return jokelist;
        }

        public List<Joke> getJokeList()
        {
            var jokelist = dbSet.ToList();
            return jokelist;
        }

    }
}
