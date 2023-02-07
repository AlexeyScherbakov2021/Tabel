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
    internal class PayUCViewModel : Observable, IDataSumm
    {
        RepositoryMSSQL<ModPerson> repoModPerson;
        public List<ModPerson> ListModPersons { get; set; }
        private bool IsModify;

        public PayUCViewModel()
        {
            repoModPerson = new RepositoryMSSQL<ModPerson>();
        }

        public void ChangePeriod(int CurrentYear, int CurrentMonth)
        {
            if (CurrentYear < 1 || CurrentMonth < 1) return;

            if(ListModPersons != null)
                foreach (var item in ListModPersons)
                    item.PropertyChanged -= Item_PropertyChanged;


            ListModPersons = repoModPerson.Items
                .Where(it => it.Mod.m_year == CurrentYear && it.Mod.m_month == CurrentMonth)
                .OrderBy(o => o.person.p_lastname)
                .ThenBy(o => o.person.p_name)
                .ToList();

            OnPropertyChanged(nameof(ListModPersons));

            foreach(var item in ListModPersons)
                item.PropertyChanged += Item_PropertyChanged;

        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsModify = true;
        }

        public void Save()
        {
            repoModPerson.Save();
            IsModify = false;
        }

        public bool ClosingFrom()
        {
            return IsModify;
        }
    }
}
