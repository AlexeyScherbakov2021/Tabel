using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;
using Tabel.Views.Admins;

namespace Tabel.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {

        public string LoginUser => App.CurrentUser.u_fio;


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Загрузить график смен
        //--------------------------------------------------------------------------------
        public ICommand ExecSmenaCommand => new LambdaCommand(OnExecSmenaCommandExecuted, CanExecSmenaCommand);
        private bool CanExecSmenaCommand(object p) => true;
        private void OnExecSmenaCommandExecuted(object p)
        {
            SmenaWindow win = new SmenaWindow();
            win.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить производственный календарь
        //--------------------------------------------------------------------------------
        public ICommand ExecCalCommand => new LambdaCommand(OnExecCalCommandExecuted, CanExecCalCommand);
        private bool CanExecCalCommand(object p) => true;
        private void OnExecCalCommandExecuted(object p)
        {
            EditTablesWindow win = new EditTablesWindow();
            win.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить табель
        //--------------------------------------------------------------------------------
        public ICommand ExecTabelCommand => new LambdaCommand(OnExecTabelCommandExecuted, CanExecTabelCommand);
        private bool CanExecTabelCommand(object p) => true;
        private void OnExecTabelCommandExecuted(object p)
        {
            TabelWindow win = new TabelWindow();
            win.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить Модель
        //--------------------------------------------------------------------------------
        public ICommand ExecModCommand => new LambdaCommand(OnExecModCommandExecuted, CanModTabelCommand);
        private bool CanModTabelCommand(object p) => true;
        private void OnExecModCommandExecuted(object p)
        {
            ModWindow win = new ModWindow();
            win.ShowDialog();
        }

        #endregion



        public MainWindowViewModel()
        {

        }


    }
}
