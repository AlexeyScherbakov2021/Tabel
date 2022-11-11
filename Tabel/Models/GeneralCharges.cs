using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    public partial class GeneralCharges : IEntity
    {
        public GeneralCharges()
        {
            ListGenMonth = new HashSet<GenChargMonth>();
        }

        [Key]
        [Column("gen_Id")]
        public int id { get; set; }

        [StringLength(150)]
        public string gen_Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GenChargMonth> ListGenMonth { get; set; }
    }
}
