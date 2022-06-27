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
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class EditTablesWindowModel : ViewModel
    {
        private readonly IRepository repo;
        public ObservableCollection<Otdel> ListOtdel { get; set; }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить отдел
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            repo.AddOtdel(null);
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить отдел
        //--------------------------------------------------------------------------------
        public ICommand DeleteCommand => new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommand);
        private bool CanDeleteCommand(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            repo.DeleteOtdel(0);
        }
        #endregion



        public EditTablesWindowModel()
        {
            repo = new RepositoryMSSQL();

            ListOtdel = new ObservableCollection<Otdel>(repo.GetOtdels());
        }
    }
}
