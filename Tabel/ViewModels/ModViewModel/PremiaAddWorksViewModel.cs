using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class PremiaAddWorksViewModel : ModViewModel
    {
        private readonly RepositoryMSSQL<AddWorks> repoAddWorks;
        public ICollection<ModPerson> ListModPerson { get; set; }

        public List<AddWorks> ListWorks { get; set; }

        private ModPerson _SelectedPerson;
        public ModPerson SelectedPerson
        {
            get => _SelectedPerson;
            set
            {
                if (_SelectedPerson == value) return;

                GetWorksFromPerson(_SelectedPerson, ListWorks);
                _SelectedPerson = value;
                SetWorksToPerson(_SelectedPerson, ListWorks);

            }
        }

        private bool _IsCheckedButton;
        public bool IsCheckedButton
        {
            get => _IsCheckedButton;
            set
            {
                _IsCheckedButton = value;
                if (!value)
                {
                    GetWorksFromPerson(_SelectedPerson, ListWorks);
                }
            }
        }


        public bool IsNotAdmin => App.CurrentUser.u_role != UserRoles.Admin && App.CurrentUser.id != 37;
        //        {
        //            get
        //            {
        //                return App.CurrentUser.u_role != UserRoles.Admin;
        //            }
        //}

        public Visibility IsVisible { get; set; } = App.CurrentUser.u_role == UserRoles.Admin ? Visibility.Visible : Visibility.Collapsed;

public PremiaAddWorksViewModel(BaseModel db) : base(db)
        {
            repoAddWorks = new RepositoryMSSQL<AddWorks>(db);
            ListWorks = repoAddWorks.Items.ToList();
        }


        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            //_SelectedOtdel = Otdel;
            _SelectMonth = Month;
            _SelectYear = Year;
            //ListModPerson = listPerson?.Where(it => it.person.p_type_id == SpecType.Рабочий).ToList(); ;
            ListModPerson = listPerson;//?.ToList(); 

            OnPropertyChanged(nameof(ListModPerson));
        }


        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            OnPropertyChanged(nameof(ListModPerson));
        }

        //--------------------------------------------------------------------------------
        // Отметка в списке работ для сотрудника
        //--------------------------------------------------------------------------------
        private void SetWorksToPerson(ModPerson person, ICollection<AddWorks> ListWorks)
        {
            if (person is null || ListWorks is null) return;

            foreach (AddWorks work in ListWorks)
            {
                work.IsChecked = person.ListAddWorks.Any(it => it.id == work.id);
                work.OnPropertyChanged(nameof(work.IsChecked));
            }

        }

        //--------------------------------------------------------------------------------
        // Получение работ для сотрудника
        //--------------------------------------------------------------------------------
        private void GetWorksFromPerson(ModPerson person, ICollection<AddWorks> ListWorks)
        {
            if (person is null || ListWorks is null) return;

            foreach (AddWorks work in ListWorks)
            {
                if (work.IsChecked)
                    person.ListAddWorks.Add(work);
                else
                    person.ListAddWorks.Remove(work);
            }

            person.OnPropertyChanged(nameof(person.ListAddWorks));

        }

    }
}
