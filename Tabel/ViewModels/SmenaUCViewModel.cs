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
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class SmenaUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private readonly RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Smena> repoSmena = new RepositoryMSSQL<Smena>();
        private readonly RepositoryMSSQL<SmenaPerson> repoSmenaPersonal = new RepositoryMSSQL<SmenaPerson>();

        public string[] ListKind { get; set; } = { "1см", "2см", "В", "О" };

        // Текщий график смен
        public Smena SmenaShedule { get; set; }
        public ObservableCollection<SmenaPerson> ListSmenaPerson { get; set; }

        //private DateTime _CurrentDate;

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (SmenaShedule != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.OK)
                    return;
                repoSmena.Remove(SmenaShedule);
            }

            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => (it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                     || listOtdels.Contains(it.p_otdel_id.Value)).ToList();

            SmenaShedule = new Smena();
            SmenaShedule.sm_UserId = App.CurrentUser.id;
            SmenaShedule.sm_OtdelId = _SelectedOtdel.id;
            SmenaShedule.sm_Month = _SelectMonth;
            SmenaShedule.sm_Year = _SelectYear;
            SmenaShedule.sm_DateCreate = DateTime.Now;
            SmenaShedule.SmenaPerson = new ObservableCollection<SmenaPerson>();

            // количество дней в месяце
            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                SmenaPerson sp = new SmenaPerson();
                sp.sp_PersonId = item.id;
                sp.SmenaDays = new List<SmenaDay>();

                for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
                {
                    SmenaDay sd = new SmenaDay();
                    sd.sd_Day = IndexDate.Day;
                    if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                        sd.sd_Kind = SmenaKind.DayOff;
                    else
                        sd.sd_Kind = SmenaKind.First;
                    sp.SmenaDays.Add(sd);
                }

                SmenaShedule.SmenaPerson.Add(sp);
            }

            if (SmenaShedule.SmenaPerson.Count > 0)
                repoSmena.Add(SmenaShedule, true);

            SmenaShedule = repoSmena.Items
                .Where(it => it.id == SmenaShedule.id)
                .Include(inc => inc.SmenaPerson.Select(s => s.personal))
                .FirstOrDefault();

            ListSmenaPerson = new ObservableCollection<SmenaPerson>(SmenaShedule.SmenaPerson);

            OnPropertyChanged(nameof(ListSmenaPerson));
            OnPropertyChanged(nameof(SmenaShedule));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => _SelectedOtdel != null && SmenaShedule != null ;
        private void OnSaveCommandExecuted(object p)
        {
            repoSmenaPersonal.Save();
            repoSmena.Save();
        }

        //--------------------------------------------------------------------------------
        // 
        //--------------------------------------------------------------------------------
        public ICommand SelectKindCommand => new LambdaCommand(OnSelectKindCommandExecuted, CanSelectKindCommand);
        private bool CanSelectKindCommand(object p) => SmenaShedule != null;
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
            ListSmenaPerson = null;


            if (SelectOtdel is null) return;


            if (_SelectedOtdel.ot_parent is null)
            {
                SmenaShedule = repoSmena.Items
                .Where(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.id)
                .Include(inc => inc.SmenaPerson)
                .FirstOrDefault();
                if (SmenaShedule != null)
                    ListSmenaPerson = new ObservableCollection<SmenaPerson>(repoSmenaPersonal.Items
                        .Where(it => it.sp_SmenaId == SmenaShedule.id)
                        .OrderBy(o => o.personal.p_lastname)
                        .ThenBy(o => o.personal.p_name)
                        );
            }
            else
            {
                SmenaShedule = repoSmena.Items
                .Where(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.ot_parent)
                .Include(inc => inc.SmenaPerson)
                .FirstOrDefault();
                if (SmenaShedule != null)
                    ListSmenaPerson = new ObservableCollection<SmenaPerson>(repoSmenaPersonal.Items
                        .Where(it => it.sp_SmenaId == SmenaShedule.id && it.personal.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.personal.p_lastname)
                        .ThenBy(o => o.personal.p_name)
                        );
            }

            SetTypeDays();

            OnPropertyChanged(nameof(ListSmenaPerson));
            OnPropertyChanged(nameof(SmenaShedule));

        }

        //--------------------------------------------------------------------------------------
        // расстановка топв дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (SmenaShedule is null) return;

            RepositoryMSSQL<WorkCalendar> repoDays = new RepositoryMSSQL<WorkCalendar>();
            // количество дней в месяце
            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            List<WorkCalendar> cal = repoDays.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear && it.cal_date.Month == _SelectMonth).ToList();

            foreach (var item in ListSmenaPerson)
            {
                // расставляем выходные по каледнарю
                foreach (var day in item.SmenaDays)
                {
                    DayOfWeek week = new DateTime(_SelectYear, _SelectMonth, day.sd_Day).DayOfWeek;
                    if (week == DayOfWeek.Sunday || week == DayOfWeek.Saturday)
                    {
                        day.OffDay = true;
                    }
                }

                // дополняем измененные дни
                foreach (var day in cal)
                {
                    SmenaDay sd = item.SmenaDays.FirstOrDefault(it => it.sd_Day == day.cal_date.Day);
                    sd.OffDay = day.cal_type == TypeDays.Holyday;
                }

            }

        }

    }
}
