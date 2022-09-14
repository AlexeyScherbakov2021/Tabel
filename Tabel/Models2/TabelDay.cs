using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    [Table("TabelDay")]
    public partial class TabelDay : IEntity
    {
        [Key]
        [Column("td_Id")]
        public int id { get; set; }

        public int? td_TabelPersonId { get; set; }

        public int td_Day { get; set; }
        public int? td_Kind { get; set; }
        public int td_Hours { get; set; }

        public virtual typeDay Kind { get; set; }

        public virtual Personal person { get; set; }

        public virtual TabelPerson TabelPerson { get; set; }
    }
}
