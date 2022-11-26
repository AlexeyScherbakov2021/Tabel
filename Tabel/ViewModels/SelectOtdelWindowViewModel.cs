using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class SelectOtdelWindowViewModel
    {
        public ObservableCollection<Otdel> ListOtdel { get; set; }
        public Otdel SelectOtdel;

        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Выбрать
        //--------------------------------------------------------------------------------
        public ICommand SelectOtdelCommand => new LambdaCommand(OnSelectOtdelCommandExecuted, CanSelectOtdelCommand);
        private bool CanSelectOtdelCommand(object p) => true;
        private void OnSelectOtdelCommandExecuted(object p)
        {
            if (p is Otdel otdel)
            {
                SelectOtdel = otdel;
                SelectOtdelWindow win = App.Current.Windows.OfType<SelectOtdelWindow>().FirstOrDefault();
                win.DialogResult = true;
            }
        }

        #endregion

        public SelectOtdelWindowViewModel()
        {
            RepositoryOtdel repo = new RepositoryOtdel();// AllRepo.GetRepoOtdel();
            ListOtdel = new ObservableCollection<Otdel>(repo.Items);
        }
    }
}
