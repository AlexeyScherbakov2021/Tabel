using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Models
{
    [Table("ModOtdelSumFP")]
    public partial class ModOtdelSumFP : IEntity
    {
        [Key]
        [Column("mo_Id")]
        public int id { get; set; }

        //[Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? mo_mod_id { get; set; }

        //[Key]
        //[Column(Order = 1)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? mo_otdel_id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? mo_sumFP { get; set; }

        public virtual Mod Mod { get; set; }

        public virtual Otdel otdel { get; set; }
    }
}
