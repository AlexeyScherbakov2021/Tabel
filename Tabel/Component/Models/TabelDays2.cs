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

        //private decimal? OverHours { get; set; } = 0;
        //[NotMapped]
        //public decimal? OverHours { get; set; } = 0;


    }
}
