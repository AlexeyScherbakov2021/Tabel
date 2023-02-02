using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tabel.Commands
{
    internal class CloseWindowCommand : Command
    {

        public override bool CanExecute(object parameter) => parameter is Window || App.FocusedWindow != null || App.ActiveWindow != null;

        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            var window = (parameter as Window ?? App.FocusedWindow ?? App.ActiveWindow);
            window.Close();
        }
    }

    internal class CloseDialogCommand : Command
    {
        public bool? DialogResult { get; set; }

        public override bool CanExecute(object parameter) => parameter is Window || App.FocusedWindow != null || App.ActiveWindow != null;

        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            var window = (parameter as Window ?? App.FocusedWindow ?? App.ActiveWindow);
            window.DialogResult = DialogResult;
            window.Close();
        }
    }
}
