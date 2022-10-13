using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("AddWorks")]
    public partial class AddWorks : IEntity
    {
        [Key]
        [Column("aw_Id")]
        public int id { get; set; }
        
        [StringLength(150)]
        public string aw_Name { get; set; }

        [Column(TypeName = "numeric")]
        public decimal aw_Tarif { get; set; }

        public virtual ICollection<ModPerson> ListModPerson { get; set; }

        [NotMapped]
        public bool IsChecked { get; set; }
    }
}
