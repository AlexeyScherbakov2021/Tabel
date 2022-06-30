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
    /// Логика взаимодействия для MonthControl.xaml
    /// </summary>
    public partial class MonthControl : UserControl
    {
        public enum TypeDays { Work, Short, Holyday };


        public static DependencyProperty MonthProperty = DependencyProperty.Register(
            "Month",
            typeof(int),
            typeof(MonthControl),
            new FrameworkPropertyMetadata(defaultValue: 1, propertyChangedCallback: OnDateChanged)
            );

        [Category("Общие")]
        [Description("Порядковый номер месяца")]
        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }


        public static DependencyProperty YearProperty = DependencyProperty.Register(
            "Year",
            typeof(int),
            typeof(MonthControl),
            
            new FrameworkPropertyMetadata(defaultValue: 2022, propertyChangedCallback: OnDateChanged)
            );

        [Category("Общие")]
        [Description("Год")]
        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }


        public static DependencyProperty ExDaysProperty = DependencyProperty.Register(
            "ExDays",
            typeof(Dictionary<int, MonthControl.TypeDays>),
            typeof(MonthControl),
            new FrameworkPropertyMetadata(defaultValue: new Dictionary<int, MonthControl.TypeDays>(), propertyChangedCallback: OnDaysChanged)
            );

        [Category("Общие")]
        [Description("Измененные дни")]
        public Dictionary<int, MonthControl.TypeDays> ExDays
        {
            get { return (Dictionary<int, MonthControl.TypeDays>)GetValue(ExDaysProperty); }
            set { SetValue(ExDaysProperty, value); }
        }


        static void OnDaysChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MonthControl mc = (MonthControl)sender;

            foreach (var item in mc.ExDays)
            {
                switch (item.Value)
                {
                    case TypeDays.Holyday:
                        //(DayGrid.Children[day] as TextBlock).Foreground = Brushes.Red;
                        (mc.DayGrid.Children[item.Key] as TextBlock).Background = Brushes.Red;
                        break;
                    case TypeDays.Short:
                        //(DayGrid.Children[day] as TextBlock).Foreground = Brushes.Green;
                        (mc.DayGrid.Children[item.Key] as TextBlock).Background = Brushes.LightGreen;
                        break;
                    case TypeDays.Work:
                        (mc.DayGrid.Children[item.Key] as TextBlock).Foreground = Brushes.Black;
                        (mc.DayGrid.Children[item.Key] as TextBlock).Background = Brushes.LightGray;
                        break;
                }
            }

        }

        //private Dictionary<int, MonthControl.TypeDays> _ExDays;
        //public Dictionary<int, MonthControl.TypeDays> ExDays
        //{
        //    get => _ExDays;
        //    set
        //    {
        //        _ExDays = value;
        //        foreach (var item in _ExDays)
        //        {
        //            switch (item.Value)
        //            {
        //                case TypeDays.Holyday:
        //                    //(DayGrid.Children[day] as TextBlock).Foreground = Brushes.Red;
        //                    (DayGrid.Children[item.Key - 1] as TextBlock).Background = Brushes.Red;
        //                    break;
        //                case TypeDays.Short:
        //                    //(DayGrid.Children[day] as TextBlock).Foreground = Brushes.Green;
        //                    (DayGrid.Children[item.Key - 1] as TextBlock).Background = Brushes.LightGreen;
        //                    break;
        //                case TypeDays.Work:
        //                    (DayGrid.Children[item.Key - 1] as TextBlock).Foreground = Brushes.Black;
        //                    (DayGrid.Children[item.Key - 1] as TextBlock).Background = Brushes.LightGray;
        //                    break;
        //            }
        //        }
        //    }
        //}



        private Dictionary<int, TypeDays> _exDays = new Dictionary<int, TypeDays>();
        private DateTime StartDate;
        private int _startDay;
        //public string NameMonth { get; set; }


        public MonthControl()
        {
            InitializeComponent();
            PaintCalendar();
        }


        //public void AddExDay(int day, TypeDays type)
        //{
        //    _exDays.Add(day, type);
        //    day += _startDay - 1;
        //    switch (type)
        //    {
        //        case TypeDays.Holyday:
        //            //(DayGrid.Children[day] as TextBlock).Foreground = Brushes.Red;
        //            (DayGrid.Children[day] as TextBlock).Background = Brushes.Red;
        //            break;
        //        case TypeDays.Short:
        //            //(DayGrid.Children[day] as TextBlock).Foreground = Brushes.Green;
        //            (DayGrid.Children[day] as TextBlock).Background = Brushes.LightGreen;
        //            break;
        //        case TypeDays.Work:
        //            (DayGrid.Children[day] as TextBlock).Foreground = Brushes.Black;
        //            (DayGrid.Children[day] as TextBlock).Background = Brushes.LightGray;
        //            break;
        //    }

        //}


        void PaintCalendar()
        {
            StartDate = new DateTime(Year, Month, 1);
            monthName.Text = StartDate.ToString("MMMM");

            foreach (var item in DayGrid.Children)
            {
                (item as TextBlock).Text = "";
                (item as TextBlock).Foreground = Brushes.Black;
            }

            _startDay = (int)StartDate.DayOfWeek - 1;
            if (_startDay < 0)
                _startDay = 6;

            for (int i = _startDay, n = 1; i < 42 && StartDate.Month == Month; i++, n++)
            {
                (DayGrid.Children[i] as TextBlock).Text = n.ToString();
                if(i % 7 > 4)
                    (DayGrid.Children[i] as TextBlock).Foreground= Brushes.Red;

                //if (_exDays.ContainsKey(n))
                //{
                //    switch (_exDays[n])
                //    {
                //        case TypeDays.Holyday:
                //            (DayGrid.Children[i] as TextBlock).Foreground = Brushes.Red;
                //            break;
                //        case TypeDays.Short:
                //            (DayGrid.Children[i] as TextBlock).Foreground = Brushes.Orange;
                //            break;
                //        case TypeDays.Work:
                //            (DayGrid.Children[i] as TextBlock).Foreground = Brushes.Black;
                //            break;
                //    }
                //}

                StartDate = StartDate.AddDays(1);
            }

        }



        static void OnDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MonthControl mc = (MonthControl)sender;
            mc.PaintCalendar();
        }

    }
}
