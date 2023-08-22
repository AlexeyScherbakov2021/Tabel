using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    [Table("AddOnceWork")]
    public class AddOnceWork : Observable, IEntity
    {
        public int id { get; set; }
        public int? ao_ModPersonId { get; set; }

        //[StringLength(150)]
        public string ao_name { get; set; }

        private decimal? _ao_summa;
        public decimal? ao_summa { get => _ao_summa; set { Set(ref _ao_summa, value); } }

        public virtual ModPerson ModPerson { get; set; }

    }
}
