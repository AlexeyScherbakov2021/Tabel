using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public WorkTabel Tabel { get; set; }
        public IEnumerable<typeDay> ListTypeDays { get; set; }

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

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
            IEnumerable<WorkCalendar> cal = repo.Items.AsNoTracking().Where(it => it.cal_year == _SelectYear
                    && it.cal_date.Month == _SelectMonth);


            repoPersonal = new RepositoryMSSQL<Personal>();
            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == _SelectedOtdel.id).ToList();

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

            OnPropertyChanged(nameof(Tabel));

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

            if (otdel is null)
                return;

            Tabel = repoTabel.Items.FirstOrDefault(it => it.t_year == Year
                && it.t_month == Month
                && it.t_otdel_id == otdel.id);

            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == otdel.id).ToList();

            if (Tabel != null)
            {
                foreach (var item in Tabel.tabelPersons)
                {
                    item.SetCalendarTypeDays();
                }
            }

            OnPropertyChanged(nameof(Tabel));
        }
    }
}
