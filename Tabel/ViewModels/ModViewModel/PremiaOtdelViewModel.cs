using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class PremiaOtdelViewModel : ViewModel, IModViewModel
    {
        private int _SelectMonth;
        private int _SelectYear;
        public ObservableCollection<ModPerson> ListModPerson { get; set; }

        public void ChangeListPerson(ObservableCollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            //_SelectedOtdel = Otdel;
            _SelectMonth = Month;
            _SelectYear = Year;
            ListModPerson = listPerson;


            OnPropertyChanged(nameof(ListModPerson));
        }

    }
}
