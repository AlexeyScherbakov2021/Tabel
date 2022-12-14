using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class PremiaOtdelViewModel : ModViewModel
    {
        public ICollection<ModPerson> ListModPerson { get; set; }
        public decimal? SetProcPrem { get; set; }

        public PremiaOtdelViewModel(BaseModel db) : base(db)
        {
                
        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            ListModPerson = listPerson?.Where(it => it.person.p_type_id == SpecType.ИТР).ToList(); ;

            LoadFromCategory(ListModPerson);


            OnPropertyChanged(nameof(ListModPerson));
        }


        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            LoadFromCategory(listPerson);

            OnPropertyChanged(nameof(ListModPerson));

        }

        private void LoadFromCategory(ICollection<ModPerson> listPerson)
        {
            if (listPerson is null)
                return;

            foreach (var item in listPerson)
            {
                item.md_prem_otdel = item.TabelHours * item.person.p_premTarif;
            }
        }

        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Применить % премии к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetProcPremCommand => new LambdaCommand(OnSetProcPremCommandExecuted, CanSetProcPremCommand);
        private bool CanSetProcPremCommand(object p) => true;
        private void OnSetProcPremCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_prem_otdel_proc = SetProcPrem;
                    item.OnPropertyChanged(nameof(item.md_prem_otdel_proc));
                }
            }

        }
        #endregion

    }
}
