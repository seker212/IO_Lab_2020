using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.DAL;
using TCPServer.Models;

namespace TCPServer
{
    class UserManager
    {
        public UserRepository ur { get; set; }

        public UserManager(ServerDatabaseContext context)
        {
            this.ur = new UserRepository(context);
        }

        public bool addUser(string login, string password) 
        {
            User u = new User();
            User u2 = this.ur.getUser(login);
            u.login = login;
            u.password = password;
            u.isAdmin = false;

            if(u2 == null)
            {
                this.ur.Insert(u);
                ur.Commit();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool deleteUser(string login, string password) 
        {
            User u = this.ur.getUser(login);

            if(u == null)
            {
                return false;
            }
            else if(u.password != password)
            {
                return false;
            }
            else
            {
                this.ur.Delete(u);
                ur.Commit();
                return true;
            }
        }
        public bool updateUser(string login, string password, string newlogin, string newpassword)
        {
            User u = new User();
            User u2 = this.ur.getUser(login);
            User u3 = new User();
            u.ID = u2.ID;
            u.login = login;
            u.password = password;
            u.isAdmin = false;

            if(u2 == null)
            {
                return false;
            }
            else
            {
                u3.ID = u2.ID;
                u3.login = newlogin;
                u3.password = newpassword;
                u3.isAdmin = false;
                this.ur.Update(u3);
                ur.Commit();
                return false;
            }
        }
        public User verifyUser(string login, string password)
        {
            User u = this.ur.getUser(login);
            if(u == null)
            {
                return null;
            }
            else if(u.password != password)
            {
                return null;
            }
            else
            {
                return u;
            }
        }
    }
}
