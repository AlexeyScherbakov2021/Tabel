using System;
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
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class EditTablesWindowModel : ViewModel
    {
        private readonly RepositoryOtdel repo;
        private readonly IRepository<Personal> repoPerson;
        public ObservableCollection<Otdel> ListOtdel { get; set; }

        private bool _IsEditing = false;

        CollectionViewSource _listPersonalViewSource;
        public ICollectionView ListPersonalView => _listPersonalViewSource?.View;

        private Personal _SelectedPerson;
        public Personal SelectedPerson 
        { 
            get => _SelectedPerson; 
            set 
            {
                if (_SelectedPerson != null && string.IsNullOrEmpty(_SelectedPerson.p_lastname))
                    ListPersonal.Remove(_SelectedPerson);

                Set(ref _SelectedPerson, value); 
                EnabledDetailPerson = _SelectedPerson != null;
                OnPropertyChanged(nameof(EnabledDetailPerson));
            } 
        }

        //private bool _EnabledDetailPerson = false;
        public bool EnabledDetailPerson { get; set; }

        private ObservableCollection<Personal> _ListPersonal;
        public ObservableCollection<Personal> ListPersonal 
        { 
            get => _ListPersonal; 
            set
            {
                if (Set(ref _ListPersonal, value))
                {
                    _listPersonalViewSource = new CollectionViewSource();
                    _listPersonalViewSource.Source = value;
                    //_listPersonalViewSource.Filter += OnFilterList;
                    _listPersonalViewSource.View.Refresh();
                }
            }

        }


        private string _FilterName;
        public string FilterName
        {
            get => _FilterName;
            set
            {
                if (Set(ref _FilterName, value))
                    _listPersonalViewSource.View.Refresh();
            }
        }


        //private bool _IsSelected;
        //public bool IsSelected { get => _IsSelected; set { Set(ref _IsSelected, value); } }

        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel { get => _SelectedOtdel; set { Set(ref _SelectedOtdel, value); } }


        //-----------------------------------------------------------------------------------------
        // Функция для фильтования наименований
        //-----------------------------------------------------------------------------------------
        private void OnFilterList(object Sender, FilterEventArgs E)
        {
            //if (!(E.Item is Wares w) || string.IsNullOrEmpty(FilterName)) return;

            //if (!w.Name.ToLower().Contains(FilterName.ToLower()))
            //    E.Accepted = false;
        }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить отдел
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            AddFormStringWindow dlg = new AddFormStringWindow("Добавить отдел");
            if (dlg.ShowDialog() == false)
                return;

            TreeView tree = p as TreeView;

            Otdel parent = tree.SelectedItem as Otdel;

            Otdel otdel = new Otdel {
            ot_name = dlg.resultText.Text,
            parent = parent,
            ot_parent = SelectedOtdel?.id,
            subOtdels = new ObservableCollection<Otdel>()
            };

            repo.Add(otdel, true);
            //Otdel parentOtdel = ListOtdel.SingleOrDefault(it => it.id == otdel.parent.id);

            TreeViewItem tvi = tree.ItemContainerGenerator.ContainerFromItem(tree.SelectedItem) as TreeViewItem;
            if(tvi != null)
                tvi.IsExpanded = true;

            if (parent is null)
                ListOtdel.Add(otdel);
            //else
            //{
            //    parent.subOtdels.Add(otdel);
            //}


        }

        //--------------------------------------------------------------------------------
        // Команда Удалить отдел
        //--------------------------------------------------------------------------------
        public ICommand DeleteCommand => new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommand);
        private bool CanDeleteCommand(object p) => p is Otdel;
        private void OnDeleteCommandExecuted(object p)
        {
            if(p is Otdel otdel)
            {
                if (otdel.parent is null)
                    ListOtdel.Remove(otdel);
                else
                    otdel.parent?.subOtdels.Remove(otdel);

                repo.Delete(otdel.id, true);
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => true;
        private void OnAddPersonCommandExecuted(object p)
        {
            Personal newPerson = new Personal();
            ListPersonal.Add(newPerson);
            ListPersonalView.MoveCurrentToLast();

            repoPerson.Add(newPerson);
            _IsEditing = true;

        }

        //--------------------------------------------------------------------------------
        // Команда Удалить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand DeletePersonCommand => new LambdaCommand(OnDeletePersonCommandExecuted, CanDeletePersonCommand);
        private bool CanDeletePersonCommand(object p) => true;
        private void OnDeletePersonCommandExecuted(object p)
        {
            repoPerson.Delete(SelectedPerson.id);
            ListPersonal.Remove(SelectedPerson);
            _IsEditing = true;

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand SavePersonCommand => new LambdaCommand(OnSavePersonCommandExecuted, CanSavePersonCommand);
        private bool CanSavePersonCommand(object p) => true;
        private void OnSavePersonCommandExecuted(object p)
        {
            repoPerson.Save();
            _IsEditing = false;
        }

        //--------------------------------------------------------------------------------
        // Команда Открыть список оделов
        //--------------------------------------------------------------------------------
        public ICommand OpenOtdelsCommand => new LambdaCommand(OnOpenOtdelCommandExecuted, CanOpenOtdelCommand);
        private bool CanOpenOtdelCommand(object p) => true;
        private void OnOpenOtdelCommandExecuted(object p)
        {
            SelectOtdelWindow dlg = new SelectOtdelWindow();
            dlg.Owner = App.Current.Windows.OfType<EditTablesWindow>().FirstOrDefault();
            if (dlg.ShowDialog() == true)
            {
                //SelectedPerson.p_tab_number = "0123456789";
                SelectedPerson.p_otdel_id = (dlg.DataContext as SelectOtdelWindowViewModel).SelectOtdel.id;
                SelectedPerson.otdel = null;
                repoPerson.Update(SelectedPerson);
                //OnPropertyChanged(nameof(SelectedPerson));
                //SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.otdel));
            }
        }


        #endregion



        public EditTablesWindowModel()
        {
            repo = new RepositoryOtdel();
            repoPerson = new RepositoryMSSQL<Personal>();

            ListOtdel = new ObservableCollection<Otdel>(repo.Items);

            ListPersonal = new ObservableCollection<Personal>(repoPerson.Items
                .OrderBy(it => it.p_lastname)
                .ThenBy(it => it.p_name)
                .ThenBy(it => it.p_midname));

            //TreeViewItem tvi = tree.ItemContainerGenerator.ContainerFromItem(ListOtdel[0]) as TreeViewItem;
            //if (tvi != null)
            //    tvi.IsExpanded = true;

        }

    }
}
