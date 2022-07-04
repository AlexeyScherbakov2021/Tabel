using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IRepository<Personal> repoPerson;
        public User CurrentUser { get; set; }
        public List<Personal> PersonalList { get; set; }
        public Personal SelectedPerson { get; set; }

        private readonly IRepository<WorkTabel> repoTabel;
        public WorkTabel CurrentTabel { get; set; }

        private readonly IRepository<TabelPerson> repoTabelPerson;
        public List<TabelPerson> ListTabelPerson { get; set; }

        public List<MonthStart1> ListMonth { get; set; }


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
            }


        }


        //----------------------------------------------------------------------------------------------------------
        // загрузка дней для сотрудника
        //----------------------------------------------------------------------------------------------------------
        private void LoadTabelPerson()
        {
            ListTabelPerson = repoTabelPerson.Items
                .Where(it => it.person.id == SelectedPerson.id && it.tabel.id == CurrentTabel.id)
                .ToList();
        }
    }
}
