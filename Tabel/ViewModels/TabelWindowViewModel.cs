using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    internal class TabelWindowViewModel : ViewModel
    {
        private readonly RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>();

        public string[] ListKind { get; set; } = { "1см", "2см", "В", "О" };
        public User User { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        private DateTime _CurrentDate;

        public List<Otdel> ListOtdel { get; set; }

        private Otdel _SelectedOtdel;
        public Otdel SelectedOtdel
        {
            get => _SelectedOtdel;
            set
            {
                if (Set(ref _SelectedOtdel, value))
                {
                    LoadPersonForOtdel(_SelectedOtdel);
                }
            }
        }



        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Выбрать тип дня
        //--------------------------------------------------------------------------------
        public ICommand SelectTypeCommand => new LambdaCommand(OnSelectTypeCommandExecuted, CanSelectTypeCommand);
        private bool CanSelectTypeCommand(object p) => true;
        private void OnSelectTypeCommandExecuted(object p)
        {
        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        public ICommand LoadDefCommand => new LambdaCommand(OnLoadDefCommandExecuted, CanLoadDefCommand);
        private bool CanLoadDefCommand(object p) => true;
        private void OnLoadDefCommandExecuted(object p)
        {

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoTabel.Save();
        }


        #endregion


        public TabelWindowViewModel()
        {
            _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;
            User = App.CurrentUser;
            User = new User() { u_otdel_id = 44, u_login = "Petrov", id = 10, u_fio = "Петров" };
            ListOtdel = repoOtdel.Items.Where(it => it.id == User.u_otdel_id).ToList();

        }


        //----------------------------------------------------------------------------------------------------------
        // загрузка дней для сотрудника
        //----------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------
        private void LoadPersonForOtdel(Otdel otdel)
        {
            //SmenaShedule = repoSmena.Items.FirstOrDefault(it => it.sm_Year == CurrentYear
            //        && it.sm_Month == CurrentMonth
            //        && it.sm_OtdelId == otdel.id);
            //List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == SelectedOtdel.id).ToList();

            //if (SmenaShedule != null)
            //{
            //    // если график присутствует
            //    foreach (var item in PersonsFromOtdel)
            //    {
            //        SmenaPerson pers = repoSmenaPersonal.Items.FirstOrDefault(it => it.sp_SmenaId == SmenaShedule.id && it.sp_PersonId == item.id);
            //        if (pers is null)
            //        {
            //            // формируем новый график на месяц для сотрудника, который отсутствовал
            //        }

            //    }

            //}

            //OnPropertyChanged(nameof(SmenaShedule));

        }

    }
}
