using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        //private readonly IRepository<User> repo;
        private readonly IRepository<typeDay> repoTypeDay;
        public List<typeDay> ListTypeDay { get; set; }

        private readonly IRepository<WorkCalendar> repoCalendar;

        //private string _FIO;
        public string FIO { get; set; }
        //{
        //    get => _FIO;
        //    set { Set(ref _FIO, SelectedPerson?.p_lastname);   }
            //get => SelectedPerson?.p_lastname + " " + SelectedPerson?.p_name + " " + SelectedPerson?.p_midname;
            //set {   OnPropertyChanged(nameof(FIO)); }
        //}


        private readonly IRepository<Personal> repoPerson;

        public User CurrentUser { get; set; }
        public List<Personal> PersonalList { get; set; }
        private Personal _SelectedPerson;
        public Personal SelectedPerson 
        { 
            get => _SelectedPerson; 
            set
            {
                if (Set(ref _SelectedPerson, value))
                {
                    FIO = SelectedPerson?.p_lastname + " " + SelectedPerson?.p_name + " " + SelectedPerson?.p_midname;
                    OnPropertyChanged(nameof(FIO));
                    LoadTabelPerson();
                }
            }
        }

        //private bool _IsOpenPopup = false;
        //public bool IsOpenPopup { get => _IsOpenPopup; set { Set(ref _IsOpenPopup , value); } }


        private readonly IRepository<WorkTabel> repoTabel;
        public WorkTabel CurrentTabel { get; set; }

        private readonly IRepository<TabelPerson> repoTabelPerson;


        private ObservableCollection<TabelPerson> _ListTabelPerson; 
        public ObservableCollection<TabelPerson> ListTabelPerson { get => _ListTabelPerson; set { Set(ref _ListTabelPerson, value); } } 

        public List<MonthStart1> ListMonth { get; set; }



        #region Команды
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
            List<WorkCalendar> listCal = repoCalendar.Items.Where(it => it.cal_year == CurrentTabel.t_year 
                        && it.cal_date.Value.Month == CurrentTabel.t_month).ToList();

            
            foreach(var item in  ListTabelPerson)
            {
                var cal = listCal.FirstOrDefault(it => it.cal_date.Value.Day == item.d_day);
                if(cal is null)
                {
                    int WeekDay = (int) new DateTime(CurrentTabel.t_year, CurrentTabel.t_month, item.d_day).DayOfWeek;
                    if (WeekDay == 0 || WeekDay == 6)
                    {
                        item.d_type = 2;
                        item.d_hours = null;
                    }
                    else
                    {
                        item.d_type = 1;
                        item.d_hours = 8;
                    }
                }
                else
                {
                    item.d_type = cal.cal_type;
                    item.d_hours = 8;
                }

                item.OnPropertyChanged(nameof(item.d_type));
                item.OnPropertyChanged(nameof(item.d_hours));

            }

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {




            // получаем список измененных дней календаря
            List<WorkCalendar> listCal = repoCalendar.Items.Where(it => it.cal_year == CurrentTabel.t_year
                        && it.cal_date.Value.Month == CurrentTabel.t_month).ToList();

            // удаление всех предыдущих записей из базы
            foreach (var item in ListTabelPerson)
                repoTabelPerson.Delete(item, true);

            //repoTabelPerson.Save();
            //repoTabelPerson.ReamoveForPersonMonth(CurrentTabel.id, SelectedPerson.id, true);

            //repoTabel.Add(CurrentTabel, true);

            foreach (var item in ListTabelPerson)
            {
                int type;
                var wc = listCal.FirstOrDefault(it => it.cal_date.Value.Day == item.d_day);
                if (wc is null)
                {
                    // в измененных датах нет
                    // получаем номер дня недели
                    type = (int)new DateTime(CurrentTabel.t_year, CurrentTabel.t_month, item.d_day).DayOfWeek;
                    if (type == 0 || type == 6)
                        // это выходной
                        type = 2;
                    else
                        // рабочий день
                        type = 1;
                }
                else
                    type = wc.cal_type;

                if (item.d_type != null && type != item.d_type)
                    repoTabelPerson.Add(item, true);
            }
    
            //repoTabelPerson.Save();
        }


        #endregion



        public MainWindowViewModel()
        {
            ListMonth = new List<MonthStart1> { 
                new MonthStart1 { Name = "Январь", Number = 1 },
                new MonthStart1 { Name = "Февраль", Number = 2 },
                new MonthStart1 { Name = "Март", Number = 3 },
                new MonthStart1 { Name = "Апрель", Number = 4 },
                new MonthStart1 { Name = "Май", Number = 5 },
                new MonthStart1 { Name = "Июнь", Number = 6 },
                new MonthStart1 { Name = "Июль", Number = 7 },
                new MonthStart1 { Name = "Август", Number = 8 },
                new MonthStart1 { Name = "Сентябрь", Number = 9 },
                new MonthStart1 { Name = "Октябрь", Number = 10 },
                new MonthStart1 { Name = "Ноябрь", Number = 11 },
                new MonthStart1 { Name = "Декабрь", Number = 12 },
            };


            CurrentUser = App.CurrentUser;
            repoPerson = new RepositoryMSSQL<Personal>();
            repoTabel = new RepositoryMSSQL<WorkTabel>();
            repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
            repoTypeDay = new RepositoryMSSQL<typeDay>();
            repoCalendar = new RepositoryMSSQL<WorkCalendar>();

            ListTypeDay = repoTypeDay.Items.ToList();

            PersonalList = repoPerson.Items
                .Where(it => it.p_otdel_id == CurrentUser.u_otdel_id)
                .OrderBy(it => it.p_lastname)
                .ToList();

            // получение текущего табеля
            CurrentTabel = repoTabel.Items
                .Where(it => it.t_otdel_id == CurrentUser.u_otdel_id)
                .OrderByDescending(it => it.t_year)
                .ThenByDescending(it => it.t_month)
                .FirstOrDefault();

            if(CurrentTabel is null)
            {
                // табеля для отдела еще нет, создаем
                CurrentTabel = new WorkTabel
                {
                    t_otdel_id = CurrentUser.u_otdel_id,
                    t_date_create = DateTime.Now,
                    t_month = DateTime.Now.Month,
                    t_year = DateTime.Now.Year,
                    t_author = CurrentUser.u_login,
                    t_status = 0
                };
                repoTabel.Add(CurrentTabel, true);

                // добавить табеля для каждого сотрудника

            }


        }


        //----------------------------------------------------------------------------------------------------------
        // загрузка дней для сотрудника
        //----------------------------------------------------------------------------------------------------------
        private void LoadTabelPerson()
        {
            //ListTabelPerson.Clear();
            ListTabelPerson = new ObservableCollection<TabelPerson>( repoTabelPerson.Items
                .Where(it => it.person.id == SelectedPerson.id && it.tabel.id == CurrentTabel.id));

            //List<TabelPerson> listFromBase = repoTabelPerson.Items
            //    .Where(it => it.person.id == SelectedPerson.id && it.tabel.id == CurrentTabel.id)
            //    .ToList();

            //ListTabelPerson = new ObservableCollection<TabelPerson>();
            //ListTabelPerson.Clear();
            //DateTime date = new DateTime(CurrentTabel.t_year, CurrentTabel.t_month, 1);
            //int maxDay =  date.AddMonths(1).AddDays(-1).Day;

            //for (int i = 1; i <= maxDay; i++)
            //{
            //    var tp = new TabelPerson
            //    {
            //        d_day = i,
            //        d_tabel_id = CurrentTabel.id,
            //        d_person_id = SelectedPerson.id,
            //        d_type = 1,
            //        d_hours = 8
            //    };

            //    int dw = (int)new DateTime(CurrentTabel.t_year, CurrentTabel.t_month, i).DayOfWeek;
            //    if (dw == 0 || dw == 6)
            //    {
            //        tp.d_type = 2;
            //        tp.d_hours = null;
            //    }

            //    ListTabelPerson.Add(tp);
            //}

            //// заменяем установленные дни
            //foreach (var item in listFromBase)
            //{
            //    TabelPerson tp = ListTabelPerson.FirstOrDefault(it => it.d_day == item.d_day);
            //    tp.d_hours = item.d_hours;
            //    tp.d_type = item.d_type;
            //    tp.id = item.id;

            //}


        }
    }
}
