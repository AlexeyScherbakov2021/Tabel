using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public abstract class BasePremia : Observable, IDisposable
    {
        //protected decimal koef;

        //protected readonly BaseModel db;
        private decimal? _SummaNoNDFL;
        public decimal? SummaNoNDFL
        {
            get => _SummaNoNDFL;
            set { Set(ref _SummaNoNDFL, value); }
        }


        private decimal? _Summa;
        public decimal? Summa { get => _Summa; 
            set {
                if (Set(ref _Summa, value))
                {
                    SummaNoNDFL = _Summa * 0.87m;
                    model.OnPropertyChanged(nameof(model.PremiaItogo));
                    model.OnPropertyChanged(nameof(model.Itogo));
                    model.OnPropertyChanged(nameof(model.ItogoNoNDFL));
                }

            } }

        protected ModPerson model;

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public BasePremia(ModPerson person)
        {
            model = person;
            model.PropertyChanged += PremiaPropertyChanged;

        }

        //-------------------------------------------------------------------------------------------------------
        // Деструктор
        //-------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            model.PropertyChanged -= PremiaPropertyChanged;
        }

        //-------------------------------------------------------------------------------------------------------
        // Событие изменения влияющих полей
        //-------------------------------------------------------------------------------------------------------
        protected virtual void PremiaPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        { }


        //-------------------------------------------------------------------------------------------------------
        // Инициализация
        //-------------------------------------------------------------------------------------------------------
        //public virtual void Initialize(int id)
        //{

        //}


        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        public virtual decimal GetPremia()
        {
            //Calculation();
            return Summa ?? 0;
        }


        //-------------------------------------------------------------------------------------------------------
        // Расчет премии
        //-------------------------------------------------------------------------------------------------------
        public abstract void Calculation();


    }
}
