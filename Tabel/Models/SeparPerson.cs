using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("SeparPerson")]
    public  class SeparPerson : IEntity
    {
        public int id { get; set; }
        public int? sp_separId { get; set; }
        public int sp_personalId { get; set; }

        public virtual Separate Separate { get; set; }
        public virtual Personal person { get; set; }

        [NotMapped]
        public int? TabelDays { get; set; }

        [NotMapped]
        public decimal? TabelHours { get; set; }

        [NotMapped]
        public decimal? Oklad { get; set; }


    }
}
