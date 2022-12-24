using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Tabel.Component.OtpuskPanel
{
    internal class OtpuskCountConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {

            double width = double.Parse(value[1].ToString());
            if (width == 0) return 0;

            double CountDays = double.Parse( value[0].ToString());
            double tick = width / 396;
            return CountDays * tick;

        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
