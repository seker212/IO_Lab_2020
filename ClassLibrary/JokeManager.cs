using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.DAL;
using TCPServer.Models;

namespace TCPServer
{
    class JokeManager
    {

        public JokeRepository jr { get; set; }
        public JokeManager(ServerDatabaseContext context)
        {
            this.jr = new JokeRepository(context);
        }

    }
}
