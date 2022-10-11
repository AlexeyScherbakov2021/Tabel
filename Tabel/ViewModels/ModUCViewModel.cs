using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class ModUCViewModel : ViewModel, IBaseUCViewModel
    {
        private RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>();
        private readonly RepositoryMSSQL<Mod> repoModel = new RepositoryMSSQL<Mod>();
        private readonly RepositoryMSSQL<Smena> repoSmena = new RepositoryMSSQL<Smena>();
        private readonly RepositoryMSSQL<ModPerson> repoModPerson = new RepositoryMSSQL<ModPerson>();
        private readonly RepositoryMSSQL<Transport> repoTransport = new RepositoryMSSQL<Transport>();

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        public ObservableCollection<ModPerson> ListModPerson { get; set; }

        public Mod CurrentMod { get; set; }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (CurrentMod != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Подтверждаете?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.OK)
                    return;

                repoModel.Remove(CurrentMod);
            }

            CurrentMod = new Mod();
            CurrentMod.m_author = App.CurrentUser.id;
            CurrentMod.m_month = _SelectMonth;
            CurrentMod.m_year = _SelectYear;
            CurrentMod.m_otdelId = _SelectedOtdel.id;
            CurrentMod.ModPersons = new List<ModPerson>();


            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
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
            repoModel.Save();
        }


        #endregion


        //-------------------------------------------------------------------------------------------------------
        // конструктор
        //-------------------------------------------------------------------------------------------------------
        public ModUCViewModel()
        {
            DateTime _CurrentDate = DateTime.Now;
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

            //CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_month == _SelectMonth && it.m_year == _SelectYear 
            //&& it.m_otdelId == _SelectedOtdel.id);

            LoadFromTabel();
            LoadFromSmena();
            LoadFromTransprot();
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

            foreach (var item in ListModPerson)
            {
                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
                item.TabelDays = pers.DaysMonth;
                item.TabelHours = pers.HoursMonth;
                item.TabelAbsent = 0;
                item.TabelWorkOffDay = pers.WorkedOffDays;
                item.DayOffSumma = item.TabelWorkOffDay * item.md_tarif_offDay;
                item.Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value;
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


    }
}
