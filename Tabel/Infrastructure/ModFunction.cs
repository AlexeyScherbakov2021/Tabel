using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models.Mod;
using Tabel.Component.Models;
using Tabel.Component.MonthPanel;
using Tabel.Models;
using Tabel.Repository;
using DocumentFormat.OpenXml.EMMA;
using System.Threading;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Collections.ObjectModel;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Tabel.Infrastructure
{
    public class ModFunction
    {
        //private readonly BaseModel _db;
        private readonly int _year;
        private readonly int _month;

        public static decimal HoursDefault;
        public static int DaysDefault;
        public static int WorkDaysDefault;
        
        private readonly bool IsClosed;
        decimal? koeff15;
        decimal? koeff2;
        public static decimal? WorkOffKoeff;
        public static decimal? NightKoeff;

        private static readonly decimal MinTarifOffDay = 1500;

        RepositoryMSSQL<TransPerson> repoTransPerson;
        RepositoryMSSQL<TabelPerson> repoTabPerson;
        RepositoryMSSQL<SmenaPerson> repoSmenaPerson;
        RepositoryMSSQL<CategorySet> repoCategorySet;
        RepositoryCalendar repoCal;

        //--------------------------------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------------------------------
        public ModFunction()
        {
            koeff15 = 1.5m;
            koeff2 = 2;
        }


        public ModFunction(BaseModel db, int year, int month, bool IsClosed)
        {
            //_db = db;
            this.IsClosed = IsClosed;
            _year = year;
            _month = month;
            repoTransPerson = new RepositoryMSSQL<TransPerson>(db);
            repoTabPerson = new RepositoryMSSQL<TabelPerson>(db);
            repoSmenaPerson = new RepositoryMSSQL<SmenaPerson>(db);
            repoCategorySet = new RepositoryMSSQL<CategorySet>(db);

            repoCal = new RepositoryCalendar(db);

            var listDays = repoCal.GetListDays(_year, _month);
            WorkDaysDefault = listDays.Where(it => it.KindDay == TypeDays.Work).Count();
            HoursDefault = WorkDaysDefault * 8;
            HoursDefault += listDays.Where(it => it.KindDay == TypeDays.ShortWork).Count() * 7;
            WorkDaysDefault += listDays.Where(it => it.KindDay == TypeDays.ShortWork).Count();
            DaysDefault = listDays.Count;

            RepositoryMSSQL<GenChargMonth> repoGenChargMonth = new RepositoryMSSQL<GenChargMonth>();

            koeff15 = repoGenChargMonth.Items.Where(it => it.gm_GenId == (int)koeffPere.Pere15
                && it.gm_Month <= _month && it.gm_Year <= _year)
                .OrderByDescending(o => o.gm_Year)
                .ThenByDescending(o => o.gm_Month)
                .FirstOrDefault()?.gm_Value;

            koeff2 = repoGenChargMonth.Items.Where(it => it.gm_GenId == (int)koeffPere.Pere2
                && it.gm_Month <= _month && it.gm_Year <= _year)
                .OrderByDescending(o => o.gm_Year)
                .ThenByDescending(o => o.gm_Month)
                .FirstOrDefault()?.gm_Value;

            WorkOffKoeff = repoGenChargMonth.Items.Where(it => it.gm_GenId == (int)koeffPere.WorkOffKoeff
                && it.gm_Month <= _month && it.gm_Year <= _year)
                .OrderByDescending(o => o.gm_Year)
                .ThenByDescending(o => o.gm_Month)
                .FirstOrDefault()?.gm_Value;

            NightKoeff = repoGenChargMonth.Items.Where(it => it.gm_GenId == (int)koeffPere.NightKoeff
                && it.gm_Month <= _month && it.gm_Year <= _year)
                .OrderByDescending(o => o.gm_Year)
                .ThenByDescending(o => o.gm_Month)
                .FirstOrDefault()?.gm_Value;


        }


        //--------------------------------------------------------------------------------------------------------
        // получение всех связанных данных
        //--------------------------------------------------------------------------------------------------------
        public void ModPersonFilling(IEnumerable<ModPerson> ListModPerson, CancellationToken token = default)
        {

            if (IsClosed == true)
            {
                // для закрытого периода
                foreach (var mPerson in ListModPerson)
                {
                    mPerson.md_cat_tarif = mPerson.person.p_type_id == SpecType.N2
                        ? (mPerson.person.p_oklad ?? 0) / HoursDefault
                        : mPerson.md_cat_tarif ?? 0;
                    SetPereWork(mPerson);
                    //mPerson.pereWork15summ = mPerson.md_pereWork15 * mPerson.md_cat_tarif * koeff15 * mPerson.person.p_stavka;    // переработка 1.5 часа
                    //mPerson.pereWork2summ = mPerson.md_pereWork2 * mPerson.md_cat_tarif * koeff2 * mPerson.person.p_stavka;         // переработка 2 часа

                    mPerson.premiaNight.NightOklad = mPerson.md_cat_tarif * NightKoeff;
                    mPerson.premiaNight.NightHours = mPerson.md_nightHours;

                    mPerson.premOffDays.Calculation();
                    mPerson.premiaNight.Calculation();
                    mPerson.premiaBonus.Calculation();
                    mPerson.premiaOtdel.Calculation();
                    mPerson.premiStimul.Calculation();
                    mPerson.premiaTransport.Calculation();
                    mPerson.premiaAddWorks.Calculation();
                    mPerson.premiaPrize.Calculation();
                    mPerson.premiaFP.Calculation();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }

            }

            else
            {
                DateTime curDate = new DateTime(_year, _month, 1);
                IEnumerable<Category> ListCat = repoCategorySet.Items
                        .AsNoTracking()
                        .Where(it => it.cg_date <= curDate)
                        .OrderByDescending(it => it.cg_date)
                        .FirstOrDefault()
                        .ListCategory;

                foreach (var mPerson in ListModPerson)
                {
                    mPerson.person.category = ListCat.FirstOrDefault(it => it.idCategory == mPerson.person.p_cat_id);
                    mPerson.md_category = mPerson.person.category.idCategory;
                    mPerson.md_cat_tarif = mPerson.person.category.cat_tarif;
                    mPerson.md_stavka = mPerson.person.p_stavka;

                    mPerson.md_cat_tarif = mPerson.person.p_type_id == SpecType.N2
                        ? (mPerson.person.p_oklad ?? 0) / HoursDefault
                        : mPerson.md_cat_tarif ?? 0;

                    LoadFromTabel(mPerson);
                    LoadFromTransport(mPerson);

                    mPerson.premiaBonus.Calculation();
                    mPerson.premiaFP.Calculation();
                    mPerson.premiaAddWorks.Calculation();
                    mPerson.premiaOtdel.Calculation();
                    mPerson.premiStimul.Calculation();
                    mPerson.premiaTransport.Calculation();
                    mPerson.premiaPrize.Calculation();
                    mPerson.premOffDays.Calculation();
                    mPerson.premiaNight.Calculation();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }
  


        //--------------------------------------------------------------------------------------------------------
        // получение данных из использования транспорта
        //--------------------------------------------------------------------------------------------------------
        private void LoadFromTransport(ModPerson mPerson)
        {
            // получение соответствующего сотрудника
            //RepositoryMSSQL<TransPerson> repoTransPerson = new RepositoryMSSQL<TransPerson>(_db);
            TransPerson TranspPerson = repoTransPerson.Items
                .AsNoTracking()
                .FirstOrDefault(it => it.person.id == mPerson.person.id && it.Transport.tr_Year == _year && it.Transport.tr_Month == _month);

            if (TranspPerson is null) return;
            mPerson.md_summaTransport = TranspPerson.Summa;
        }

        //--------------------------------------------------------------------------------------------------------
        // получение данных из графика смен
        //--------------------------------------------------------------------------------------------------------
        //private void LoadFromSmena(ModPerson mPerson)
        //{
        //    // получение соответствующего сотрудника
        //    RepositoryMSSQL<SmenaPerson> repoSmenaPerson = new RepositoryMSSQL<SmenaPerson>(_db);
        //    SmenaPerson SmenaPerson = repoSmenaPerson.Items
        //        .AsNoTracking()
        //        .FirstOrDefault(it => it.personal.id == mPerson.person.id && it.smena.sm_Year == _year && it.smena.sm_Month == _month);

        //    if (SmenaPerson is null) return;

        //    mPerson.premiaNight.NightOklad = mPerson.person?.category?.cat_tarif * 0.2m;

        //    var listDaysSmena = SmenaPerson.SmenaDays.Where(s => s.sd_Kind == SmenaKind.Second);



        //    mPerson.premiaNight.NightHours = SmenaPerson.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;

        //}

        //--------------------------------------------------------------------------------------------------------
        // получение данных из табеля
        //--------------------------------------------------------------------------------------------------------
        private void LoadFromTabel(ModPerson mPerson)
        {
            // получение соответствующего сотрудника из табеля
            TabelPerson TabPerson = repoTabPerson.Items
                .AsNoTracking()
                .FirstOrDefault(it => it.person.id == mPerson.person.id && it.tabel.t_year == _year && it.tabel.t_month == _month);

            if (TabPerson is null) return;

            // получение количества рабочих дней в указанном месяце
            var listDays = repoCal.GetListDays(_year, _month);
            int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);   // число рабочих дней из календаря

            // количество дней командировки
            //decimal hour_kom_calc = 0;
            //foreach (var dk in TabPerson.TabelDays)
            //{
            //    if (dk.td_KindId == (int)TabelKindDays.Komandir)
            //    {
            //        hour_kom_calc += (8 - dk.td_Hours.Value);
            //    }
            //}

            mPerson.md_workDays = listDays.Count;      // дни из табеля
            //mPerson.AddingHours = (TabPerson.tp_AddingHours ?? 0);
            mPerson.md_workHours = TabPerson.HoursMonth /*+ mPerson.AddingHours*/;        // часы из табеля
            mPerson.md_workOffHours = TabPerson.WorkedOffHours;
            mPerson.md_workOffDays = (int) Math.Ceiling((mPerson.md_workOffHours ?? 0) / 8);      // отработанные выходные дни

            mPerson.md_overHours = TabPerson.OverWork ?? 0;            // часы переработки

            // получение оплаты переработанных часов
            mPerson.md_pereWork15 = TabPerson.WorkedHours15;
            mPerson.md_pereWork2 = TabPerson.WorkedHours2;

            SetTarifOffDay(mPerson);
            SetPereWork(mPerson);
            SetOklad(mPerson);

            int CountWorkDaysPerson = TabPerson.TabelDays.Count(it => it.td_KindId == (int)TabelKindDays.Worked
                    || it.td_KindId == (int)TabelKindDays.DistWork);   // количество отработанных дней

            mPerson.md_absentDays = CountWorkDays - CountWorkDaysPerson;                      // получение количества дней отсутствия
            if (mPerson.md_absentDays < 0) mPerson.md_absentDays = 0;

            // получение соответствующего сотрудника из графика смен
            SmenaPerson SmenaPerson = repoSmenaPerson.Items
                .AsNoTracking()
                .FirstOrDefault(it => it.personal.id == mPerson.person.id && it.smena.sm_Year == _year && it.smena.sm_Month == _month);

            if (SmenaPerson is null) return;

            // кстановка ночного часового тарифа
            mPerson.premiaNight.NightOklad = mPerson.md_cat_tarif * NightKoeff;

            // отработанные дни
            var listWorkDays = TabPerson.TabelDays.Where(it => it.td_KindId == (int)TabelKindDays.Worked && it.td_Hours > 0);
            decimal HightHours = 0;
            foreach(var item in listWorkDays)
            {
                if(SmenaPerson.SmenaDays.Any(s => s.sd_Kind == SmenaKind.Second && s.sd_Day == item.td_Day) )
                {
                    decimal hours = item.td_Hours.Value - 3.5m;
                    HightHours += hours < 0 ? 0 : hours;
                }
            }

            mPerson.premiaNight.NightHours = HightHours;
            mPerson.md_nightHours = HightHours;

        }


        //--------------------------------------------------------------------------------------------------------
        // установка сверхурочных
        //--------------------------------------------------------------------------------------------------------
        public void SetPereWork(ModPerson mPerson)
        {
            mPerson.pereWork15summ = mPerson.md_pereWork15 * mPerson.md_cat_tarif * koeff15 * mPerson.person.p_stavka;           // переработтка 1.5 часа
            mPerson.pereWork2summ = mPerson.md_pereWork2 * mPerson.md_cat_tarif * koeff2 * mPerson.person.p_stavka;             // переработка 2 часа
        }



        //--------------------------------------------------------------------------------------------------------
        // установка тарифа выходного дня
        //--------------------------------------------------------------------------------------------------------
        public static void SetTarifOffDay(ModPerson mPerson)
        {

            mPerson.md_tarif_offDay = mPerson.md_cat_tarif * mPerson.person.p_stavka;

            //if (mPerson.md_workOffDays > 0 )
            //{
            //    decimal minTarif = MinTarifOffDay * mPerson.person.p_stavka;

            //    // установка тарифа за выходной день
            //    mPerson.md_tarif_offDay = (mPerson.md_cat_tarif /*+ (mPerson.md_person_achiev / 162 ?? 0)*/) * 8 * mPerson.person.p_stavka;
            //    // не меньше установленного тарифа
            //    if (mPerson.md_tarif_offDay < minTarif)
            //        mPerson.md_tarif_offDay = minTarif;
            //}
        }


        //---------------------------------------------------------------------------
        // установка размера оклада, учитывается добавка из доп.работ
        //---------------------------------------------------------------------------
        public static void SetOklad(ModPerson mPerson)
        {
            //if (mPerson.Mod.m_IsClosed == true) return;

            decimal hours;

            //decimal hours = mPerson.person.p_type_id == SpecType.ИТР && mPerson.Mod.m_year >= 2023 && mPerson.Mod.m_month > 2
            //    ? 162 + mPerson.AddingHours
            //    : mPerson.TabelHours;

            if(mPerson.person?.p_type_id == SpecType.N2 /*&& mPerson.Mod.m_year >= 2023 && mPerson.Mod.m_month > 2*/)
            {
                //hours = 162 + mPerson.AddingHours;
                decimal DiffHours = HoursDefault - mPerson.md_workHours /*- mPerson.AddingHours)*/;
                if (DiffHours > 0)
                {
                    //decimal? oklad = mPerson.person.p_oklad;
                    //decimal? cost_hours = oklad / HoursDefault /*- mPerson.AddingHours)*/;
                    //oklad -= DiffHours * cost_hours;
                    //mPerson.md_Oklad = oklad * mPerson.person.p_stavka;
                    mPerson.md_Oklad = mPerson.person.p_oklad - DiffHours * (mPerson.person.p_oklad / HoursDefault);
                    mPerson.md_addition = mPerson.person.p_addition - DiffHours * (mPerson.person.p_addition / HoursDefault);
                }
                else
                {
                    mPerson.md_Oklad = mPerson.person.p_oklad * mPerson.person.p_stavka;
                    mPerson.md_addition = mPerson.person.p_addition;
                }

                mPerson.md_premia = mPerson.md_Oklad * mPerson.person.p_premia / 100;

            }
            else
            {
                hours = mPerson.md_workHours
                    - (mPerson.md_pereWork15 ?? 0)
                    - (mPerson.md_pereWork2 ?? 0)
                    - (mPerson.md_workOffHours ?? 0)
                    ;

                mPerson.md_Oklad = mPerson.md_cat_tarif is null      // установка оклада по часам из тарифа грейда
                    ? 0
                    : (hours * mPerson.md_cat_tarif /*+ (mPerson.md_person_achiev / 162 ?? 0)*/) * mPerson.person.p_stavka;
            }
        }
    }
}
