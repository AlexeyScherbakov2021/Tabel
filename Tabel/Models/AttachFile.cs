using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("AttachFile")]
    public class AttachFile : IEntity
    {
        [Key]
        public int id { get; set; }
        public int? mod_id { get; set; }
        public int task_id;
        public string FileName { get; set; }
        public string FullName;


        public virtual Mod mod { get; set; }

    }
}
