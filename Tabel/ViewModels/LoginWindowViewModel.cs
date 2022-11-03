﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class LoginWindowViewModel : ViewModel
    {
        private readonly IRepository<User> repo;
        private readonly LoginWindow winLogin;


        public User SelectUser { get; set; }
        public IEnumerable<User> ListUser { get; set; }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда 
        //--------------------------------------------------------------------------------
        //private readonly ICommand _OkCommand = null;
        public ICommand OkCommand => new LambdaCommand(OnOkCommandExecuted, CanOkCommand);
        private bool CanOkCommand(object p) => true;
        private void OnOkCommandExecuted(object p)
        {
            if(p is PasswordBox pass )
            {
                // если праоль неверный, то ничего не делаем - возврат
                if (pass.Password != SelectUser?.u_pass)
                    return;

                App.CurrentUser = SelectUser;


                //if(App.CurrentUser.u_role == Infrastructure.UserRoles.Admin)
                //{
                //    // если это администратор, то запускаем настройки
                //    EditTablesWindow win = new EditTablesWindow();
                //    win.Show();
                //    App.Current.MainWindow = win;
                //}
                //else
                //{
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
                //}


            }

            winLogin.Close();
        }

        #endregion


        public LoginWindowViewModel()
        {
            App.Log.WriteLineLog("Запуск конструктора LoginWindowViewModel");

            winLogin = App.Current.Windows.OfType<LoginWindow>().First();
            //winLogin.Closing += Win_Closing;

            App.Log.WriteLineLog($"winLogin = {winLogin}" );

            repo = AllRepo.GetRepoUser();
            App.Log.WriteLineLog($"repo = {repo}");

            ListUser = repo.Items.OrderBy(o => o.u_login).ToArray();
            App.Log.WriteLineLog($"ListUser = {ListUser}");

            string login = "Admin";
            RegistryKey SoftKey = Registry.CurrentUser.OpenSubKey("SOFTWARE");

            App.Log.WriteLineLog($"SoftKey = {SoftKey}");

            RegistryKey ProgKey = SoftKey.OpenSubKey("TabelNGK");
            App.Log.WriteLineLog($"ProgKey = {ProgKey}");

            if (ProgKey != null)
            {
                login = ProgKey.GetValue("login", "Admin").ToString();
                App.Log.WriteLineLog($"login = {login}");
                ProgKey.Close();
            }
            SoftKey.Close();

            SelectUser = ListUser.FirstOrDefault(it => it.u_login == login);
            App.Log.WriteLineLog($"SelectUser = {SelectUser}");

        }
    }
}
