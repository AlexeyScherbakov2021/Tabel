namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ModPerson")]
    public partial class ModPerson : IEntity
    {
        [Key]
        [Column("md_Id")]
        public int id { get; set; }

        public int? md_modId { get; set; }

        public int md_personalId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_hoursFP { get; set; }

        private decimal? _md_premFP;
        [Column(TypeName = "numeric")]
        public decimal? md_premFP { get => _md_premFP; set { Set(ref _md_premFP, value); } }

        private string _md_group;
        [StringLength(20)]
        public string md_group { get => _md_group; set { Set(ref _md_group, value); } }

        private decimal? _md_sumFromFP;
        public decimal? md_sumFromFP { get => _md_sumFromFP; set { Set(ref _md_sumFromFP, value); } }

        [Column(TypeName = "numeric")]
        public decimal? md_prem1_tarif { get; set; }

        private bool _md_bonus_exec;
        public bool md_bonus_exec { get => _md_bonus_exec; set { Set(ref _md_bonus_exec, value); } }

        private decimal? _md_bonus_max;
        [Column(TypeName = "numeric")]
        public decimal? md_bonus_max { get => _md_bonus_max; set { Set(ref _md_bonus_max, value); } }

        private decimal? _md_bonus_proc;
        [Column(TypeName = "numeric")]
        public decimal? md_bonus_proc { get => _md_bonus_proc; set { Set(ref _md_bonus_proc, value); } }

        private decimal? _md_prem_otdel;
        [Column(TypeName = "numeric")]
        public decimal? md_prem_otdel { get => _md_prem_otdel; set { Set(ref _md_prem_otdel, value); } }

        private decimal? _md_prem_otdel_proc;
        [Column(TypeName = "numeric")]
        public decimal? md_prem_otdel_proc { get => _md_prem_otdel_proc; set { Set(ref _md_prem_otdel_proc, value); } }

        [Column(TypeName = "numeric")]
        public decimal? md_tarif_offDay { get; set; }


        private decimal? _md_kvalif_prem;
        [Column(TypeName = "numeric")]
        public decimal? md_kvalif_prem { get => _md_kvalif_prem; set { Set(ref _md_kvalif_prem, value); } }

        private decimal? _md_kvalif_tarif;
        [Column(TypeName = "numeric")]
        public decimal? md_kvalif_tarif { get => _md_kvalif_tarif; set { Set(ref _md_kvalif_tarif, value); } }

        private string _md_kvalif_name;
        [StringLength(250)]
        public string md_kvalif_name { get => _md_kvalif_name; set { Set(ref _md_kvalif_name, value); } }

        private decimal? _md_cat_prem_tarif;
        [Column(TypeName = "numeric")]
        public decimal? md_cat_prem_tarif { get => _md_cat_prem_tarif; set { Set(ref _md_cat_prem_tarif, value); } }


        public virtual Mod Mod { get; set; }
        public virtual Personal person { get; set; }

        public virtual ICollection<AddWorks> ListAddWorks { get; set; }
    }
}
