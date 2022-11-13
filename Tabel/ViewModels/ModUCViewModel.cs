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

namespace Tabel.ViewModels
{
    internal class ModUCViewModel : ViewModel, IBaseUCViewModel
    {
        private readonly RepositoryMSSQL<ModPerson> repoModPerson = AllRepo.GetRepoModPerson();
        private readonly RepositoryMSSQL<AddWorks> repoAddWorks = AllRepo.GetRepoAddWorks();
        private readonly RepositoryMSSQL<Transport> repoTransport = AllRepo.GetRepoTransport();
        private RepositoryMSSQL<Personal> repoPersonal = AllRepo.GetRepoPersonal();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = AllRepo.GetRepoTabel();
        private readonly RepositoryMSSQL<Mod> repoModel = AllRepo.GetRepoModel();
        private readonly RepositoryMSSQL<Smena> repoSmena = AllRepo.GetRepoSmena();

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        public Visibility IsVisibleITR { get; private set; }
        public Visibility IsVisibleNoITR { get; private set; }

        public bool IsCheckBonus { get; set; }
        public decimal SetProcPrem { get; set; }
        public decimal SetMaxPrem { get; set; }

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

            }
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
                if(index == 0)
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


            RepositoryMSSQL<Otdel> repoOtdel = AllRepo.GetRepoAllOtdels();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            var persons = repoPersonal.Items
                .AsNoTracking()
                .Where(it => (it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false) || listOtdels.Contains(it.p_otdel_id.Value))
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name);
;
            foreach (var pers in persons)
            {
                ModPerson newPerson = new ModPerson();
                newPerson.md_group = pers.p_otdel_id.ToString();
                newPerson.md_personalId = pers.id;
                newPerson.md_tarif_offDay = pers.category?.cat_tarif * 8;
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

        //--------------------------------------------------------------------------------
        // Команда Применить сумму к выбранным
        //--------------------------------------------------------------------------------
        //public ICommand SetSummaCommand => new LambdaCommand(OnSetSummaCommandExecuted, CanSetSummaCommand);
        //private bool CanSetSummaCommand(object p) => true;
        //private void OnSetSummaCommandExecuted(object p)
        //{
        //    if( p is DataGrid dg)
        //    {
        //        foreach (ModPerson item in dg.SelectedItems)
        //        {
        //            item.md_bonus_proc = SetProcPrem;
        //            item.OnPropertyChanged(nameof(item.md_bonus_proc));
        //        }
        //    }

        //}

        //--------------------------------------------------------------------------------
        // Команда Применить сумму к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetMaxSummaCommand => new LambdaCommand(OnSetMaxSummaCommandExecuted, CanSetMaxSummaCommand);
        private bool CanSetMaxSummaCommand(object p) => true;
        private void OnSetMaxSummaCommandExecuted(object p)
        {
            if( p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_bonus_max = SetMaxPrem;
                    item.OnPropertyChanged(nameof(item.md_bonus_max));
                }
            }

        }

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
                RepositoryMSSQL<TabelPerson> repoTabelPerson = AllRepo.GetRepoTabelPerson();
                List<TabelPerson> ListTabelPerson = new List<TabelPerson>();

                foreach (var tabel in ListTabel)
                {
                    List<TabelPerson> listPerson = repoTabelPerson.Items
                        .Where(it => it.tp_tabel_id == tabel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name).ToList();

                    ListTabelPerson.AddRange(listPerson);
                }

                ListTabelPerson.Sort((item1, item2) =>
                {
                    return item1.person.p_lastname.CompareTo(item2.person.p_lastname);
                });

                // берем часы переработки
                FormExport fomExport = new FormExport();

                fomExport.ListPersonToListExport(ListTabelPerson, ListModPerson);

                RepositoryCSV repoFile = new RepositoryCSV(fomExport);
                repoFile.SaveFile(_SelectYear, _SelectMonth);
            }
        }


        #endregion


        //-------------------------------------------------------------------------------------------------------
        // конструктор
        //-------------------------------------------------------------------------------------------------------
        public ModUCViewModel()
        {
            DateTime _CurrentDate = DateTime.Now;
            repoAddWorks = AllRepo.GetRepoAddWorks();
            ListWorks = repoAddWorks.Items.ToList();

        }


        //-------------------------------------------------------------------------------------------------------
        // загрузка выбранных данных
        //-------------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;

             if(_SelectedOtdel?.ot_itr == 1)
             {
                IsVisibleITR = Visibility.Visible;
                IsVisibleNoITR = Visibility.Collapsed;
             }
             else
             {
                IsVisibleNoITR = Visibility.Visible;
                IsVisibleITR = Visibility.Collapsed;
             }

            OnPropertyChanged(nameof(IsVisibleITR));
            OnPropertyChanged(nameof(IsVisibleNoITR));

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

            if (CurrentMod != null)
            {
                LoadFromTabel();
                LoadFromSmena();
                LoadFromTransprot();

                RepositoryMSSQL<GenChargMonth> repoGetAll = new RepositoryMSSQL<GenChargMonth>();
                decimal? BonusProc = repoGetAll.Items
                    .FirstOrDefault(it => it.gm_Year == Year && it.gm_Month == Month && it.gm_GenId == (int)EnumKind.BonusProc)?.gm_Value;


                // Подписка на изменение элеменов списка сотрудников
                foreach (var modPerson in ListModPerson)
                {
                    // расчет премии из ФП
                    modPerson.premiaFP.Calculation();
                    //рассчет суммарных процентов в премии ФП
                    modPerson.premiaFP.CalcChangeProcent();

                    modPerson.BonusForAll = BonusProc;
                    modPerson.premiaBonus.Calculation();
                }
            }
            OnPropertyChanged(nameof(ListModPerson));
            OnPropertyChanged(nameof(CurrentMod));

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
                //var pers = Transp.TransportPerson.FirstOrDefault(it => it.tp_PersonId == item.md_personalId);
                //item.premiaTrnasport.Summa = pers?.Summa;
                item.premiaTrnasport.Initialize(Transp.id);
            }

        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из табеля
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTabel()
        {
            if (CurrentMod is null)
                return;

            WorkTabel tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == _SelectYear
                && it.t_month == _SelectMonth
                && it.t_otdel_id == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));

            if (ListModPerson is null || tabel is null) return;

            // получение количества рабочих дней в указанном месяце
            RepositoryCalendar repoCal = AllRepo.GetRepoCalendar();
            var listDays = repoCal.GetListDays(_SelectYear, _SelectMonth);
            int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);

            foreach (var item in ListModPerson)
            {

                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
                item.TabelDays = listDays.Count;
                item.TabelHours = pers.HoursMonth;
                item.TabelWorkOffDay = pers.WorkedOffDays;
                item.OverHours = pers.OverWork ?? 0;
                //item.DayOffSumma = item.TabelWorkOffDay * item.md_tarif_offDay;
                item.Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value * item.person.p_stavka;

                int CountWorkDaysPerson = pers.TabelDays.Count(it => it.td_KindId == 1);
                item.TabelAbsent = CountWorkDays - CountWorkDaysPerson ;
                if (item.TabelAbsent < 0) item.TabelAbsent = 0;
                item.premiaPrize.Calculation();

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
                //var pers = smena.SmenaPerson.FirstOrDefault(it => it.sp_PersonId == item.md_personalId);
                //item.premiaNight.NightHours = pers.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;
                item.premiaNight.Initialize(smena.id);
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
            //person.UpdateUI();

        }


    }
}
