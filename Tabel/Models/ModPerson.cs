namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Component.Models.Mod;
    using Tabel.Component.Models;
    using Tabel.Infrastructure;
    using DocumentFormat.OpenXml.EMMA;

    [Table("ModPerson")]
    public partial class ModPerson : Observable, IEntity
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

        private string _md_prem_stimul_name;
        public string md_prem_stimul_name { get => _md_prem_stimul_name; set { Set(ref _md_prem_stimul_name, value); } }

        [Column(TypeName = "numeric")]
        private decimal? _md_tarif_offDay;
        public decimal? md_tarif_offDay { get => _md_tarif_offDay; set { Set(ref _md_tarif_offDay, value); } }


        private decimal? _md_kvalif_prem;
        [Column(TypeName = "numeric")]
        public decimal? md_kvalif_prem { get => _md_kvalif_prem; set { Set(ref _md_kvalif_prem, value); } }

        private decimal? _md_kvalif_tarif;
        [Column(TypeName = "numeric")]
        public decimal? md_kvalif_tarif { get => _md_kvalif_tarif; set { Set(ref _md_kvalif_tarif, value); } }

        private string _md_kvalif_name;
        //[StringLength(250)]
        public string md_kvalif_name { get => _md_kvalif_name; set { Set(ref _md_kvalif_name, value); } }

        private decimal? _md_cat_prem_tarif;
        [Column(TypeName = "numeric")]
        public decimal? md_cat_prem_tarif { get => _md_cat_prem_tarif; set { Set(ref _md_cat_prem_tarif, value); } }

        private bool _md_quality_check;
        public bool md_quality_check { get => _md_quality_check; set { Set(ref _md_quality_check, value); } }

        private decimal? _md_kvalif_proc;

        [Column("md_kvalif_proc")]
        public decimal? md_kvalif_proc { get => _md_kvalif_proc; set { Set(ref _md_kvalif_proc, value); } }

        private decimal? _md_person_achiev;
        public decimal? md_person_achiev { get => _md_person_achiev; set { Set(ref _md_person_achiev, value); } }

        private decimal? _md_bolnich;
        public decimal? md_bolnich 
        { 
            get => _md_bolnich; 
            set 
            {
                if (Set(ref _md_bolnich, value))
                {
                    OnPropertyChanged(nameof(PremiaItogo));
                    OnPropertyChanged(nameof(Itogo));
                }
            } 
        }

        private decimal? _md_RealPay ;
        public decimal? md_RealPay 
        { 
            get => _md_RealPay; 
            set
            {
                if(Set(ref _md_RealPay, value))
                    OnPropertyChanged(nameof(DiffPremia));
            }
        }

        [NotMapped]
        public decimal? DiffPremia => Itogo - md_RealPay;

        public virtual Mod Mod { get; set; }
        public virtual Personal person { get; set; }

        public virtual ICollection<AddWorks> ListAddWorks { get; set; }
        public virtual ICollection<TargetTask> ListTargetTask { get; set; }


        [NotMapped]
        public decimal? PlanTarifOtdel { get; set; }

        [NotMapped]
        public PremiaFP premiaFP { get; set; }

        [NotMapped]
        public PremiaBonus premiaBonus { get; set; }

        [NotMapped]
        public PremiaOtdel premiaOtdel { get; set; }

        [NotMapped]
        public PremiaStimul premiStimul { get; set; }

        [NotMapped]
        public PremOffDays premOffDays { get; set; }

        [NotMapped]
        public PremiaAddWorks premiaAddWorks { get; set; }

        [NotMapped]
        public PremiaTransport premiaTransport { get; set; }

        [NotMapped]
        public premiaNight premiaNight { get; set; }

        [NotMapped]
        public PremiaPrize premiaPrize { get; set; }

        [NotMapped]
        public decimal? QualityTarif { get; set; } = 1000;

        [NotMapped]
        public decimal? PereWorkHours15 { get; set; }
        [NotMapped]
        public decimal? PereWorkHours2 { get; set; }

        [NotMapped]
        public decimal? PereWork15 { get; set; }
        [NotMapped]
        public decimal? PereWork2 { get; set; }



        public ModPerson()
        {
            ListAddWorks = new HashSet<AddWorks>();
            ListTargetTask = new HashSet<TargetTask>();

            //premiaFP = new PremiaFP(this);
            //premiaBonus = new PremiaBonus(this);
            //premiaOtdel = new PremiaKvalif(this);
            //premiStimul = new PremiaOtdel(this);
            //premOffDays = new PremOffDays(this);
            //premiaAddWorks = new PremiaAddWorks(this);
            //premiaTransport = new PremiaTransport(this);
            //premiaNight = new premiaNight(this);
            //premiaPrize = new PremiaPrize(this);
        }


        private decimal _TabelHours;
        [NotMapped]
        public decimal TabelHours { get => _TabelHours; set { Set(ref _TabelHours, value); } }
        
        [NotMapped]
        public int TabelDays { get; set; }


        private int _TabelAbsent = 0;
        [NotMapped]
        public int TabelAbsent { get => _TabelAbsent; set { Set(ref _TabelAbsent, value); } }
        //[NotMapped]

        private decimal? _md_Oklad;
        public decimal? md_Oklad { get => _md_Oklad; set { Set(ref _md_Oklad, value); } }
        
        [NotMapped]
        public decimal OverHours;

        [NotMapped]
        public decimal? TransportPremia;


        private decimal? _TabelWorkOffDay;
        [NotMapped]
        public decimal? TabelWorkOffDay { get => _TabelWorkOffDay; set { Set(ref _TabelWorkOffDay, value); } }


        public decimal? PremiaItogo =>
            premiaFP.GetPremia()
            + premiaBonus.GetPremia()
            + premiaOtdel.GetPremia()
            + premiStimul.GetPremia()
            + premiaTransport.GetPremia()
            + premiaAddWorks.GetPremia()
            + premiaPrize.GetPremia();


        public decimal? Itogo => 
            md_Oklad
            + premiaNight.GetPremia()
            + premOffDays.GetPremia()
            + PremiaItogo
            + (PereWork15 ?? 0)
            + (PereWork2 ?? 0)
            + (md_bolnich ?? 0);

    }
}
