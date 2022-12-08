using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.Admins
{
    internal class PersonUCViewModel : ViewModel
    {
        private readonly BaseModel db;

        private bool IsModify;

        private readonly RepositoryMSSQL<Personal> repoPerson;

        public Personal SelectedPerson { get; set; }

        private List<Personal> _ListPersonal;
        public List<Personal> ListPersonal 
        {
            get => _ListPersonal;
            set
            {
                if (_ListPersonal == value) return;

                if (_ListPersonal != null)
                {
                    foreach (var item in _ListPersonal)
                        item.PropertyChanged -= Item_PropertyChanged;
                }

                _ListPersonal = value;

                _listPersonViewSource = new CollectionViewSource();
                _listPersonViewSource.Source = value;
                _listPersonViewSource.View.Refresh();
                _listPersonViewSource.Filter += _ListView_Filter;

                foreach (var item in _ListPersonal)
                    item.PropertyChanged += Item_PropertyChanged;

            }
        }

        CollectionViewSource _listPersonViewSource;
        public ICollectionView ListPersonView => _listPersonViewSource?.View;

        private string _FilterName = "";
        public string FilterName { get => _FilterName; set { if (Set(ref _FilterName, value)) { _listPersonViewSource.View.Refresh(); } } }




        // Отделы -----------------------------------------------------
        private readonly RepositoryOtdel repoOtdel;
        public List<Otdel> ListOtdel { get; set; }

        // Разряды ----------------------------------------------------
        private readonly RepositoryMSSQL<Category> repoCat;
        public List<Category> ListCategory { get; set; } 

        public PersonUCViewModel()
        {
            repoPerson = new RepositoryMSSQL<Personal>();
            ListPersonal = repoPerson.Items
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name)
                .ToList();

            db = repoPerson.GetDB();

            repoOtdel = new RepositoryOtdel(db);
            ListOtdel = repoOtdel.Items.ToList();

            repoCat = new RepositoryMSSQL<Category>(db);
            ListCategory = repoCat.Items.OrderBy(o => o.id).ToList();

            IsModify = false;
        }


        //--------------------------------------------------------------------------------
        // Событие изменениея тарифа или разряда
        //--------------------------------------------------------------------------------
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "p_cat_id" || e.PropertyName == "p_premTarif")
            {
                Category cat = repoCat.Items.AsNoTracking().FirstOrDefault(it => it.id == SelectedPerson.p_cat_id);
                if (cat != null)
                {
                    if (SelectedPerson.p_premTarif > cat.cat_max_level)
                        SelectedPerson.p_premTarif = cat.cat_max_level;

                    if (SelectedPerson.p_premTarif < cat.cat_min_level)
                        SelectedPerson.p_premTarif = cat.cat_min_level;
                }
            }

            IsModify = true;

        }


        //--------------------------------------------------------------------------------------
        // функция фильтрации 
        //--------------------------------------------------------------------------------------
        private void _ListView_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterName))
                return;

            string LastName = (e.Item as Personal).p_lastname;

            if (LastName == null || !LastName.ToLower().StartsWith(FilterName.ToLower()))
                e.Accepted = false;
        }



        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => IsModify;
        private void OnSaveCommandExecuted(object p)
        {
            repoPerson.Save();
        }

    #endregion

    }
}
