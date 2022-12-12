using FontAwesome5;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Tabel.Models;

namespace Tabel.Component.TransPanel
{
    internal class TransImageConvert : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageAwesome img = new ImageAwesome();


            if (value is KindTrans status)
            {
                img = new ImageAwesome();
                if(status == KindTrans.Used)
                {
                    img.Icon = EFontAwesomeIcon.Solid_Truck;
                    img.ToolTip = "Использовался транспорт";
                    //img.Foreground = Brushes.Green;
                }
            }

            return img?.Icon;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
