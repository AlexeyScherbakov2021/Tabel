﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class EditTablesWindowModel : ViewModel
    {
        RepositoryMSSQL<Category> repo = new RepositoryMSSQL<Category>();


        #region Команды
        public ICommand ClosingCommand => new LambdaCommand(OnClosingCommandExecuted, CanClosingCommand);
        private bool CanClosingCommand(object p) => true;
        private void OnClosingCommandExecuted(object p)
        {
            repo.Save();
            //UserViewModel.SaveUsers();
        }
        #endregion


    }
}
