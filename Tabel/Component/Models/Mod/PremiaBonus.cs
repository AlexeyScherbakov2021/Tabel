using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaBonus : BasePremia, IDisposable
    {
        private decimal? _BonusForAll;
        public decimal? BonusForAll
        { 
            get => _BonusForAll; 
            set
            {
                if (_BonusForAll == value) return;
                _BonusForAll = value;
                Calculation();
            }
        }


        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaBonus(ModPerson person) : base(person)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        //-------------------------------------------------------------------------------------------------------
        // Деструктор
        //-------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            model.PropertyChanged -= Model_PropertyChanged;
        }


        //-------------------------------------------------------------------------------------------------------
        // Событие изменения полей
        //-------------------------------------------------------------------------------------------------------
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_bonus_exec":
                //case "BonusForAll":
                case "md_bonus_max":
                    Calculation();
                    break;
            }

        }

        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            Summa = (model.TabelDays == 0 || !model.md_bonus_exec) 
                ? null 
                : model.md_bonus_max * BonusForAll / 100 * (model.TabelDays - model.TabelAbsent) / model.TabelDays;

        }
    }
}
