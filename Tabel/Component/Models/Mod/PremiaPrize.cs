using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaPrize : BasePremia
    {
        public PremiaPrize(ModPerson person) : base(person)
        {
        }


        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        protected override void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_person_achiev":
                    Calculation();
                    break;
            }
        }


        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            Summa = null;
            if (model.TabelHours == 0 || model.OverHours == 0)
                return;

            Summa = (model.person.category.cat_tarif + (model.md_person_achiev / 162 ?? 0)) * model.OverHours * model.person.p_stavka * 2;

            //model.md_Oklad / model.TabelHours * model.OverHours * 2;
        }

    }
}
