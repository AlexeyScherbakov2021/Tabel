﻿using System;
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
            Summa = model.Oklad / model.TabelHours * model.OverHours * 2;
        }

        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        public override decimal? GetPremia()
        {
            Calculation();
            return Summa ?? 0;
        }

    }
}
