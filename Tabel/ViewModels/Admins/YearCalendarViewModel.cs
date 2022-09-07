using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views.Admins;

namespace Tabel.ViewModels
{
    internal class YearCalendarViewModel : ViewModel
    {
        private readonly RepositoryCalendar RepoCal = new RepositoryCalendar();

        public ObservableCollection<MonthDays>[] list12Month { get; set; }

        private ObservableCollection<MonthDays> _listMonth01;
        public ObservableCollection<MonthDays> listMonth01 { get => _listMonth01; set { Set( ref _listMonth01, value); }  }
        
        public ObservableCollection<MonthDays> listMonth02 { get; set; }
        public ObservableCollection<MonthDays> listMonth03 { get; set; }
        public ObservableCollection<MonthDays> listMonth04 { get; set; }
        public ObservableCollection<MonthDays> listMonth05 { get; set; }
        public ObservableCollection<MonthDays> listMonth06 { get; set; }
        public ObservableCollection<MonthDays> listMonth07 { get; set; }
        public ObservableCollection<MonthDays> listMonth08 { get; set; }
        public ObservableCollection<MonthDays> listMonth09 { get; set; }
        public ObservableCollection<MonthDays> listMonth10 { get; set; }
        public ObservableCollection<MonthDays> listMonth11 { get; set; }
        public ObservableCollection<MonthDays> listMonth12 { get; set; }


        private int _CurrentYear;
        public int CurrentYear 
        { 
            get => _CurrentYear; 
            set 
            {
                if (Set(ref _CurrentYear, value))
                    LoadExDays(_CurrentYear);
            } 
        }

        public ObservableCollection<int> ListYears { get; set ; }


        //--------------------------------------------------------------------------------------------------
        // конструктор
        //--------------------------------------------------------------------------------------------------
        public YearCalendarViewModel()
        {
            list12Month = new ObservableCollection<MonthDays>[12];
            listMonth01 = new ObservableCollection<MonthDays>();
            listMonth02 = new ObservableCollection<MonthDays>();
            listMonth03 = new ObservableCollection<MonthDays>();
            listMonth04 = new ObservableCollection<MonthDays>();
            listMonth05 = new ObservableCollection<MonthDays>();
            listMonth06 = new ObservableCollection<MonthDays>();
            listMonth07 = new ObservableCollection<MonthDays>();
            listMonth08 = new ObservableCollection<MonthDays>();
            listMonth09 = new ObservableCollection<MonthDays>();
            listMonth10 = new ObservableCollection<MonthDays>();
            listMonth11 = new ObservableCollection<MonthDays>();
            listMonth12 = new ObservableCollection<MonthDays>();

            list12Month[0] = listMonth01;
            list12Month[1] = listMonth02;
            list12Month[2] = listMonth03;
            list12Month[3] = listMonth04;
            list12Month[4] = listMonth05;
            list12Month[5] = listMonth06;
            list12Month[6] = listMonth07;
            list12Month[7] = listMonth08;
            list12Month[8] = listMonth09;
            list12Month[9] = listMonth10;
            list12Month[10] = listMonth11;
            list12Month[11] = listMonth12;

            //list12Month.Add(listMonth01);
            //list12Month.Add(listMonth02);
            //list12Month.Add(listMonth03);
            //list12Month.Add(listMonth04);
            //list12Month.Add(listMonth05);
            //list12Month.Add(listMonth06);
            //list12Month.Add(listMonth07);
            //list12Month.Add(listMonth08);
            //list12Month.Add(listMonth09);
            //list12Month.Add(listMonth10);
            //list12Month.Add(listMonth11);
            //list12Month.Add(listMonth12);

            //list12Month = new ObservableCollection<MonthDays>[12];
            //for (int i = 0; i < 12; i++)
            //    list12Month[i] = new ObservableCollection<MonthDays>();

            ListYears = new ObservableCollection<int>( RepoCal.GetYears());
            CurrentYear = ListYears.Last();
            ListYears.Add(CurrentYear + 1);
        }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand SelectCommand => new LambdaCommand(OnSelectCommandExecuted, CanSelectCommand);
        private bool CanSelectCommand(object p) => true;
        private void OnSelectCommandExecuted(object p)
        {

        }

        //--------------------------------------------------------------------------------
        // Команда сменить тип дня 
        //--------------------------------------------------------------------------------
        public ICommand SetExDayCommand => new LambdaCommand(OnSetExDayCommandExecuted, CanSetExDayCommand);
        private bool CanSetExDayCommand(object p) => true;
        private void OnSetExDayCommandExecuted(object p)
        {
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить год 
        //--------------------------------------------------------------------------------
        public ICommand NewYearCommand => new LambdaCommand(OnNewYearCommandExecuted, CanNewYearCommand);
        private bool CanNewYearCommand(object p) => true;
        private void OnNewYearCommandExecuted(object p)
        {

        }

        #endregion

        
        //--------------------------------------------------------------------------------------------------
        // загрузка выделенных дней из базы
        //--------------------------------------------------------------------------------------------------
        void LoadExDays(int year)
        {
            //IEnumerable<ObservableCollection<MonthDays>> list = new IEnumerable<ObservableCollection<MonthDays>>();

            for (int i = 0; i < 12; i++)
                list12Month[i].Clear();

            IEnumerable<WorkCalendar> days = RepoCal.GetDaysForYear(CurrentYear);
            foreach(var item in days)
            {
                MonthDays md = new MonthDays(item.cal_date.Value.Day, (TypeDays)item.cal_type);

                switch (item.cal_date.Value.Month)
                {
                    case 1:
                        listMonth01.Add(md);
                        break;
                }



                //list12Month[item.cal_date.Value.Month - 1].Add(md);
            }


            //listMonth01 = new ObservableCollection<MonthDays>();

            //for (int i = 0; i < 12; i++)
            //    list12Month[i] = list[i];

            //OnPropertyChanged(nameof(CurrentYear));
            //listMonth01 = new ObservableCollection<MonthDays>();
            //listMonth01.Add(new MonthDays(10, TypeDays.Holyday));
            //listMonth01.Add(new MonthDays(11, TypeDays.Holyday));
            //OnPropertyChanged(nameof(listMonth01));
        }


        //--------------------------------------------------------------------------------------------------
        // выделение отмеченных дней
        //--------------------------------------------------------------------------------------------------
        private void SetDaysForMonth(IEnumerable<WorkCalendar> ListDays)
        {

        }

    }
}
