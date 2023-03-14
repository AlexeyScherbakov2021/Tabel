﻿using System;
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
    internal class CategoryUCViewModel : ViewModel
    {
        //private readonly RepositoryMSSQL<Category> repoCategory;
        private readonly RepositoryMSSQL<CategorySet> repoCatSet;

        //public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<CategorySet> ListCatSet { get; set; }

        private CategorySet _SelectedCatSet;
        public CategorySet SelectedCatSet { get => _SelectedCatSet; set { Set(ref _SelectedCatSet, value); } }

        public DateTime SelectedDate { get; set; }

        private DateTime _StartDate;
        public DateTime StartDate { get => _StartDate; set { Set(ref _StartDate, value); } }

        public decimal Procent { get; set; }

        public Category SelectedCategory { get; set; }

        public CategoryUCViewModel()
        {
            //repoCategory = new RepositoryMSSQL<Category>();
            //Categories = new ObservableCollection<Category>(repoCategory.Items);
            repoCatSet = new RepositoryMSSQL<CategorySet>();
            ListCatSet = new ObservableCollection<CategorySet>(repoCatSet.Items);

            StartDate = ListCatSet.Max(it => it.cg_date.Value).AddDays(1);
            SelectedDate = StartDate > DateTime.Now ? StartDate : DateTime.Now;
            SelectedCatSet = ListCatSet.Last();
        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить 
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            int NextCat = SelectedCatSet.ListCategory.Max(it => it.id) + 1;

            Category newCat = new Category { id = NextCat, cat_tarif = 0 };
            SelectedCatSet.ListCategory.Add(newCat);
            //Categories.Add(newCat);
            //repoCategory.Add(newCat, true);
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
                SelectedCatSet.ListCategory.Remove(SelectedCategory);
                //repoCategory.Delete(SelectedCategory, true);
                //Categories.Remove(SelectedCategory);
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Сохрнаить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) =>true;
        private void OnSaveCommandExecuted(object p)
        {
            repoCatSet.Save();
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить индексацию
        //--------------------------------------------------------------------------------
        public ICommand AddIndexCommand => new LambdaCommand(OnAddIndexCommandExecuted, CanAddIndexCommand);
        private bool CanAddIndexCommand(object p) => true;
        private void OnAddIndexCommandExecuted(object p)
        {
            CategorySet NewCatSet = new CategorySet()
            {
                //cg_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                cg_date = SelectedDate,
                cg_value = Procent
            };

            CategorySet LastSet = ListCatSet.Last();

            foreach (var item in LastSet.ListCategory)
            {
                var cat = new Category();
                cat.idCategory = item.idCategory;
                //cat.categorySet = NewCatSet;
                cat.cat_tarif = item.cat_tarif + item.cat_tarif * Procent / 100;
                cat.cat_max_level= item.cat_max_level;
                cat.cat_min_level = item.cat_min_level;
                NewCatSet.ListCategory.Add(cat);
            }

            repoCatSet.Add(NewCatSet);
            ListCatSet.Add(NewCatSet);
            SelectedCatSet = NewCatSet;
            StartDate = SelectedDate.AddDays(1);
            SelectedDate = StartDate;
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить индексацию
        //--------------------------------------------------------------------------------
        public ICommand DelIndexCommand => new LambdaCommand(OnDelIndexCommandExecuted, CanDelIndexCommand);
        private bool CanDelIndexCommand(object p) => ListCatSet.Count > 1;
        private void OnDelIndexCommandExecuted(object p)
        {
            //if (ListCatSet.Count < 2) return;

            repoCatSet.Remove(ListCatSet.Last());
            ListCatSet.Remove(ListCatSet.Last());
            SelectedCatSet = ListCatSet.Last();
            SelectedDate = SelectedCatSet.cg_date.Value.AddDays(1);

        }

        #endregion

    }
}
