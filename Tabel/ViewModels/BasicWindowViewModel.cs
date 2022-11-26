using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class BasicWindowViewModel : ViewModel
    {
        public UserControl SourceContent { get; set; }
        private IBaseUCViewModel _UCViewModel;

        //private string _Title;
        //public string Title { get => _Title; set { Set(ref _Title, value); } }
        public string Title { get; set; }
        
        //private readonly RepositoryMSSQL<Otdel> repoOtdel = AllRepo.GetRepoOtdel();
        public List<Otdel> ListOtdel { get; set; }

        public List<Months> ListMonth => App.ListMonth;
        public User User { get; set; }
        
        private int _CurrentMonth;
        public int CurrentMonth
        {
            get => _CurrentMonth;
            set
            {
                if (Set(ref _CurrentMonth, value)) 
                    _UCViewModel.OtdelChanged(_SelectedOtdel, _CurrentYear, _CurrentMonth);
            }
        }


        public List<int> ListYears { get; set; }


        private int _CurrentYear;
        public int CurrentYear 
        { 
            get => _CurrentYear;
            set
            {
                if (Set(ref _CurrentYear, value))
                    _UCViewModel.OtdelChanged(_SelectedOtdel, _CurrentYear, _CurrentMonth);
            }
        }



        private DateTime _CurrentDate;

        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel
        {

            get => _SelectedOtdel;
            set
            {
                if (Set(ref _SelectedOtdel, value))
                {
                    _UCViewModel.OtdelChanged(_SelectedOtdel, _CurrentYear, _CurrentMonth);
                }
            }
        }


        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        //public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        //private bool CanSaveCommand(object p) => true;
        //private void OnSaveCommandExecuted(object p)
        //{
        //    //repoTabel.Save();
        //}
        //--------------------------------------------------------------------------------
        // событие закрытия программы
        //--------------------------------------------------------------------------------
        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool isModify = _UCViewModel.ClosingFrom();

            if (isModify == true)
            {
                MessageBoxResult res;
                res = MessageBox.Show("Сохранить измененные данные?", "Предупреждение",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                    _UCViewModel.SaveForm();
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }


        #endregion



        //--------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------
        public BasicWindowViewModel() { }

        public BasicWindowViewModel(BasicWindow win,  UserControl control, string title)
        {
            win.Closing += MainWindow_Closing;

            Title = title;
            SourceContent = control;
            _UCViewModel = control.DataContext as IBaseUCViewModel;

            _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;
            //User = App.CurrentUser;

            RepositoryCalendar repoCal = new RepositoryCalendar();// AllRepo.GetRepoCalendar();
            ListYears = repoCal.GetYears().ToList();

            RepositoryOtdel repo = new RepositoryOtdel(repoCal.GetDB());// AllRepo.GetRepoOtdel();
            RepositoryMSSQL<User> repoUser = new RepositoryMSSQL<User>(repoCal.GetDB());

            User = repoUser.Items.FirstOrDefault(it => it.id == App.CurrentUser.id);

            ListOtdel = repo.GetTreeOtdelsNoTracking(User.otdels).ToList();

            if (ListOtdel?.Count > 0)
                SelectedOtdel = ListOtdel[0];
        }
    }
}
