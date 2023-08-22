using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class AddOnceWorkWindowViewModel : ViewModel
    {

        private bool IsReadOnly = false;
        private readonly int Year;
        private readonly int IDModPerson;

        public string Title { get; set; }

        private decimal? _summa;
        public decimal? summa { get => _summa; set { Set(ref _summa, value); } }

        public ObservableCollection<AddOnceWork> ListWorkOnce { get; set; }

        public AddOnceWorkWindowViewModel()
        {                
        }

        public AddOnceWorkWindowViewModel(ModPerson person)
        {
            Title = "Одноразовые работы " + person.person.FIO;

            IsReadOnly = person.Mod.m_IsClosed == true;
            Year = person.Mod.m_year;
            IDModPerson = person.id;
            ListWorkOnce = new ObservableCollection<AddOnceWork>(person.ListAddOnceWork);
            foreach (AddOnceWork item in ListWorkOnce)
                item.PropertyChanged += Item_PropertyChanged;
            ListWorkOnce.CollectionChanged += ListTarget_CollectionChanged;
            summa = ListWorkOnce.Sum(it => it.ao_summa);

        }

        private void ListTarget_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems?.Count > 0)
            {
                foreach (AddOnceWork item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            if (e.OldItems?.Count > 0)
            {
                foreach (AddOnceWork item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
                summa = ListWorkOnce.Sum(it => it.ao_summa);
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            summa = ListWorkOnce.Sum(it => it.ao_summa);
        }



        #region Команды =================================

        //--------------------------------------------------------------------------------
        // Команда Кнопка ОК
        //--------------------------------------------------------------------------------
        public ICommand OkCommand => new LambdaCommand(OnOkCommandExecuted, CanOkCommand);
        private bool CanOkCommand(object p) => !IsReadOnly;
        private void OnOkCommandExecuted(object p)
        {
            AddOnceWorkWindow win = App.Current.Windows.OfType<AddOnceWorkWindow>().FirstOrDefault();
            win.DialogResult = true;
            win.Close();

        }
        //--------------------------------------------------------------------------------
        // Команда Кнопка Отменить
        //--------------------------------------------------------------------------------
        //public ICommand CancelCommand => new LambdaCommand(OnCancelCommandExecuted, CanCancelCommand);
        //private bool CanCancelCommand(object p) => true;
        //private void OnCancelCommandExecuted(object p)
        //{
        //    AddOnceWorkWindow win = App.Current.Windows.OfType<AddOnceWorkWindow>().FirstOrDefault();
        //    win.Close();
        //}


        //--------------------------------------------------------------------------------
        // Команда удалить строку
        //--------------------------------------------------------------------------------
        public ICommand DeleteLineCommand => new LambdaCommand(OnDeleteLineCommandExecuted, CanDeleteLineCommand);
        private bool CanDeleteLineCommand(object p) => true;
        private void OnDeleteLineCommandExecuted(object p)
        {
            if (p is AddOnceWork aw)
            {
                if (MessageBox.Show($"Удалить строку ­«{aw.ao_name}»", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ListWorkOnce.Remove(aw);
                }

            }

        }

        #endregion


    }

}
