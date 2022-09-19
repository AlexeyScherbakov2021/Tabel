using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    [Table("ModPerson")]
    public partial class ModPerson : IEntity
    {
        [Key]
        [Column("md_Id")]
        public int id { get; set; }
        public int md_modId { get; set; }
        public int md_personalId { get; set; }
        //public int mod_categoryId { get; set; }
        public decimal? md_hoursFP { get; set; }
        public decimal? md_premFP { get; set; }
        public decimal? md_prem1_tarif { get; set; }
        public decimal? md_bonus_max { get; set; }
        public decimal? md_bonus_proc { get; set; }
        public decimal? md_prem_otdel { get; set; }
        public decimal? md_prem_otdel_proc { get; set; }
        public decimal? md_tarif_offDay { get; set; }

        public virtual Personal person { get; set; }
        //public virtual Category category { get; set; }
        public virtual Mod Mod { get; set; }

    }
}
