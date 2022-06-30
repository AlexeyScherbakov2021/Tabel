using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tabel.Views.Admins
{
    /// <summary>
    /// Логика взаимодействия для YearCalendar.xaml
    /// </summary>
    public partial class YearCalendar : UserControl
    {

        //public static DependencyProperty ExDaysProperty = DependencyProperty.Register(
        //    "ExDays",
        //    typeof(Dictionary<DateTime, MonthControl.TypeDays>),
        //    typeof(YearCalendar),
        //    new FrameworkPropertyMetadata(defaultValue: new Dictionary<DateTime, MonthControl.TypeDays>(), propertyChangedCallback: OnDateChanged)
        //    );

        //[Category("Общие")]
        //[Description("Измененные дни")]
        //public Dictionary<DateTime, MonthControl.TypeDays> ExDays
        //{
        //    get { return (Dictionary<DateTime, MonthControl.TypeDays>)GetValue(ExDaysProperty); }
        //    set { SetValue(ExDaysProperty, value); }
        //}

        //static void OnDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    YearCalendar mc = (YearCalendar)sender;

        //    foreach(var item in mc.ExDays)
        //    {
        //        (mc.uniGrig.Children[item.Key.Month - 1] as MonthControl).AddExDay(item.Key.Day, item.Value);
        //    }

        //}

        //private Dictionary<DateTime, MonthControl.TypeDays> _ExDays;
        //public Dictionary<DateTime, MonthControl.TypeDays> ExDays
        //{
        //    get => _ExDays;
        //    set 
        //    {
        //        _ExDays = value;
        //        foreach (var item in _ExDays)
        //        {
        //            (uniGrig.Children[item.Key.Month - 1] as MonthControl).AddExDay(item.Key.Day, item.Value);
        //        }
        //    }
        //}



        public YearCalendar()
        {
            InitializeComponent();

        }

    }
}
