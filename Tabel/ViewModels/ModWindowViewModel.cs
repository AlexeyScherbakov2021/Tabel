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
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTabel()
        {
            if (CurrentMod is null)
                return;

            var tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == CurrentYear
                && it.t_month == CurrentMonth
                && it.t_otdel_id == SelectedOtdel.id);


            foreach(var item in CurrentMod.ModPersons)
            {
                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
                item.TabelDays = pers.DaysMonth;
                item.TabelHours = pers.HoursMonth;
                item.TabelAbsent = 0;
                item.Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value;
            }

        }


        //-------------------------------------------------------------------------------------------------------
        // загрузка выбранных данных
        //-------------------------------------------------------------------------------------------------------
        private void LoadPersonForModel()
        {
            CurrentMod = repoModel.Items.FirstOrDefault(it => it.m_month == CurrentMonth && it.m_year == CurrentYear && it.m_otdelId == SelectedOtdel.id);
            LoadFromTabel();
            OnPropertyChanged(nameof(CurrentMod));
        }
    }
}
