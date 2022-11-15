using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.MonthPanel;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal class ModMainViewModel : ViewModel, IModViewModel
    {
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = AllRepo.GetRepoTabel();
        private readonly RepositoryMSSQL<Smena> repoSmena = AllRepo.GetRepoSmena();

        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;
        public ObservableCollection<ModPerson> ListModPerson { get; set; }

        public void ChangeListPerson(ObservableCollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            ListModPerson = listPerson;
            _SelectYear= Year;
            _SelectMonth= Month;
            _SelectedOtdel = Otdel;

            LoadFromTabel();
            LoadFromSmena();
            OnPropertyChanged(nameof(ListModPerson));
        }


        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из табеля
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromTabel()
        {
            if (ListModPerson is null)
                return;

            WorkTabel tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == _SelectYear
                && it.t_month == _SelectMonth
                && it.t_otdel_id == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));

            if (ListModPerson is null || tabel is null) return;

            // получение количества рабочих дней в указанном месяце
            RepositoryCalendar repoCal = AllRepo.GetRepoCalendar();
            var listDays = repoCal.GetListDays(_SelectYear, _SelectMonth);
            int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);

            foreach (var item in ListModPerson)
            {

                var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
                item.TabelDays = listDays.Count;
                item.TabelHours = pers.HoursMonth;
                item.TabelWorkOffDay = pers.WorkedOffDays;
                item.OverHours = pers.OverWork ?? 0;
                //item.DayOffSumma = item.TabelWorkOffDay * item.md_tarif_offDay;
                item.Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value * item.person.p_stavka;

                int CountWorkDaysPerson = pers.TabelDays.Count(it => it.td_KindId == 1);
                item.TabelAbsent = CountWorkDays - CountWorkDaysPerson;
                if (item.TabelAbsent < 0) item.TabelAbsent = 0;
                item.premiaPrize.Calculation();

            }

        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из графика смен
        //-------------------------------------------------------------------------------------------------------
        private void LoadFromSmena()
        {
            if (ListModPerson is null)
                return;

            var smena = repoSmena.Items.AsNoTracking().FirstOrDefault(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));

            if (ListModPerson is null || smena is null) return;

            foreach (var item in ListModPerson)
            {
                item.premiaNight.Initialize(smena.id);
            }

        }

    }
}
