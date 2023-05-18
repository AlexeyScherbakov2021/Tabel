using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    public class PremiaFPViewModel : ModViewModel
    {
        //private ICollection<ModPerson> _ListModPerson;
        public ObservableCollection<ModPerson> ListModPerson { get; set; }
        //public Visibility IsVisibleITR { get; set; }

        public decimal? SetProcPrem { get; set; }
        public decimal? SetTarif { get; set; }
        public string SetGroupName { get; set; }


        public PremiaFPViewModel(BaseModel db) : base(db)
        {

        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {

            //IsVisibleITR = Otdel.ot_itr == 2 ? Visibility.Collapsed : Visibility.Visible;

            //_SelectedOtdel = Otdel;
            _SelectMonth = Month;
            _SelectYear = Year;

            ListModPerson = new ObservableCollection<ModPerson>(listPerson?.Where(it => it.person?.p_type_id == SpecType.Рабочий));

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

        public override void AddPersons(ICollection<ModPerson> listNewPerson)
        {
            if (listNewPerson != null)
            {
                foreach (var modPerson in listNewPerson)
                {
                    // расчет премии из ФП
                    modPerson.premiaFP.Calculation();
                    //рассчет суммарных процентов в премии ФП
                    modPerson.premiaFP.CalcChangeProcent();
                    if (modPerson.person.p_type_id == SpecType.Рабочий)
                        ListModPerson.Add(modPerson);
                }
            }
            //OnPropertyChanged(nameof(ListModPerson));
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
                    item.md_premFP = SetProcPrem;
                    item.OnPropertyChanged(nameof(item.md_premFP));
                }
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Применить тариф премии к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetTarifPremCommand => new LambdaCommand(OnTarifProcPremCommandExecuted, CanTarifProcPremCommand);
        private bool CanTarifProcPremCommand(object p) => true;
        private void OnTarifProcPremCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_cat_prem_tarif = SetTarif;
                    item.OnPropertyChanged(nameof(item.md_cat_prem_tarif));
                }
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Применить группу к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetGroupCommand => new LambdaCommand(OnSetGroupCommandExecuted, CanSetGroupCommand);
        private bool CanSetGroupCommand(object p) => true;
        private void OnSetGroupCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_group = SetGroupName;
                    item.OnPropertyChanged(nameof(item.md_group));
                }
            }

        }

        #endregion

    }
}
