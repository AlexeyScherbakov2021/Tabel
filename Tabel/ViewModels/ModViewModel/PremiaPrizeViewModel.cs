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
    internal class PremiaPrizeViewModel : ModViewModel
    {
        public ObservableCollection<ModPerson> ListModPerson { get; set; }


        public PremiaPrizeViewModel(BaseModel db) : base(db)
        {

        }

        public override void ChangeListPerson(ObservableCollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {

            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = Otdel;
            ListModPerson = listPerson;

            if (ListModPerson != null)
            {
                foreach (var item in ListModPerson)
                    item.premiaPrize.Calculation();
            }

            OnPropertyChanged(nameof(ListModPerson));
        }
    }
}
