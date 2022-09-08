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

        public User User { get; set; }
        //public ObservableCollection<Personal> ListPersonal { get; set; }

        public ObservableCollection<Smena> ListPersonal { get; set; }

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
            User = new User() { u_otdel_id=44, u_login="Ivanov" };
            ListOtdel =  repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();
        }


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдеоа
        //--------------------------------------------------------------------------------------------------
        private void LoadPersonForOtdel(Otdel otdel)
        {
            ListPersonal = new ObservableCollection<Smena>( repoSmena.Items.Where(it => it.sm_OtdelId == otdel.id));
            //ListPersonal = new ObservableCollection<Personal>( otdel.personals );
        }

    }
}
