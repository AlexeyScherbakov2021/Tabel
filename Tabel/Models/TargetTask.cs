using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    [Table("TargetTask")]
    public class TargetTask : Observable, IEntity
    {
        public int id { get; set; }
        public int? tt_ModPersonId { get; set; }

        //[StringLength(150)]
        public string tt_name { get; set; }
        
        private decimal? _tt_proc_task;
        public decimal? tt_proc_task { get => _tt_proc_task; set { Set(ref _tt_proc_task, value); } }

        private decimal? _tt_proc_fact;
        public decimal? tt_proc_fact { get => _tt_proc_fact; set { Set(ref _tt_proc_fact, value); } }

        public virtual ModPerson ModPerson { get; set; }

        private string _AttachFile;
        public string tt_AttachFile { get => _AttachFile; set { Set(ref _AttachFile, value); } }

        public int? tt_idFile { get; set; }

        public decimal? _tt_sum_fact;
        [NotMapped]
        public decimal? tt_sum_fact { get => _tt_sum_fact; set { Set(ref _tt_sum_fact, value); } }
    }
}
