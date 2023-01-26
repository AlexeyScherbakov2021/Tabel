﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaAddWorks : BasePremia
    {
        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------

        public PremiaAddWorks(ModPerson person) : base(person)
        {
        }


        public override void Calculation()
        {
            Summa = (model.ListAddWorks?.Sum(it => it.aw_Tarif) + (model.md_person_achiev ?? 0)) * koef;
        }

        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        protected override void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_person_achiev":
                    ModFunction.SetTarifOffDay(model);
                    ModFunction.SetOklad(model);
                    Calculation();
                    break;

                case "ListAddWorks":
                case "TabelAbsent":
                case "TabelHours":
                    Calculation();
                    break;
            }
        }

    }
}
