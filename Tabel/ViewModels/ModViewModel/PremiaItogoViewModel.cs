using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    public class PremiaItogoViewModel : ModViewModel
    {
        public ICollection<ModPerson> ListModPerson { get; set; }
        
        public Visibility IsVisibleITR { get; set; }


        public PremiaItogoViewModel(BaseModel db) : base(db)
        {

        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {

            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = Otdel;
            ListModPerson = listPerson;

            //if (ListModPerson != null)
            //{
            //    foreach (var item in ListModPerson)
            //        item.premiaPrize.Calculation();
            //}

            IsVisibleITR = Otdel.ot_itr == 2 ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(IsVisibleITR));

            OnPropertyChanged(nameof(ListModPerson));
        }

        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            //foreach (var item in listPerson)
            //{
            //    item.premiaPrize.Calculation();
            //}

            OnPropertyChanged(nameof(ListModPerson));

        }


    }
}
