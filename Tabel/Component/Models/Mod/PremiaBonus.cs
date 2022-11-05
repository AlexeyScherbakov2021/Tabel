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
        private decimal? _Bonus;
        public decimal? Bonus { get => _Bonus; set { Set(ref _Bonus, value); } }

        public PremiaBonus(ModPerson person) : base(person)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
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
                case "md_bonus_proc":
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
            Bonus = (model.TabelDays == 0 || !model.md_bonus_exec) 
                ? null 
                : model.md_bonus_max * model.md_bonus_proc / 100 * (model.TabelDays - model.TabelAbsent) / model.TabelDays;
        }

        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        public override decimal? GetPremia()
        {
            return Bonus ?? 0;
        }

    }
}
