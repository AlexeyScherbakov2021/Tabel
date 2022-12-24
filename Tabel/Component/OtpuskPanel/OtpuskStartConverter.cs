﻿//using DocumentFormat.OpenXml.Drawing.Charts;
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

            double width = double.Parse(value[1].ToString());
            if (width == 0) return new Thickness();

            DateTime StartDate = (DateTime)value[0];

            double tick = width / 396;
            double start = (StartDate.DayOfYear - 1) * tick;

            Thickness result = new Thickness(start, 0,0,0);
            return result;

        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}