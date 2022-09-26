using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models2;
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

        public string[] ListKind { get; set; } = { "", "1см", "2см", "В", "О" };

        // Текщий график смен
        public Smena SmenaShedule { get; set; }

        private DateTime _CurrentDate;


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => true;
        private void OnCreateCommandExecuted(object p)
        {
            if (SmenaShedule != null)
            {
                repoSmena.Remove(SmenaShedule);
            }

            List<Personal> PersonsFromOtdel = repoPersonal.Items
                .Where(it => it.p_otdel_id == _SelectedOtdel.id)
                .OrderBy(o => o.FIO)
                .ToList();

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

            OnPropertyChanged(nameof(SmenaShedule));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoSmena.Save();
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

            if (SelectOtdel is null) return;

            SmenaShedule = repoSmena.Items
                .Where(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.id)
                .Include(inc => inc.SmenaPerson)
                .FirstOrDefault();
            List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == _SelectedOtdel.id).ToList();

            //if (SmenaShedule != null)
            //{
            //    // если график присутствует
            //    foreach (var item in PersonsFromOtdel)
            //    {
            //        SmenaPerson pers = repoSmenaPersonal.Items.FirstOrDefault(it => it.sp_SmenaId == SmenaShedule.id && it.sp_PersonId == item.id);
            //        if (pers is null)
            //        {
            //            // формируем новый график на месяц для сотрудника, который отсутствовал
            //        }

            //    }

            //}

            OnPropertyChanged(nameof(SmenaShedule));


        }
    }
}
