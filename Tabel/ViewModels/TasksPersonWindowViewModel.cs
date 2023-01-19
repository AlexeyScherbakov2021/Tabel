using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tabel.Commands;
using Tabel.Models;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class TasksPersonWindowViewModel : ViewModel
    {
        public string Title { get; set; } = "Выполненные задания для сотрудника";
        public ObservableCollection<TargetTask> ListTarget { get; set; }

        public TasksPersonWindowViewModel() { }

        public TasksPersonWindowViewModel(ModPerson person)
        {
            ListTarget = new ObservableCollection<TargetTask>( person.ListTargetTask);
        }

        #region Команды =================================

        //--------------------------------------------------------------------------------
        // Команда Кнопка ОК
        //--------------------------------------------------------------------------------
        public ICommand OkCommand => new LambdaCommand(OnOkCommandExecuted, CanOkCommand);
        private bool CanOkCommand(object p) => true;
        private void OnOkCommandExecuted(object p)
        {
            TasksPersonWindow win = App.Current.Windows.OfType<TasksPersonWindow>().FirstOrDefault();
            win.DialogResult = true;
            win.Close();

        }
        //--------------------------------------------------------------------------------
        // Команда Кнопка Отменить
        //--------------------------------------------------------------------------------
        public ICommand CancelCommand => new LambdaCommand(OnCancelCommandExecuted, CanCancelCommand);
        private bool CanCancelCommand(object p) => true;
        private void OnCancelCommandExecuted(object p)
        {
            TasksPersonWindow win = App.Current.Windows.OfType<TasksPersonWindow>().FirstOrDefault();
            win.Close();

        }

        #endregion


    }
}
