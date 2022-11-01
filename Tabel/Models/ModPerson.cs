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

        //public decimal? md_premFP { get; set; }

        private decimal? _md_premFP;
        [Column(TypeName = "numeric")]
        public decimal? md_premFP
        {
            get => _md_premFP;
            set
            {
                Set(ref _md_premFP, value);
                //if (Equals(_md_premFP, value)) return;
                //_md_premFP = value;
                //UpdateUI();
            }
        }

        [StringLength(20)]
        public string md_group { get => _md_group; set { Set(ref _md_group, value); } }
        public string _md_group;

        public decimal? md_sumFromFP { get => _md_sumFromFP; set { Set(ref _md_sumFromFP, value); } }
        public decimal? _md_sumFromFP;

        [Column(TypeName = "numeric")]
        public decimal? md_prem1_tarif { get; set; }

        //public bool md_bonus_exec { get; set; }
        private bool _md_bonus_exec;
        public bool md_bonus_exec
        {
            get => _md_bonus_exec;
            set
            {
                if (Equals(_md_bonus_exec, value)) return;
                _md_bonus_exec = value;
                UpdateUI();
            }
        }


        //public decimal? md_bonus_max { get; set; }
        private decimal? _md_bonus_max;
        [Column(TypeName = "numeric")]
        public decimal? md_bonus_max
        {
            get => _md_bonus_max;
            set
            {
                if (Equals(_md_bonus_max, value)) return;
                _md_bonus_max = value;
                UpdateUI();
            }
        }

        //public decimal? md_bonus_proc { get; set; }
        private decimal? _md_bonus_proc;
        [Column(TypeName = "numeric")]
        public decimal? md_bonus_proc
        {
            get => _md_bonus_proc;
            set
            {
                if (Equals(_md_bonus_proc, value)) return;
                _md_bonus_proc = value;
                UpdateUI();
            }
        }

        //public decimal? md_prem_otdel { get; set; }
        private decimal? _md_prem_otdel;
        [Column(TypeName = "numeric")]
        public decimal? md_prem_otdel
        {
            get => _md_prem_otdel;
            set
            {
                if (Equals(_md_prem_otdel, value)) return;
                _md_prem_otdel = value;
                UpdateUI();
            }
        }

        //public decimal? md_prem_otdel_proc { get; set; }
        private decimal? _md_prem_otdel_proc;
        [Column(TypeName = "numeric")]
        public decimal? md_prem_otdel_proc
        {
            get => _md_prem_otdel_proc;
            set
            {
                if (Equals(_md_prem_otdel_proc, value)) return;
                _md_prem_otdel_proc = value;
                UpdateUI();
            }
        }

        [Column(TypeName = "numeric")]
        public decimal? md_tarif_offDay { get; set; }



        private decimal? _md_kvalif_prem;
        [Column(TypeName = "numeric")]
        public decimal? md_kvalif_prem 
        {
            get => _md_kvalif_prem;
            set
            {
                if (Equals(_md_kvalif_prem, value)) return;
                _md_kvalif_prem = value;
                UpdateUI();
            }
        }

        private decimal? _md_cat_prem_tarif;
        [Column(TypeName = "numeric")]
        public decimal? md_cat_prem_tarif
        {
            get => _md_cat_prem_tarif;
            set
            {

                if (value > person?.category?.cat_max_level)
                    value = person.category.cat_max_level;
                if (Set(ref _md_cat_prem_tarif, value)) 
                    UpdateUI();
            }
        }



        public virtual Mod Mod { get; set; }
        public virtual Personal person { get; set; }

        public virtual ICollection<AddWorks> ListAddWorks { get; set; }
    }
}
