using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
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
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class ModUCViewModel : ViewModel, IBaseUCViewModel
    {
        private readonly BaseModel db;
        private readonly RepositoryMSSQL<ModPerson> repoModPerson;
        private RepositoryMSSQL<Personal> repoPersonal;
        private readonly RepositoryMSSQL<WorkTabel> repoTabel;
        private readonly RepositoryMSSQL<Mod> repoModel;
        private decimal? BonusProc;

        public PremiaBonusViewModel premiaBonusViewModel { get; set; }
        public ModMainViewModel modMainViewModel { get; set; }
        public PremiaFPViewModel premiaFPViewModel { get; set; }
        public PremiaKvalifViewModel premiaKvalifViewModel { get; set; }
        public PremiaOtdelViewModel premiaOtdelViewModel { get; set; }
        //public PremiaQualityViewModel premiaQualityViewModel { get; set; }
        public PremiaAddWorksViewModel premiaAddWorksViewModel { get; set; }
        public PremiaTransportViewModel premiaTransportViewModel { get; set; }
        public PremiaPrizeViewModel premiaPrizeViewModel { get; set; }
        public PremiaItogoViewModel premiaItogoViewModel { get; set; }

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;
        private bool IsModify;

        public Visibility IsVisibleITR { get; private set; }
        //public Visibility IsVisibleNoITR { get; private set; }
        public Visibility IsVisibleAdmin { get; set; } = App.CurrentUser.u_role == UserRoles.Admin ? Visibility.Visible : Visibility.Collapsed;


        public decimal SetProcPrem { get; set; }

        private ObservableCollection<ModPerson> _ListModPerson;
        public ObservableCollection<ModPerson> ListModPerson 
        { 
            get => _ListModPerson; 
            set
            {
                if(_ListModPerson == value) return;

                if(_ListModPerson != null)
                {
                    foreach(var item in _ListModPerson)
                        item.PropertyChanged -= Item_PropertyChanged;
                }

                _ListModPerson = value;
                if (_ListModPerson == null) return;

                foreach (var item in _ListModPerson)
                    item.PropertyChanged += Item_PropertyChanged;

            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "md_kvalif_tarif"
                || e.PropertyName == "md_prem_otdel"
                || e.PropertyName == "Itogo"
                || e.PropertyName == "TabelWorkOffDay"
                || e.PropertyName == "TabelHours"
                || e.PropertyName == "TabelAbsent"
                || e.PropertyName == "PremiaItogo")
                
                return;
            IsModify = true;
        }

        private ModPerson _SelectedModPerson;
        public ModPerson SelectedModPerson { get => _SelectedModPerson; set { Set(ref _SelectedModPerson, value); } }

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
            premiaBonusViewModel = new PremiaBonusViewModel(db, BonusProc);
            premiaFPViewModel = new PremiaFPViewModel(db);
            premiaKvalifViewModel = new PremiaKvalifViewModel(db);
            premiaOtdelViewModel = new PremiaOtdelViewModel(db);
            //premiaQualityViewModel = new PremiaQualityViewModel(db);
            premiaAddWorksViewModel = new PremiaAddWorksViewModel(db);
            premiaTransportViewModel = new PremiaTransportViewModel(db);
            premiaPrizeViewModel = new PremiaPrizeViewModel(db);
            premiaItogoViewModel = new PremiaItogoViewModel(db);

            DateTime _CurrentDate = DateTime.Now;

            IsModify = false;

        }

        //-------------------------------------------------------------------------------------------------------
        // загрузка процента бонуса для текущего месяца
        //-------------------------------------------------------------------------------------------------------
        private void LoadBonusProcent()
        {
            RepositoryMSSQL<GenChargMonth> repoGetAll = new RepositoryMSSQL<GenChargMonth>(db);
            BonusProc = repoGetAll.Items
                .FirstOrDefault(it => it.gm_Year == _SelectYear && it.gm_Month == _SelectMonth && it.gm_GenId == (int)EnumKind.BonusProc)?.gm_Value;

        }


        //-------------------------------------------------------------------------------------------------------
        // загрузка выбранных данных
        //-------------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {

            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;

            if (SelectOtdel is null) return;

            IsVisibleITR = SelectOtdel.ot_itr == 2 ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(IsVisibleITR));


            ListModPerson = null;

            LoadBonusProcent();

            premiaBonusViewModel.SetBonus(BonusProc);

            if (_SelectedOtdel.ot_parent is null)
            {
                CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_year == Year
                    && it.m_month == Month
                    && it.m_otdelId == _SelectedOtdel.id);
                if (CurrentMod != null)
                    ListModPerson = new ObservableCollection<ModPerson>(repoModPerson.Items
                        .Where(it => it.md_modId == CurrentMod.id)
                        //.Include(inc => inc.ListTargetTask)
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
                        //.Include(inc => inc.ListTargetTask)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }


            modMainViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaBonusViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaFPViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaKvalifViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaOtdelViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //premiaQualityViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaAddWorksViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaTransportViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaPrizeViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaItogoViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);

        }

        public bool ClosingFrom()
        {
            return IsModify;
        }

        public void SaveForm()
        {
            repoModPerson.Save();
            repoModel.Save();
            IsModify = false;
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
                    foreach (var item in ListModPerson)
                    {
                        item.OnPropertyChanged(nameof(item.Itogo));
                        item.OnPropertyChanged(nameof(item.PremiaItogo));
                    }
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

                //if (newPerson.TabelWorkOffDay > 0)
                //{
                //    newPerson.md_tarif_offDay = pers.category?.cat_tarif * 8;
                //    if (newPerson.md_tarif_offDay < 1500)
                //        newPerson.md_tarif_offDay = 1500;
                //}


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
                    newPerson.md_kvalif_proc = PrevModPerson.md_kvalif_proc;
                    newPerson.md_person_achiev = PrevModPerson.md_person_achiev;
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
            //premiaQualityViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaAddWorksViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaTransportViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaPrizeViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            premiaItogoViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => /*CurrentMod != null && _SelectedOtdel != null &&*/ IsModify;
        private void OnSaveCommandExecuted(object p)
        {
            foreach(var item in  ListModPerson)
            {
                item.md_ItogPremia1 = item.premiaBonus.Summa;
                item.md_ItogPremia2vyr = item.premiaFP.Summa;
                item.md_ItogPremiaAddWork = item.premiaAddWorks.Summa;
                item.md_ItogPremiaTransport = item.premiaTransport.Summa;
                item.md_ItogPremia2Otdel = item.premiaKvalif.Summa;
                item.md_ItogPremia3Stimul = item.premiaOtdel.Summa;
                item.md_ItogPremiaPrize = item.premiaPrize.Summa;
                item.md_ItogPremiaNight = item.premiaNight.Summa;
                item.md_ItogPremiaOffDays = item.premOffDays.Summa;
            }

            SaveForm();
        }


        //--------------------------------------------------------------------------------
        // Команда Экспорт в 1С
        //--------------------------------------------------------------------------------
        public ICommand ExportCSVCommand => new LambdaCommand(OnExportCSVCommandExecuted, CanExportCSVCommand);
        private bool CanExportCSVCommand(object p) => App.CurrentUser.u_role == UserRoles.Admin;
        private void OnExportCSVCommandExecuted(object p)
        {

            // берем часы переработки
            FormExport fomExport = new FormExport();

            //fomExport.ListPersonToListExport(ListTabelPerson, ListModAllPerson,  BonusProc);
            fomExport.ListPersonToListExport(_SelectYear, _SelectMonth, BonusProc);

            RepositoryCSV repoFile = new RepositoryCSV(fomExport);
            repoFile.SaveFile(_SelectYear, _SelectMonth);

            //return;

            //IEnumerable<WorkTabel> ListTabel = repoTabel.Items.Where(it => it.t_year == _SelectYear
            //        && it.t_month == _SelectMonth);


            //if (ListTabel != null)
            //{
            //    RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>(db);
            //    List<TabelPerson> ListTabelPerson = new List<TabelPerson>();

            //    foreach (var tabel in ListTabel)
            //    {
            //        List<TabelPerson> listPerson = repoTabelPerson.Items
            //            .AsNoTracking()
            //            .Where(it => it.tp_tabel_id == tabel.id)
            //            .OrderBy(o => o.person.p_lastname)
            //            .ThenBy(o => o.person.p_name).ToList();

            //        ListTabelPerson.AddRange(listPerson);
            //    }

            //    ListTabelPerson.Sort((item1, item2) =>
            //    {
            //        return item1.person.p_lastname.CompareTo(item2.person.p_lastname);
            //    });

            //    List<ModPerson> ListModAllPerson = repoModPerson.Items
            //        .AsNoTracking()
            //        .Where(it => it.Mod.m_year == _SelectYear && it.Mod.m_month == _SelectMonth)
            //        .ToList();

            //    // берем часы переработки
            //    FormExport fromExport = new FormExport();

            //    //fomExport.ListPersonToListExport(ListTabelPerson, ListModAllPerson,  BonusProc);
            //    fomExport.ListPersonToListExport(_SelectYear, _SelectMonth, BonusProc);

            //    RepositoryCSV repoFile = new RepositoryCSV(fomExport);
            //    repoFile.SaveFile(_SelectYear, _SelectMonth);
            //}
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
                    //newPerson.md_tarif_offDay = pers.category?.cat_tarif * 8;
                    //if (newPerson.md_tarif_offDay < 1500)
                    //    newPerson.md_tarif_offDay = 1500;

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
                //premiaQualityViewModel.AddPersons(ListNewPerson);
                premiaAddWorksViewModel.AddPersons(ListNewPerson);
                premiaTransportViewModel.AddPersons(ListNewPerson);
                premiaPrizeViewModel.AddPersons(ListNewPerson);

                OnPropertyChanged(nameof(ListModPerson));
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

        //--------------------------------------------------------------------------------
        // Команда Печать
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintCommand);
        private bool CanPrintCommand(object p) => true;
        private void OnPrintCommandExecuted(object p)
        {

            ICollection<ModPerson> ListAllModPerson = repoModPerson.Items
                .AsNoTracking()
                .Where(it => it.Mod.m_month == _SelectMonth && it.Mod.m_year == _SelectYear)
                .OrderBy(o => o.person.p_lastname)
                .ThenBy(o => o.person.p_name).ToList();


            //ListAllModPerson = ListModPerson;


            //foreach (var item in ListAllModPerson)
            //{
            //    modMainViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaBonusViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaFPViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaKvalifViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaOtdelViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    //premiaQualityViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaAddWorksViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaTransportViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaPrizeViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);
            //    premiaItogoViewModel.ChangeListPerson(ListModPerson, _SelectYear, _SelectMonth, _SelectedOtdel);

            //}

            try
            {
                using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Модель.xlsx"))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    var ws2 = wb.Worksheets.Worksheet(2);
                    var ws3 = wb.Worksheets.Worksheet(3);

                    // Заполение шапки
                    //ws.Cell("C1").Value = CurrentMod.otdel.ot_name;

                    //DateTime startDate = new DateTime(_SelectYear, CurrentMod.m_month, 1);
                    //ws.Cell("C2").Value = startDate.ToString("dd.MM.yyyy");
                    //DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                    //ws.Cell("D2").Value = endDate.ToString("dd.MM.yyyy");
                    //ws.Cell("AA15").Value = "Составил: " + App.CurrentUser.u_fio;

                    int RowNum = 6;
                    int RowNum2 = 6;
                    int RowNum3 = 6;
                    //ws.Row(5).InsertRowsBelow(ListModPerson.Count() - 1 );
                    //var range = ws.Range(RowNum, 1, RowNum, 75);

                    //for (int i = 0; i < ListModPerson.Count() - 1; i++)
                    //{
                    //    RowNum++;
                    //    range.CopyTo(ws.Cell(RowNum, 1));
                    //}
                    ws.Cell("A2").Value = "'" + App.ListMonth[_SelectMonth-1] .Name+ " " + _SelectYear;
                    ws2.Cell("A2").Value = "'" + App.ListMonth[_SelectMonth-1].Name + " " + _SelectYear;
                    ws3.Cell("A2").Value = "'" + App.ListMonth[_SelectMonth-1].Name + " " + _SelectYear;


                    foreach (var item in ListAllModPerson)
                    {
                        decimal? PremiaItogo = (item.md_ItogPremia1 ?? 0)
                            + (item.md_ItogPremia2vyr ?? 0)
                            + (item.md_ItogPremia2Otdel ?? 0)
                            + (item.md_ItogPremia3Stimul ?? 0)
                            + (item.md_ItogPremiaAddWork ?? 0)
                            + (item.md_ItogPremiaTransport ?? 0)
                            + (item.md_ItogPremiaPrize ?? 0);

                        decimal? Itogo = (PremiaItogo ?? 0) + item.md_Oklad
                            + (item.md_ItogPremiaOffDays ?? 0)
                            + (item.md_ItogPremiaNight ?? 0);


                        if (item.person.p_tab_number == "ГПХ")
                        {
                            ws3.Cell(RowNum3, 1).Value = item.person.p_tab_number;
                            ws3.Cell(RowNum3, 2).Value = item.person.otdel.parent?.ot_name ?? item.person.otdel.ot_name;
                            ws3.Cell(RowNum3, 3).Value = item.person.FIO;
                            ws3.Cell(RowNum3, 4).Value = item.person.p_profession;
                            ws3.Cell(RowNum3, 5).Value = item.md_ItogPremia1;
                            ws3.Cell(RowNum3, 6).Value = item.md_ItogPremia2vyr;
                            ws3.Cell(RowNum3, 7).Value = item.md_ItogPremia2Otdel;
                            ws3.Cell(RowNum3, 8).Value = item.md_ItogPremia3Stimul;
                            if(item.premiaAddWorks.Summa != 0)
                                ws3.Cell(RowNum3, 9).Value = item.md_ItogPremiaAddWork;
                            ws3.Cell(RowNum3, 10).Value = item.md_ItogPremiaTransport;
                            ws3.Cell(RowNum3, 11).Value = item.md_ItogPremiaPrize;
                            ws3.Cell(RowNum3, 12).Value = PremiaItogo;
                            ws3.Cell(RowNum3, 13).Value = Itogo;
                            ws3.Row(RowNum3).InsertRowsBelow(1);
                            RowNum3++;
                        }

                        else if(!string.IsNullOrEmpty(item.person.p_tab_number))
                        {
                            ws.Cell(RowNum, 1).Value = item.person.p_tab_number;
                            ws.Cell(RowNum, 2).Value = item.person.otdel.parent?.ot_name ?? item.person.otdel.ot_name;
                            ws.Cell(RowNum, 3).Value = item.person.FIO;
                            ws.Cell(RowNum, 4).Value = item.person.p_profession;
                            ws.Cell(RowNum, 5).Value = item.md_ItogPremia1;
                            ws.Cell(RowNum, 6).Value = item.md_ItogPremia2vyr;
                            ws.Cell(RowNum, 7).Value = item.md_ItogPremia2Otdel;
                            ws.Cell(RowNum, 8).Value = item.md_ItogPremia3Stimul;
                            if (item.premiaAddWorks.Summa != 0)
                                ws.Cell(RowNum, 9).Value = item.md_ItogPremiaAddWork;
                            ws.Cell(RowNum, 10).Value = item.md_ItogPremiaTransport;
                            ws.Cell(RowNum, 11).Value = item.md_ItogPremiaPrize;
                            ws.Cell(RowNum, 12).Value = PremiaItogo;
                            ws.Cell(RowNum, 13).Value = Itogo;
                            ws.Row(RowNum).InsertRowsBelow(1);
                            RowNum++;
                        }

                        ws2.Cell(RowNum2, 1).Value = item.person.p_tab_number;
                        ws2.Cell(RowNum2, 2).Value = item.person.otdel.parent?.ot_name ?? item.person.otdel.ot_name;
                        ws2.Cell(RowNum2, 3).Value = item.person.FIO;
                        ws2.Cell(RowNum2, 4).Value = item.person.p_profession;
                        ws2.Cell(RowNum2, 5).Value = item.md_ItogPremia1;
                        ws2.Cell(RowNum2, 6).Value = item.md_ItogPremia2vyr;
                        ws2.Cell(RowNum2, 7).Value = item.md_ItogPremia2Otdel;
                        ws2.Cell(RowNum2, 8).Value = item.md_ItogPremia3Stimul;
                        if (item.premiaAddWorks.Summa != 0)
                            ws2.Cell(RowNum2, 9).Value = item.md_ItogPremiaAddWork;
                        ws2.Cell(RowNum2, 10).Value = item.md_ItogPremiaTransport;
                        ws2.Cell(RowNum2, 11).Value = item.md_ItogPremiaPrize;
                        ws2.Cell(RowNum2, 12).Value = PremiaItogo;
                        ws2.Cell(RowNum2, 13).Value = Itogo;
                        ws2.Row(RowNum2).InsertRowsBelow(1);
                        RowNum2++;
                    }

                    string TempFile = System.IO.Path.GetTempFileName();
                    TempFile = System.IO.Path.ChangeExtension(TempFile, "xlsx");
                    wb.SaveAs(TempFile);
                    Process.Start(TempFile);
                }
            }
            catch
            {
                MessageBox.Show("Не найден шаблон модели", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Печать расчетки
        //--------------------------------------------------------------------------------
        public ICommand PrintPersonCommand => new LambdaCommand(OnPrintPersonCommandExecuted, CanPrintPersonCommand);
        private bool CanPrintPersonCommand(object p) => SelectedModPerson != null;
        private void OnPrintPersonCommandExecuted(object p)
        {
            try
            {
                using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Расчетка.xlsx"))
                {

                    //int NumPP = 1;
                    var ws = wb.Worksheets.Worksheet(1);

                    // Заполение шапки
                    DateTime Date = new DateTime(_SelectYear, CurrentMod.m_month, 1);
                    ws.Cell("B2").Value = "`" + Date.ToString("Y");
                    ws.Cell("A3").Value = SelectedModPerson.person.FIO + " (" + SelectedModPerson.person.p_tab_number + ")";
                    ws.Cell("B4").Value = CurrentMod.otdel.ot_name;
                    ws.Cell("B5").Value = SelectedModPerson.person.p_profession;

                    ws.Cell("B9").Value = SelectedModPerson.TabelDays;
                    ws.Cell("C9").Value = SelectedModPerson.TabelHours;
                    
                    ws.Cell("D9").Value = SelectedModPerson.Itogo;
                    ws.Cell("D10").Value = SelectedModPerson.md_Oklad;
                    ws.Cell("D11").Value = SelectedModPerson.PremiaItogo;
                    
                    ws.Cell("B6").Value = SelectedModPerson.person.category.cat_tarif;

                    int curRow = 12;

                    if (SelectedModPerson.premiaBonus.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "Премия (бонус)";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaBonus.Summa;
                    }

                    if (SelectedModPerson.premiaFP.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "Премия (по выработке)";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaFP.Summa;
                    }

                    if (SelectedModPerson.premiaAddWorks.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "Доплата (доп.раб.)";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaAddWorks.Summa;
                    }

                    if (SelectedModPerson.premiaTransport.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "компенс. за доставку";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaTransport.Summa;
                    }

                    if (SelectedModPerson.premiaKvalif.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "Премия (по отделу)";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaKvalif.Summa;
                    }

                    if (SelectedModPerson.premiaOtdel.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "Премия (стимул.)";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaOtdel.Summa;
                    }

                    if (SelectedModPerson.premiaPrize.Summa > 0)
                    {
                        ws.Row(curRow).InsertRowsBelow(1);
                        ws.Cell(curRow, 1).Value = "Премия (добр.труд)";
                        ws.Cell(curRow++, 4).Value = SelectedModPerson.premiaPrize.Summa;
                    }


                    //DateTime startDate = new DateTime(_SelectYear, CurrentMod.m_month, 1);
                    //ws.Cell("C2").Value = startDate.ToString("dd.MM.yyyy");
                    //DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                    //ws.Cell("D2").Value = endDate.ToString("dd.MM.yyyy");
                    //ws.Cell("AA15").Value = "Составил: " + App.CurrentUser.u_fio;

                    //int RowNum = 5;
                    //ws.Row(5).InsertRowsBelow(ListModPerson.Count() - 1 );
                    //var range = ws.Range(RowNum, 1, RowNum, 75);

                    //for (int i = 0; i < ListModPerson.Count() - 1; i++)
                    //{
                    //    RowNum++;
                    //    range.CopyTo(ws.Cell(RowNum, 1));
                    //}

                    //RowNum = 5;
                    //foreach (var item in ListModPerson)
                    //{
                    //    ws.Cell(RowNum, 1).Value = item.person.p_tab_number;
                    //    ws.Cell(RowNum, 2).Value = item.person.FIO;
                    //    ws.Cell(RowNum, 3).Value = item.person.p_profession;
                    //    ws.Cell(RowNum, 4).Value = item.premiaBonus.Summa;
                    //    ws.Cell(RowNum, 5).Value = item.premiaFP.Summa;
                    //    ws.Cell(RowNum, 6).Value = item.premiaKvalif.Summa;
                    //    ws.Cell(RowNum, 7).Value = item.premiaOtdel.Summa;
                    //    ws.Cell(RowNum, 8).Value = item.premiaQuality.Summa;
                    //    ws.Cell(RowNum, 9).Value = item.premiaAddWorks.Summa;
                    //    ws.Cell(RowNum, 10).Value = item.premiaTransport.Summa;
                    //    ws.Cell(RowNum, 11).Value = item.premiaPrize.Summa;
                    //    ws.Cell(RowNum, 12).Value = item.PremiaItogo;
                    //    ws.Cell(RowNum, 13).Value = item.Itogo;
                    //    RowNum++;
                    //}

                    string TempFile = System.IO.Path.GetTempFileName();
                    TempFile = System.IO.Path.ChangeExtension(TempFile, "xlsx");
                    wb.SaveAs(TempFile);
                    Process.Start(TempFile);
                }
            }
            catch
            {
                MessageBox.Show("Не найден шаблон модели", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //--------------------------------------------------------------------------------
        // Команда кнопки Список Премии по отделу
        //--------------------------------------------------------------------------------
        public ICommand BtnPremiaOtdelCommand => new LambdaCommand(OnBtnPremiaOtdelCommandExecuted, CanBtnPremiaOtdelCommand);
        private bool CanBtnPremiaOtdelCommand(object p) => SelectedModPerson != null;
        private void OnBtnPremiaOtdelCommandExecuted(object p)
        {

            TasksPersonWindow win = new TasksPersonWindow();
            TasksPersonWindowViewModel vm = new TasksPersonWindowViewModel(SelectedModPerson);
            win.DataContext = vm;

            FrameworkElement elem = p as FrameworkElement;
            Point pt = Mouse.GetPosition(elem);
            Point pt2 = elem.PointToScreen(pt);
            win.Left = pt2.X - win.Width;
            win.Top = pt2.Y - win.Height + 250;
            if (win.Top < 0) win.Top = 0;
            else if (win.Top + win.Height > SystemParameters.PrimaryScreenHeight)
                win.Top -= (win.Top + win.Height) - SystemParameters.PrimaryScreenHeight;

            if (win.ShowDialog() == true)
            {
                SelectedModPerson.ListTargetTask = vm.ListTarget;
                decimal summa = 0;
                string name = "";
                foreach (var item in vm.ListTarget)
                {
                    summa += item.tt_proc_task;
                    name += item.tt_name.Length > 15 ? item.tt_name.Substring(0, 15) + "...; " : item.tt_name + ";";
                }
                SelectedModPerson.md_kvalif_proc = summa;
                SelectedModPerson.md_kvalif_name = name;
            }
        }

        #endregion


    }
}
