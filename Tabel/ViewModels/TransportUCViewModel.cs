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
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class TransportUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private readonly RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Transport> repoTransp = new RepositoryMSSQL<Transport>();
        private readonly RepositoryMSSQL<TransPerson> repoTrnaspPersonal = new RepositoryMSSQL<TransPerson>();
        private readonly RepositoryMSSQL<TransPerson> repoTransPerson = new RepositoryMSSQL<TransPerson>();

        public Transport Transp { get; set; }
        public ObservableCollection<TransPerson> ListTransPerson { get; set; }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать 
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (Transp != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.OK)
                    return;
                repoTransp.Remove(Transp);
            }

            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == _SelectedOtdel.id
                || listOtdels.Contains(it.p_otdel_id.Value)).ToList();
    

            Transp = new Transport();
            Transp.tr_UserId = App.CurrentUser.id;
            Transp.tr_OtdelId = _SelectedOtdel.id;
            Transp.tr_Month = _SelectMonth;
            Transp.tr_Year = _SelectYear;
            Transp.tr_DateCreate = DateTime.Now;
            Transp.TransportPerson = new ObservableCollection<TransPerson>();

            // количество дней в месяце
            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TransPerson tp = new TransPerson();
                tp.tp_PersonId = item.id;
                //tp.person = item;
                tp.TransDays = new List<TransDay>();

                for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
                {
                    TransDay td = new TransDay();
                    td.td_Day = IndexDate.Day;
                    //if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                    //    td.OffDay = true;
                    tp.TransDays.Add(td);
                }

                Transp.TransportPerson.Add(tp);
            }

            SetTypeDays();

            if (Transp.TransportPerson.Count > 0)
                repoTransp.Add(Transp, true);

            Transp = repoTransp.Items.Where(it => it.id == Transp.id)
                .Include(i => i.TransportPerson.Select(s => s.person))
                .FirstOrDefault();

            ListTransPerson = new ObservableCollection<TransPerson>(Transp.TransportPerson);

            OnPropertyChanged(nameof(ListTransPerson));
            OnPropertyChanged(nameof(Transp));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => _SelectedOtdel != null && Transp != null;
        private void OnSaveCommandExecuted(object p)
        {
            repoTransPerson.Save();
        }

        //--------------------------------------------------------------------------------
        // 
        //--------------------------------------------------------------------------------
        public ICommand SelectKindCommand => new LambdaCommand(OnSelectKindCommandExecuted, CanSelectKindCommand);
        private bool CanSelectKindCommand(object p) => true;
        private void OnSelectKindCommandExecuted(object p)
        {

        }
        #endregion

        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------

        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;
            ListTransPerson = null;

            if (SelectOtdel is null) return;


            if (_SelectedOtdel.ot_parent is null)
            {
                Transp = repoTransp.Items.FirstOrDefault(it => it.tr_Year == Year
                    && it.tr_Month == Month
                    && it.tr_OtdelId == _SelectedOtdel.id);
                if (Transp != null)
                    ListTransPerson = new ObservableCollection<TransPerson>(repoTransPerson.Items
                        .Where(it => it.tp_TranspId == Transp.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else
            {
                Transp = repoTransp.Items.FirstOrDefault(it => it.tr_Year == Year
                    && it.tr_Month == Month
                    && it.tr_OtdelId == _SelectedOtdel.ot_parent);
                if (Transp != null)
                    ListTransPerson = new ObservableCollection<TransPerson>(repoTransPerson.Items
                        .Where(it => it.tp_TranspId == Transp.id && it.person.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }


            //Transp = repoTransp.Items
            //    .Where(it => it.tr_Year == _SelectYear
            //    && it.tr_Month == _SelectMonth
            //    && it.tr_OtdelId == _SelectedOtdel.id)
            //    .Include(inc => inc.TransportPerson)
            //    .FirstOrDefault();
            
            //List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == _SelectedOtdel.id).ToList();

            SetTypeDays();

            OnPropertyChanged(nameof(ListTransPerson));
            OnPropertyChanged(nameof(Transp));

        }

        //--------------------------------------------------------------------------------------
        // расстановка топв дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (Transp is null) return;

            RepositoryMSSQL<WorkCalendar> repoDays = new RepositoryMSSQL<WorkCalendar>();
            // количество дней в месяце
            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            List<WorkCalendar> cal = repoDays.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear && it.cal_date.Month == _SelectMonth).ToList();

            foreach (var item in ListTransPerson)
            {
                // расставляем выходные по каледнарю
                foreach ( var day in item.TransDays)
                {
                    DayOfWeek week = new DateTime(_SelectYear, _SelectMonth, day.td_Day).DayOfWeek;
                    if (week == DayOfWeek.Sunday || week == DayOfWeek.Saturday)
                    {
                        day.OffDay = true;
                    }
                }

                // дополняем измененные дни
                foreach(var day in cal)
                {
                    TransDay td = item.TransDays.FirstOrDefault(it => it.td_Day == day.cal_date.Day);
                    td.OffDay = day.cal_type == TypeDays.Holyday;
                }

            }

        }


    }
}
