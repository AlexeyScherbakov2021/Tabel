﻿using System;
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
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class ModUCViewModel : ViewModel, IBaseUCViewModel
    {
        //private RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        //private readonly RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>();
        //private readonly RepositoryMSSQL<Mod> repoModel = new RepositoryMSSQL<Mod>();
        //private readonly RepositoryMSSQL<Smena> repoSmena = new RepositoryMSSQL<Smena>();
        //private readonly RepositoryMSSQL<ModPerson> repoModPerson = new RepositoryMSSQL<ModPerson>();
        //private readonly RepositoryMSSQL<Transport> repoTransport = new RepositoryMSSQL<Transport>();
        //private readonly RepositoryMSSQL<AddWorks> repoAddWorks = new RepositoryMSSQL<AddWorks>();
        //private readonly RepositoryMSSQL<ModOtdelSumFP> repoFP = new RepositoryMSSQL<ModOtdelSumFP>();

        private readonly RepositoryMSSQL<ModPerson> repoModPerson = AllRepo.GetRepoModPerson();
        private readonly RepositoryMSSQL<AddWorks> repoAddWorks = AllRepo.GetRepoAddWorks();
        private readonly RepositoryMSSQL<Transport> repoTransport = AllRepo.GetRepoTransport();
        private RepositoryMSSQL<Personal> repoPersonal = AllRepo.GetRepoPersonal();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = AllRepo.GetRepoTabel();
        private readonly RepositoryMSSQL<Mod> repoModel = AllRepo.GetRepoModel();
        private readonly RepositoryMSSQL<Smena> repoSmena = AllRepo.GetRepoSmena();

        static bool lockPropertyChanged = false;


        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        public bool IsCheckBonus { get; set; }

        public ObservableCollection<ModPerson> ListModPerson { get; set; }

        public List<AddWorks> ListWorks { get; set; }

        public Mod CurrentMod { get; set; }


        private bool _IsCheckedButton;
        public bool IsCheckedButton
        {
            get => _IsCheckedButton;
            set
            {
                _IsCheckedButton = value;
                if(!value)
                {
                    GetWorksFromPerson(_SelectedPerson, ListWorks);
                }
            }
        }


        private ModPerson _SelectedPerson;
        public ModPerson SelectedPerson
        {
            get => _SelectedPerson;
            set
            {
                if (_SelectedPerson == value) return;

                GetWorksFromPerson(_SelectedPerson, ListWorks);
                _SelectedPerson = value;
                SetWorksToPerson(_SelectedPerson, ListWorks);
                //repoModPerson.Save();

            }
        }

        //private ModOtdelSumFP _SumFP;
        //public ModOtdelSumFP SumFP
        //{
        //    get => _SumFP;
        //    set { Set(ref _SumFP, value); }
        //}



        #region Команды
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


            RepositoryMSSQL<Otdel> repoOtdel = AllRepo.GetRepoOtdel();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            var persons = repoPersonal.Items.AsNoTracking().Where(it => (it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                        || listOtdels.Contains(it.p_otdel_id.Value));

            foreach (var pers in persons)
            {
                ModPerson newPerson = new ModPerson();
                newPerson.md_personalId = pers.id;
                newPerson.md_tarif_offDay = pers.category.cat_tarif * 8;
                if (newPerson.md_tarif_offDay < 1500)
                    newPerson.md_tarif_offDay = 1500;

                //newPerson.person = pers;

                CurrentMod.ModPersons.Add(newPerson);
            }

            repoModel.Add(CurrentMod, true);

            CurrentMod = repoModel.Items
                .Where(it => it.id == CurrentMod.id)
                .Include(inc => inc.ModPersons.Select(s => s.person))
                .FirstOrDefault();

            ListModPerson = new ObservableCollection<ModPerson>(CurrentMod.ModPersons);
            LoadFromTabel();
            LoadFromSmena();
            LoadFromTransprot();

            OnPropertyChanged(nameof(ListModPerson));
            OnPropertyChanged(nameof(CurrentMod));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => CurrentMod != null && _SelectedOtdel != null;
        private void OnSaveCommandExecuted(object p)
        {
            repoModPerson.Save();
            repoModel.Save();
            //repoFP.Save();
        }

        //--------------------------------------------------------------------------------
        // Команда Отметить выбранные
        //--------------------------------------------------------------------------------
        public ICommand CheckCommand => new LambdaCommand(OnCheckCommandExecuted, CanCheckCommand);
        private bool CanCheckCommand(object p) => true;
        private void OnCheckCommandExecuted(object p)
        {
            if( p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_bonus_exec = IsCheckBonus;
                    item.OnPropertyChanged(nameof(item.md_bonus_exec));
                }
            }

        }

        #endregion


        //-------------------------------------------------------------------------------------------------------
        // конструктор
        //-------------------------------------------------------------------------------------------------------
        public ModUCViewModel()
        {
            DateTime _CurrentDate = DateTime.Now;

            //repoAddWorks = new RepositoryMSSQL<AddWorks>(repoModPerson.GetDB());
            repoAddWorks = AllRepo.GetRepoAddWorks();

            ListWorks = repoAddWorks.Items.ToList();

            //User = App.CurrentUser;
            //User = new User() { u_otdel_id = 44, u_login = "Petrov", id = 10, u_fio = "Петров" };

        }


        //-------------------------------------------------------------------------------------------------------
        // загрузка выбранных данных
        //-------------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;
            ListModPerson = null;

            if (SelectOtdel is null) return;


            // удаление подписки на изменение элеменов списка сотрудников
            if (ListModPerson != null)
            {
                foreach (var modPerson in ListModPerson)
                {
                    modPerson.PropertyChanged -= ModPerson_PropertyChanged;
                }
            }

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

            if (CurrentMod != null)
            {
                LoadFromTabel();
                LoadFromSmena();
                LoadFromTransprot();

                // Подписка на изменение элеменов списка сотрудников
                foreach (var modPerson in ListModPerson)
                {
                    modPerson.PropertyChanged += ModPerson_PropertyChanged;
                    // расчет премии из ФП
                    CalcHoursAndPrem(modPerson);

                    // рассчет суммарных процентов в премии ФП
                    List<string> ListGroups = ListModPerson.Select(it => it.md_group).Distinct().ToList();
                    foreach (var group in ListGroups)
                    {
                        List<ModPerson> groupPerson = ListModPerson.Where(it => it.md_group == group).ToList();
                        CalcChangeProcent(groupPerson);
                    }

                }
            }
            OnPropertyChanged(nameof(ListModPerson));
            OnPropertyChanged(nameof(CurrentMod));

        }


        //-------------------------------------------------------------------------------------------------------
        // установка расчетных часов и премии
        //-------------------------------------------------------------------------------------------------------
        private void CalcHoursAndPrem(ModPerson modPerson)
        {
            modPerson.SummaHoursFP = modPerson.md_sumFromFP * modPerson.md_premFP / 100;
            modPerson.SummaPremFP = modPerson.TabelDays == 0
                ? 0
                : modPerson.SummaHoursFP * modPerson.md_cat_prem_tarif * (modPerson.TabelDays - modPerson.TabelAbsent)
                        / modPerson.TabelDays;

            modPerson.OnPropertyChanged(nameof(modPerson.SummaHoursFP));
            modPerson.OnPropertyChanged(nameof(modPerson.SummaPremFP));

        }


        //-------------------------------------------------------------------------------------------------------
        // установка суммарного процента по группе
        //-------------------------------------------------------------------------------------------------------
        private void CalcChangeProcent(IEnumerable<ModPerson> groupPerson)
        {

            decimal? SummaProc = groupPerson.Sum(it => it.md_premFP) / 100m;
            if (groupPerson != null)
            {
                foreach (var item in groupPerson)
                {
                    item.ProcGroup = SummaProc;
                    item.OnPropertyChanged(nameof(item.ProcGroup));
                }
            }

        }



        //-------------------------------------------------------------------------------------------------------
        // событие изменения полей Группа и сумма из ФП
        //-------------------------------------------------------------------------------------------------------
        private void ModPerson_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            List<ModPerson> groupPerson;
            var modPerson = sender as ModPerson;

            switch(e.PropertyName)
            {
                case "md_premFP":
                case "md_cat_prem_tarif":
                    CalcHoursAndPrem(modPerson);

                    groupPerson = ListModPerson.Where(it => it.md_group == modPerson.md_group
                      /*|| (it.md_group is null && modPerson.md_group == "")*/).ToList();

                    CalcChangeProcent(groupPerson);
                    break;

                case "md_sumFromFP":
                    if (lockPropertyChanged) return;

                    lockPropertyChanged = true;

                    groupPerson = ListModPerson.Where(it => it.md_group == modPerson.md_group
                      /*|| (it.md_group is null && modPerson.md_group == "")*/).ToList();

                    if (groupPerson != null)
                    {
                        decimal? summFP = modPerson is null ? 0 : modPerson.md_sumFromFP;

                        foreach (var item in groupPerson)
                        {
                            item.md_sumFromFP = summFP;
                            item.OnPropertyChanged(nameof(item.md_sumFromFP));
                            CalcHoursAndPrem(item);
                        }
                    }
                    lockPropertyChanged = false;
                    break;

                case "md_group":

                    decimal? summ = ListModPerson
                        .Where(it => it.md_group == modPerson.md_group && it.id != modPerson.id)
                        .Select(s => s.md_sumFromFP).FirstOrDefault();
                    if(summ != null)
                        modPerson.md_sumFromFP = summ;
                    CalcHoursAndPrem(modPerson);

                    List<string> ListGroups = ListModPerson.Select(it => it.md_group).Distinct().ToList();

                    foreach(var group in ListGroups)
                    {
                        groupPerson = ListModPerson.Where(it => it.md_group == group).ToList();
                        CalcChangeProcent(groupPerson);
                    }

                    break;
            }
        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из данных по транспорту
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTransprot()
        {
            if (CurrentMod is null)
                return;

            Transport Transp;

            if (_SelectedOtdel.ot_parent is null)
            {
                Transp = repoTransport.Items.AsNoTracking().FirstOrDefault(it => it.tr_Year == _SelectYear
                        && it.tr_Month == _SelectMonth
                        && it.tr_OtdelId == _SelectedOtdel.id);
            }
            else
            {
                Transp = repoTransport.Items.AsNoTracking().FirstOrDefault(it => it.tr_Year == _SelectYear
                    && it.tr_Month == _SelectMonth
                    && it.tr_OtdelId == _SelectedOtdel.ot_parent);
            }


            if (ListModPerson is null || Transp is null) return;

            foreach (var item in ListModPerson)
            {
                var pers = Transp.TransportPerson.FirstOrDefault(it => it.tp_PersonId == item.md_personalId);
                item.TransportSumma = pers?.Summa;
            }

        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из табеля
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTabel()
        {
            if (CurrentMod is null)
                return;

            var tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == _SelectYear
                && it.t_month == _SelectMonth
                && it.t_otdel_id == _SelectedOtdel.id);

            if (ListModPerson is null || tabel is null) return;

            // получение количества рабочих дней в указанном месяце
            RepositoryCalendar repoCal = AllRepo.GetRepoCalendar();
            var listDays = repoCal.GetListDays(_SelectYear, _SelectMonth);
            int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);

            foreach (var item in ListModPerson)
            {

                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
                //item.TabelDays = pers.DaysMonth;
                item.TabelDays = listDays.Count;
                item.TabelHours = pers.HoursMonth;
                item.TabelWorkOffDay = pers.WorkedOffDays;
                item.DayOffSumma = item.TabelWorkOffDay * item.md_tarif_offDay;
                item.Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value;

                int CountWorkDaysPerson = pers.TabelDays.Count(it => it.td_KindId == 1);
                item.TabelAbsent = CountWorkDays - CountWorkDaysPerson ;
                if (item.TabelAbsent < 0) item.TabelAbsent = 0;

            }

        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из графика смен
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromSmena()
        {
            if (CurrentMod is null)
                return;

            var smena = repoSmena.Items.AsNoTracking().FirstOrDefault(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.id);

            if (ListModPerson is null || smena is null) return;

            foreach (var item in ListModPerson)
            {
                var pers = smena.SmenaPerson.FirstOrDefault(it => it.sp_PersonId == item.md_personalId);
                item.NightHours = pers.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;
            }

        }


        //--------------------------------------------------------------------------------
        // Отметка в списке работ для сотрудника
        //--------------------------------------------------------------------------------
        private void SetWorksToPerson(ModPerson person, ICollection<AddWorks> ListWorks)
        {
            if (person is null || ListWorks is null) return;

            foreach (AddWorks work in ListWorks)
            {
                work.IsChecked = person.ListAddWorks.Any(it => it.id == work.id);
                work.OnPropertyChanged(nameof(work.IsChecked));
            }

        }

        //--------------------------------------------------------------------------------
        // Получение работ для сотрудника
        //--------------------------------------------------------------------------------
        private void GetWorksFromPerson(ModPerson person, ICollection<AddWorks> ListWorks)
        {
            if (person is null || ListWorks is null) return;

            foreach (AddWorks work in ListWorks)
            {
                if (work.IsChecked)
                    person.ListAddWorks.Add(work);
                else
                    person.ListAddWorks.Remove(work);

            }

            person.OnPropertyChanged(nameof(person.ListAddWorks));
            //person.OnPropertyChanged(nameof(person.AddWorksSumma));
            person.UpdateUI();

        }


    }
}
