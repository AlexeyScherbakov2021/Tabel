using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tabel.Commands;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.Admins
{
    internal class CategoryUCViewModel : ViewModel
    {
        private readonly RepositoryMSSQL<Category> repoCategory;

        public ObservableCollection<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }

        public CategoryUCViewModel()
        {
            repoCategory = new RepositoryMSSQL<Category>();

            Categories = new ObservableCollection<Category>( repoCategory.Items );
        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            int NextCat = Categories.Max(it => it.id) + 1;

            Category newCat = new Category { id = NextCat, cat_tarif = 0 };
            Categories.Add(newCat);
            repoCategory.Add(newCat, true);
        }
        //--------------------------------------------------------------------------------
        // Команда Удалить
        //--------------------------------------------------------------------------------
        public ICommand DeleteCommand => new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommand);
        private bool CanDeleteCommand(object p) => SelectedCategory != null;
        private void OnDeleteCommandExecuted(object p)
        {
            if (MessageBox.Show($"Удалить разряд {SelectedCategory.id} ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                repoCategory.Delete(SelectedCategory, true);
                Categories.Remove(SelectedCategory);
            }
        }
    #endregion

    }
}
