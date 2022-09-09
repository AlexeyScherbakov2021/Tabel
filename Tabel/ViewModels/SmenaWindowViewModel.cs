using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public string[] ListKind { get; set; } = { "1см","2см","В","О" };

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


        //--------------------------------------------------------------------------------------------------
        // коструктор
        //--------------------------------------------------------------------------------------------------
        public SmenaWindowViewModel( )
        {
            CurrentMonth = 1;
            CurrentYear = 2022;
            User = new User() { u_otdel_id=44, u_login="Ivanov" };
            ListOtdel =  repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();
        }


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдеоа
        //--------------------------------------------------------------------------------------------------
        private void LoadPersonForOtdel(Otdel otdel)
        {
            Smena smena = repoSmena.Items.FirstOrDefault(it => it.sm_Year == 2022 && it.sm_Month == 1 && it.sm_UserId == User.u_otdel_id);

            if(smena is null)
            {
                // записей нет
                smena = new Smena();
                smena.UserCreater = User;
                smena.sm_Month = CurrentMonth;
                smena.sm_Year = CurrentYear;
                smena.ListSmenaPerson = new List<SmenaPersonal>();

                List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();
                foreach(var item in PersonsFromOtdel)
                {
                    SmenaPersonal sp = new SmenaPersonal();
                    sp.Person = item;
                    sp.ListSmenaDays = new List<SmenaDay>();
                    for (int i = 1; i < 31; i++)
                    {
                        SmenaDay sd = new SmenaDay();
                        sd.sd_Day = i;
                        sd.sd_Kind = Infrastructure.SmenaKind.First;
                        sp.ListSmenaDays.Add(sd);
                    }
                    smena.ListSmenaPerson.Add(sp);
                }
            }

            ListPersonal = new ObservableCollection<SmenaPersonal>( smena.ListSmenaPerson);
            OnPropertyChanged(nameof(ListPersonal));
        }

    }
}
