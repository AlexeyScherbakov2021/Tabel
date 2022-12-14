using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class PremiaTransportViewModel : ModViewModel
    {
        private readonly RepositoryMSSQL<Transport> repoTransport;

        public ICollection<ModPerson> ListModPerson { get; set; }

        public PremiaTransportViewModel(BaseModel db) : base(db)
        {
            repoTransport = new RepositoryMSSQL<Transport>(db);
        }

        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = Otdel;
            ListModPerson = listPerson;
            //LoadFromTransport(ListModPerson);
            if (ListModPerson != null)
            {
                foreach (var item in ListModPerson)
                    item.premiaTransport.Calculation();
            }

            OnPropertyChanged(nameof(ListModPerson));
        }

        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            //LoadFromTransport(listPerson);

            OnPropertyChanged(nameof(ListModPerson));

        }
        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из данных по транспорту
        //-------------------------------------------------------------------------------------------------------
        //private void LoadFromTransport(ICollection<ModPerson> listPerson)
        //{
        //    if (listPerson is null)
        //        return;

        //    Transport Transp;

        //    Transp = repoTransport.Items.AsNoTracking().FirstOrDefault(it => it.tr_Year == _SelectYear
        //        && it.tr_Month == _SelectMonth
        //        && it.tr_OtdelId == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));

        //    if (listPerson is null || Transp is null) return;

        //    foreach (var item in listPerson)
        //    {
        //       //item.premiaTrnasport.Initialize(Transp.id);
        //    }
        //}

    }
}
