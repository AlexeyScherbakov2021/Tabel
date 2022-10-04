using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class TabelUCViewModel : ViewModel, IBaseUCViewModel
    {
        private RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>();
        private readonly RepositoryMSSQL<typeDay> repoTypeDay = new RepositoryMSSQL<typeDay>();
        private readonly RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>();

        public WorkTabel Tabel { get; set; }
        public IEnumerable<typeDay> ListTypeDays { get; set; }
        public ObservableCollection<TabelPerson> ListTabelPerson { get; set; }

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {

            if (Tabel != null)
            {
                if (MessageBox.Show("Текущий табель будет удален. Подтверждаете?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.OK)
                    return;

                repoTabel.Remove(Tabel);
            }

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar();
            IEnumerable<WorkCalendar> cal = repo.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear
                    && it.cal_date.Month == _SelectMonth);


            // получение сотрудников отдела
            repoPersonal = new RepositoryMSSQL<Personal>();

            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();


            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == _SelectedOtdel.id
                || listOtdels.Contains(it.p_otdel_id.Value)).ToList();

            Tabel = new WorkTabel();
            Tabel.t_author_id = App.CurrentUser.id;
            Tabel.t_otdel_id = _SelectedOtdel.id;
            Tabel.t_month = _SelectMonth;
            Tabel.t_year = _SelectYear;
            Tabel.t_date_create = DateTime.Now;
            Tabel.tabelPersons = new ObservableCollection<TabelPerson>();

            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TabelPerson tp = new TabelPerson();
                tp.tp_person_id = item.id;
                //tp.person = item;
                tp.TabelDays = new ObservableCollection<TabelDay>();

                for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
                {
                    WorkCalendar ChangeDay = cal.FirstOrDefault(it => it.cal_date == IndexDate);

                    TabelDay td = new TabelDay();
                    td.td_Day = IndexDate.Day;

                    if (ChangeDay != null)
                    {
                        td.CalendarTypeDay = ChangeDay.cal_type;
                        switch (td.CalendarTypeDay)
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


            Tabel = repoTabel.Items
                .Where(it => it.id == Tabel.id)
                .Include(inc => inc.tabelPersons.Select(s => s.person))
                .FirstOrDefault();

            OnPropertyChanged(nameof(Tabel));

        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        public ICommand LoadDefCommand => new LambdaCommand(OnLoadDefCommandExecuted, CanLoadDefCommand);
        private bool CanLoadDefCommand(object p) => Tabel != null && _SelectedOtdel != null;
        private void OnLoadDefCommandExecuted(object p)
        {

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => _SelectedOtdel != null;
        private void OnSaveCommandExecuted(object p)
        {
            //repoTabel.Save();
            repoTabelPerson.Save();
        }

        #endregion



        //--------------------------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------------------------
        public TabelUCViewModel()
        {
            ListTypeDays = repoTypeDay.Items.ToList();
            OnPropertyChanged(nameof(ListTypeDays));
        }

        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel otdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = otdel;
            ListTabelPerson = null;

            if (otdel is null)
                return;

            if (_SelectedOtdel.ot_parent is null)
            {
                Tabel = repoTabel.Items.FirstOrDefault(it => it.t_year == Year
                    && it.t_month == Month
                    && it.t_otdel_id == otdel.id);
                if(Tabel != null)
                    ListTabelPerson = new ObservableCollection<TabelPerson>(repoTabelPerson.Items
                        .Where(it => it.tp_tabel_id == Tabel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else 
            {
                Tabel = repoTabel.Items.FirstOrDefault(it => it.t_year == Year
                    && it.t_month == Month
                    && it.t_otdel_id == otdel.ot_parent);
                if (Tabel != null)
                    ListTabelPerson = new ObservableCollection<TabelPerson> (repoTabelPerson.Items
                        .Where(it => it.tp_tabel_id == Tabel.id && it.person.p_otdel_id == otdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }


            if (ListTabelPerson != null)
            {
                foreach (var item in ListTabelPerson)
                    item.SetCalendarTypeDays();
            }

            OnPropertyChanged(nameof(ListTabelPerson));
            OnPropertyChanged(nameof(Tabel));
        }

    }
}
