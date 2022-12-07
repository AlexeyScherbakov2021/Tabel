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
    internal class PremiaOtdelViewModel : ModViewModel
    {
        public ICollection<ModPerson> ListModPerson { get; set; }

        public PremiaOtdelViewModel(BaseModel db) : base(db)
        {
                
        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            ListModPerson = listPerson;



            OnPropertyChanged(nameof(ListModPerson));
        }


        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            //foreach (var item in listPerson)
            //    ListModPerson.Add(item);

            OnPropertyChanged(nameof(ListModPerson));

        }
    }
}
