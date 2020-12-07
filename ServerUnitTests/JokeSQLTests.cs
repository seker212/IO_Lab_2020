using Microsoft.VisualStudio.TestTools.UnitTesting;
using TCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace TCPServer.Tests
{
    [TestClass()]
    public class JokeSQLTests
    {
        static private ServerDatabaseContext _context;
        static private JokeSQL _JokeSQL;

        [TestInitialize()]
        public void init()
        {
            var opBuilder = new DbContextOptionsBuilder<ServerDatabaseContext>();
            var conStringBuilder = new SqliteConnectionStringBuilder();
            conStringBuilder.DataSource = @"..\..\..\DBIO.db";
            opBuilder.UseSqlite(conStringBuilder.ConnectionString);
            _context = new ServerDatabaseContext(opBuilder.Options);
            _JokeSQL = new JokeSQL(_context);
        }

        [TestMethod()]
        public void GetJokeTest_correct()
        {
            var result = _JokeSQL.GetJoke();
            Assert.IsInstanceOfType(result, typeof(string));
        }

        [TestMethod()]
        public void AddJokeTest_correct()
        {
            var result = _JokeSQL.AddJoke("ala ma kota");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void AddJokeTest_incorrect()
        {
            var result = _JokeSQL.AddJoke("");
            Assert.IsFalse(result);
        }
    }
}