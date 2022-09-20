using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models2;
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

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;


        public Mod CurrentMod { get; set; }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => true;
        private void OnCreateCommandExecuted(object p)
        {
            CurrentMod = new Mod();
            CurrentMod.m_author = App.CurrentUser.id;
            CurrentMod.m_month = _SelectMonth;
            CurrentMod.m_year = _SelectYear;
            CurrentMod.m_otdelId = _SelectedOtdel.id;
            CurrentMod.ModPersons = new List<ModPerson>();

            var persons = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == _SelectedOtdel.id);

            foreach (var pers in persons)
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

            if (SelectOtdel is null) return;

            CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_month == _SelectMonth && it.m_year == _SelectYear && it.m_otdelId == _SelectedOtdel.id);

            LoadFromTabel();
            LoadFromSmena();
            OnPropertyChanged(nameof(CurrentMod));


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

            if (CurrentMod.ModPersons is null) return;

            foreach (var item in CurrentMod.ModPersons)
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

            var smena = repoSmena.Items.AsNoTracking().FirstOrDefault(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.id);

            if (CurrentMod.ModPersons is null) return;

            foreach (var item in CurrentMod.ModPersons)
            {
                var pers = smena.SmenaPerson.FirstOrDefault(it => it.sp_PersonId == item.md_personalId);
                item.NightHours = pers.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;
            }

        }


    }
}
