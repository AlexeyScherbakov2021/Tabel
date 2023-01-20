using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaAddWorks : BasePremia, IDisposable
    {
        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------

        public PremiaAddWorks(ModPerson person) : base(person)
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

        public override void Calculation()
        {
            decimal koef = model.TabelDays == 0 ? 1 : (decimal)(model.TabelDays - model.TabelAbsent) / (decimal)model.TabelDays;

            //Summa = model.ListAddWorks is null 
            //    ? 0 
            //    : model.ListAddWorks.Sum(it => it.aw_Tarif) + (model.md_person_achiev ?? 0);

            Summa = (model.ListAddWorks?.Sum(it => it.aw_Tarif) + (model.md_person_achiev ?? 0)) * koef;

            //Summa *= koef;
        }


        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ListAddWorks":
                case "md_person_achiev":
                case "TabelAbsent":
                case "TabelHours":
                    Calculation();
                    break;
            }
        }

        public override decimal? GetPremia()
        {
            Calculation(); 
            return base.GetPremia();
        }
    }
}
