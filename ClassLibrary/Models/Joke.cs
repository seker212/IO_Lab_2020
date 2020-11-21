using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Models
{
    [Table("Joke")]
    class Joke
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Content")]
        public string Content { get; set; }
        
    }
}
