using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaOtdel : BasePremia
    {
        //private decimal? _Kvalif_Summa;
        //public decimal? Kvalif_Summa { get => _Kvalif_Summa; set { Set(ref _Kvalif_Summa, value); } }


        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaOtdel(ModPerson person) : base(person)
        {

        }


        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        protected override void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_kvalif_prem":
                case "md_kvalif_tarif":
                case "md_kvalif_proc":
                case "TabelAbsent":
                    Calculation();
                    break;
            }
        }

        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {

            Summa = (model.md_kvalif_prem * model.md_kvalif_tarif / 100) * model.md_kvalif_proc/100 * model.person.p_stavka;
        }

    }
}
