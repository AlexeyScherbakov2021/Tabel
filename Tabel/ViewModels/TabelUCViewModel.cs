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
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views.PrintForm;

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

        public Otdel SelectedOtdel { get; set; }
        private int _SelectMonth;
        private int _SelectYear;

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => SelectedOtdel != null && SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {

            if (Tabel != null)
            {
                if (MessageBox.Show("Текущий табель будет удален. Подтверждаете?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                repoTabel.Remove(Tabel);
            }

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            //IEnumerable<WorkCalendar> cal = repo.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear
            //        && it.cal_date.Month == _SelectMonth);


            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == SelectedOtdel.id).Select(s => s.id).ToList();

            // получение сотрудников отдела
            repoPersonal = new RepositoryMSSQL<Personal>();

            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => (it.p_otdel_id == SelectedOtdel.id && it.p_delete == false)
                || listOtdels.Contains(it.p_otdel_id.Value)).ToList();

            Tabel = new WorkTabel();
            Tabel.t_author_id = App.CurrentUser.id;
            Tabel.t_otdel_id = SelectedOtdel.id;
            Tabel.t_month = _SelectMonth;
            Tabel.t_year = _SelectYear;
            Tabel.t_date_create = DateTime.Now;
            Tabel.tabelPersons = new ObservableCollection<TabelPerson>();

            //DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TabelPerson tp = new TabelPerson();
                tp.tp_person_id = item.id;
                //tp.person = item;
                tp.TabelDays = new ObservableCollection<TabelDay>();

                foreach(var listItem in ListDays)
                {
                    TabelDay td = new TabelDay();
                    td.td_Day = listItem.Day;
                    td.CalendarTypeDay = listItem.KindDay;
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

                    tp.TabelDays.Add(td);

                }


            //    for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
            //    {
            //        WorkCalendar ChangeDay = cal.FirstOrDefault(it => it.cal_date == IndexDate);

            //        TabelDay td = new TabelDay();
            //        td.td_Day = IndexDate.Day;

            //        if (ChangeDay != null)
            //        {
            //            td.CalendarTypeDay = ChangeDay.cal_type;
            //            switch (td.CalendarTypeDay)
            //            {
            //                case TypeDays.Holyday:
            //                    td.td_KindId = 2;
            //                    td.td_Hours = 0;
            //                    break;
            //                case TypeDays.Work:
            //                    td.td_KindId = 1;
            //                    td.td_Hours = 8;
            //                    break;
            //                case TypeDays.ShortWork:
            //                    td.td_KindId = 1;
            //                    td.td_Hours = 7;
            //                    break;
            //            }
            //        }
            //        else
            //        {
            //            if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
            //            {
            //                td.td_KindId = 2;
            //                td.CalendarTypeDay = TypeDays.Holyday;
            //                td.td_Hours = 0;
            //            }
            //            else
            //            {
            //                td.td_KindId = 1;
            //                td.CalendarTypeDay = TypeDays.Work;
            //                td.td_Hours = 8;
            //            }
            //        }
            //        tp.TabelDays.Add(td);
            //    }

            //    //tp.SetCalendarTypeDays();
                Tabel.tabelPersons.Add(tp);
            }

            if (Tabel.tabelPersons.Count > 0)
                repoTabel.Add(Tabel, true);

            Tabel = repoTabel.Items
                .Where(it => it.id == Tabel.id)
                .Include(inc => inc.tabelPersons.Select(s => s.person))
                .FirstOrDefault();

            ListTabelPerson = new ObservableCollection<TabelPerson>(Tabel.tabelPersons);

            OnPropertyChanged(nameof(ListTabelPerson));
            OnPropertyChanged(nameof(Tabel));

        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintCommand);
        private bool CanPrintCommand(object p) => Tabel != null && SelectedOtdel != null;
        private void OnPrintCommandExecuted(object p)
        {
            TabelPrint print = new TabelPrint();
            print.DataContext = this;
            print.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => SelectedOtdel != null && Tabel != null;
        private void OnSaveCommandExecuted(object p)
        {
            repoTabelPerson.Save();
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
            SelectedOtdel = otdel;
            ListTabelPerson = null;

            if (otdel is null)  return;

            if (SelectedOtdel.ot_parent is null)
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

            SetTypeDays();
            //if (ListTabelPerson != null)
            //{
            //    foreach (var item in ListTabelPerson)
            //        item.SetCalendarTypeDays();
            //}

            OnPropertyChanged(nameof(ListTabelPerson));
            OnPropertyChanged(nameof(Tabel));
        }

        //--------------------------------------------------------------------------------------
        // расстановка топв дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (Tabel is null) return;

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            foreach (var item in ListTabelPerson)
            {
                // расставляем выходные по каледнарю
                int i = 0;
                foreach (var day in item.TabelDays)
                {
                    day.CalendarTypeDay = ListDays[i].KindDay;
                    i++;
                }

            }


        }


    }
}
