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
            IsOpenPopup = false;
            repoUser.Save();
        }

        CollectionViewSource _listUserViewSource;
        public ICollectionView ListUserView => _listUserViewSource?.View;

        public User SelectedUser { get; set; }

        private bool _IsOpenPopup = false;
        public bool IsOpenPopup { get => _IsOpenPopup; set { Set(ref _IsOpenPopup, value); } }



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
                SelectedUser.u_otdel_id = otdel.id;
                SelectedUser.otdel = null;
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
            repoOtdel = new RepositoryOtdel();
            ListUser = new ObservableCollection<User>(repoUser.Items);
            ListOtdel = new ObservableCollection<Otdel>(repoOtdel.Items);
        }

    }
}
