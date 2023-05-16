using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;

namespace Tabel.ViewModels.DataSummForm
{
    internal class PlanUCViewModel: Observable, IDataSumm
    {
        RepositoryMSSQL<GenChargMonth> repoGen;
        RepositoryMSSQL<GeneralCharges> repoGeneral;

        public List<GenChargMonth> ListGenChargMonths { get; set; }


        public PlanUCViewModel()
        {
            repoGeneral = new RepositoryMSSQL<GeneralCharges>(); 
            repoGen = new RepositoryMSSQL<GenChargMonth>(repoGeneral.GetDB());
        }



        public void ChangePeriod(int CurrentYear, int CurrentMonth)
        {
            if (CurrentYear < 1 || CurrentMonth < 1) return;

            ListGenChargMonths = repoGen.Items
                .Where(it => it.gm_Year == CurrentYear && it.gm_Month == CurrentMonth && it.gm_GenId ==  (int)koeffPere.Bonus).ToList();

            if (ListGenChargMonths.Count() == 0)
            {
                var list = repoGeneral.Items.Where(it => it.id == (int)koeffPere.Bonus).ToList();

                foreach (var item in list)
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



        public void Save()
        {
            repoGen.Save();
        }

        public bool ClosingFrom()
        {
            return false;
        }
    }
}
