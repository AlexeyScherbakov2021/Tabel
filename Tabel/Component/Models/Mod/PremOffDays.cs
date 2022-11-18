using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Summa = model.TabelWorkOffDay * model.md_tarif_offDay;
        }

        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        //public override decimal? GetPremia()
        //{
        //    Calculation();
        //    return Summa ?? 0;
        //}
    }
}
