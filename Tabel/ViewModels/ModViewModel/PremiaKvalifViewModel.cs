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
    internal class PremiaKvalifViewModel : ModViewModel
    {
        public ObservableCollection<ModPerson> ListModPerson { get; set; }

        public PremiaKvalifViewModel(BaseModel db) : base(db)
        {

        }

        public override void ChangeListPerson(ObservableCollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            ListModPerson = listPerson;


            OnPropertyChanged(nameof(ListModPerson));
        }

    }
}
