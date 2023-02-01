using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class LoginWindowViewModel : ViewModel
    {
        private readonly RepositoryMSSQL<User> repo;
        private readonly LoginWindow winLogin;


        public User SelectUser { get; set; }
        public IEnumerable<User> ListUser { get; set; }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда 
        //--------------------------------------------------------------------------------
        //private readonly ICommand _OkCommand = null;
        public ICommand OkCommand => new LambdaCommand(OnOkCommandExecuted, CanOkCommand);
        private bool CanOkCommand(object p) => p != null && !string.IsNullOrEmpty( (p as PasswordBox).Password);
        private void OnOkCommandExecuted(object p)
        {
            if(p is PasswordBox pass )
            {
                bool res = false;

                if (SelectUser?.u_pass2 != null)
                {
                    string hash = Encrypt.Crypt(pass.Password);
                    res = hash == SelectUser?.u_pass2;
                }
                else
                    res = pass.Password == SelectUser?.u_pass;

                if (res)
                {
                    App.CurrentUser = SelectUser;

                    // записываем в реестр
                    RegistryKey SoftKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                    RegistryKey ProgKey = SoftKey.CreateSubKey("TabelNGK");
                    ProgKey.SetValue("login", SelectUser.u_login);
                    ProgKey.Close();
                    SoftKey.Close();


                    // если пользователь, то запускаем табель
                    MainWindow win = new MainWindow();
                    win.Show();
                    App.Current.MainWindow = win;
                    winLogin.Close();
                }
                else
                    MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        //--------------------------------------------------------------------------------
        // Команда 
        //--------------------------------------------------------------------------------
        //private readonly ICommand _OkCommand = null;
        public ICommand CancelCommand => new LambdaCommand(OnCancelCommandExecuted, CanCancelCommand);
        private bool CanCancelCommand(object p) =>  true;
        private void OnCancelCommandExecuted(object p)
        {
            winLogin.Close();
        }
        #endregion


        public LoginWindowViewModel()
        {

            winLogin = App.Current.Windows.OfType<LoginWindow>().First();

            //repo = AllRepo.GetRepoUser();
            repo = new RepositoryMSSQL<User>();

            ListUser = repo.Items.OrderBy(o => o.u_login).ToArray();

            string login = "Admin";
            RegistryKey SoftKey = Registry.CurrentUser.OpenSubKey("SOFTWARE");

            RegistryKey ProgKey = SoftKey.OpenSubKey("TabelNGK");

            if (ProgKey != null)
            {
                login = ProgKey.GetValue("login", "Admin").ToString();
                ProgKey.Close();
            }
            SoftKey.Close();

            SelectUser = ListUser.FirstOrDefault(it => it.u_login == login);
        }
    }
}
