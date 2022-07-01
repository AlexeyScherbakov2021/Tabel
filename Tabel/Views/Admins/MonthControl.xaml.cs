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

        // месяц
        public static DependencyProperty MonthProperty = DependencyProperty.Register(
            "Month",
            typeof(int),
            typeof(MonthControl),
            new FrameworkPropertyMetadata(defaultValue: 0, propertyChangedCallback: OnDateChanged)
            );

        [Category("Общие")]
        [Description("Порядковый номер месяца")]
        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }

        // год
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


        // измененные дни
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


        // выбранный день
        public static DependencyProperty SelectedDayProperty = DependencyProperty.Register(
            "SelectedDay",
            typeof(int),
            typeof(MonthControl),
            new FrameworkPropertyMetadata(defaultValue: 0, propertyChangedCallback: OnSelectChanged, 
                coerceValueCallback: OnCoerceValue)
            );

        [Category("Общие")]
        [Description("Выбранный день")]
        public int SelectedDay
        {
            get { return (int)GetValue(SelectedDayProperty); }
            set { SetValue(SelectedDayProperty, value); }
        }
        private int OldSelectedDay = 0;


        public static readonly RoutedEvent SelectedDayChangedEvent =
       EventManager.RegisterRoutedEvent("SelectedDayChanged", RoutingStrategy.Bubble,
           typeof(RoutedEventHandler), typeof(MonthControl));

        public event RoutedEventHandler SelectedDayChanged
        {
            add { AddHandler(SelectedDayChangedEvent, value); }
            remove { RemoveHandler(SelectedDayChangedEvent, value); }
        }



        private static object OnCoerceValue(DependencyObject sender, object baseValue)
        {
            MonthControl mc = (MonthControl)sender;

            if ((int)baseValue > mc.DayGrid.Children.Count)
                return 0;

            return baseValue;
        }


        //----------------------------------------------------------------------------------------------------
        // Событие изменения типа даты
        //----------------------------------------------------------------------------------------------------
        private static void OnDaysChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MonthControl mc = (MonthControl)sender;

            foreach (var item in mc.ExDays)
            {
                Brush bg = mc.GetBackgroundDay(item.Key);
                (mc.DayGrid.Children[item.Key - 1] as TextBlock).Background = bg;
            }

        }

        private Dictionary<int, TypeDays> _exDays = new Dictionary<int, TypeDays>();


        //----------------------------------------------------------------------------------------------------
        // конструктор
        //----------------------------------------------------------------------------------------------------
        public MonthControl()
        {
            InitializeComponent();
        }


        //----------------------------------------------------------------------------------------------------
        // Получение цвета измененного дня
        //----------------------------------------------------------------------------------------------------
        private Brush GetBackgroundDay(int day)
        {
            Brush bg = null;

            if (day > 0)
            {
                if (ExDays.ContainsKey(day))
                {
                    switch (ExDays[day])
                    {
                        case TypeDays.Holyday:
                            bg = Brushes.Red;
                            break;
                        case TypeDays.Short:
                            bg = Brushes.LightGreen;
                            break;
                        case TypeDays.Work:
                            bg = Brushes.LightGray;
                            break;
                    }

                }

            }

            return bg;
        }
            //----------------------------------------------------------------------------------------------------
            // выделение текущего дня
            //----------------------------------------------------------------------------------------------------
        private static void OnSelectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MonthControl mc = (MonthControl)sender;
            Brush bg = null;

            if (mc.OldSelectedDay > 0)
            {
                bg = mc.GetBackgroundDay(mc.OldSelectedDay);

                (mc.DayGrid.Children[mc.OldSelectedDay - 1] as TextBlock).Background = bg;
                (mc.DayGrid.Children[mc.OldSelectedDay - 1] as TextBlock).Foreground = Brushes.Black;
            }

            if (mc.SelectedDay <= mc.DayGrid.Children.Count && mc.SelectedDay > 0)
            {
                (mc.DayGrid.Children[mc.SelectedDay - 1] as TextBlock).Background = Brushes.Blue;
                (mc.DayGrid.Children[mc.SelectedDay - 1] as TextBlock).Foreground = Brushes.White;
            }

            //if (mc.OldSelectedDay != mc.SelectedDay)
            //{
            //    RoutedEventArgs args;
            //    args = new RoutedEventArgs(SelectedDayChangedEvent);
            //    args.Source = mc;
            //    mc.RaiseEvent(args);
            //}

            mc.OldSelectedDay = mc.SelectedDay;

        }


        //----------------------------------------------------------------------------------------------------
        // Отрисовка месяца
        //----------------------------------------------------------------------------------------------------
        void PaintCalendar()
        {
            DateTime StartDate = new DateTime(Year, Month, 1);
            monthName.Text = StartDate.ToString("MMMM");

            DayGrid.Children.Clear();
            int _startDay = (int)StartDate.DayOfWeek - 1;
            if (_startDay < 0)
                _startDay = 6;

            int col = 0;
            int row = _startDay;

            while(StartDate.Month == Month)
            {
                TextBlock tb = new TextBlock();
                tb.Text = StartDate.Day.ToString();
                tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.TextAlignment = TextAlignment.Center;
                tb.MouseLeftButtonDown += Tb_MouseLeftButtonDown;
                if (row % 7 > 4)
                    tb.Foreground = Brushes.Red;

                Grid.SetColumn(tb, col);
                Grid.SetRow(tb, row);
                DayGrid.Children.Add(tb);

                if(++row > 6)
                {
                    col++;
                    row = 0;
                }

                StartDate = StartDate.AddDays(1);
            }

        }

        //----------------------------------------------------------------------------------------------------
        // Событие клика левой кнопкой мыши
        //----------------------------------------------------------------------------------------------------
        private void Tb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int month = DayGrid.Children.IndexOf((UIElement)sender) + 1;

            RoutedEventArgs args;
            args = new RoutedEventArgs(SelectedDayChangedEvent);
            args.Source = this;
            RaiseEvent(args);

            SelectedDay = month;
        }



        //----------------------------------------------------------------------------------------------------
        // Событие изменения года или месяца
        //----------------------------------------------------------------------------------------------------
        private static void OnDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MonthControl mc = (MonthControl)sender;
            mc.PaintCalendar();
        }

    }
}
