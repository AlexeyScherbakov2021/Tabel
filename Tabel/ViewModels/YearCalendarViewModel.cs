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

        IEnumerable<WorkCalendar> WorkCalendars;

        // выбранные дни в каждом месяце
        private int _SelectDay1;
        public int SelectDay1 { get => _SelectDay1;  set { Set(ref _SelectDay1, value); ChangeSelectedDate(0); } }

        private int _SelectDay2;
        public int SelectDay2 { get => _SelectDay2;  set { Set(ref _SelectDay2, value); ChangeSelectedDate(1); } }

        private int _SelectDay3;
        public int SelectDay3 { get => _SelectDay3;  set { Set(ref _SelectDay3, value); ChangeSelectedDate(2); }}

        private int _SelectDay4;
        public int SelectDay4 { get => _SelectDay4;  set { Set(ref _SelectDay4, value); ChangeSelectedDate(3); }}

        private int _SelectDay5;
        public int SelectDay5 { get => _SelectDay5;  set { Set(ref _SelectDay5, value); ChangeSelectedDate(4); }}

        private int _SelectDay6;
        public int SelectDay6 { get => _SelectDay6;  set { Set(ref _SelectDay6, value); ChangeSelectedDate(5); }}

        private int _SelectDay7;
        public int SelectDay7 { get => _SelectDay7;  set { Set(ref _SelectDay7, value); ChangeSelectedDate(6); }}

        private int _SelectDay8;
        public int SelectDay8 { get => _SelectDay8;  set { Set(ref _SelectDay8, value); ChangeSelectedDate(7); }}

        private int _SelectDay9;
        public int SelectDay9 { get => _SelectDay9;  set { Set(ref _SelectDay9, value); ChangeSelectedDate(8); }}

        private int _SelectDay10;
        public int SelectDay10 { get => _SelectDay10;  set { Set(ref _SelectDay10, value); ChangeSelectedDate(9); }}

        private int _SelectDay11;
        public int SelectDay11 { get => _SelectDay11;  set { Set(ref _SelectDay11, value); ChangeSelectedDate(10); }}

        private int _SelectDay12;
        public int SelectDay12 { get => _SelectDay12;  set { Set(ref _SelectDay12, value); ChangeSelectedDate(11); }}

        //List<int> ListSelectedDays;


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

        private int OldSelectedMonth = 0;


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

            LoadExDays(2022);

        }


        void ChangeSelectedDate(int index)
        {

            //for (int i = 0; i < ListSelectedDays1.Length; i++)
            //{
            //    if(i != index)
            //        ListSelectedDays1[i] = 0;
            //}
        }


        //--------------------------------------------------------------------------------------------------
        // загрузка выделенных дней из базы
        //--------------------------------------------------------------------------------------------------
        void LoadExDays(int year)
        {
            WorkCalendars = RepoCal.Items.Where(it => it.cal_year == year);
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

    }
}
