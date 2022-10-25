namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ModOtdelSumFP")]
    public partial class ModOtdelSumFP
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int mo_mod_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int mo_otdel_id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? mo_sumFP { get; set; }

        public virtual Mod Mod { get; set; }

        public virtual otdel otdel { get; set; }
    }
}
