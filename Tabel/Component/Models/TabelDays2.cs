using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    public partial class TabelDay : Observable
    {
        private Visibility _VisibilityHours = Visibility.Collapsed;
        [NotMapped]
        public Visibility VisibilityHours { get => _VisibilityHours; set { Set(ref _VisibilityHours, value); } }

        [NotMapped]
        public TypeDays CalendarTypeDay { get; set; }

        private decimal _WhiteHours;
        [NotMapped]
        public decimal WhiteHours { get => _WhiteHours; set { Set(ref _WhiteHours, value); } }

        public string DayString
        {
            get
            {
                DateTime dt = new DateTime(TabelPerson.tabel.t_year, TabelPerson.tabel.t_month, td_Day);
                return dt.ToString("d ddd").ToLower();
            }
        }



    }
}
