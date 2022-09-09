using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    [Table("SmenaDay")]
    public class SmenaDay : IEntity
    {
        [Key]
        [Column("sd_id")]
        public int id { get; set; }
        public int? sd_SmenaPersonId { get; set; }
        public int sd_Day { get; set; }
        public SmenaKind sd_Kind { get; set; }

        public virtual SmenaPersonal SmenaPerson { get; set; }
    }
}
