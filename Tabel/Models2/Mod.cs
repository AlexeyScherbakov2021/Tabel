using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    [Table("Mod")]
    public partial class Mod : IEntity
    {
        public Mod()
        {
            ModPersons = new HashSet<ModPerson>();
        }

        [Key]
        [Column("m_Id")]
        public int id {get;set;}
        public int m_year { get; set; }
        public int m_month { get; set; }
        public int m_author { get; set; }
        public int m_otdelId { get; set; }

        public virtual User Author { get; set; }
        public virtual Otdel Otdel { get; set; }

        public virtual ICollection<ModPerson> ModPersons { get; set; }  
    }
}
