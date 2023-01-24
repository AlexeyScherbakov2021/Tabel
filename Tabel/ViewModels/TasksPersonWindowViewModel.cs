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
        
        private decimal _proc100;
        public decimal proc100 { get => _proc100; set { Set(ref _proc100, value); } }

        public TasksPersonWindowViewModel() { }

        public TasksPersonWindowViewModel(ModPerson person)
        {
            ListTarget = new ObservableCollection<TargetTask>( person.ListTargetTask);
            foreach (TargetTask item in ListTarget)
                item.PropertyChanged += Item_PropertyChanged;
            ListTarget.CollectionChanged += ListTarget_CollectionChanged;
            proc100 = ListTarget.Sum(it => it.tt_proc_task);
        }

        private void ListTarget_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems?.Count > 0)
            {
                foreach (TargetTask item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            if (e.OldItems?.Count > 0)
            {
                foreach (TargetTask item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
                proc100 = ListTarget.Sum(it => it.tt_proc_task);
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            proc100 = ListTarget.Sum(it => it.tt_proc_task);
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
