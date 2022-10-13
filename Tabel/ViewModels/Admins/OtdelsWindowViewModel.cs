using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.Admins
{
    internal class OtdelsWindowViewModel : ViewModel
    {
        // Отделы -----------------------------------------------------

        private readonly RepositoryOtdel repoOtdel;
        //private readonly RepositoryMSSQL<Otdel> repoOtdel;
        // Список всех отделов
        public ObservableCollection<Otdel> ListOtdel { get; set; }

        public Visibility VisibleAdmin => App.CurrentUser.u_role == Infrastructure.UserRoles.Admin ? Visibility.Visible : Visibility.Collapsed;


        // выбранный отдел
        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel 
        { 
            get => _SelectedOtdel;
            set 
            {
                if (Set(ref _SelectedOtdel, value))
                {
                    _SelectedOtdel = value;
                    ListPersonal = new ObservableCollection<Personal>(repoPerson.Items.Where(it => it.p_otdel_id == _SelectedOtdel.id));
                }
            } 
        }

        // Разряды ----------------------------------------------------
        private readonly RepositoryMSSQL<Category> repoCat = new RepositoryMSSQL<Category>();

        public List<Category> ListCategory { get; set; } //= repoCat.Items.AsNoTracking().OrderBy(o => o.id).ToList();


        // Персонал отдела --------------------------------------------

        private readonly RepositoryMSSQL<Personal> repoPerson;

        public Personal SelectedPerson { get; set; }

        private ObservableCollection<Personal> _ListPersonal;
        public ObservableCollection<Personal> ListPersonal 
        { 
            get => _ListPersonal; 
            set 
            { 
                if(Set(ref _ListPersonal, value))
                {
                    //_ListCollectionPerson = new CollectionViewSource();
                    if(_ListCollectionPerson.View != null)
                        _ListCollectionPerson.View.CurrentChanged -= ListPersonalView_CurrentChanged;
                    _ListCollectionPerson.Source = value;
                    _ListCollectionPerson.View.CurrentChanged += ListPersonalView_CurrentChanged;
                    _ListCollectionPerson.View.Refresh();
                    OnPropertyChanged(nameof(ListPersonalView));
                }
            } 
        }

        private CollectionViewSource _ListCollectionPerson = new CollectionViewSource();
        public ICollectionView ListPersonalView => _ListCollectionPerson?.View;



        //--------------------------------------------------------------------------------
        // конструктор
        //--------------------------------------------------------------------------------
        public OtdelsWindowViewModel()
        {
            repoOtdel = new RepositoryOtdel();
            //repoOtdel = new RepositoryMSSQL<Otdel>();
            repoPerson = new RepositoryMSSQL<Personal>();

            if (App.CurrentUser.u_role == UserRoles.Admin)
            {
                ListOtdel = new ObservableCollection<Otdel>(repoOtdel.Items.Where(it => it.ot_parent == null));
                //ListOtdel = new ObservableCollection<Otdel>(repoOtdel.Items);
            }
            else
            {
                ListOtdel = new ObservableCollection<Otdel>(repoOtdel.GetTreeOtdels(App.CurrentUser.otdels));

                //ListOtdel = new ObservableCollection<Otdel>(repoOtdel.Items.Where(it => it.id == App.CurrentUser.u_otdel_id));
            }

            SelectedOtdel = ListOtdel.Count > 0 ?  ListOtdel[0] : null;


            ListCategory = repoCat.Items.AsNoTracking().OrderBy(o => o.id).ToList();
        }

        private void ListPersonalView_CurrentChanged(object sender, EventArgs e)
        {
            repoPerson.Save();
        }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить отдел
        //--------------------------------------------------------------------------------
        public ICommand AddOtdelCommand => new LambdaCommand(OnAddOtdelCommandExecuted, CanAddOtdelCommand);
        private bool CanAddOtdelCommand(object p) => true;
        private void OnAddOtdelCommandExecuted(object p)
        {
            Otdel NewOtdel = new Otdel();
            NewOtdel.ot_name = "Новый отдел";
            //NewOtdel.ot_parent = SelectedOtdel.id;
            repoOtdel.Add(NewOtdel, true);
            ListOtdel.Add(NewOtdel);

            //SelectedOtdel.subOtdels.Add(NewOtdel);
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить группу
        //--------------------------------------------------------------------------------
        public ICommand AddGroupCommand => new LambdaCommand(OnAddGroupCommandExecuted, CanAddGroupCommand);
        private bool CanAddGroupCommand(object p) => SelectedOtdel != null;
        private void OnAddGroupCommandExecuted(object p)
        {
            Otdel NewOtdel = new Otdel();
            NewOtdel.ot_name = "Новая группа";
            NewOtdel.ot_parent = SelectedOtdel.ot_parent ?? SelectedOtdel.id;

            repoOtdel.Add(NewOtdel, true);
            //SelectedOtdel.subOtdels.Add(NewOtdel);

            SelectedOtdel.OnPropertyChanged(nameof(SelectedOtdel.subOtdels));
            OnPropertyChanged(nameof(SelectedOtdel));
            OnPropertyChanged(nameof(ListOtdel));
        }

        //--------------------------------------------------------------------------------
        // Команда потеря фокуса
        //--------------------------------------------------------------------------------
        public ICommand LostFocusCommand => new LambdaCommand(OnLostFocusCommandExecuted, CanLostFocusCommand);
        private bool CanLostFocusCommand(object p) => SelectedOtdel != null;
        private void OnLostFocusCommandExecuted(object p)
        {
            repoOtdel.Save();
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить отдел
        //--------------------------------------------------------------------------------
        public ICommand DeleteOtdelCommand => new LambdaCommand(OnDeleteOtdelCommandExecuted, CanDeleteOtdelCommand);
        private bool CanDeleteOtdelCommand(object p) => SelectedOtdel != null && SelectedOtdel.subOtdels.Count == 0;
        private void OnDeleteOtdelCommandExecuted(object p)
        {
            if(MessageBox.Show($"Удалить «{SelectedOtdel.ot_name}»","Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var parentID = SelectedOtdel.ot_parent;

                try
                {
                    repoOtdel.Delete(SelectedOtdel, true);
                    //repoOtdel.Remove(SelectedOtdel);
                    //SelectedOtdel.parent.subOtdels.Remove(SelectedOtdel);
                    if (parentID == null)
                        ListOtdel.Remove(SelectedOtdel);
                }
                catch
                {
                   
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => SelectedOtdel != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            Personal NewPerson = new Personal();
            NewPerson.p_lastname = "Новый сотрудник";
            NewPerson.p_otdel_id = SelectedOtdel.id;
            NewPerson.p_delete = false;

            repoPerson.Add(NewPerson, true);
            ListPersonal.Add(NewPerson);

            ListPersonalView.MoveCurrentToLast();

            DataGrid dgrid = p as DataGrid;
            FocusManager.SetFocusedElement(dgrid, dgrid);
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand DeletePersonCommand => new LambdaCommand(OnDeletePersonCommandExecuted, CanDeletePersonCommand);
        private bool CanDeletePersonCommand(object p) => SelectedOtdel != null;
        private void OnDeletePersonCommandExecuted(object p)
        {
            if(MessageBox.Show($"Удалить «{SelectedPerson.p_lastname} {SelectedPerson.p_name} {SelectedPerson.p_midname}»","Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                repoPerson.Delete(SelectedPerson, true);
                ListPersonal.Remove(SelectedPerson);
            }
        }

        #endregion

    }
}
