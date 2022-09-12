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
using Tabel.Models;
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
        private readonly RepositoryMSSQL<SmenaPersonal> repoSmenaPersonal = new RepositoryMSSQL<SmenaPersonal>();

        public string[] ListKind { get; set; } = { "1см","2см","В","О" };
        public object SelectedDays { get; set; }

        // Текщий график смен
        public Smena SmenaShedule { get; set; }

        public User User { get; set; }
        //public ObservableCollection<Personal> ListPersonal { get; set; }

        public ObservableCollection<SmenaPersonal> ListPersonal { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }


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
        // Команда Выбрать тип дня
        //--------------------------------------------------------------------------------
        public ICommand SelectKindCommand => new LambdaCommand(OnSelectKindCommandExecuted, CanSelectKindCommand);
        private bool CanSelectKindCommand(object p) => true;
        private void OnSelectKindCommandExecuted(object p)
        {
        }

        //--------------------------------------------------------------------------------
        // Команда Выбрать тип дня
        //--------------------------------------------------------------------------------
        public ICommand SelectCommand => new LambdaCommand(OnSelectCommandExecuted, CanSelectCommand);
        private bool CanSelectCommand(object p) => true;
        private void OnSelectCommandExecuted(object p)
        {

            repoSmena.Save();
        }
        #endregion

        //--------------------------------------------------------------------------------------------------
        // коструктор
        //--------------------------------------------------------------------------------------------------
        public SmenaWindowViewModel( )
        {
            CurrentMonth = 1;
            CurrentYear = 2022;
            User = new User() { u_otdel_id=44, u_login="Ivanov", id = 7 };
            ListOtdel =  repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();
        }


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдеоа
        //--------------------------------------------------------------------------------------------------
        private void LoadPersonForOtdel(Otdel otdel)
        {
            SmenaShedule = repoSmena.Items.FirstOrDefault(it => it.sm_Year == 2022 && it.sm_Month == 1 && it.sm_UserId == User.u_otdel_id);
            List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();

            if (SmenaShedule != null)
            {
                foreach (var item in PersonsFromOtdel)
                {
                    SmenaPersonal pers = repoSmenaPersonal.Items.FirstOrDefault(it => it.sp_SmenaId == SmenaShedule.id && it.sp_PersonId == item.id);
                    if(pers is null)
                    {
                        // формируем новый график на месяц для сотрудника
                    }

                }

            }
            else
            {
                // записей нет
                SmenaShedule = new Smena();
                //SmenaShedule.UserCreater = User;
                SmenaShedule.sm_UserId = User.id;
                SmenaShedule.sm_OtdelId = SelectedOtdel.id;
                SmenaShedule.sm_Month = CurrentMonth;
                SmenaShedule.sm_Year = CurrentYear;
                SmenaShedule.sm_DateCreate = DateTime.Now;
                SmenaShedule.ListSmenaPerson = new List<SmenaPersonal>();

                foreach(var item in PersonsFromOtdel)
                {

                    SmenaPersonal sp = new SmenaPersonal();
                    sp.sp_PersonId = item.id;
                    //sp.Person = item;
                    sp.ListSmenaDays = new List<SmenaDay>();
                    for (int i = 1; i < 31; i++)
                    {
                        SmenaDay sd = new SmenaDay();
                        sd.sd_Day = i;
                        sd.sd_Kind = Infrastructure.SmenaKind.First;
                        sp.ListSmenaDays.Add(sd);
                    }
                    SmenaShedule.ListSmenaPerson.Add(sp);
                }
                repoSmena.Add(SmenaShedule, true);
            }




            ListPersonal = new ObservableCollection<SmenaPersonal>(SmenaShedule.ListSmenaPerson);
            OnPropertyChanged(nameof(ListPersonal));
        }

    }
}
