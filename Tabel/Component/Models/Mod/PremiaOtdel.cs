using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaOtdel : BasePremia, IDisposable
    {
        public decimal? OstPrem { get; set; }

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaOtdel(ModPerson person) : base(person)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        //-------------------------------------------------------------------------------------------------------
        // Деструктор
        //-------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            model.PropertyChanged -= Model_PropertyChanged;
        }

        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
            OstPrem = 100 - model.md_kvalif_proc;
            Summa = (model.md_prem_otdel * model.md_prem_otdel_proc / 100) * OstPrem / 100;
            OnPropertyChanged(nameof(OstPrem));
        }


    }
}
