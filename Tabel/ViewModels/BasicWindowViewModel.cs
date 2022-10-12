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

namespace Tabel.ViewModels
{
    internal class BasicWindowViewModel : ViewModel
    {
        public UserControl SourceContent { get; set; }
        private IBaseUCViewModel _UCViewModel;

        //private string _Title;
        //public string Title { get => _Title; set { Set(ref _Title, value); } }
        public string Title { get; set; }
        
        private readonly RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
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
        // Команда Создать график
        //--------------------------------------------------------------------------------
        //public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        //private bool CanCreateCommand(object p) => true;
        //private void OnCreateCommandExecuted(object p)
        //{


        //}



        //--------------------------------------------------------------------------------
        // Команда Выбрать тип дня
        //--------------------------------------------------------------------------------
        //public ICommand SelectTypeCommand => new LambdaCommand(OnSelectTypeCommandExecuted, CanSelectTypeCommand);
        //private bool CanSelectTypeCommand(object p) => true;
        //private void OnSelectTypeCommandExecuted(object p)
        //{
        //}

        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        //public ICommand LoadDefCommand => new LambdaCommand(OnLoadDefCommandExecuted, CanLoadDefCommand);
        //private bool CanLoadDefCommand(object p) => true;
        //private void OnLoadDefCommandExecuted(object p)
        //{

        //}

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        //public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        //private bool CanSaveCommand(object p) => true;
        //private void OnSaveCommandExecuted(object p)
        //{
        //    //repoTabel.Save();
        //}


        #endregion



        //--------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------
        public BasicWindowViewModel() { }

        public BasicWindowViewModel(UserControl control, string title)
        {
            Title = title;
            SourceContent = control;
            _UCViewModel = control.DataContext as IBaseUCViewModel;

            _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;
            User = App.CurrentUser;
            //User = new User() { u_otdel_id = 44, u_login = "Petrov", id = 10, u_fio = "Петров" };

            RepositoryCalendar repoCal = new RepositoryCalendar();
            ListYears = repoCal.GetYears().ToList();

            //ListOtdel = repoOtdel.Items.ToList();

            RepositoryOtdel repo = new RepositoryOtdel();

            ListOtdel = repo.GetTreeOtdels(User.otdels).ToList();

            //ListOtdel = repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();
            if (ListOtdel?.Count > 0)
                SelectedOtdel = ListOtdel[0];
        }
    }
}
