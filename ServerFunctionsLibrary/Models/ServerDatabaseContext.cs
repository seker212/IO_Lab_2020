using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    public class ServerDatabaseContext : DbContext
    {
        public ServerDatabaseContext(DbContextOptions<ServerDatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Joke> Jokes { get; set; }
    }
}
