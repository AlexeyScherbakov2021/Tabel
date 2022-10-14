using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tabel.Models;

namespace Tabel.Infrastructure.Converters
{
    internal class AddWorksConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICollection<AddWorks> ListWorks = (ICollection<AddWorks>)value;

            string s = "";
            foreach (var item in ListWorks)
            {
                if (s != "")
                    s += "; ";
                s += item.aw_Name;
            }

            return s;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
