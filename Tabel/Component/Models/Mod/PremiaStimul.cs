using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaStimul : BasePremia
    {
        public decimal? _OstPrem;
        public decimal? OstPrem { get => _OstPrem; set { Set(ref _OstPrem, value); } }

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaStimul(ModPerson person) : base(person)
        {
        }

        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        protected override void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_prem_otdel":
                case "md_prem_otdel_proc":
                case "md_kvalif_proc":
                    Calculation();
                    break;
            }
        }


        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            OstPrem = 100 - model.md_kvalif_proc ?? 100;
            Summa = (model.md_prem_otdel * model.md_prem_otdel_proc / 100) * OstPrem / 100;
            //OnPropertyChanged(nameof(OstPrem));
        }


    }
}
