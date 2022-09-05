using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Component.MonthPanel
{
    public class MonthDays : INotifyPropertyChanged
    {
        public int Day { get; set; }
        public TypeDays Type { get; set; }
        public TypeDays OrigType;

        public MonthDays(int NumDay, TypeDays dweek)
        {
            Day = NumDay;
            Type = dweek; 
            OrigType = dweek;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
