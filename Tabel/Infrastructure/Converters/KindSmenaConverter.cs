using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tabel.Infrastructure.Converters
{
    internal class KindSmenaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Result = "";

            if (value is SmenaKind kind)
            {
                switch(kind)
                {
                    case SmenaKind.First:
                        Result = "1см";
                        break;

                    case SmenaKind.Second:
                        Result = "2см";
                        break;

                    case SmenaKind.Otpusk:
                        Result = "О";
                        break;

                    case SmenaKind.DayOff:
                        Result = "В";
                        break;

                }
            }


            return Result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SmenaKind kind = SmenaKind.None;

            if (value is string s)
            {
                switch(s)
                {
                    case "1см":
                        kind = SmenaKind.First;
                        break;
                    case "2см":
                        kind = SmenaKind.Second;
                        break;
                    case "О":
                        kind = SmenaKind.Otpusk;
                        break;
                    case "В":
                        kind = SmenaKind.DayOff;
                        break;
                }


                return kind;

            }

            return SmenaKind.None;
        }
    }
}
