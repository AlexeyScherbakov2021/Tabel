using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.Admins
{
    internal class OtdelsWindowViewModel : ViewModel
    {
        private readonly BaseModel db;
        // Отделы -----------------------------------------------------

        private readonly RepositoryOtdel repoOtdel;
        // Список всех отделов
        public ObservableCollection<Otdel> ListOtdel { get; set; }

        public Visibility VisibleAdmin => App.CurrentUser.u_role == Infrastructure.UserRoles.Admin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility VisibleMain => 
            App.CurrentUser.u_role == Infrastructure.UserRoles.Управление || App.CurrentUser.u_role == Infrastructure.UserRoles.Admin
            ? Visibility.Visible 
            : Visibility.Collapsed;


        // выбранный отдел
        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel 
        { 
            get => _SelectedOtdel;
            set 
            {
                if (Set(ref _SelectedOtdel, value))
                {
                    _SelectedOtdel = value;
                    LoadListPerson(_SelectedOtdel.id);

                    //var result = repoPerson.Items
                    //    .Where(it => it.p_otdel_id == _SelectedOtdel.id)
                    //    .OrderBy(o => o.p_lastname)
                    //    .ThenBy(o => o.p_name)
                    //    .ToArrayAsync();
                    //ListPersonal = new ObservableCollection<Personal>(result.Result);
                }
            } 
        }

        private void LoadListPerson(int OtdelId)
        {

            IEnumerable<Category> ListCat = repoCategorySet.Items
                    .AsNoTracking()
                    .OrderByDescending(it => it.cg_date)
                    .FirstOrDefault()
                    .ListCategory;


            var result = repoPerson.Items
                .Where(it => it.p_otdel_id == OtdelId)
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name)
                .ToArrayAsync();
            ListPersonal = new ObservableCollection<Personal>(result.Result);

            foreach(var item in ListPersonal)
                item.category = ListCat.FirstOrDefault(it => it.idCategory == item.p_cat_id);

        }


        // Разряды ----------------------------------------------------
//        private readonly RepositoryMSSQL<CategorySet> repoCatSet;
        private readonly RepositoryMSSQL<CategorySet> repoCategorySet;


        public CategorySet CategorySet { get; set; } 


        // Персонал отдела --------------------------------------------

        private readonly RepositoryMSSQL<Personal> repoPerson;

        public Personal SelectedPerson { get; set; }

        private ObservableCollection<Personal> _ListPersonal;
        public ObservableCollection<Personal> ListPersonal 
        { 
            get => _ListPersonal; 
            set 
            {
                if (_ListPersonal == value) return;

                if(_ListPersonal != null)
                {
                    foreach (var item in _ListPersonal)
                        item.PropertyChanged -= Item_PropertyChanged;
                }

                _ListPersonal = value;

                if(_ListCollectionPerson.View != null)
                    _ListCollectionPerson.View.CurrentChanged -= ListPersonalView_CurrentChanged;
                _ListCollectionPerson.Source = value;
                _ListCollectionPerson.View.CurrentChanged += ListPersonalView_CurrentChanged;
                _ListCollectionPerson.View.Refresh();
                OnPropertyChanged(nameof(ListPersonalView));

                foreach(var item in _ListPersonal)
                    item.PropertyChanged += Item_PropertyChanged;

                CategorySet = repoCategorySet.Items
                    .OrderByDescending(o => o.cg_date)
                    .FirstOrDefault();

                //ListCategory = repoCat.Items.AsNoTracking().OrderBy(o => o.idCategory).ToList();

                //OnPropertyChanged(nameof(ListPersonal));
            } 
        }



        private CollectionViewSource _ListCollectionPerson = new CollectionViewSource();
        public ICollectionView ListPersonalView => _ListCollectionPerson?.View;


        //--------------------------------------------------------------------------------
        // конструктор
        //--------------------------------------------------------------------------------
        public OtdelsWindowViewModel()
        {
            repoOtdel = new RepositoryOtdel();
            db = repoOtdel.GetDB();

            repoPerson = new RepositoryMSSQL<Personal>(db);
            //repoCatSet = new RepositoryMSSQL<CategorySet>(db);
            repoCategorySet = new RepositoryMSSQL<CategorySet>(db);

            int level = App.CurrentUser.u_role == UserRoles.Внетарифный ? 10 : 2;


            if (App.CurrentUser.u_role == UserRoles.Admin)
            {
                ListOtdel = new ObservableCollection<Otdel>(repoOtdel.Items
                    .Where(it => it.ot_parent == null && it.ot_itr <= level)
                    .OrderBy(o => o.ot_sort));
            }
            else
            {
                ListOtdel = new ObservableCollection<Otdel>(repoOtdel.GetTreeOtdels(App.CurrentUser.otdels, level));
            }
            SelectedOtdel = ListOtdel.Count > 0 ?  ListOtdel[0] : null;

        }

        //--------------------------------------------------------------------------------
        // Событие изменениея тарифа или разряда
        //--------------------------------------------------------------------------------
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "p_cat_id" || e.PropertyName == "p_premTarif")
            {
                Category cat = CategorySet.ListCategory.FirstOrDefault(it => it.idCategory == SelectedPerson.p_cat_id);

                //repoCat.Items
                //.OrderByDescending(o => o.idCategory)
                //.FirstOrDefault(it => it.idCategory == SelectedPerson.p_cat_id);

                if (cat != null)
                {
                    //SelectedPerson.category = cat;
                    SelectedPerson.p_cat_id = cat.idCategory;

                    if (SelectedPerson.p_premTarif > cat.cat_max_level)
                        SelectedPerson.p_premTarif = cat.cat_max_level;

                    if (SelectedPerson.p_premTarif < cat.cat_min_level)
                        SelectedPerson.p_premTarif = cat.cat_min_level;

                    SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.MidOklad));
                    SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.MidPrem));
                    SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.MidItog));
                    SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.MidNdfl));

                }
            }
        }


        //--------------------------------------------------------------------------------
        // Запись в базу
        //--------------------------------------------------------------------------------
        private void ListPersonalView_CurrentChanged(object sender, EventArgs e)
        {
            repoPerson.Save();
        }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить отдел
        //--------------------------------------------------------------------------------
        public ICommand AddOtdelCommand => new LambdaCommand(OnAddOtdelCommandExecuted, CanAddOtdelCommand);
        private bool CanAddOtdelCommand(object p) => true;
        private void OnAddOtdelCommandExecuted(object p)
        {
            Otdel NewOtdel = new Otdel();
            NewOtdel.ot_name = "Новый отдел";
            //NewOtdel.ot_parent = SelectedOtdel.id;
            repoOtdel.Add(NewOtdel, true);
            ListOtdel.Add(NewOtdel);

        }

        //--------------------------------------------------------------------------------
        // Команда Добавить группу
        //--------------------------------------------------------------------------------
        public ICommand AddGroupCommand => new LambdaCommand(OnAddGroupCommandExecuted, CanAddGroupCommand);
        private bool CanAddGroupCommand(object p) => SelectedOtdel != null;
        private void OnAddGroupCommandExecuted(object p)
        {
            Otdel NewOtdel = new Otdel();
            NewOtdel.ot_name = "Новая группа";
            NewOtdel.ot_parent = SelectedOtdel.ot_parent ?? SelectedOtdel.id;

            repoOtdel.Add(NewOtdel, true);

            SelectedOtdel.OnPropertyChanged(nameof(SelectedOtdel.subOtdels));
            OnPropertyChanged(nameof(SelectedOtdel));
            OnPropertyChanged(nameof(ListOtdel));
        }

        //--------------------------------------------------------------------------------
        // Команда потеря фокуса
        //--------------------------------------------------------------------------------
        public ICommand LostFocusCommand => new LambdaCommand(OnLostFocusCommandExecuted, CanLostFocusCommand);
        private bool CanLostFocusCommand(object p) => SelectedOtdel != null;
        private void OnLostFocusCommandExecuted(object p)
        {
            repoOtdel.Save();
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить отдел
        //--------------------------------------------------------------------------------
        public ICommand DeleteOtdelCommand => new LambdaCommand(OnDeleteOtdelCommandExecuted, CanDeleteOtdelCommand);
        private bool CanDeleteOtdelCommand(object p) =>
            SelectedOtdel != null // если выбран отдел
            && SelectedOtdel.subOtdels.Count == 0
            && (SelectedOtdel.ot_parent != null || App.CurrentUser.u_role == Infrastructure.UserRoles.Admin); 
        private void OnDeleteOtdelCommandExecuted(object p)
        {
            if(MessageBox.Show($"Удалить «{SelectedOtdel.ot_name}»","Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ObservableCollection<Otdel> list = SelectedOtdel.ot_parent is null
                    ? ListOtdel
                    : SelectedOtdel.parent.subOtdels;

                try
                {
                    int id = SelectedOtdel.id;
                    list.Remove(SelectedOtdel);
                    repoOtdel.Delete(id, true);
                    //repoOtdel.Save();
                }
                catch
                {
                   
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => SelectedOtdel != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            Personal NewPerson = new Personal();
            NewPerson.p_lastname = "Новый сотрудник";
            NewPerson.p_otdel_id = SelectedOtdel.id;
            NewPerson.p_delete = false;
            NewPerson.p_stavka = 1;

            NewPerson.p_cat_id = CategorySet.ListCategory.FirstOrDefault().idCategory; // repoCat.Items.FirstOrDefault().id;
            NewPerson.p_premTarif = 0;

            repoPerson.Add(NewPerson, true);
            ListPersonal.Add(NewPerson);
            NewPerson.PropertyChanged += Item_PropertyChanged;

            ListPersonalView.MoveCurrentToLast();

            DataGrid dgrid = p as DataGrid;
            FocusManager.SetFocusedElement(dgrid, dgrid);
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand DeletePersonCommand => new LambdaCommand(OnDeletePersonCommandExecuted, CanDeletePersonCommand);
        private bool CanDeletePersonCommand(object p) => SelectedOtdel != null;
        private void OnDeletePersonCommandExecuted(object p)
        {
            if(MessageBox.Show($"Удалить «{SelectedPerson.p_lastname} {SelectedPerson.p_name} {SelectedPerson.p_midname}»","Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<string> ListUsedPerson = new List<string>();

                RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
                var presTabel = repoTabelPerson.Items.AsNoTracking().Where(it => it.tp_person_id == SelectedPerson.id);
                foreach (var item in presTabel)
                    ListUsedPerson.Add("Табель " + App.ListMonth[item.tabel.t_month - 1].Name + " " + item.tabel.t_year.ToString() + "\n");

                RepositoryMSSQL<ModPerson> repoModPerson = new RepositoryMSSQL<ModPerson>();
                var presMod = repoModPerson.Items.AsNoTracking().Where(it => it.md_personalId == SelectedPerson.id);
                foreach (var item in presMod)
                    ListUsedPerson.Add("Модель " + App.ListMonth[item.Mod.m_month - 1].Name + " " + item.Mod.m_year.ToString() + "\n");

                RepositoryMSSQL<SmenaPerson> repoSmenaPerson = new RepositoryMSSQL<SmenaPerson>();
                var presSmena = repoSmenaPerson.Items.AsNoTracking().Where(it => it.sp_PersonId == SelectedPerson.id);
                foreach (var item in presSmena)
                    ListUsedPerson.Add("График смен " + App.ListMonth[item.smena.sm_Month - 1].Name + " " + item.smena.sm_Year.ToString() + "\n");

                RepositoryMSSQL<TransPerson> repoTransPerson = new RepositoryMSSQL<TransPerson>();
                var presTrans = repoTransPerson.Items.AsNoTracking().Where(it => it.tp_PersonId == SelectedPerson.id);
                foreach (var item in presTrans)
                    ListUsedPerson.Add("Транспорт " + App.ListMonth[item.Transport.tr_Month - 1].Name + " " + item.Transport.tr_Year.ToString() + "\n");

                if (ListUsedPerson.Count() > 0)
                {
                    string message = "Удалить невозможно. Сотрудник задействован в \n";
                    for(int i = 0; i < ListUsedPerson.Count && i < 14; i++)
                        message += ListUsedPerson[i];

                    MessageBox.Show(message, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                repoPerson.Delete(SelectedPerson, true);
                SelectedPerson.PropertyChanged -= Item_PropertyChanged;
                ListPersonal.Remove(SelectedPerson);
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoOtdel.Save();
            repoPerson.Save();
        }



        //--------------------------------------------------------------------------------
        // Команда Переместить сорудника
        //--------------------------------------------------------------------------------
        public ICommand DropToOtdelCommand => new LambdaCommand(OnDropToOtdelPersonCommandExecuted, CanDropToOtdelPersonCommand);
        private bool CanDropToOtdelPersonCommand(object p) => true;
        private void OnDropToOtdelPersonCommandExecuted(object p)
        {
            DragEventArgs args = p as DragEventArgs;
            Personal person = args.Data.GetData("Person") as Personal;

            //return;

            // TODO Сделать перенос с переносом в формах

            if (person != null)
            {
                if (args.OriginalSource is TextBlock tb)
                {
                    string NewOtdelName = tb.Text;
                    RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
                    var NewOtdel = repoOtdel.Items.FirstOrDefault(it => it.ot_name == NewOtdelName);

                    if (NewOtdel != null)
                    {
                        if (MessageBox.Show($"Переместить «{person.FIO}» в отдел «{NewOtdel.ot_name}»", "Предупреждение",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            // перенос в табеле
                            // перенос в графике смен
                            // перенос в графике отпусков
                            // перенос в транспорте


                            //person.p_otdel_id = NewOtdel.id;
                            //ListPersonal.Remove(person);
                            OnPropertyChanged(nameof(ListPersonal));
                        }
                    }
                }
            }
        }


        //public void DropToOtdel(object sender, DragEventArgs e)
        //{

        //}
        #endregion

    }
}
