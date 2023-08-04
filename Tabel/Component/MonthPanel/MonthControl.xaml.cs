using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
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

namespace Tabel.Component.MonthPanel
{
    /// <summary>
    /// Логика взаимодействия для MonthControl.xaml
    /// </summary>
    public partial class MonthControl : UserControl, INotifyPropertyChanged
    {
        public int CountWorkDays { get; set; }
        public int CountOffDays { get; set; }

        public int CountWorkHours { get; set; }

        public string[] WeekDay { get; set; } = {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };

        public int StartIndex { get; set; }

        public List<MonthDays> Days { get; set; }

        // список измененных дней (короткий или пустой)
        public static readonly DependencyProperty ChangedDaysProperty =
            DependencyProperty.Register("ChangedDays", typeof(ObservableCollection<MonthDays>), typeof(MonthControl),
        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDaysTypeChanged)));
        public ObservableCollection<MonthDays> ChangedDays
        {
            get { return (ObservableCollection<MonthDays>)GetValue(ChangedDaysProperty); }
            set { SetValue(ChangedDaysProperty, value); }
        }


        public static readonly DependencyProperty NumberMonthProperty =
            DependencyProperty.Register("NumberMonth", typeof(int), typeof(MonthControl),
        new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnMonthChanged)));
        public int NumberMonth
        {
            get { return (int)GetValue(NumberMonthProperty); }
            set { SetValue(NumberMonthProperty, value); }
        }

        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(MonthControl),
        new FrameworkPropertyMetadata(DateTime.Now.Year, new PropertyChangedCallback(OnMonthChanged)));
        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }


        private static void OnDaysTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MonthControl mc)
            {

                if (e.OldValue != null)
                    (e.OldValue as ObservableCollection<MonthDays>).CollectionChanged -= mc.OnCollectionChanged;

                if (e.NewValue != null)
                {
                    ObservableCollection<MonthDays> NewList = e.NewValue as ObservableCollection<MonthDays>;
                    NewList.CollectionChanged += mc.OnCollectionChanged;

                    foreach (var item in NewList)
                        mc.SetTypeDays(item);

                }

            }
        }

        private void SetTypeDays(MonthDays md)
        {
            Days[md.Day - 1].Type = md.Type;
            //Days[md.Day - 1].OnPropertyChanged("Type");
           
        }


        private void ShowCountHours()
        {
            int CountWorkDay8 = Days.Count(it => it.Type == TypeDays.Work);
            int CountWorkDay7 = Days.Count(it => it.Type == TypeDays.ShortWork);
            CountWorkDays = CountWorkDay8 + CountWorkDay7;
            CountWorkHours = CountWorkDay8 * 8 + CountWorkDay7 * 7;

            tbCountOffDays.Text = (Days.Count() - CountWorkDays).ToString();
            tbCountWorkDays.Text = CountWorkDays.ToString();
            tbCountHours.Text = CountWorkHours.ToString();

        }


        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SetTypeDays(e.NewItems[0] as MonthDays);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach(var item in Days)
                {
                    item.Type = item.OrigType;
                    item.OnPropertyChanged("Type");
                }
            }
            ShowCountHours();

        }

        private static void OnMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MonthControl mc)
            {
                if (e.Property == YearProperty)
                {
                    DateTime dt = new DateTime((int)e.NewValue, mc.NumberMonth, 1);
                    mc.SetMonthContent(dt);
                }
                if (e.Property == NumberMonthProperty)
                {
                    DateTime dt = new DateTime(mc.Year, (int)e.NewValue, 1 );
                    mc.SetMonthContent(dt);
                    mc.ShowCountHours();

                }
            }
        }


        //------------------------------------------------------------------------------------
        // Инициализация 
        //------------------------------------------------------------------------------------
        private void SetMonthContent(DateTime dt)
        {
            TBNameMonth.Text = dt.ToString("MMMM");
            //int CntDays = dt.AddMonths(1).AddDays(-1).Day;
            DateTime dt2 = dt;
            Days = new List<MonthDays>();
            TypeDays typeDay;
            while (dt2.Month == dt.Month)
            {
                if (dt2.DayOfWeek == DayOfWeek.Sunday || dt2.DayOfWeek == DayOfWeek.Saturday)
                    typeDay = TypeDays.Holyday;
                else
                    typeDay = TypeDays.Work;
                MonthDays day = new MonthDays(dt2.Day, dt.Month, typeDay);
                dt2 = dt2.AddDays(1);
                Days.Add(day);
            }

            LBdays.ItemsSource = Days;
            OnPropertyChanged(nameof(Days));
            StartIndex = dt.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)dt.DayOfWeek - 1;
            OnPropertyChanged(nameof(StartIndex));

            //this.PropertyChanged += MonthControl_PropertyChanged;
        }

        //private void MonthControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //}

        public MonthControl()
        {
            InitializeComponent();
            //ChangedDays = new ObservableCollection<MonthDays>();
            ICweek.ItemsSource = WeekDay;
            SetMonthContent(new DateTime(Year, NumberMonth, 1));
        }


        //------------------------------------------------------------------------------------
        // Установка вида дня
        //------------------------------------------------------------------------------------
        public void SetTypeDay(int day, TypeDays type)
        {
            Days[day - 1].Type = type;
            Days[day - 1].OnPropertyChanged("Type");
            if (ChangedDays != null)
            {
                var d = GetListDays();
                //ChangedDays.Clear();

                List<MonthDays> removeList = ChangedDays.ToList();
                foreach (var item in removeList)
                    ChangedDays.Remove(item);

                foreach (var item in d)
                {
                    ChangedDays.Add(item);
                }
            }
        }


        //------------------------------------------------------------------------------------
        // Реализация интерфейса обновления
        //------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        //------------------------------------------------------------------------------------
        // Контекстное меню для выбора типа дня
        //------------------------------------------------------------------------------------
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem mi)
            {
                TypeDays type = (TypeDays) int.Parse(mi.Tag.ToString());
                SetTypeDay(LBdays.SelectedIndex + 1, type);
            }

        }

        //------------------------------------------------------------------------------------
        // Получение списка дней с типами
        //------------------------------------------------------------------------------------
        public IEnumerable<MonthDays> GetListDays()
        {
            return Days.Where(it => it.Type != it.OrigType);
        }




    }
}
