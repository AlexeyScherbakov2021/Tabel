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


        private readonly IRepository<Personal> repoPerson;

        public User CurrentUser { get; set; }
        public List<Personal> PersonalList { get; set; }
        private Personal _SelectedPerson;
        public Personal SelectedPerson 
        { 
            get => _SelectedPerson; 
            set
            {
                _SelectedPerson = value;
                LoadTabelPerson();
            }
        }

        private bool _IsOpenPopup = false;
        public bool IsOpenPopup { get => _IsOpenPopup; set { Set(ref _IsOpenPopup , value); } }


        private readonly IRepository<WorkTabel> repoTabel;
        public WorkTabel CurrentTabel { get; set; }

        private readonly IRepository<TabelPerson> repoTabelPerson;
        public ObservableCollection<TabelPerson> ListTabelPerson { get; set; } = new ObservableCollection<TabelPerson>();

        public List<MonthStart1> ListMonth { get; set; }



        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Выбрать тип дня
        //--------------------------------------------------------------------------------
        public ICommand SelectTypeCommand => new LambdaCommand(OnSelectTypeCommandExecuted, CanSelectTypeCommand);
        private bool CanSelectTypeCommand(object p) => true;
        private void OnSelectTypeCommandExecuted(object p)
        {
            IsOpenPopup = !IsOpenPopup;
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
            List<TabelPerson> listFromBase = repoTabelPerson.Items
                .Where(it => it.person.id == SelectedPerson.id && it.tabel.id == CurrentTabel.id)
                .ToList();

            //ListTabelPerson = new ObservableCollection<TabelPerson>();
            ListTabelPerson.Clear();
            DateTime date = new DateTime(CurrentTabel.t_year, CurrentTabel.t_month, 1);
            int maxDay =  date.AddMonths(1).AddDays(-1).Day;

            for (int i = 1; i <= maxDay; i++)
                ListTabelPerson.Add(new TabelPerson
                {
                    d_day = i,
                    d_tabel_id = CurrentTabel.id,
                    d_person_id = SelectedPerson.id,
                });




        }
    }
}
