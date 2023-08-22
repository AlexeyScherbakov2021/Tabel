using System;
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
            decimal koef = model.md_workDays == 0 ? 1 : (decimal)(model.md_workDays - model.md_absentDays) / (decimal)model.md_workDays;

            decimal sum  = 0;
            foreach (var item in model.ListAddWorks)
            {
                sum += item.aw_Tarif * (item.aw_IsRelateHours == true ? koef : 1) * model.person.p_stavka;
            }
            //sum += (model.md_person_achiev ?? 0);
            Summa = sum ;

            //Summa = (model.ListAddWorks?.Sum(it => it.aw_Tarif) + (model.md_person_achiev ?? 0)) * koef;
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
