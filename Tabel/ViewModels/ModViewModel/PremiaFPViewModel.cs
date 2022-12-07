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
    internal class PremiaFPViewModel : ModViewModel
    {
        public ICollection<ModPerson> ListModPerson { get; set; }

        public PremiaFPViewModel(BaseModel db) : base(db)
        {

        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {

            //_SelectedOtdel = Otdel;
            _SelectMonth = Month;
            _SelectYear = Year;
            ListModPerson = listPerson;

            if (ListModPerson != null)
            {
                foreach (var modPerson in ListModPerson)
                {
                    // расчет премии из ФП
                    modPerson.premiaFP.Calculation();
                    //рассчет суммарных процентов в премии ФП
                    modPerson.premiaFP.CalcChangeProcent();
                }
            }

            OnPropertyChanged(nameof(ListModPerson));
        }

        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            if (listPerson != null)
            {
                foreach (var modPerson in listPerson)
                {
                    // расчет премии из ФП
                    modPerson.premiaFP.Calculation();
                    //рассчет суммарных процентов в премии ФП
                    modPerson.premiaFP.CalcChangeProcent();
                    ListModPerson.Add(modPerson);
                }
            }
            OnPropertyChanged(nameof(ListModPerson));
        }

    }
}
