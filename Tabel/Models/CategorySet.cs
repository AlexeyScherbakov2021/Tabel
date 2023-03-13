using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("CaterogySet")]
    public class CategorySet : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategorySet()
        {
            ListCategory = new HashSet<Category>();
        }

        [Key]
        [Column("cg_id")]
        public int id { get; set; }

        public DateTime? cg_date { get; set; }

        [Column(TypeName = "numeric")]
        public decimal cg_value { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Category> ListCategory { get; set; }
    }
}
