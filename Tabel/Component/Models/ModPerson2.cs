using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models;
using Tabel.Component.Models.Mod;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    public partial class ModPerson : Observable
    {
        [NotMapped]
        public PremiaFP premiaFP { get; set; }

        [NotMapped]
        public PremiaBonus premiaBonus { get; set; }

        [NotMapped]
        public PremiaKvalif premiaKvalif { get; set; }

        [NotMapped]
        public PremiaOtdel premiaOtdel { get; set; }

        [NotMapped]
        public PremOffDays premOffDays { get; set; }


        public ModPerson()
        {
            premiaFP = new PremiaFP(this);
            premiaBonus = new PremiaBonus(this);
            premiaKvalif = new PremiaKvalif(this);
            premiaOtdel = new PremiaOtdel(this);
            premOffDays = new PremOffDays(this);
        }

        [NotMapped]
        public decimal TabelHours { get; set; }
        [NotMapped]
        public int TabelDays { get; set; }
        [NotMapped]
        public int TabelAbsent { get; set; } = 0;
        [NotMapped]
        public decimal Oklad { get; set; }

        [NotMapped]
        public decimal? TabelWorkOffDay { get; set; }
        [NotMapped]
        //public decimal? DayOffSumma { get; set; }

        //public decimal? PremOtdel => md_prem_otdel * md_prem_otdel_proc / 100;

        // ночные часы
        public decimal? NightOklad => person?.category?.cat_tarif * 0.2m;
        
        private decimal? _NightHours;
        [NotMapped]
        public decimal? NightHours 
        { 
            get => _NightHours; 
            set
            {
                if(_NightHours == value) return;
                _NightHours = value;
                UpdateUI();
            }
        }

        public decimal? NightSumma => NightOklad * NightHours;


        public decimal? Itogo => Oklad
            + premiaFP.GetPremia()
            + premiaBonus.GetPremia()
            + premiaKvalif.GetPremia()
            + premiaOtdel.GetPremia()
            + (NightSumma ?? 0)
            + premOffDays.GetPremia()
            + (TransportSumma ?? 0)
            + (AddWorksSumma ?? 0);

        [NotMapped]
        public decimal? TransportSumma { get; set; }

        [NotMapped]
        public decimal? AddWorksSumma => ListAddWorks is null ? 0 : ListAddWorks.Sum(it => it.aw_Tarif);

        //[NotMapped]
        //public decimal? Kvalif_Summa => md_kvalif_prem;

        public void UpdateUI()
        {
            //OnPropertyChanged(nameof(Bonus));
            //OnPropertyChanged(nameof(PremOtdel));
            OnPropertyChanged(nameof(NightSumma));
            OnPropertyChanged(nameof(AddWorksSumma));
            OnPropertyChanged(nameof(Itogo));
            //OnPropertyChanged(nameof(Kvalif_Summa));
        }

    }
}
