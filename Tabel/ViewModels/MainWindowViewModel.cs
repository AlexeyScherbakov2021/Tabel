using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;
using Tabel.Views.Admins;

namespace Tabel.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
#if DEBUG
        public string Title { get; set; } = "Учет рабочего времени (Отладочная версия)";
#endif

#if DEMO
        public string Title { get; set; } = "Учет рабочего времени (ДЕМО)";
#endif

#if RELEASE
        public string Title { get; set; } = "Учет рабочего времени";
#endif

        public string LoginUser => App.CurrentUser.u_fio;

        public WindowState WinState { get; set; }

#region Команды
        //--------------------------------------------------------------------------------
        // Команда Загрузить график смен
        //--------------------------------------------------------------------------------
        public ICommand ExecSmenaCommand => new LambdaCommand(OnExecSmenaCommandExecuted, CanExecSmenaCommand);
        private bool CanExecSmenaCommand(object p) => true;
        private void OnExecSmenaCommandExecuted(object p)
        {
            BasicWindow win = new BasicWindow();
            win.WindowState = WinState;
            BasicWindowViewModel vm = new BasicWindowViewModel(win, new SmenaUC(), "График смен");
            win.DataContext = vm;
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
            BasicWindow win = new BasicWindow();
            win.WindowState = WinState;
            BasicWindowViewModel vm = new BasicWindowViewModel(win, new TabelUC(), "Табель");
            win.DataContext = vm;
            win.ShowDialog();

        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить Модель
        //--------------------------------------------------------------------------------
        public ICommand ExecModCommand => new LambdaCommand(OnExecModCommandExecuted, CanModTabelCommand);
        private bool CanModTabelCommand(object p) => true;
        private void OnExecModCommandExecuted(object p)
        {
            BasicWindow win = new BasicWindow();
            win.WindowState = WinState;
            BasicWindowViewModel vm = new BasicWindowViewModel(win, new ModUC(), "Модель расчета");
            win.DataContext = vm;
            win.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить Общие начисления
        //--------------------------------------------------------------------------------
        public ICommand DataSummaryCommand => new LambdaCommand(OnDataSummaryCommandExecuted, CanDataSummaryCommand);
        private bool CanDataSummaryCommand(object p) => true;
        private void OnDataSummaryCommandExecuted(object p)
        {
            DataSummaryView win = new DataSummaryView();
            //win.WindowState = WinState;
            win.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить Транспорт
        //--------------------------------------------------------------------------------
        public ICommand ExecTranspCommand => new LambdaCommand(OnExecTranspCommandExecuted, CanTranspCommand);
        private bool CanTranspCommand(object p) => true;
        private void OnExecTranspCommandExecuted(object p)
        {
            BasicWindow win = new BasicWindow();
            win.WindowState = WinState;
            BasicWindowViewModel vm = new BasicWindowViewModel(win, new TransportUC(), "Использование транспорта");
            win.DataContext = vm;
            win.ShowDialog();

        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить График отпусков
        //--------------------------------------------------------------------------------
        public ICommand ExecOtpuskCommand => new LambdaCommand(OnExecOtpuskpCommandExecuted, CanOtpuskCommand);
        private bool CanOtpuskCommand(object p) => true;
        private void OnExecOtpuskpCommandExecuted(object p)
        {
            BasicWindow win = new BasicWindow();
            win.WindowState = WinState;
            BasicWindowViewModel vm = new BasicWindowViewModel(win, new OtpuskUC(), "График отпусков");
            win.DataContext = vm;
            win.ShowDialog();

        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить График отпусков
        //--------------------------------------------------------------------------------
        public ICommand ExecSeparCommand => new LambdaCommand(OnExecSeparCommandExecuted, CanExecSeparCommand);
        private bool CanExecSeparCommand(object p) => true;
        private void OnExecSeparCommandExecuted(object p)
        {
            BasicWindow win = new BasicWindow();
            win.WindowState = WinState;
            BasicWindowViewModel vm = new BasicWindowViewModel(win, new SeparUC(), "Отдельная форма");
            win.DataContext = vm;
            win.ShowDialog();

        }

#endregion



        public MainWindowViewModel()
        {

        }


    }
}
