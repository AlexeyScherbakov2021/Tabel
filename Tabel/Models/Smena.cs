using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("smena")]
    public class Smena : IEntity
    {
        public Smena()
        {
            ListSmenaPerson = new HashSet<SmenaPersonal>();
        }

        [Key]
        [Column("sm_Id")]
        public int id { get; set; }
        [StringLength(20)]
        public string sm_Number { get; set; }
        public DateTime sm_DateCreate { get; set; }
        public int sm_Month { get; set; }
        public int sm_Year { get; set; }
        public int sm_UserId { get; set; }
        public int sm_OtdelId { get; set; }
        public virtual User UserCreater { get; set; }
        public virtual Otdel Otdel { get; set; }
        public virtual ICollection<SmenaPersonal> ListSmenaPerson { get; set; }

    }
}
