using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaPrize : BasePremia
    {
        public PremiaPrize(ModPerson person) : base(person)
        {
        }

        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            if (model.TabelHours == 0 || model.OverHours == 0)
                return;

            Summa = model.Oklad / model.TabelHours * model.OverHours * 2;
        }

    }
}
