using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models2
{
    public partial class TransDay : Observable
    {
        [NotMapped]
        public bool OffDay { get; set; }

    }
}
