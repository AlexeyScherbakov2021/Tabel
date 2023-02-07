using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.ViewModels.DataSummForm;

namespace Tabel.ViewModels
{
    internal class DataSummaryViewModel : ViewModel
    {

        public IDataSumm PlanViewModel {get;set;}
        public IDataSumm PayViewModel {get;set;}


        public User User => App.CurrentUser;

        public List<Months> ListMonth => App.ListMonth;
        public List<int> ListYears { get; set; }

        private int _CurrentMonth;
        public int CurrentMonth { get => _CurrentMonth; set { if (Set(ref _CurrentMonth, value)) ChangePeriod();  } }

        private int _CurrentYear;
        public int CurrentYear { get => _CurrentYear; set { if (Set(ref _CurrentYear, value)) ChangePeriod(); } }

        public Visibility IsVisiblePlan { get; set; }
        public Visibility IsVisiblePay { get; set; }

        public int SelectedIndexTab { get; set; } = 0;

        //------------------------------------------------------------------------------------------------
        // Конструктор
        //------------------------------------------------------------------------------------------------ 
        public DataSummaryViewModel()
        {
            PlanViewModel = new PlanUCViewModel();
            PayViewModel = new PayUCViewModel();

            IsVisiblePay = User.u_role == UserRoles.Admin || User.u_role.Equals(UserRoles.Бухгалтерия)
                ? Visibility.Visible : Visibility.Collapsed;

            IsVisiblePlan = User.u_role == UserRoles.Admin || User.id == 12
                ? Visibility.Visible : Visibility.Collapsed;

            if (IsVisiblePlan == Visibility.Collapsed)
                SelectedIndexTab = 1;


            RepositoryCalendar repoCal = new RepositoryCalendar();

            ListYears = repoCal.GetYears().ToList();

            DateTime _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;

        }

        //------------------------------------------------------------------------------------------------
        // Изменение периода
        //------------------------------------------------------------------------------------------------ 
        private void ChangePeriod()
        {

            if (CurrentYear < 1 || CurrentMonth < 1) return;

            if(IsVisiblePlan == Visibility.Visible)
                PlanViewModel?.ChangePeriod(CurrentYear, CurrentMonth);

            if(IsVisiblePay == Visibility.Visible)
                PayViewModel?.ChangePeriod(CurrentYear, CurrentMonth);
        }

        

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            if (IsVisiblePlan == Visibility.Visible)
                PlanViewModel.Save();

            if (IsVisiblePay == Visibility.Visible)
                PayViewModel.Save();

        }

        //--------------------------------------------------------------------------------
        // событие закрытия программы
        //--------------------------------------------------------------------------------
        public ICommand ClosingCommand => new LambdaCommand(OnClosingExecuted, CanClosingCommand);
        private bool CanClosingCommand(object p) => true;
        public void OnClosingExecuted(object p)
        {
            CancelEventArgs e = p as CancelEventArgs;

            bool isModify = PayViewModel.ClosingFrom();

            if (isModify == true)
            {
                MessageBoxResult res;
                res = MessageBox.Show("Сохранить измененные данные?", "Предупреждение",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                    PayViewModel.Save();
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

        }

        #endregion
    }
}
