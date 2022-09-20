using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class ModWindowViewModel : ViewModel
    {
        private RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>();
        private readonly RepositoryMSSQL<Mod> repoModel = new RepositoryMSSQL<Mod>();
        private readonly RepositoryMSSQL<Smena> repoSmena = new RepositoryMSSQL<Smena>();

        public User User { get; set; }

        private int _CurrentMonth;
        public int CurrentMonth
        {
            get => _CurrentMonth;
            set
            {
                if (Set(ref _CurrentMonth, value))
                    ;
                    //LoadPersonForOtdel(_SelectedOtdel);
            }
        }

        public int CurrentYear { get; set; }

        public List<Otdel> ListOtdel { get; set; }

        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel
        {

            get => _SelectedOtdel;
            set
            {
                if (Set(ref _SelectedOtdel, value))
                {
                    LoadPersonForModel();
                }
            }
        }

        public Mod CurrentMod { get; set; }


        #region Для тестирования, потом удалить
        //public ObservableCollection<ModPerson> ListModPerson { get; set; }
        //public WorkTabel Tabel { get; set; }
        #endregion


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => true;
        private void OnCreateCommandExecuted(object p)
        {
            CurrentMod = new Mod();
            CurrentMod.m_author = User.id;
            CurrentMod.m_month = CurrentMonth;
            CurrentMod.m_year = CurrentYear;
            CurrentMod.m_otdelId = SelectedOtdel.id;
            CurrentMod.ModPersons = new List<ModPerson>();

            var persons = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == SelectedOtdel.id);

            foreach(var pers in persons)
            {
                ModPerson newPerson = new ModPerson();
                newPerson.md_personalId = pers.id;


                CurrentMod.ModPersons.Add(newPerson);
            }

            repoModel.Add(CurrentMod, true);


        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoModel.Save();
        }


        #endregion


        //-------------------------------------------------------------------------------------------------------
        // конструктор
        //-------------------------------------------------------------------------------------------------------
        public ModWindowViewModel()
        {
            DateTime _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;
            User = App.CurrentUser;
            User = new User() { u_otdel_id = 44, u_login = "Petrov", id = 10, u_fio = "Петров" };
            ListOtdel = repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();

        }


        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из табеля
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTabel()
        {
            if (CurrentMod is null)
                return;

            var tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == CurrentYear
                && it.t_month == CurrentMonth
                && it.t_otdel_id == SelectedOtdel.id);

            if (CurrentMod.ModPersons is null) return;

            foreach(var item in CurrentMod.ModPersons)
            {
                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
                item.TabelDays = pers.DaysMonth;
                item.TabelHours = pers.HoursMonth;
                item.TabelAbsent = 0;
                item.TabelWorkOffDay = pers.WorkedOffDays;
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

            var smena = repoSmena.Items.AsNoTracking().FirstOrDefault(it => it.sm_Year == CurrentYear
                && it.sm_Month == CurrentMonth
                && it.sm_OtdelId == SelectedOtdel.id);

            if (CurrentMod.ModPersons is null) return;

            foreach (var item in CurrentMod.ModPersons)
            {
                var pers = smena.SmenaPerson.FirstOrDefault(it => it.sp_PersonId == item.md_personalId);
                item.NightHours = pers.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;
            }

        }


        //-------------------------------------------------------------------------------------------------------
        // загрузка выбранных данных
        //-------------------------------------------------------------------------------------------------------
        private void LoadPersonForModel()
        {
            CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_month == CurrentMonth && it.m_year == CurrentYear && it.m_otdelId == SelectedOtdel.id);

            LoadFromTabel();
            LoadFromSmena();
            OnPropertyChanged(nameof(CurrentMod));
        }

    }
}
