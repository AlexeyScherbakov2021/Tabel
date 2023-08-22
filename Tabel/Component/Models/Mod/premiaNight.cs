using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;

namespace Tabel.Component.Models.Mod
{
    public class premiaNight : BasePremia
    {
        public decimal? NightOklad { get; set; }
        public decimal? NightHours { get; set; }

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public premiaNight(ModPerson person) : base(person)
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
            Summa = NightOklad /*+ (model.md_person_achiev / 162 * ModFunction.NightKoeff ?? 0))*/ * NightHours;
        }


    }
}
