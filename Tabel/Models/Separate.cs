using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("Separate")]
    public class Separate : IEntity
    {
        public Separate()
        {
            ListSeparPerson = new HashSet<SeparPerson>();
        }

        public int id { get; set; }
        public int s_otdelId { get; set; }
        public int s_year { get; set; }
        public int s_month { get; set;}

        [StringLength(50)]
        public string s_author { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeparPerson> ListSeparPerson { get; set; }

    }
}
