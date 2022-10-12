using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class UserWindowViewModel : ViewModel
    {
        readonly IRepository<User> repoUser;
        private readonly RepositoryOtdel repoOtdel;

        public ObservableCollection<Otdel> ListOtdel { get; set; }


        private ObservableCollection<User> _ListUser;
        public ObservableCollection<User> ListUser {
            get => _ListUser;
            set
            {
                if (Set(ref _ListUser, value))
                {
                    _listUserViewSource = new CollectionViewSource();
                    _listUserViewSource.Source = value;
                    _listUserViewSource.View.Refresh();
                    ListUserView.CurrentChanged += ListUserView_CurrentChanged;
                }
            }
        }


        private void ListUserView_CurrentChanged(object sender, EventArgs e)
        {
            //if (IsOpenPopup)
            //    GetOtdelsFromUser(_SelectedUser, ListOtdel);
            IsOpenPopup = false;
            repoUser.Save();
        }

        CollectionViewSource _listUserViewSource;
        public ICollectionView ListUserView => _listUserViewSource?.View;

        private User _SelectedUser;
        public User SelectedUser 
        { 
            get => _SelectedUser; 
            set
            {
                if (_SelectedUser == value) return;

                GetOtdelsFromUser(_SelectedUser, ListOtdel);
                _SelectedUser = value;
                SetOtdelsToUser(_SelectedUser, ListOtdel);
            }
        }

        private bool _IsOpenPopup = false;
        public bool IsOpenPopup { get => _IsOpenPopup; set { Set(ref _IsOpenPopup, value); } }


        //--------------------------------------------------------------------------------
        // Отметка в списке отделов для пользователя
        //--------------------------------------------------------------------------------
        private void SetOtdelsToUser(User user,  ICollection<Otdel> ListSiblingOtdel)
        {
            if (user is null || ListSiblingOtdel?.Count == 0) return;

            foreach (Otdel otdel in ListSiblingOtdel)
            {
                otdel.IsChecked = user.otdels.Any(it => it.id == otdel.id);
                otdel.OnPropertyChanged(nameof(otdel.IsChecked));
                if (otdel.subOtdels.Count > 0)
                    SetOtdelsToUser(user, otdel.subOtdels);
            }

        }

        //--------------------------------------------------------------------------------
        // Получение отделов для пользователя
        //--------------------------------------------------------------------------------
        private void GetOtdelsFromUser(User user, ICollection<Otdel> ListSiblingOtdel)
        {
            if (user is null || ListSiblingOtdel?.Count == 0) return;

            foreach (Otdel otdel in ListSiblingOtdel)
            {
                if (otdel.IsChecked)
                    user.otdels.Add(otdel);
                else
                    user.otdels.Remove(otdel);

                if (otdel.subOtdels.Count > 0)
                    GetOtdelsFromUser(user, otdel.subOtdels);
            }

            user.OnPropertyChanged(nameof(user.otdels));
        }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            User newUser = new User { u_login = "Пользователь" };
            ListUser.Add(newUser);
            ListUserView.MoveCurrentToLast();
            repoUser.Add(newUser, true);
        }
        //--------------------------------------------------------------------------------
        // Команда Удалить
        //--------------------------------------------------------------------------------
        public ICommand DeleteCommand => new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommand);
        private bool CanDeleteCommand(object p) => SelectedUser != null;
        private void OnDeleteCommandExecuted(object p)
        {
            if (MessageBox.Show( $"Удалить {SelectedUser.u_login}","Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                repoUser.Delete(SelectedUser.id, true);
                ListUser.Remove(SelectedUser);
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Привязать отдел 
        //--------------------------------------------------------------------------------
        public ICommand OtdelCommand => new LambdaCommand(OnOtdelCommandExecuted, CanOtdelCommand);
        private bool CanOtdelCommand(object p) => true;
        private void OnOtdelCommandExecuted(object p)
        {
            if (IsOpenPopup)
                GetOtdelsFromUser(_SelectedUser, ListOtdel);
            IsOpenPopup = !IsOpenPopup;
        }

        //--------------------------------------------------------------------------------
        // Событие окончания редактирования ячейки
        //-------------------------------------------------------------------------------
        public ICommand SelectOtdelCommand => new LambdaCommand(OnSelectOtdelCommandExecuted, CanSelectOtdelCommand);
        private bool CanSelectOtdelCommand(object p) => true;
        private void OnSelectOtdelCommandExecuted(object p)
        {
            if(p is Otdel otdel)
            {
                //SelectedUser.u_otdel_id = otdel.id;
                //SelectedUser.otdel = null;
                repoUser.Update(SelectedUser);
                IsOpenPopup = false;
                ListUserView.Refresh();
            }
        }

        #endregion

        //--------------------------------------------------------------------------------
        // Конструктор 
        //--------------------------------------------------------------------------------
        public UserWindowViewModel()
        {
            repoUser = new RepositoryMSSQL<User>();
            ListUser = new ObservableCollection<User>(repoUser.Items);
            repoOtdel = new RepositoryOtdel();
            ListOtdel = new ObservableCollection<Otdel>(repoOtdel.Items);
        }

    }
}
