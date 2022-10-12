using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Models
{
    public partial class Otdel : Observable
    {

        [NotMapped]
        public bool IsChecked { get; set; }

    }
}
