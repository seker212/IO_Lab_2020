using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Models;

namespace TCPServer.DAL
{
    class UserRepository:BaseRepository<User>
    {
        public UserRepository(ServerDatabaseContext context) : base(context)
        {
        }

        public User getUser(string login)
        {
            return dbSet.SingleOrDefault(x => x.login == login);
        }
    }
}
