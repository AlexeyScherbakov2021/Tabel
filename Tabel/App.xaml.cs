using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.Views;

namespace Tabel
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser;

        public static List<Months> ListMonth = new List<Months>()
        {
            new Months("Январь", 1),
            new Months("Февраль", 2),
            new Months("Март", 3),
            new Months("Апрель", 4),
            new Months("Май", 5),
            new Months("Июнь", 6),
            new Months("Июль", 7),
            new Months("Август", 8),
            new Months("Сентябрь", 9),
            new Months("Октябрь", 10),
            new Months("Ноябрь", 11),
            new Months("Декабрь", 12),
        };


        public App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                            typeof(FrameworkElement),
                            new FrameworkPropertyMetadata(
                            XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

    }
}
