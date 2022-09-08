using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Component.MonthPanel
{
    public class MonthDays : Observable
    {
        public int Day { get; set; }
        public int Month;
        private TypeDays _Type;
        public TypeDays Type { get => _Type; set { Set(ref _Type, value); } }
        public TypeDays OrigType;

        public MonthDays(int NumDay, int month, TypeDays dweek)
        {
            Day = NumDay;
            Month = month;
            Type = dweek; 
            OrigType = dweek;
        }


        //public event PropertyChangedEventHandler PropertyChanged;
        //public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    var handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}


    }
}
