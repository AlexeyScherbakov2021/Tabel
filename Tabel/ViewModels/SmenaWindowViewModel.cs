using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class SmenaWindowViewModel : ViewModel
    {
        //private readonly RepositoryMSSQL<User> repoUser = new RepositoryMSSQL<User>();
        private readonly RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
        private readonly RepositoryMSSQL<Smena> repoSmena = new RepositoryMSSQL<Smena>();
        private readonly RepositoryMSSQL<SmenaPerson> repoSmenaPersonal = new RepositoryMSSQL<SmenaPerson>();

        public string[] ListKind { get; set; } = { "1см","2см","В","О" };
        //public object SelectedDays { get; set; }

        // Текщий график смен
        public Smena SmenaShedule { get; set; }

        public User User { get; set; }

        public ObservableCollection<SmenaPerson> ListPersonal { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        private DateTime _CurrentDate;


        public List<Otdel> ListOtdel { get; set; }

        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel 
        { 
            get => _SelectedOtdel;
            set 
            { 
                if(Set(ref _SelectedOtdel, value))
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
            if(SmenaShedule != null)
            {
                repoSmena.Remove(SmenaShedule);
            }

            List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();

            SmenaShedule = new Smena();
            SmenaShedule.sm_UserId = User.id;
            SmenaShedule.sm_OtdelId = SelectedOtdel.id;
            SmenaShedule.sm_Month = CurrentMonth;
            SmenaShedule.sm_Year = CurrentYear;
            SmenaShedule.sm_DateCreate = DateTime.Now;
            SmenaShedule.SmenaPerson = new List<SmenaPerson>();

            // количество дней в месяце
            //int DaysInMonth = new DateTime(CurrentYear, CurrentMonth, 1).AddMonths(1).AddDays(-1).Day;
            DateTime StartDay = new DateTime(CurrentYear, CurrentMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                SmenaPerson sp = new SmenaPerson();
                sp.sp_PersonId = item.id;
                //sp.personal = item;
                sp.SmenaDays = new List<SmenaDay>();

                for (DateTime IndexDate = StartDay; IndexDate.Month == CurrentMonth; IndexDate = IndexDate.AddDays(1))
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

            //SmenaShedule.SmenaPerson = repoSmenaPersonal.Items.Where(it => it.sp_SmenaId == SmenaShedule.id).ToList();
            ListPersonal = new ObservableCollection<SmenaPerson>(SmenaShedule.SmenaPerson);
            OnPropertyChanged(nameof(ListPersonal));

            //repoSmena.Delete(SmenaShedule, true);

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
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SelectKindCommand => new LambdaCommand(OnSelectKindCommandExecuted, CanSelectKindCommand);
        private bool CanSelectKindCommand(object p) => true;
        private void OnSelectKindCommandExecuted(object p)
        {

        }
        #endregion

        //--------------------------------------------------------------------------------------------------
        // коструктор
        //--------------------------------------------------------------------------------------------------
        public SmenaWindowViewModel( )
        {
            _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;
            User = App.CurrentUser;
            User = new User() { u_otdel_id=44, u_login="Petrov", id = 10, u_fio="Петров" };
            ListOtdel =  repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();
        }


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------
        private void LoadPersonForOtdel(Otdel otdel)
        {
            SmenaShedule = repoSmena.Items.FirstOrDefault(it => it.sm_Year == CurrentYear && it.sm_Month == CurrentMonth && it.sm_OtdelId == otdel.id);
            //SmenaShedule = repoSmena.Items.FirstOrDefault(it => it.sm_Year == CurrentYear && it.sm_Month == CurrentMonth && it.sm_OtdelId == otdel.id);
            List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();
            ListPersonal = null;

            if (SmenaShedule != null)
            {
                // если график присутствует
                foreach (var item in PersonsFromOtdel)
                {
                    SmenaPerson pers = repoSmenaPersonal.Items.FirstOrDefault(it => it.sp_SmenaId == SmenaShedule.id && it.sp_PersonId == item.id);
                    if(pers is null)
                    {
                        // формируем новый график на месяц для сотрудника, который отсутствовал
                    }

                }

                ListPersonal = new ObservableCollection<SmenaPerson>(SmenaShedule.SmenaPerson);
                //ListPersonal = new ObservableCollection<SmenaPerson>(repoSmenaPersonal.Items.Where(it => it.sp_SmenaId == SmenaShedule.id));

            }

            OnPropertyChanged(nameof(ListPersonal));

            //else
            //{
            //    // записей нет
            //    SmenaShedule = new Smena();
            //    SmenaShedule.sm_UserId = User.id;
            //    SmenaShedule.sm_OtdelId = SelectedOtdel.id;
            //    SmenaShedule.sm_Month = CurrentMonth;
            //    SmenaShedule.sm_Year = CurrentYear;
            //    SmenaShedule.sm_DateCreate = DateTime.Now;
            //    SmenaShedule.SmenaPerson = new List<SmenaPerson>();

            //    // количество дней в месяце
            //    //int DaysInMonth = new DateTime(CurrentYear, CurrentMonth, 1).AddMonths(1).AddDays(-1).Day;
            //    DateTime StartDay = new DateTime(CurrentYear, CurrentMonth, 1);

            //    // если есть персонал в отделе, добавляем его и формируем дни
            //    foreach (var item in PersonsFromOtdel)
            //    {
            //        SmenaPerson sp = new SmenaPerson();
            //        sp.sp_PersonId = item.id;
            //        sp.SmenaDays = new List<SmenaDay>();


            //        for(DateTime IndexDate = StartDay; IndexDate.Month == CurrentMonth; IndexDate = IndexDate.AddDays(1))
            //        {
            //            SmenaDay sd = new SmenaDay();
            //            sd.sd_Day = IndexDate.Day;
            //            if(IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
            //                sd.sd_Kind = SmenaKind.DayOff;
            //            else
            //                sd.sd_Kind = SmenaKind.First;
            //            sp.SmenaDays.Add(sd);
            //        }

            //        //for (int i = 1; i <= DaysInMonth; i++)
            //        //{
            //        //    SmenaDay sd = new SmenaDay();
            //        //    sd.sd_Day = i;

            //        //    sd.sd_Kind = SmenaKind.First;
            //        //    sp.SmenaDays.Add(sd);
            //        //}
            //        SmenaShedule.SmenaPerson.Add(sp);
            //    }
            //    if(SmenaShedule.SmenaPerson.Count > 0)
            //        repoSmena.Add(SmenaShedule, true);
            //}

        }

    }
}
