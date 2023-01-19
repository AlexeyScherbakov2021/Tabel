using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("TargetTask")]
    public class TargetTask : IEntity
    {
        public int id { get; set; }
        public int? tt_ModPersonId { get; set; }

        [StringLength(150)]
        public string tt_name { get; set; }
        public decimal tt_proc_task { get; set; }

        public virtual ModPerson ModPerson { get; set; }

    }
}
