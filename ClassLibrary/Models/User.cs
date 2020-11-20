using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    class User
    {
        //TODO: User Model by LW
        [Key]
        public int ID { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public bool isAdmin { get; set; }
    }
}
