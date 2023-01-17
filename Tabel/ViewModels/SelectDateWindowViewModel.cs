using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class SelectDateWindowViewModel : ViewModel
    {
        public List<DateTime> listDates { get; set; }
        public DateTime SelectedDate { get; set; }

        public SelectDateWindowViewModel()
        {

        }

        public SelectDateWindowViewModel(IEnumerable<int> days, int month, int year)
        {
            listDates = new List<DateTime>();
            foreach (var item in days)
            {
                DateTime dt = new DateTime(year, month, item);
                listDates.Add(dt);
            }
        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Печать
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintCommand);
        private bool CanPrintCommand(object p) => listDates != null && listDates.Contains(SelectedDate);
        private void OnPrintCommandExecuted(object p)
        {
            SelectDateWindow win = App.Current.Windows.OfType<SelectDateWindow>().FirstOrDefault();
            win.DialogResult = true;
            win.Close();

        }

        public ICommand CancelCommand => new LambdaCommand(OnCancelCommandExecuted, CanCancelCommand);
        private bool CanCancelCommand(object p) => true;
        private void OnCancelCommandExecuted(object p)
        {
            SelectDateWindow win =  App.Current.Windows.OfType<SelectDateWindow>().FirstOrDefault();
            //win.DialogResult = false;
            win.Close();
        }



        #endregion
    }
}
