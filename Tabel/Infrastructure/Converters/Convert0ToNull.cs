using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tabel.Infrastructure.Converters
{
    internal class Convert0ToNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal n = (Decimal)value;

            if (n == 0)
                return "";

            return n.ToString("0.#");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal result;
            string s = value.ToString();
            Decimal.TryParse(s, out result);

            return result;

            //return s == "" ? 0 : Decimal.TryParse(s, out result);
        }
    }
}
