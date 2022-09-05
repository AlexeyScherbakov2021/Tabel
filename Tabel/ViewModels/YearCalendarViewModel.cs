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
        private readonly IRepository<WorkCalendar> RepoCal;

        public List<MonthDays>[] list12Month { get; set; } 

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
            list12Month = new List<MonthDays>[12];
            for (int i = 0; i < 12; i++)
                list12Month[i] = new List<MonthDays>();

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
        }


        //--------------------------------------------------------------------------------------------------
        // выделение отмеченных дней
        //--------------------------------------------------------------------------------------------------
        private void SetDaysForMonth(IEnumerable<WorkCalendar> ListDays)
        {

        }

    }
}
