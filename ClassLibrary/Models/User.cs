using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    public class User
    {
        
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Login")]
        public string login { get; set; }
        [Column("Password")]
        public string password { get; set; }
        [Column("isAdmin")]
        public bool isAdmin { get; set; }
    }
}
