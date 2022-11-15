﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class PremiaQualityViewModel : ViewModel, IModViewModel
    {
        private int _SelectMonth;
        private int _SelectYear;
        public ObservableCollection<ModPerson> ListModPerson { get; set; }
        public bool IsCheckQuality { get; set; }


        public void ChangeListPerson(ObservableCollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            //_SelectedOtdel = Otdel;
            _SelectMonth = Month;
            _SelectYear = Year;
            ListModPerson = listPerson;


            OnPropertyChanged(nameof(ListModPerson));
        }

        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Отметить выбранные за качество
        //--------------------------------------------------------------------------------
        public ICommand CheckQualCommand => new LambdaCommand(OnCheckQualCommandExecuted, CanCheckQualCommand);
        private bool CanCheckQualCommand(object p) => true;
        private void OnCheckQualCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_quality_check = IsCheckQuality;
                    item.OnPropertyChanged(nameof(item.md_quality_check));
                }
            }
        }

        #endregion
    }
}
