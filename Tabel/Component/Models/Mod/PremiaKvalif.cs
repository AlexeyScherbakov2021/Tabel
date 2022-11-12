using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Component.Models.Mod
{
    public class PremiaKvalif : BasePremia, IDisposable
    {
        //private decimal? _Kvalif_Summa;
        //public decimal? Kvalif_Summa { get => _Kvalif_Summa; set { Set(ref _Kvalif_Summa, value); } }


        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaKvalif(ModPerson person) : base(person)
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
                case "md_kvalif_prem":
                case "md_kvalif_tarif":
                    Calculation();
                    break;
            }
        }

        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            Summa = model.md_kvalif_prem * model.md_kvalif_tarif / 100;
        }


        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        //public override decimal? GetPremia()
        //{
        //    return Kvalif_Summa ?? 0;
        //}
    }
}
