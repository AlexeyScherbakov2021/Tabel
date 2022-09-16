using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class TabelWindowViewModel : ViewModel
    {
        //int count = 2;

        public List<Months> ListMonth => App.ListMonth;

        private RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>();
        //private readonly RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
        private readonly RepositoryMSSQL<typeDay> repoTypeDay = new RepositoryMSSQL<typeDay>();

        public WorkTabel Tabel { get; set; }

        public IEnumerable<typeDay> ListTypeDays { get; set; }

        public User User { get; set; }

        private int _CurrentMonth;
        public int CurrentMonth 
        { 
            get => _CurrentMonth; 
            set
            {
                if (Set(ref _CurrentMonth, value))
                    LoadPersonForOtdel(_SelectedOtdel);
            }
        }
        
        public int CurrentYear { get; set; }
        private DateTime _CurrentDate;

        public List<Otdel> ListOtdel { get; set; }


        #region Удалить после тестирования

        //public List<int> ItemsInt { get; set; } = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        //public ObservableCollection<TabelDay> TestTabelDays { get; set; }


        //private void InitTest(int cnt)
        //{
        //    TestTabelDays = new ObservableCollection<TabelDay>()
        //    {
        //        new TabelDay() { td_Day = 1, td_Hours = 8, VisibilityHours = Visibility.Visible, td_Hours2 = 4, Kind = ListTypeDays[cnt] },
        //        new TabelDay() { td_Day = 2, td_Hours = 7, Kind = ListTypeDays[cnt] },
        //        new TabelDay() { td_Day = 3, td_Hours = 6, Kind = ListTypeDays[cnt]  },
        //        new TabelDay() { td_Day = 4, td_Hours = 8, Kind = ListTypeDays[cnt]  },
        //        new TabelDay() { td_Day = 5, td_Hours = 3, Kind = ListTypeDays[cnt]  },
        //        new TabelDay() { td_Day = 6, td_Hours = 7, },
        //        new TabelDay() { td_Day = 7, td_Hours = 5, },
        //        new TabelDay() { td_Day = 8, td_Hours = 2, },
        //        new TabelDay() { td_Day = 9, td_Hours = 3, },
        //    };

        //    OnPropertyChanged(nameof(TestTabelDays));

        //}

        #endregion


        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel
        {

            get => _SelectedOtdel;
            set
            {
                if (Set(ref _SelectedOtdel, value))
                {
                    LoadPersonForOtdel(_SelectedOtdel);
                }
            }
        }



        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => true;
        private void OnCreateCommandExecuted(object p)
        {

            if (Tabel != null)
                repoTabel.Remove(Tabel);

            RepositoryCalendar repo = new RepositoryCalendar();
            IEnumerable<WorkCalendar> cal = repo.Items.AsNoTracking().Where(it => it.cal_year == CurrentYear
                    && it.cal_date.Month == CurrentMonth);


            repoPersonal = new RepositoryMSSQL<Personal>();
            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();

            Tabel = new WorkTabel();
            Tabel.t_author_id = User.id;
            Tabel.t_otdel_id = SelectedOtdel.id;
            Tabel.t_month = CurrentMonth;
            Tabel.t_year = CurrentYear;
            Tabel.t_date_create = DateTime.Now;
            Tabel.tabelPersons = new ObservableCollection<TabelPerson>();

            DateTime StartDay = new DateTime(CurrentYear, CurrentMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TabelPerson tp = new TabelPerson();
                tp.tp_person_id = item.id;
                tp.TabelDays = new ObservableCollection<TabelDay>();

                for (DateTime IndexDate = StartDay; IndexDate.Month == CurrentMonth; IndexDate = IndexDate.AddDays(1))
                {
                    WorkCalendar ChangeDay = cal.FirstOrDefault(it => it.cal_date == IndexDate);

                    TabelDay td = new TabelDay();
                    td.td_Day = IndexDate.Day;

                    if (ChangeDay != null)
                    {
                        td.CalendarTypeDay = ChangeDay.cal_type;
                        switch(td.CalendarTypeDay)
                        {
                            case TypeDays.Holyday:
                                td.td_KindId = 2;
                                td.td_Hours = 0;
                                break;
                            case TypeDays.Work:
                                td.td_KindId = 1;
                                td.td_Hours = 8;
                                break;
                            case TypeDays.ShortWork:
                                td.td_KindId = 1;
                                td.td_Hours = 7;
                                break;
                        }
                    }
                    else
                    {
                        if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            td.td_KindId = 2;
                            td.CalendarTypeDay = TypeDays.Holyday;
                            td.td_Hours = 0;
                        }
                        else
                        {
                            td.td_KindId = 1;
                            td.CalendarTypeDay = TypeDays.Work;
                            td.td_Hours = 8;
                        }
                    }
                    tp.TabelDays.Add(td);
                }

                //tp.SetCalendarTypeDays();
                Tabel.tabelPersons.Add(tp);
            }

            if (Tabel.tabelPersons.Count > 0)
                repoTabel.Add(Tabel, true);

            OnPropertyChanged(nameof(Tabel));

        }



        //--------------------------------------------------------------------------------
        // Команда Выбрать тип дня
        //--------------------------------------------------------------------------------
        public ICommand SelectTypeCommand => new LambdaCommand(OnSelectTypeCommandExecuted, CanSelectTypeCommand);
        private bool CanSelectTypeCommand(object p) => true;
        private void OnSelectTypeCommandExecuted(object p)
        {
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        public ICommand LoadDefCommand => new LambdaCommand(OnLoadDefCommandExecuted, CanLoadDefCommand);
        private bool CanLoadDefCommand(object p) => true;
        private void OnLoadDefCommandExecuted(object p)
        {

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoTabel.Save();
        }


        #endregion


        public TabelWindowViewModel()
        {
            _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;
            User = App.CurrentUser;
            User = new User() { u_otdel_id = 44, u_login = "Petrov", id = 10, u_fio = "Петров" };
            ListOtdel = repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();
            ListTypeDays = repoTypeDay.Items.ToList();
            OnPropertyChanged(nameof(ListTypeDays));

        }


        //----------------------------------------------------------------------------------------------------------
        // загрузка дней для сотрудника
        //----------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------
        private void LoadPersonForOtdel(Otdel otdel)
        {
            if (otdel is null)
                return;

            //InitTest(count++);
            //if (count > 5) count = 2;

            Tabel = repoTabel.Items.FirstOrDefault(it => it.t_year == CurrentYear
                && it.t_month == CurrentMonth
                && it.t_otdel_id == otdel.id);

            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();

            if (Tabel != null)
            {
                foreach(var item in Tabel.tabelPersons)
                {
                    item.SetCalendarTypeDays();
                }


                // если график присутствует
                //foreach (var item in PersonsFromOtdel)
                //{
                //    TabelPerson pers = repoTabelPerson.Items.FirstOrDefault(it => it.tp_tabel_id == Tabel.id && it.tp_person_id == item.id);
                    
                //    if (pers is null)
                //    {
                //        // формируем новый график на месяц для сотрудника, который отсутствовал
                //    }

                //}

            }

            OnPropertyChanged(nameof(Tabel));
        }

    }
}
