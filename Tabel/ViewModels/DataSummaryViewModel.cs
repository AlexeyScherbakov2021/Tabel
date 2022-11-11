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
        public List<GenChargMonth> ListGenChargMonths { get; set; }

        public User User => App.CurrentUser;

        public List<Months> ListMonth => App.ListMonth;

        public int CurrentMonth { get; set; }

        public List<int> ListYears { get; set; }

        public int CurrentYear { get; set; }


        public DataSummaryViewModel()
        {

            DateTime _CurrentDate = DateTime.Now;
            CurrentMonth = _CurrentDate.Month;
            CurrentYear = _CurrentDate.Year;

            RepositoryCalendar repoCal = AllRepo.GetRepoCalendar();
            ListYears = repoCal.GetYears().ToList();

            repoGen = new RepositoryMSSQL<GenChargMonth>();
            repoGen = AllRepo.GetRepoGenChargesMonth();
            ListGenChargMonths = repoGen.Items
                .Where(it => it.gm_Year == CurrentYear && it.gm_Month == CurrentMonth).ToList();

            if (ListGenChargMonths.Count() == 0)
            {
                ListGenChargMonths.Add(new GenChargMonth()
                {
                    gm_Month = CurrentMonth,
                    gm_Year = CurrentYear,
                });
            }


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
