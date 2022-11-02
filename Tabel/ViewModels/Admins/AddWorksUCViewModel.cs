using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.Admins
{
    internal class AddWorksUCViewModel : ViewModel
    {
        private readonly RepositoryMSSQL<AddWorks> repoAddWorks;
        public ObservableCollection<AddWorks> ListAddWorks { get; set; }
        public AddWorks SelectedWork { get; set; }

        public AddWorksUCViewModel()
        {
            repoAddWorks = AllRepo.GetRepoAddWorks();

            ListAddWorks = new ObservableCollection<AddWorks>(repoAddWorks.Items);

        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            
            AddWorks newWork = new AddWorks { aw_Name = "Новая работа", aw_Tarif = 0 };
            ListAddWorks.Add(newWork);
            repoAddWorks.Add(newWork, true);
        }
        //--------------------------------------------------------------------------------
        // Команда Удалить
        //--------------------------------------------------------------------------------
        public ICommand DeleteCommand => new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommand);
        private bool CanDeleteCommand(object p) => SelectedWork != null;
        private void OnDeleteCommandExecuted(object p)
        {
            if (MessageBox.Show($"Удалить работу {SelectedWork.aw_Name} ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                repoAddWorks.Delete(SelectedWork, true);
                ListAddWorks.Remove(SelectedWork);
            }
        }
        #endregion

    }
}
