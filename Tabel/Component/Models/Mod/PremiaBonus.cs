using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels;

namespace Tabel.Component.Models.Mod
{
    public class PremiaBonus : BasePremia
    {

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaBonus(ModPerson person) : base(person)
        {
        }


        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        protected override void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_bonus_exec":
                case "md_bonus_max":
                    Calculation();
                    break;
            }

        }

        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            decimal koef = model.TabelDays == 0 ? 1 : (decimal)(model.TabelDays - model.TabelAbsent) / (decimal)model.TabelDays;
            Summa = !model.md_bonus_exec
                ? null
                : model.md_bonus_max * ModUCViewModel.BonusProc / 100 * koef * model.person.p_stavka;

        }
    }
}
