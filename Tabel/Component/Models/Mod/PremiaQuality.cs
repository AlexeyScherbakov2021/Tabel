using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaQuality : BasePremia, IDisposable
    {
        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaQuality(ModPerson person) : base(person)
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
                case "md_quality_check":
                    Calculation();
                    break;
            }

        }

        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            Summa = !model.md_quality_check
                ? null
                : model.QualityTarif;
        }

    }
}
