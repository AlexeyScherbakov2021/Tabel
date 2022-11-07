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
            Summa = 0;
        }

        public override void Calculation()
        {
        }
    }
}
