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
        [NotMapped]
        public Visibility VisibilityHours { get; set; } = Visibility.Collapsed;

        [NotMapped]
        public TypeDays CalendarTypeDay { get; set; }

    }
}
