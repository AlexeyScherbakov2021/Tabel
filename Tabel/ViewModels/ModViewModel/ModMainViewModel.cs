using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels.ModViewModel
{
    internal class ModMainViewModel : ModViewModel
    {

        public ICollection<ModPerson> ListModPerson { get; set; }

        public ModMainViewModel(BaseModel ctx) : base(ctx)
        {

        }


        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            ListModPerson = listPerson;
            //_SelectYear= Year;
            //_SelectMonth= Month;
            //_SelectedOtdel = Otdel;

            //ModFunction ModFunc = new ModFunction(db, _SelectYear, _SelectMonth);
            //foreach(var item in ListModPerson)
            //    ModFunc.ModPersonFilling(item);


            //LoadFromTabel(ListModPerson);
            //LoadFromSmena(ListModPerson);
            //LoadFromTransport(listPerson);
            OnPropertyChanged(nameof(ListModPerson));
        }


        public override void AddPersons(ICollection<ModPerson> listPerson)
        {
            //LoadFromTabel(listPerson);
            //LoadFromSmena(listPerson);
            //LoadFromTransport(listPerson);
            //ModFunction ModFunc = new ModFunction(db, _SelectYear, _SelectMonth);
            //foreach (var item in ListModPerson)
            //    ModFunc.ModPersonFilling(item);

            OnPropertyChanged(nameof(ListModPerson));

        }

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из транспорта
        //-------------------------------------------------------------------------------------------------------
        //private void LoadFromTransport(ICollection<ModPerson> listPerson)
        //{
        //    if (listPerson is null)
        //        return;

        //    Transport Transp;

        //    if (_SelectedOtdel is null)
        //    {
        //        RepositoryMSSQL<Transport> repoTransport = new RepositoryMSSQL<Transport>(db);
        //        Transp = repoTransport.Items.AsNoTracking().FirstOrDefault(it => it.tr_Year == _SelectYear
        //            && it.tr_Month == _SelectMonth);
        //    }
        //    else
        //    {
        //        RepositoryMSSQL<Transport> repoTransport = new RepositoryMSSQL<Transport>(db);
        //        Transp = repoTransport.Items.AsNoTracking().FirstOrDefault(it => it.tr_Year == _SelectYear
        //            && it.tr_Month == _SelectMonth
        //            && it.tr_OtdelId == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));
        //    }

        //    if (Transp is null) return;

        //    RepositoryMSSQL<TransPerson> repoTransPerson = new RepositoryMSSQL<TransPerson>(db);

        //    foreach (var item in listPerson)
        //    {
        //        var pers = repoTransPerson.Items.FirstOrDefault(it => it.tp_TranspId == Transp.id && it.tp_PersonId == item.md_personalId);
        //        item.TransportPremia = pers?.Summa;
        //    }


        //}


        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из табеля
        //-------------------------------------------------------------------------------------------------------
        //private void LoadFromTabel(ICollection<ModPerson> listPerson)
        //{
        //    if (listPerson is null)
        //        return;

        //    //RepositoryMSSQL<WorkTabel> repoTabel = new RepositoryMSSQL<WorkTabel>(db);
        //    RepositoryMSSQL<TabelPerson> repoTabPerson = new RepositoryMSSQL<TabelPerson>(db);
        //    IEnumerable<TabelPerson> ListTabelPerson;
        //    //WorkTabel tabel;

        //    if (_SelectedOtdel is null)
        //    {
        //        ListTabelPerson = repoTabPerson.Items
        //            .AsNoTracking()
        //            .Where(it => it.tabel.t_year == _SelectYear && it.tabel.t_month == _SelectMonth);

        //        //tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == _SelectYear
        //        //    && it.t_month == _SelectMonth);
        //    }
        //    else
        //    {
        //        ListTabelPerson = repoTabPerson.Items
        //            .AsNoTracking()
        //            .Where(it => it.tabel.t_year == _SelectYear && it.tabel.t_month == _SelectMonth 
        //                && it.tabel.t_otdel_id == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));

        //        //tabel = repoTabel.Items.AsNoTracking().FirstOrDefault(it => it.t_year == _SelectYear
        //        //    && it.t_month == _SelectMonth
        //        //    && it.t_otdel_id == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));
        //    }


        //    //if (listPerson is null || tabel is null) return;
        //    if (listPerson is null || ListTabelPerson is null) return;

        //    // получение количества рабочих дней в указанном месяце
        //    RepositoryCalendar repoCal = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
        //    var listDays = repoCal.GetListDays(_SelectYear, _SelectMonth);
        //    int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);

        //    foreach (var item in listPerson)
        //    {
        //        //var pers = tabel.tabelPersons.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
        //        var pers = ListTabelPerson.FirstOrDefault(it => it.tp_person_id == item.md_personalId);
        //        if (pers != null)
        //        {
        //            item.TabelDays = listDays.Count;
        //            item.TabelHours = pers.HoursMonth;
        //            item.TabelWorkOffDay = pers.WorkedOffDays;
        //            if (item.TabelWorkOffDay > 0)
        //            {
        //                item.md_tarif_offDay = pers.person.category?.cat_tarif * 8;
        //                if (item.md_tarif_offDay < 1500)
        //                    item.md_tarif_offDay = 1500;
        //            }


        //            item.OverHours = pers.OverWork ?? 0;
        //            item.md_Oklad = item.person.category is null ? 0 : item.TabelHours * item.person.category.cat_tarif.Value * item.person.p_stavka;

        //            int CountWorkDaysPerson = pers.TabelDays.Count(it => it.td_KindId == TabelKindDays.Worked);
        //            item.TabelAbsent = CountWorkDays - CountWorkDaysPerson;
        //            if (item.TabelAbsent < 0) item.TabelAbsent = 0;
        //        }
        //    }

        //}

        //-------------------------------------------------------------------------------------------------------
        // подгрузка данных из графика смен
        //-------------------------------------------------------------------------------------------------------
        //private void LoadFromSmena(ICollection<ModPerson> listPerson)
        //{
        //    if (listPerson is null)
        //        return;

        //    RepositoryMSSQL<Smena> repoSmena = new RepositoryMSSQL<Smena>(db);
        //    Smena smena;


        //    if (_SelectedOtdel is null)
        //    {
        //        smena = repoSmena.Items.AsNoTracking().FirstOrDefault(it => it.sm_Year == _SelectYear
        //            && it.sm_Month == _SelectMonth);
        //    }
        //    else
        //    {
        //        smena = repoSmena.Items.AsNoTracking().FirstOrDefault(it => it.sm_Year == _SelectYear
        //            && it.sm_Month == _SelectMonth
        //            && it.sm_OtdelId == (_SelectedOtdel.ot_parent ?? _SelectedOtdel.id));
        //    }

        //    if (listPerson is null || smena is null) return;

        //    foreach (var item in listPerson)
        //    {
        //        item.premiaNight.Initialize(smena.id);
        //    }

        //}

    }
}
