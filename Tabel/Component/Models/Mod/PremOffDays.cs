using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremOffDays : BasePremia
    {

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremOffDays(ModPerson person) : base(person)
        {
        }


        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            decimal stavka = model.person == null ? 0 : model.person.p_stavka;
            Summa = model.md_workOffDays * model.md_tarif_offDay * ModFunction.WorkOffKoeff * stavka;
        }


        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        protected override void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_tarif_offDay":
                case "TabelWorkOffDay":
                    Calculation();
                    break;
            }
        }

    }
}
