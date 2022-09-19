using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    public partial class ModPerson
    {
        [NotMapped]
        public decimal TabelHours { get; set; }
        [NotMapped]
        public int TabelDays { get; set; }
        [NotMapped]
        public int TabelAbsent { get; set; }
        [NotMapped]
        public decimal Oklad { get; set; }

        //public decimal Proc
    }
}
