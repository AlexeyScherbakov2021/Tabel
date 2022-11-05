using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public abstract class BasePremia : Observable
    {
        protected ModPerson model;

        public BasePremia(ModPerson person )
        {
            model = person;
        }
        public abstract decimal? GetPremia();
        public abstract void Calculation();


    }
}
