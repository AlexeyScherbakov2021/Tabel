using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    [Table("Transport")]
    public class Transport : IEntity
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transport()
        {
            //TransportPerson = new List<TransPerson>();
        }


        [Key]
        [Column("tr_Id")]
        public int id { get; set; }
        [StringLength(20)]
        public string tr_Number { get; set; }

        public DateTime tr_DateCreate { get; set; }

        public int tr_Month { get; set; }

        public int tr_Year { get; set; }

        public int? tr_UserId { get; set; }

        public int? tr_OtdelId { get; set; }

        public virtual Otdel otdel { get; set; }

        public virtual User user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<TransPerson> TransportPerson { get; set; }



    }
}
