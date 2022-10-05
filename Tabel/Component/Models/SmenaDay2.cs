using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    public partial class SmenaDay
    {
        [NotMapped]
        public bool OffDay { get; set; }
    }
}
