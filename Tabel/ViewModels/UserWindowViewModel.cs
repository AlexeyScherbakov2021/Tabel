using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        IRepository<User> repoUser;

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
                }
            }
        }
        CollectionViewSource _listUserViewSource;
        public ICollectionView ListUserView => _listUserViewSource?.View;



        public User SelectedUser { get; set; }

        public UserWindowViewModel()
        {
            repoUser = new RepositoryMSSQL<User>();
            ListUser = new ObservableCollection<User>(repoUser.Items);
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
            if (MessageBox.Show(  $"Удалить {SelectedUser.u_login}","Прудепреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
        }

        #endregion

    }
}
