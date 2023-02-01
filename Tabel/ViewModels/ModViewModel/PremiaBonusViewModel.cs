using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class PremiaBonusViewModel : ModViewModel
    {
        public ICollection<ModPerson> ListModPerson { get; set; }
        public bool IsCheckBonus { get; set; }
        public decimal? SetMaxPrem { get; set; }

        private decimal? _BonusProc;
        public decimal? BonusProc { get => _BonusProc; set { Set(ref _BonusProc, value); } }

        public PremiaBonusViewModel(BaseModel db) : base(db)
        {
            //BonusProc = bonusProc;
        }


        public void SetBonusProc(decimal? bonusProc)
        {
            BonusProc = bonusProc;
        }


        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            //_SelectedOtdel = Otdel;
            _SelectMonth= Month;
            _SelectYear = Year;
            ListModPerson = listPerson;

            //SetPersonBonusProc(ListModPerson);
            OnPropertyChanged(nameof(ListModPerson));
        }

        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            //SetPersonBonusProc(listPerson);
            OnPropertyChanged(nameof(ListModPerson));

        }


        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных общего расчета 
        //-------------------------------------------------------------------------------------------------------
        //private void SetPersonBonusProc(ICollection<ModPerson> listPerson)
        //{
        //    if (listPerson is null) return;

        //    //RepositoryMSSQL<GenChargMonth> repoGetAll = new RepositoryMSSQL<GenChargMonth>(db);
        //    //decimal? BonusProc = repoGetAll.Items
        //    //    .FirstOrDefault(it => it.gm_Year == _SelectYear && it.gm_Month == _SelectMonth && it.gm_GenId == (int)EnumKind.BonusProc)?.gm_Value;

        //    foreach (var modPerson in listPerson)
        //    {
        //        //modPerson.premiaBonus.BonusForAll = BonusProc;
        //        modPerson.md_bonus_exec = (BonusProc != null && BonusProc > 0);
        //    }

        //}

        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Отметить выбранные
        //--------------------------------------------------------------------------------
        public ICommand CheckCommand => new LambdaCommand(OnCheckCommandExecuted, CanCheckCommand);
        private bool CanCheckCommand(object p) => true;
        private void OnCheckCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_bonus_exec = IsCheckBonus;
                    item.OnPropertyChanged(nameof(item.md_bonus_exec));
                }
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Применить сумму к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetMaxSummaCommand => new LambdaCommand(OnSetMaxSummaCommandExecuted, CanSetMaxSummaCommand);
        private bool CanSetMaxSummaCommand(object p) => true;
        private void OnSetMaxSummaCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_bonus_max = SetMaxPrem;
                    item.OnPropertyChanged(nameof(item.md_bonus_max));
                }
            }

        }


        #endregion
    }
}
