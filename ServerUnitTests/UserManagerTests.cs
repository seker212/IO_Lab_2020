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
    public class UserManagerTests
    {
        static private ServerDatabaseContext _context;
        static private UserManager _userManager;

        [TestInitialize()]
        public void init()
        {
            var opBuilder = new DbContextOptionsBuilder<ServerDatabaseContext>();
            var conStringBuilder = new SqliteConnectionStringBuilder();
            conStringBuilder.DataSource = @"..\..\..\DBIO.db";
            opBuilder.UseSqlite(conStringBuilder.ConnectionString);
            _context = new ServerDatabaseContext(opBuilder.Options);
            _userManager = new UserManager(_context);
        }

        [TestMethod()]
        public void addUserTest_correctUser()
        {
            var result = _userManager.addUser("login", "pasy");
            _userManager.deleteUser("login", "pasy");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void addUserTest_incorrectUser()
        {
            var result = _userManager.addUser("", "");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void deleteUserTest_nonexistantUser()
        {
            var result = _userManager.deleteUser("kloc", "colk");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void deleteUserTest_incorrectPassword()
        {
            _userManager.addUser("test1", "test2");
            var result = _userManager.deleteUser("test1", "incorrect");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void deleteUserTest_correctUser()
        {
            _userManager.addUser("test4", "test5");
            var result = _userManager.deleteUser("test4", "test5");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void updateUserTest_correctUser()
        {
            _userManager.addUser("test8", "test9");
            var result = _userManager.updateUser("test8", "test9", "8test", "9test");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void updateUserTest_nonexistanttUser()
        {
            _userManager.addUser("test0", "test1");
            var result = _userManager.updateUser("test6", "test7", "2test", "1test");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void verifyUserTest_defaultAdmin()
        {
            var result = _userManager.verifyUser("admin", "admin");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.isAdmin);
        }

        [TestMethod()]
        public void verifyUserTest_noneExistantUser()
        {
            var result = _userManager.verifyUser("asdfasfds", "o0iejrgfoi0jwe");
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void verifyUserTest_wrongAdminPassword()
        {
            var result = _userManager.verifyUser("admin", "o0iejrgfoi0jwe");
            Assert.IsNull(result);
        }
    }
}