using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tabel.Commands;
using Tabel.ViewModels.Base;
using System.Security.Policy;
using System.Security;
using Tabel.Views.Admins;
using System.Security.Cryptography;
using Tabel.Repository;
using Tabel.Models;

namespace Tabel.ViewModels.Admins
{
    internal class ProfileUserUCViewModel : ViewModel
    {
        RepositoryMSSQL<User> RepoUser;

        public string OldPass { get; set; } = "123";
        public string NewPass1 { get; set; } = "12345";
        public string NewPass2 { get; set; } = "12345";


        public ProfileUserUCViewModel()
        {
            RepoUser = new RepositoryMSSQL<User>();
        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Изменить пароль 
        //--------------------------------------------------------------------------------
        public ICommand ChangeCommand => new LambdaCommand(OnChangeCommandExecuted, CanChangeCommand);
        private bool CanChangeCommand(object p) => true;
        private void OnChangeCommandExecuted(object p)
        {
            PassWindow win = new PassWindow();
            if(win.ShowDialog() == true)
            {
                PassWindowViewModel vm = win.DataContext as PassWindowViewModel;
                User user = RepoUser.Items.FirstOrDefault(it => it.id == App.CurrentUser.id);
                if (user != null)
                {
                    user.u_pass2 = vm.HashPass;
                    RepoUser.Save();
                }
            }

        }
        #endregion

    }
}
