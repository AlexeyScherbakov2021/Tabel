using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models2
{
    public partial class ModPerson : Observable
    {
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

        public decimal? SummaHoursFP => (md_premFP * Mod?.m_HoursFromFP) / 100m;
        public decimal? SummaPremFP =>
            TabelDays == 0 ? 0 : SummaHoursFP * md_prem1_tarif * (TabelDays - TabelAbsent) / TabelDays;

        public decimal? Bonus => TabelDays == 0 ? 0 : md_bonus_max * md_bonus_proc / 100 * (TabelDays - TabelAbsent) / TabelDays;


        public decimal? PremOtdel => md_prem_otdel * md_prem_otdel_proc / 100;


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

        public decimal? Itogo => Oklad + SummaPremFP + Bonus + PremOtdel + NightSumma;

        public void UpdateUI()
        {
            OnPropertyChanged(nameof(SummaHoursFP));
            OnPropertyChanged(nameof(SummaPremFP));
            OnPropertyChanged(nameof(Bonus));
            OnPropertyChanged(nameof(PremOtdel));
            OnPropertyChanged(nameof(NightSumma));
            OnPropertyChanged(nameof(Itogo));
        }
            

    }
}
