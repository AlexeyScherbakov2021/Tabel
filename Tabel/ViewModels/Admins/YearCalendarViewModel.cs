using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly RepositoryCalendar RepoCal = AllRepo.GetRepoCalendar();

        public Visibility VisibleAdmin => App.CurrentUser.u_role == Infrastructure.UserRoles.Admin ? Visibility.Visible : Visibility.Hidden;  

        public ObservableCollection<MonthDays>[] list12Month { get; set; }

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
            
            for (int i = 0; i < 12; i++)
                list12Month[i] = new ObservableCollection<MonthDays>();

            ListYears = new ObservableCollection<int>( RepoCal.GetYears());
            CurrentYear = 2000;
            //ListYears.Add(CurrentYear + 1);
        }


        #region Команды

        //--------------------------------------------------------------------------------
        // Событие загрузки
        //--------------------------------------------------------------------------------
        public ICommand OnLoadedCommand => new LambdaCommand(OnOnLoadedCommandExecuted, CanOnLoadedCommand);
        private bool CanOnLoadedCommand(object p) => true;
        private void OnOnLoadedCommandExecuted(object p)
        {
            CurrentYear = ListYears.Last();
        }



        //--------------------------------------------------------------------------------
        // Команда Сохранить 
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            List<MonthDays> AllDays = new List<MonthDays>();

            foreach (var item in list12Month)
            {
                AllDays.AddRange(item);
            }

            RepoCal.SaveDays(AllDays, CurrentYear);
        }

        //--------------------------------------------------------------------------------
        // Команда По календарю
        //--------------------------------------------------------------------------------
        public ICommand SetAsCalendarCommand => new LambdaCommand(OnSetAsCalendarCommandExecuted, CanSetAsCalendarCommand);
        private bool CanSetAsCalendarCommand(object p) => true;
        private void OnSetAsCalendarCommandExecuted(object p)
        {
            for (int i = 0; i < 12; i++)
                list12Month[i].Clear();
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить год 
        ////--------------------------------------------------------------------------------
        public ICommand NewYearCommand => new LambdaCommand(OnNewYearCommandExecuted, CanNewYearCommand);
        private bool CanNewYearCommand(object p) => true;
        private void OnNewYearCommandExecuted(object p)
        {
            CurrentYear = ListYears.Last() + 1;
            ListYears.Add(CurrentYear);
        }

        #endregion


        //--------------------------------------------------------------------------------------------------
        // загрузка выделенных дней из базы
        //--------------------------------------------------------------------------------------------------
        void LoadExDays(int year)
        {
            for (int i = 0; i < 12; i++)
                list12Month[i].Clear();

            IEnumerable<WorkCalendar> days = RepoCal.GetDaysForYear(CurrentYear);
            foreach(var item in days)
            {
                MonthDays md = new MonthDays(item.cal_date.Day, item.cal_date.Month, (TypeDays)item.cal_type);
                list12Month[item.cal_date.Month-1].Add(md);
            }
        }


        //--------------------------------------------------------------------------------------------------
        // выделение отмеченных дней
        //--------------------------------------------------------------------------------------------------
        //private void SetDaysForMonth(IEnumerable<WorkCalendar> ListDays)
        //{

        //}

    }
}
