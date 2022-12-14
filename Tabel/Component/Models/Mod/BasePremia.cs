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
        //protected readonly BaseModel db;
        private decimal? _Summa;
        public decimal? Summa { get => _Summa; set { Set(ref _Summa, value); } }

        protected ModPerson model;

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public BasePremia(ModPerson person)
        {
            model = person;
            //this.db = db;
        }

        //-------------------------------------------------------------------------------------------------------
        // Инициализация
        //-------------------------------------------------------------------------------------------------------
        public virtual void Initialize(int id)
        {

        }


        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        public virtual decimal? GetPremia()
        {
            Calculation();
            return Summa ?? 0;
        }


        //-------------------------------------------------------------------------------------------------------
        // Расчет премии
        //-------------------------------------------------------------------------------------------------------
        public abstract void Calculation();


    }
}
