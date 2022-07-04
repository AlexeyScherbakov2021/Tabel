using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;



namespace Tabel.Models
{
    [Table("typeDay")]
    public partial class typeDay : IEntity
    {

        public typeDay()
        {
            TabelPersons = new HashSet<TabelPerson>();
        }

        [Key]
        [Column("idTypeDay")]
        public int id { get; set; }

        [StringLength(2)]
        public string t_name { get; set; }

        [StringLength(200)]
        public string t_desc { get; set; }

        public virtual ICollection<TabelPerson> TabelPersons { get; set; }

    }
}
