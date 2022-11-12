using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class DataSummaryViewModel : ViewModel
    {
        RepositoryMSSQL<GenChargMonth> repoGen;
        RepositoryMSSQL<GeneralCharges> repoGeneral;

        public List<GenChargMonth> ListGenChargMonths { get; set; }

        public User User => App.CurrentUser;

        public List<Months> ListMonth => App.ListMonth;
        public List<int> ListYears { get; set; }

        private int _CurrentMonth;
        public int CurrentMonth { get => _CurrentMonth; set { if (Set(ref _CurrentMonth, value)) ChangePeriod();  } }

        private int _CurrentYear;
        public int CurrentYear { get => _CurrentYear; set { if (Set(ref _CurrentYear, value)) ChangePeriod(); } }


        //------------------------------------------------------------------------------------------------
        // Конструктор
        //------------------------------------------------------------------------------------------------ 
        public DataSummaryViewModel()
        {
            RepositoryCalendar repoCal = AllRepo.GetRepoCalendar();
            ListYears = repoCal.GetYears().ToList();

            repoGeneral = AllRepo.GetRepoGenCharges();
            repoGen = AllRepo.GetRepoGenChargesMonth();

            DateTime _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;

        }

        //------------------------------------------------------------------------------------------------
        // Изменение периода
        //------------------------------------------------------------------------------------------------ 
        private void ChangePeriod()
        {
            if (CurrentYear < 1 || CurrentMonth < 1) return;

            ListGenChargMonths = repoGen.Items
                .Where(it => it.gm_Year == CurrentYear && it.gm_Month == CurrentMonth).ToList();

            if (ListGenChargMonths.Count() == 0)
            {
                var list = repoGeneral.Items.ToList();

                foreach(var item in list)
                {
                    GenChargMonth newItem = new GenChargMonth()
                    {
                        gm_Month = CurrentMonth,
                        gm_Year = CurrentYear,
                        GenCarhge = item
                    };

                    ListGenChargMonths.Add(newItem);
                    repoGen.Add(newItem);
                }
            }

            OnPropertyChanged(nameof(ListGenChargMonths));
        }

        

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoGen.Save();
        }

        #endregion
    }
}
