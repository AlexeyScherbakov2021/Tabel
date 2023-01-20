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

namespace Tabel.ViewModels.Admins
{
    internal class ProfileUserUCViewModel : ViewModel
    {

        public string OldPass { get; set; } = "123";
        public string NewPass1 { get; set; } = "12345";
        public string NewPass2 { get; set; } = "12345";


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand ChangeCommand => new LambdaCommand(OnChangeCommandExecuted, CanChangeCommand);
        private bool CanChangeCommand(object p) => true;
        private void OnChangeCommandExecuted(object p)
        {
            PassWindow win = new PassWindow();
            win.ShowDialog();


            if(NewPass1 != NewPass2)
            {
                MessageBox.Show("Пароль не одинаковы. Повторите.","Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if(App.CurrentUser.u_pass != OldPass)
            {
                MessageBox.Show("Неверный старый пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }


        }
        #endregion

    }
}
