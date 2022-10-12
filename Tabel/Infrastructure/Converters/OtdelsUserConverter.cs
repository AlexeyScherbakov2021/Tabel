using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tabel.Models;

namespace Tabel.Infrastructure.Converters
{
    internal class OtdelsUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICollection<Otdel> ListOtdels = (ICollection < Otdel > )value;

            string s = "";
            foreach (var item in ListOtdels)
            {
                if(s != "")
                    s += "; ";
                s += item.ot_name;
            }

            return s;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
