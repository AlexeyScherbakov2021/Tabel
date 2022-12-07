using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tabel.Commands;
using Tabel.Component.Models;
using Tabel.Component.Models.Mod;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.ViewModels.ModViewModel;

namespace Tabel.ViewModels
{
    internal class ModUCViewModel : ViewModel, IBaseUCViewModel
    {
        private readonly BaseModel db;
        private readonly RepositoryMSSQL<ModPerson> repoModPerson;
        private RepositoryMSSQL<Personal> repoPersonal;
        private readonly RepositoryMSSQL<WorkTabel> repoTabel;
        private readonly RepositoryMSSQL<Mod> repoModel;

        public PremiaBonusViewModel premiaBonusViewModel { get; set; }
        public ModMainViewModel modMainViewModel { get; set; }
        public PremiaFPViewModel premiaFPViewModel { get; set; }
        public PremiaKvalifViewModel premiaKvalifViewModel { get; set; }
        public PremiaOtdelViewModel premiaOtdelViewModel { get; set; }
        public PremiaQualityViewModel premiaQualityViewModel { get; set; }
        public PremiaAddWorksViewModel premiaAddWorksViewModel { get; set; }
        public PremiaTransportViewModel premiaTransportViewModel { get; set; }
        public PremiaPrizeViewModel premiaPrizeViewModel { get; set; }


        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        public Visibility IsVisibleITR { get; private set; }
        public Visibility IsVisibleNoITR { get; private set; }

        public decimal SetProcPrem { get; set; }

        public ObservableCollection<ModPerson> ListModPerson { get; set; }
        public ModPerson SelectedModPerson { get; set; }

        public Mod CurrentMod { get; set; }

        //-------------------------------------------------------------------------------------------------------
        // конструктор
        //-------------------------------------------------------------------------------------------------------
        public ModUCViewModel()
        {
            repoModPerson = new RepositoryMSSQL<ModPerson>();
            db = repoModPerson.GetDB();
            repoPersonal = new RepositoryMSSQL<Personal>(db);
            repoTabel = new RepositoryMSSQL<WorkTabel>(db);
            repoModel = new RepositoryMSSQL<Mod>(db);

            modMainViewModel = new ModMainViewModel(db);
            premiaBonusViewModel = new PremiaBonusViewModel(db);
            premiaFPViewModel = new PremiaFPViewModel(db);
            premiaKvalifViewModel = new PremiaKvalifViewModel(db);
            premiaOtdelViewModel = new PremiaOtdelViewModel(db);
            premiaQualityViewModel = new PremiaQualityViewModel(db);
            premiaAddWorksViewModel = new PremiaAddWorksViewModel(db);
            premiaTransportViewModel = new PremiaTransportViewModel(db);
            premiaPrizeViewModel = new PremiaPrizeViewModel(db);

            DateTime _CurrentDate = DateTime.Now;

        }

        #region Команды

        //--------------------------------------------------------------------------------
        // Событие выбора закладки
        //--------------------------------------------------------------------------------
        public ICommand TabChangedCommand => new LambdaCommand(OnTabChangedCommandExecuted, CanTabChangedCommand);
        private bool CanTabChangedCommand(object p) => true;
        private void OnTabChangedCommandExecuted(object p)
        {
            if(p is int index)
            {
                if(index == 0 && ListModPerson != null)
                {
                    foreach(var item in ListModPerson)
                        item.OnPropertyChanged(nameof(item.Itogo));
                }
            }    
        }


        //--------------------------------------------------------------------------------
        // Команда Создать 
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (CurrentMod != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Подтверждаете?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                repoModel.Remove(CurrentMod);
            }

            CurrentMod = new Mod();
            CurrentMod.m_author = App.CurrentUser.id;
            CurrentMod.m_month = _SelectMonth;
            CurrentMod.m_year = _SelectYear;
            CurrentMod.m_otdelId = _SelectedOtdel.id;
            CurrentMod.ModPersons = new List<ModPerson>();


            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>(db);// AllRepo.GetRepoAllOtdels();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            // получение списка людей для отделов и групп
            var persons = repoPersonal.Items
                .AsNoTracking()
                .Where(it => (it.p_otdel_id == _SelectedOtdel.id  || listOtdels.Contains(it.p_otdel_id.Value)) && it.p_delete == false)
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name);

            foreach (var pers in persons)
            {
                ModPerson newPerson = new ModPerson();
                newPerson.md_group = pers.p_otdel_id.ToString();
                newPerson.md_personalId = pers.id;
                newPerson.md_tarif_offDay = pers.category?.cat_tarif * 8;
                if (newPerson.md_tarif_offDay < 1500)
                    newPerson.md_tarif_offDay = 1500;

                // получение этого сотрудника из предыдущей существующей модели
                ModPerson PrevModPerson = repoModPerson.Items
                    .AsNoTracking()
                    .Where(it => it.md_personalId == newPerson.md_personalId 
                            && (
                                ( it.Mod.m_year == _SelectYear && it.Mod.m_month < _SelectMonth)
                                || it.Mod.m_year < _SelectYear 
                               ))
                    .OrderByDescending(o => o.Mod.m_year)
                    .ThenByDescending(o => o.Mod.m_month)
                    .FirstOrDefault();


                // если был предыдущий месяц, то копируем нужные тарифы
                if(PrevModPerson != null)
                {
                    // копирование тарифа бонусов
                    newPerson.md_bonus_max = PrevModPerson.md_bonus_max;
                    newPerson.md_cat_prem_tarif = PrevModPerson.md_cat_prem_tarif;
                }

                CurrentMod.ModPersons.Add(newPerson);
            }

            repoModel.Add(CurrentMod, true);

            CurrentMod = repoModel.Items
                .Where(it => it.id == CurrentMod.id)
                .Include(inc => inc.ModPersons.Select(s => s.person))
                .FirstOrDefault();

            ListModPerson = new ObservableCollection<ModPerson>(CurrentMod.ModPersons);

            modMainViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaBonusViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaFPViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaKvalifViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaOtdelViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaQualityViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaAddWorksViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaTransportViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaPrizeViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);

            //OnPropertyChanged(nameof(ListModPerson));
            //OnPropertyChanged(nameof(CurrentMod));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => CurrentMod != null && _SelectedOtdel != null;
        private void OnSaveCommandExecuted(object p)
        {
            SaveForm();
        }


        ////--------------------------------------------------------------------------------
        //// Команда Отметить выбранные за качество
        ////--------------------------------------------------------------------------------
        //public ICommand CheckQualCommand => new LambdaCommand(OnCheckQualCommandExecuted, CanCheckQualCommand);
        //private bool CanCheckQualCommand(object p) => true;
        //private void OnCheckQualCommandExecuted(object p)
        //{
        //    if( p is DataGrid dg)
        //    {
        //        foreach (ModPerson item in dg.SelectedItems)
        //        {
        //            item.md_quality_check = IsCheckQuality;
        //            item.OnPropertyChanged(nameof(item.md_quality_check));
        //        }
        //    }
        //}


        //--------------------------------------------------------------------------------
        // Команда Экспорт в 1С
        //--------------------------------------------------------------------------------
        public ICommand ExportCSVCommand => new LambdaCommand(OnExportCSVCommandExecuted, CanExportCSVCommand);
        private bool CanExportCSVCommand(object p) => App.CurrentUser.u_role == UserRoles.Admin;
        private void OnExportCSVCommandExecuted(object p)
        {

            IEnumerable<WorkTabel> ListTabel = repoTabel.Items.Where(it => it.t_year == _SelectYear
                    && it.t_month == _SelectMonth);


            if (ListTabel != null)
            {
                RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>(db);// AllRepo.GetRepoTabelPerson();
                List<TabelPerson> ListTabelPerson = new List<TabelPerson>();

                foreach (var tabel in ListTabel)
                {
                    List<TabelPerson> listPerson = repoTabelPerson.Items
                        .AsNoTracking()
                        .Where(it => it.tp_tabel_id == tabel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name).ToList();

                    ListTabelPerson.AddRange(listPerson);
                }

                ListTabelPerson.Sort((item1, item2) =>
                {
                    return item1.person.p_lastname.CompareTo(item2.person.p_lastname);
                });

                List<ModPerson> ListModAllPerson = repoModPerson.Items
                    .AsNoTracking()
                    .Where(it => it.Mod.m_year == _SelectYear && it.Mod.m_month == _SelectMonth)
                    .ToList();

                // берем часы переработки
                FormExport fomExport = new FormExport();

                fomExport.ListPersonToListExport(ListTabelPerson, ListModAllPerson);

                RepositoryCSV repoFile = new RepositoryCSV(fomExport);
                repoFile.SaveFile(_SelectYear, _SelectMonth);
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => _SelectedOtdel != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            List<Personal> ListPersonal = repoPersonal.Items
                //.AsNoTracking()
                .Where(it => it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                .ToList();

            // составляем список добавленных людей
            foreach (var item in ListModPerson)
            {
                var person = ListPersonal.FirstOrDefault(it => it.id == item.person.id);
                if (person != null)
                    ListPersonal.Remove(person);
            }

            List<ModPerson> ListNewPerson = new List<ModPerson>();

            if (ListPersonal.Count == 0)
            {
                MessageBox.Show("Новых людей для отдела не обнаружено.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (MessageBox.Show($"Найдено людей: {ListPersonal.Count}. Добавлять?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {

                foreach (var pers in ListPersonal)
                {
                    ModPerson newPerson = new ModPerson();
                    newPerson.md_group = pers.p_otdel_id.ToString();
                    newPerson.md_modId = CurrentMod.id;
                    //newPerson.md_personalId = pers.id;
                    newPerson.person = pers;
                    newPerson.md_tarif_offDay = pers.category?.cat_tarif * 8;
                    if (newPerson.md_tarif_offDay < 1500)
                        newPerson.md_tarif_offDay = 1500;

                    // получение этого сотрудника из предыдущей существующей модели
                    ModPerson PrevModPerson = repoModPerson.Items
                        .AsNoTracking()
                        .Where(it => it.md_personalId == newPerson.md_personalId
                                && (
                                    (it.Mod.m_year == _SelectYear && it.Mod.m_month < _SelectMonth)
                                    || it.Mod.m_year < _SelectYear
                                   ))
                        .OrderByDescending(o => o.Mod.m_year)
                        .ThenByDescending(o => o.Mod.m_month)
                        .FirstOrDefault();


                    // если был предыдущий месяц, то копируем нужные тарифы
                    if (PrevModPerson != null)
                    {
                        // копирование тарифа бонусов
                        newPerson.md_bonus_max = PrevModPerson.md_bonus_max;
                        newPerson.md_cat_prem_tarif = PrevModPerson.md_cat_prem_tarif;
                    }

                    repoModPerson.Add(newPerson, true);
                    ListModPerson.Add(newPerson);
                    ListNewPerson.Add(newPerson);

                }

                modMainViewModel.AddPersons(ListNewPerson);
                premiaBonusViewModel.AddPersons(ListNewPerson);
                premiaFPViewModel.AddPersons(ListNewPerson);
                premiaKvalifViewModel.AddPersons(ListNewPerson);
                premiaOtdelViewModel.AddPersons(ListNewPerson);
                premiaQualityViewModel.AddPersons(ListNewPerson);
                premiaAddWorksViewModel.AddPersons(ListNewPerson);
                premiaTransportViewModel.AddPersons(ListNewPerson);
                premiaPrizeViewModel.AddPersons(ListNewPerson);

                OnPropertyChanged(nameof(ListModPerson));
                //IsModify = true;
            }

        }




        //--------------------------------------------------------------------------------
        // Команда Удалить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand DeletePersonCommand => new LambdaCommand(OnDeletePersonCommandExecuted, CanDeletePersonCommand);
        private bool CanDeletePersonCommand(object p) => SelectedModPerson != null;
        private void OnDeletePersonCommandExecuted(object p)
        {
            if (MessageBox.Show($"Удалить {SelectedModPerson.person.FIO}?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                repoModPerson.Remove(SelectedModPerson, true);
                ListModPerson.Remove(SelectedModPerson);
                //IsModify = true;
            }
        }


        #endregion


        //-------------------------------------------------------------------------------------------------------
        // загрузка выбранных данных
        //-------------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;

             //if(_SelectedOtdel?.ot_itr == 1)
             //{
             //   IsVisibleITR = Visibility.Visible;
             //   IsVisibleNoITR = Visibility.Collapsed;
             //}
             //else
             //{
             //   IsVisibleNoITR = Visibility.Visible;
             //   IsVisibleITR = Visibility.Collapsed;
             //}

            //OnPropertyChanged(nameof(IsVisibleITR));
            //OnPropertyChanged(nameof(IsVisibleNoITR));

            if (SelectOtdel is null) return;

            ListModPerson = null;

            if (_SelectedOtdel.ot_parent is null)
            {
                CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_year == Year
                    && it.m_month == Month
                    && it.m_otdelId == _SelectedOtdel.id);
                if (CurrentMod != null)
                    ListModPerson = new ObservableCollection<ModPerson>(repoModPerson.Items
                        .Where(it => it.md_modId == CurrentMod.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else
            {
                CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_year == Year
                    && it.m_month == Month
                    && it.m_otdelId == _SelectedOtdel.ot_parent);
                if (CurrentMod != null)
                    ListModPerson = new ObservableCollection<ModPerson>(repoModPerson.Items
                        .Where(it => it.md_modId == CurrentMod.id && it.person.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }

            modMainViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaBonusViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaFPViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaKvalifViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaOtdelViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaQualityViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaAddWorksViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaTransportViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaPrizeViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);

            //OnPropertyChanged(nameof(ListModPerson));
            //OnPropertyChanged(nameof(CurrentMod));

        }

        public bool ClosingFrom()
        {
            return false;
        }

        public void SaveForm()
        {
            repoModPerson.Save();
            repoModel.Save();
        }
    }
}
