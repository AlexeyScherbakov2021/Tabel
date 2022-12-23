//using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Tabel.Infrastructure;

namespace Tabel.Component.OtpuskPanel
{
    internal class OtpuskStartConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {

            DateTime StartDate = (DateTime)value[0];
            double width = double.Parse( value[1].ToString());

            double tick = width / 365;
            double start = StartDate.DayOfYear * tick;

            Thickness result = new Thickness(start, 0,0,0);
            return result;

        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
