using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using Tabel.Views.Admins;

namespace Tabel.ViewModels.Admins
{
    internal class PassWindowViewModel : ViewModel
    {

        public string OldPass { get; set; }
        public string NewPass1 { get; set; }
        public string NewPass2 { get; set; }
        public string HashPass;

        public PassWindowViewModel()
        {
        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Изменить пароль 
        //--------------------------------------------------------------------------------
        public ICommand ChangeCommand => new LambdaCommand(OnChangeCommandExecuted, CanChangeCommand);
        private bool CanChangeCommand(object p) => true;
        private void OnChangeCommandExecuted(object p)
        {
            if (NewPass1 != NewPass2)
            {
                MessageBox.Show("Пароль не одинаковы. Повторите.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (App.CurrentUser.u_pass != OldPass)
            {
                MessageBox.Show("Неверный старый пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            HashPass = Encrypt.Crypt(NewPass1);

/*            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(NewPass1));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            HashPass = sBuilder.ToString();
*/
            PassWindow win = App.Current.Windows.OfType<PassWindow>().FirstOrDefault();
            win.DialogResult = true;
        }

        //--------------------------------------------------------------------------------
        // Команда Изменить пароль 
        //--------------------------------------------------------------------------------
        public ICommand CancelCommand => new LambdaCommand(OnCancelCommandExecuted, CanCancelCommand);
        private bool CanCancelCommand(object p) => true;
        private void OnCancelCommandExecuted(object p)
        {
            App.Current.Windows.OfType<PassWindow>().FirstOrDefault()?.Close();
        }

        #endregion

    }
}
