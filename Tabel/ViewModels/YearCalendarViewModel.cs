using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.ViewModels.Base;
using Tabel.Views.Admins;

namespace Tabel.ViewModels
{
    internal class YearCalendarViewModel : ViewModel
    {
        private Dictionary<int, MonthControl.TypeDays> _exDays;
        public Dictionary<int, MonthControl.TypeDays> exDays { get => _exDays; set { Set(ref _exDays, value); } }


        public YearCalendarViewModel()
        {
            //Dictionary<int, MonthControl.TypeDays> exDays = new Dictionary<int, MonthControl.TypeDays>();
            //exDays.Add(12, MonthControl.TypeDays.Short);
            //exDays.Add(7, MonthControl.TypeDays.Work);
            //exDays.Add(15, MonthControl.TypeDays.Holyday);
        }

    }
}
