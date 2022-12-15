using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.ViewModels.ModViewModel;
using Tabel.Component.MonthPanel;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Tabel.ViewModels
{
    internal class SeparUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private readonly BaseModel db;
        private readonly RepositoryMSSQL<SeparPerson> repoSepPerson;
        //private RepositoryMSSQL<Personal> repoPersonal;
        //private readonly RepositoryMSSQL<WorkTabel> repoTabel;
        private readonly RepositoryMSSQL<Separate> repoSep;

        private bool IsModify;

        public Separate CurrentSeparate { get; set; }
        public ObservableCollection<SeparPerson> ListSeparPerson { get; set; }
        public SeparPerson SelectedPerson { get; set; }



        //-------------------------------------------------------------------------------------------------------
        // конструктор
        //-------------------------------------------------------------------------------------------------------
        public SeparUCViewModel()
        {
            repoSepPerson = new RepositoryMSSQL<SeparPerson>();
            db = repoSepPerson.GetDB();
            //repoPersonal = new RepositoryMSSQL<Personal>(db);
            //repoTabel = new RepositoryMSSQL<WorkTabel>(db);
            repoSep = new RepositoryMSSQL<Separate>(db);

            IsModify = false;
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

            ListSeparPerson = null;

            if (_SelectedOtdel.ot_parent is null)
            {
                CurrentSeparate = repoSep.Items.FirstOrDefault(it => it.s_year == Year
                    && it.s_month == Month
                    && it.s_otdelId == _SelectedOtdel.id);
                if (CurrentSeparate != null)
                    ListSeparPerson = new ObservableCollection<SeparPerson>(repoSepPerson.Items
                        .Where(it => it.sp_separId == CurrentSeparate.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else
            {
                CurrentSeparate = repoSep.Items.FirstOrDefault(it => it.s_year == Year
                    && it.s_month == Month
                    && it.s_otdelId == _SelectedOtdel.ot_parent);
                if (CurrentSeparate != null)
                    ListSeparPerson = new ObservableCollection<SeparPerson>(repoSepPerson.Items
                        .Where(it => it.sp_separId == CurrentSeparate.id && it.person.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }

            LoadFromTabel(ListSeparPerson);

            OnPropertyChanged(nameof(ListSeparPerson));
        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из табеля
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTabel(ICollection<SeparPerson> listPerson)
        {
            if (listPerson is null)
                return;

            RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>(db);
            WorkTabel tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == _SelectYear
                && it.t_month == _SelectMonth
                && it.t_otdel_id == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));

            if (listPerson is null || tabel is null) return;

            // получение количества рабочих дней в указанном месяце
            RepositoryCalendar repoCal = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
            var listDays = repoCal.GetListDays(_SelectYear, _SelectMonth);
            int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);

            foreach (var item in listPerson)
            {
                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.sp_personalId);
                if (pers != null)
                {
                    item.TabelDays = listDays.Count;
                    item.TabelHours = pers.HoursMonth;
                    //item.TabelWorkOffDay = pers.WorkedOffDays;
                    //if (item.TabelWorkOffDay > 0)
                    //{
                    //    item.md_tarif_offDay = pers.person.category?.cat_tarif * 8;
                    //    if (item.md_tarif_offDay < 1500)
                    //        item.md_tarif_offDay = 1500;
                    //}

                    //item.OverHours = pers.OverWork ?? 0;
                    item.Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value * item.person.p_stavka;

                    //int CountWorkDaysPerson = pers.TabelDays.Count(it => it.td_KindId == 1);
                    //item.TabelAbsent = CountWorkDays - CountWorkDaysPerson;
                    //if (item.TabelAbsent < 0) item.TabelAbsent = 0;
                }
            }

        }




        public bool ClosingFrom()
        {
            return IsModify;
        }

        public void SaveForm()
        {
            repoSepPerson.Save();
            repoSep.Save();
            IsModify = false;
        }

        #region Команды

        //--------------------------------------------------------------------------------
        // Событие выбора закладки
        //--------------------------------------------------------------------------------
        //public ICommand TabChangedCommand => new LambdaCommand(OnTabChangedCommandExecuted, CanTabChangedCommand);
        //private bool CanTabChangedCommand(object p) => true;
        //private void OnTabChangedCommandExecuted(object p)
        //{
        //    if (p is int index)
        //    {
        //        if (index == 0 && ListSeparPerson != null)
        //        {
        //            foreach (var item in ListSeparPerson)
        //            {
        //                item.OnPropertyChanged(nameof(item.Itogo));
        //                item.OnPropertyChanged(nameof(item.PremiaItogo));
        //            }
        //        }
        //    }
        //}


        //--------------------------------------------------------------------------------
        // Команда Создать 
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (CurrentSeparate != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Подтверждаете?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                repoSep.Remove(CurrentSeparate);
            }

            CurrentSeparate = new Separate();
            //CurrentSeparate.m_author = App.CurrentUser.id;
            CurrentSeparate.s_month = _SelectMonth;
            CurrentSeparate.s_year = _SelectYear;
            CurrentSeparate.s_otdelId = _SelectedOtdel.id;
            CurrentSeparate.ListSeparPerson = new List<SeparPerson>();

            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>(db);
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>(db);

            // получение списка людей для отделов и групп
            var persons = repoPersonal.Items
                .AsNoTracking()
                .Where(it => (it.p_otdel_id == _SelectedOtdel.id || listOtdels.Contains(it.p_otdel_id.Value)) && it.p_delete == false)
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name);

            foreach (var pers in persons)
            {
                SeparPerson newPerson = new SeparPerson();
                newPerson.sp_personalId = pers.id;
                newPerson.sp_separId = CurrentSeparate.id;

                //if (newPerson.TabelWorkOffDay > 0)
                //{
                //    newPerson.md_tarif_offDay = pers.category?.cat_tarif * 8;
                //    if (newPerson.md_tarif_offDay < 1500)
                //        newPerson.md_tarif_offDay = 1500;
                //}


                // получение этого сотрудника из предыдущей существующей модели
                //SeparPerson PrevSepPerson = repoSepPerson.Items
                //    .AsNoTracking()
                //    .Where(it => it.sp_personalId == newPerson.md_personalId
                //            && (
                //                (it.Separate.s_year == _SelectYear && it.Separate.s_month < _SelectMonth)
                //                || it.Separate.s_year < _SelectYear
                //               ))
                //    .OrderByDescending(o => o.Separate.s_year)
                //    .ThenByDescending(o => o.Separate.s_month)
                //    .FirstOrDefault();


                //// если был предыдущий месяц, то копируем нужные тарифы
                //if (PrevSepPerson != null)
                //{
                //    // копирование тарифа бонусов
                //    newPerson.md_bonus_max = PrevModPerson.md_bonus_max;
                //    newPerson.md_cat_prem_tarif = PrevModPerson.md_cat_prem_tarif;
                //    newPerson.md_kvalif_proc = PrevModPerson.md_kvalif_proc;
                //}

                CurrentSeparate.ListSeparPerson.Add(newPerson);
            }

            repoSep.Add(CurrentSeparate, true);

            CurrentSeparate = repoSep.Items
                .Where(it => it.id == CurrentSeparate.id)
                .Include(inc => inc.ListSeparPerson.Select(s => s.person))
            .FirstOrDefault();

            ListSeparPerson = new ObservableCollection<SeparPerson>(CurrentSeparate.ListSeparPerson);
            OnPropertyChanged(nameof(ListSeparPerson));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => /*CurrentMod != null && _SelectedOtdel != null &&*/ IsModify;
        private void OnSaveCommandExecuted(object p)
        {
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
            //FormExport fomExport = new FormExport();

            ////fomExport.ListPersonToListExport(ListTabelPerson, ListModAllPerson,  BonusProc);
            //fomExport.ListPersonToListExport(_SelectYear, _SelectMonth, BonusProc);

            //RepositoryCSV repoFile = new RepositoryCSV(fomExport);
            //repoFile.SaveFile(_SelectYear, _SelectMonth);

        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => _SelectedOtdel != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>(db);

            List<Personal> ListPersonal = repoPersonal.Items
                .Where(it => it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                .ToList();

            // составляем список добавленных людей
            foreach (var item in ListSeparPerson)
            {
                var person = ListPersonal.FirstOrDefault(it => it.id == item.person.id);
                if (person != null)
                    ListPersonal.Remove(person);
            }

            List<SeparPerson> ListNewPerson = new List<SeparPerson>();

            if (ListPersonal.Count == 0)
            {
                MessageBox.Show("Новых людей для отдела не обнаружено.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (MessageBox.Show($"Найдено людей: {ListPersonal.Count}. Добавлять?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {

                foreach (var pers in ListPersonal)
                {
                    SeparPerson newPerson = new SeparPerson();
                    newPerson.sp_separId = CurrentSeparate.id;
                    newPerson.person = pers;

                    // получение этого сотрудника из предыдущей существующей модели
                    //SeparPerson PrevModPerson = repoSepPerson.Items
                    //    .AsNoTracking()
                    //    .Where(it => it.sp_personalId == newPerson.sp_personalId
                    //            && (
                    //                (it.Separate.s_year == _SelectYear && it.Separate.s_month < _SelectMonth)
                    //                || it.Separate.s_year < _SelectYear
                    //               ))
                    //    .OrderByDescending(o => o.Separate.s_year)
                    //    .ThenByDescending(o => o.Separate.s_month)
                    //    .FirstOrDefault();


                    // если был предыдущий месяц, то копируем нужные тарифы
                    //if (PrevModPerson != null)
                    //{
                    //    // копирование тарифа бонусов
                    //    newPerson.md_bonus_max = PrevModPerson.md_bonus_max;
                    //    newPerson.md_cat_prem_tarif = PrevModPerson.md_cat_prem_tarif;
                    //}

                    repoSepPerson.Add(newPerson, true);
                    ListSeparPerson.Add(newPerson);
                    ListNewPerson.Add(newPerson);
                }

                OnPropertyChanged(nameof(ListSeparPerson));
            }

        }



        //--------------------------------------------------------------------------------
        // Команда Удалить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand DeletePersonCommand => new LambdaCommand(OnDeletePersonCommandExecuted, CanDeletePersonCommand);
        private bool CanDeletePersonCommand(object p) => SelectedPerson != null;
        private void OnDeletePersonCommandExecuted(object p)
        {
            if (MessageBox.Show($"Удалить {SelectedPerson.person.FIO}?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                repoSepPerson.Remove(SelectedPerson, true);
                ListSeparPerson.Remove(SelectedPerson);
                //IsModify = true;
            }
        }


        #endregion

    }
}
