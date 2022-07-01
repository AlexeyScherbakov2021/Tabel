using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views.Admins;

namespace Tabel.ViewModels
{
    internal class YearCalendarViewModel : ViewModel
    {
        private readonly IRepository<WorkCalendar> RepoCal;

        List<WorkCalendar> WorkCalendars;

        public int CurrentYear { get; set; }
        public List<int> ListYears { get; set; }


        // выбранные дни в каждом месяце
        private int _SelectDay1;
        public int SelectDay1 { get => _SelectDay1;  set { Set(ref _SelectDay1, value); } }

        private int _SelectDay2;
        public int SelectDay2 { get => _SelectDay2;  set { Set(ref _SelectDay2, value); } }

        private int _SelectDay3;
        public int SelectDay3 { get => _SelectDay3;  set { Set(ref _SelectDay3, value); }}

        private int _SelectDay4;
        public int SelectDay4 { get => _SelectDay4;  set { Set(ref _SelectDay4, value); }}

        private int _SelectDay5;
        public int SelectDay5 { get => _SelectDay5;  set { Set(ref _SelectDay5, value); }}

        private int _SelectDay6;
        public int SelectDay6 { get => _SelectDay6;  set { Set(ref _SelectDay6, value); }}

        private int _SelectDay7;
        public int SelectDay7 { get => _SelectDay7;  set { Set(ref _SelectDay7, value); }}

        private int _SelectDay8;
        public int SelectDay8 { get => _SelectDay8;  set { Set(ref _SelectDay8, value); }}

        private int _SelectDay9;
        public int SelectDay9 { get => _SelectDay9;  set { Set(ref _SelectDay9, value); }}

        private int _SelectDay10;
        public int SelectDay10 { get => _SelectDay10;  set { Set(ref _SelectDay10, value); }}

        private int _SelectDay11;
        public int SelectDay11 { get => _SelectDay11;  set { Set(ref _SelectDay11, value); }}

        private int _SelectDay12;
        public int SelectDay12 { get => _SelectDay12;  set { Set(ref _SelectDay12, value); }}


        // январь
        private Dictionary<int, MonthControl.TypeDays> _exDays1 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays1 { get => _exDays1; set { Set(ref _exDays1, value); } }

        // февраль
        private Dictionary<int, MonthControl.TypeDays> _exDays2 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays2 { get => _exDays2; set { Set(ref _exDays2, value); } }

        // март
        private Dictionary<int, MonthControl.TypeDays> _exDays3 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays3 { get => _exDays3; set { Set(ref _exDays3, value); } }

        // апрель
        private Dictionary<int, MonthControl.TypeDays> _exDays4 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays4 { get => _exDays4; set { Set(ref _exDays4, value); } }

        // май
        private Dictionary<int, MonthControl.TypeDays> _exDays5 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays5 { get => _exDays5; set { Set(ref _exDays5, value); } }

        // июнь
        private Dictionary<int, MonthControl.TypeDays> _exDays6 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays6 { get => _exDays6; set { Set(ref _exDays6, value); } }

        // июль
        private Dictionary<int, MonthControl.TypeDays> _exDays7 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays7 { get => _exDays7; set { Set(ref _exDays7, value); } }

        // авнуст
        private Dictionary<int, MonthControl.TypeDays> _exDays8 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays8 { get => _exDays8; set { Set(ref _exDays8, value); } }

        // снетябрь
        private Dictionary<int, MonthControl.TypeDays> _exDays9 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays9 { get => _exDays9; set { Set(ref _exDays9, value); } }

        // октябрь
        private Dictionary<int, MonthControl.TypeDays> _exDays10 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays10 { get => _exDays10; set { Set(ref _exDays10, value); } }

        // ноябрь
        private Dictionary<int, MonthControl.TypeDays> _exDays11 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays11 { get => _exDays11; set { Set(ref _exDays11, value); } }

        // декабрь
        private Dictionary<int, MonthControl.TypeDays> _exDays12 = new Dictionary<int, MonthControl.TypeDays>();
        public Dictionary<int, MonthControl.TypeDays> exDays12 { get => _exDays12; set { Set(ref _exDays12, value); } }


        // список ссылок на месяцы
        List<Dictionary<int, MonthControl.TypeDays>> ListMonthDays;


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand SelectCommand => new LambdaCommand(OnSelectCommandExecuted, CanSelectCommand);
        private bool CanSelectCommand(object p) => true;
        private void OnSelectCommandExecuted(object p)
        {
            SelectDay1 = 0;
            SelectDay2 = 0;
            SelectDay3 = 0;
            SelectDay4 = 0;
            SelectDay5= 0;
            SelectDay6 = 0;
            SelectDay7 = 0;
            SelectDay8 = 0;
            SelectDay9 = 0;
            SelectDay10 = 0;
            SelectDay11 = 0;
            SelectDay12 = 0;
        }

        //--------------------------------------------------------------------------------
        // Команда Выходной 
        //--------------------------------------------------------------------------------
        public ICommand SetExDayCommand => new LambdaCommand(OnSetExDayCommandExecuted, CanSetExDayCommand);
        private bool CanSetExDayCommand(object p) => true;
        private void OnSetExDayCommandExecuted(object p)
        {
            MonthControl.TypeDays type = (MonthControl.TypeDays)p;
            DateTime curDate = GetSelectedDay();

            WorkCalendar wc = WorkCalendars.SingleOrDefault(it =>  it.cal_date == curDate );

            if (type == MonthControl.TypeDays.Default)
            {
                ListMonthDays[curDate.Month - 1].Remove(curDate.Day);
                RepoCal.Delete(wc.id, true);
            }

            else if (ListMonthDays[curDate.Month - 1].ContainsKey(curDate.Day))
            {
                ListMonthDays[curDate.Month - 1][curDate.Day] = type;
                wc.cal_type = (int)type;
                RepoCal.Update(wc, true);
            }
            else
            {
                ListMonthDays[curDate.Month - 1].Add(curDate.Day, type);
                wc = new WorkCalendar { cal_date = curDate, cal_year = CurrentYear, cal_type = (int)type };
                RepoCal.Add(wc, true);
            }


        }

        #endregion

        //--------------------------------------------------------------------------------------------------
        // конструктор
        //--------------------------------------------------------------------------------------------------
        public YearCalendarViewModel()
        {
            RepoCal = new RepositoryMSSQL<WorkCalendar>();
            ListMonthDays = new List<Dictionary<int, MonthControl.TypeDays>>
            {
                exDays1, exDays2, exDays3, exDays4, exDays5, exDays6, 
                exDays7, exDays8, exDays9, exDays10, exDays11, exDays12
            };


            ListYears = RepoCal.Items.Select(it => it.cal_year).Distinct().OrderBy(it => it).ToList();

            IRepository<WorkTabel> tabel = new RepositoryMSSQL<WorkTabel>();
            List<int> list = tabel.Items.Select(it => it.t_year).Distinct().OrderBy(it => it).ToList();
            ListYears.AddRange(list);
            ListYears.Sort();

            if(ListYears.Count == 0)
            {
                CurrentYear = DateTime.Now.Year;
                ListYears.Add(CurrentYear);
            }
            else
                CurrentYear = ListYears.Last();

            LoadExDays(CurrentYear);

        }

        
        //--------------------------------------------------------------------------------------------------
        // загрузка выделенных дней из базы
        //--------------------------------------------------------------------------------------------------
        void LoadExDays(int year)
        {
            WorkCalendars = RepoCal.Items.Where(it => it.cal_year == year).ToList();
            SetDaysForMonth(WorkCalendars);
        }


        //--------------------------------------------------------------------------------------------------
        // выделение отмеченных дней
        //--------------------------------------------------------------------------------------------------
        private void SetDaysForMonth(IEnumerable<WorkCalendar> ListDays)
        {
            foreach(var item in ListDays)
            {
                ListMonthDays[item.cal_date.Value.Month-1].Add(item.cal_date.Value.Day, (MonthControl.TypeDays)item.cal_type);
            }

        }

        //--------------------------------------------------------------------------------------------------
        // Получение выделенного дня
        //--------------------------------------------------------------------------------------------------
        private DateTime GetSelectedDay()
        {
            DateTime dt = new DateTime(1, 1, 1);

            int[] array = { SelectDay1, SelectDay2, SelectDay3, SelectDay4,
                        SelectDay5, SelectDay6, SelectDay7, SelectDay8,
                        SelectDay9, SelectDay10, SelectDay11, SelectDay12
            };

            for(int month = 0; month < 12; month++)
            {
                if (array[month] != 0)
                {
                    dt = new DateTime(CurrentYear, month+1, array[month]);
                    break;
                }
            }


            return dt;
        }

    }
}
