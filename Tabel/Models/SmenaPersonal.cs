using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("SmenaPerson")]
    public class SmenaPersonal : IEntity
    {
        public SmenaPersonal()
        {
            ListSmenaDays = new HashSet<SmenaDay>();
        }

        [Key]
        [Column("sp_Id")]
        public int id { get; set; }
        public int sp_PersonId { get; set; }
        public int? sp_SmenaId { get; set; }
        public int sp_Group { get; set; }
        public virtual Smena Smena { get; set; }
        public virtual Personal Person { get; set; }
        public virtual ICollection< SmenaDay> ListSmenaDays { get; set; }
    }
}
