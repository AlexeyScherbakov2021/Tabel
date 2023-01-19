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
    internal class PremiaKvalifViewModel : ModViewModel
    {
        public ICollection<ModPerson> ListModPerson { get; set; }

        public decimal? SetProcPrem { get; set; }
        public decimal? SetProcFull { get; set; }



        public PremiaKvalifViewModel(BaseModel db) : base(db)
        {

        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            _SelectMonth = Month;
            _SelectYear = Year;


            ListModPerson = listPerson?.Where(it => it.person.p_type_id == SpecType.ИТР).ToList(); ;

            LoadFromCategory(listPerson);

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
                item.md_kvalif_tarif = item.TabelHours * item.person.p_premTarif;
                item.PlanTarifKvalif = 162m * item.person.p_premTarif;
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
                    item.md_kvalif_prem = SetProcPrem;
                    item.OnPropertyChanged(nameof(item.md_kvalif_prem));
                }
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Применить % от полной к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetProcFullCommand => new LambdaCommand(OnSetProcFullCommandExecuted, CanSetProcFullCommand);
        private bool CanSetProcFullCommand(object p) => true;
        private void OnSetProcFullCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_kvalif_proc = SetProcFull;
                    item.OnPropertyChanged(nameof(item.md_kvalif_proc));
                }
            }

        }
        #endregion

    }
}
