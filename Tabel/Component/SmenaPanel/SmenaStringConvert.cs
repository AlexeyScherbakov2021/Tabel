using FontAwesome5;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.SmenaPanel
{
    internal class SmenaStringConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //ImageAwesome img = new ImageAwesome();
            string result = "";


            if (value is SmenaKind status)
            {
                //img = new ImageAwesome();
                switch(status)
                {
                    case SmenaKind.First:
                        //img.Icon = EFontAwesomeIcon.Solid_Truck;
                        //img.ToolTip = "Первая смена";
                        //img.Foreground = Brushes.Green;
                        result = "1";
                        break;

                    case SmenaKind.Second:
                        //img.Icon = EFontAwesomeIcon.Solid_Truck;
                        //img.ToolTip = "Вторая смена";
                        //img.Foreground = Brushes.Green;
                        result = "2";
                        break;

                    case SmenaKind.Otpusk:
                        //img.Icon = EFontAwesomeIcon.Solid_Truck;
                        //img.ToolTip = "Отпуск";
                        //img.Foreground = Brushes.Green;
                        result = "О";
                        break;

                    case SmenaKind.DayOff:
                        //img.Icon = EFontAwesomeIcon.Solid_Truck;
                        //img.ToolTip = "Выходной день";
                        //img.Foreground = Brushes.Green;
                        result = "В";
                        break;
                }


            }

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
