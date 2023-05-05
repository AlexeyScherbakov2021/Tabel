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
    internal class ModFunction
    {
        //private readonly BaseModel _db;
        private readonly int _year;
        private readonly int _month;
        static decimal HoursDefault;

        private static readonly decimal MinTarifOffDay = 1500;

        RepositoryMSSQL<TransPerson> repoTransPerson;
        RepositoryMSSQL<TabelPerson> repoTabPerson;
        RepositoryMSSQL<SmenaPerson> repoSmenaPerson;
        RepositoryMSSQL<CategorySet> repoCategorySet;
        RepositoryCalendar repoCal;

        //--------------------------------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------------------------------
        public ModFunction(BaseModel db, int year, int month)
        {
            //_db = db;
            _year = year;
            _month = month;
            repoTransPerson = new RepositoryMSSQL<TransPerson>(db);
            repoTabPerson = new RepositoryMSSQL<TabelPerson>(db);
            repoSmenaPerson = new RepositoryMSSQL<SmenaPerson>(db);
            repoCategorySet = new RepositoryMSSQL<CategorySet>(db);

            repoCal = new RepositoryCalendar(db);

            var listDays = repoCal.GetListDays(_year, _month);
            HoursDefault = listDays.Where(it => it.KindDay == TypeDays.Work).Count() * 8;
            HoursDefault += listDays.Where(it => it.KindDay == TypeDays.ShortWork).Count() * 7;

        }


        //--------------------------------------------------------------------------------------------------------
        // получение всех связанных данных
        //--------------------------------------------------------------------------------------------------------
        public void ModPersonFilling(IEnumerable<ModPerson> ListModPerson, CancellationToken token = default)
        {
            //try
            //{
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
                if(mPerson.Mod.m_IsClosed != true)
                    mPerson.md_cat_tarif = mPerson.person.category?.cat_tarif;

                //if (mPerson.premiaBonus is null)
                //{
                //    mPerson.premiaFP = new PremiaFP(mPerson);
                //    mPerson.premiaBonus = new PremiaBonus(mPerson);
                //    mPerson.premiaOtdel = new PremiaOtdel(mPerson);
                //    mPerson.premiStimul = new PremiaStimul(mPerson);
                //    mPerson.premOffDays = new PremOffDays(mPerson);
                //    mPerson.premiaAddWorks = new PremiaAddWorks(mPerson);
                //    mPerson.premiaTransport = new PremiaTransport(mPerson);
                //    mPerson.premiaNight = new premiaNight(mPerson);
                //    mPerson.premiaPrize = new PremiaPrize(mPerson);
                //}

                LoadFromTabel(mPerson);
                //LoadFromSmena(mPerson);
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

                //yield return mPerson;
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
            mPerson.TransportPremia = TranspPerson.Summa;
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
            //RepositoryMSSQL<TabelPerson> repoTabPerson = new RepositoryMSSQL<TabelPerson>(_db);
            TabelPerson TabPerson = repoTabPerson.Items
                .AsNoTracking()
                .FirstOrDefault(it => it.person.id == mPerson.person.id && it.tabel.t_year == _year && it.tabel.t_month == _month);

            if (TabPerson is null) return;

            // получение количества рабочих дней в указанном месяце
            //RepositoryCalendar repoCal = new RepositoryCalendar(_db);// AllRepo.GetRepoCalendar();
            var listDays = repoCal.GetListDays(_year, _month);
            int CountWorkDays = listDays.Count(it => it.KindDay != TypeDays.Holyday);   // число рабочих дней из календаря
            
            mPerson.TabelDays = listDays.Count;                     // дни из табеля
            mPerson.AddingHours = (TabPerson.tp_AddingHours ?? 0);
            mPerson.TabelHours = TabPerson.HoursMonth + mPerson.AddingHours;              // часы из табеля
            mPerson.TabelWorkOffDay = TabPerson.WorkedOffDays;      // отработанные выходные дни
            
            SetTarifOffDay(mPerson);
            //if (mPerson.TabelWorkOffDay > 0)
            //{
            //    // установка тарифа за выходной день
            //    mPerson.md_tarif_offDay = mPerson.person.category?.cat_tarif * 8;
            //    // не меньше установленного тарифа
            //    if (mPerson.md_tarif_offDay < MinTarifOffDay)
            //        mPerson.md_tarif_offDay = MinTarifOffDay;
            //}

            mPerson.OverHours = TabPerson.OverWork ?? 0;            // часы переработки

            SetOklad(mPerson);

            // поллучение оплаты переработанных часов
            //if (mPerson.person.category != null)
            //{
                mPerson.PereWorkHours15 = TabPerson.WorkedHours15;
                mPerson.PereWorkHours2 = TabPerson.WorkedHours2;
                mPerson.PereWork15 = TabPerson.WorkedHours15 * mPerson.md_cat_tarif /*mPerson.person.category.cat_tarif*/ * 0.5m;           // переработтка 1.5 часа
                mPerson.PereWork2 = TabPerson.WorkedHours2 * mPerson.md_cat_tarif /*mPerson.person.category.cat_tarif*/;             // переработка 2 часа
            //}

            //mPerson.md_Oklad = mPerson.person.category is null      // установка оклада по часам из тарифа грейда
            //    ? 0 
            //    : mPerson.TabelHours * mPerson.person.category.cat_tarif.Value * mPerson.person.p_stavka;

            //int CountWorkDaysPerson = TabPerson.TabelDays.Count(it => it.td_KindId == (int)TabelKindDays.Worked
            //  || it.td_KindId == (int)TabelKindDays.DistWork);   // количество отработанных дней

            mPerson.TabelAbsent = TabPerson.TabelDays.Count(it => it.td_KindId == (int)TabelKindDays.Bolnich
              || it.td_KindId == (int)TabelKindDays.Otpusk
              || it.td_KindId == (int)TabelKindDays.OtpuskNoMoney
              || it.td_KindId == (int)TabelKindDays.DopOtpusk
              || it.td_KindId == (int)TabelKindDays.BolnichNoMoney
              );


            //mPerson.TabelAbsent = CountWorkDays - CountWorkDaysPerson;                      // получение количества дней отсутствия
            if (mPerson.TabelAbsent < 0) mPerson.TabelAbsent = 0;

            // получение соответствующего сотрудника из графика смен
            //RepositoryMSSQL<SmenaPerson> repoSmenaPerson = new RepositoryMSSQL<SmenaPerson>(_db);
            SmenaPerson SmenaPerson = repoSmenaPerson.Items
                .AsNoTracking()
                .FirstOrDefault(it => it.personal.id == mPerson.person.id && it.smena.sm_Year == _year && it.smena.sm_Month == _month);

            if (SmenaPerson is null) return;

            mPerson.premiaNight.NightOklad = mPerson.md_cat_tarif /*mPerson.person?.category?.cat_tarif*/ * 0.2m;

            // отработанные дни
            var listWorkDays = TabPerson.TabelDays.Where(it => it.td_KindId == (int)TabelKindDays.Worked && it.td_Hours > 0);
            decimal? HightHours = 0;
            foreach(var item in listWorkDays)
            {
                if(SmenaPerson.SmenaDays.Any(s => s.sd_Kind == SmenaKind.Second && s.sd_Day == item.td_Day) )
                {
                    decimal? hours = item.td_Hours - 3.5m;
                    HightHours += hours < 0 ? 0 : hours;
                }
            }

            mPerson.premiaNight.NightHours = HightHours; //SmenaPerson.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;

        }



        //--------------------------------------------------------------------------------------------------------
        // установка тарифа выходного дня
        //--------------------------------------------------------------------------------------------------------
        public static void SetTarifOffDay(ModPerson mPerson)
        {
            if (mPerson.TabelWorkOffDay > 0 && mPerson.Mod.m_IsClosed != true)
            {
                // установка тарифа за выходной день
                mPerson.md_tarif_offDay = (mPerson.md_cat_tarif /*mPerson.person.category?.cat_tarif*/ + (mPerson.md_person_achiev / 162 ?? 0)) * 8;
                // не меньше установленного тарифа
                if (mPerson.md_tarif_offDay < MinTarifOffDay)
                    mPerson.md_tarif_offDay = MinTarifOffDay;
            }
        }


        //---------------------------------------------------------------------------
        // установка размера оклада, учитывается добавка из доп.работ
        //---------------------------------------------------------------------------
        public static void SetOklad(ModPerson mPerson)
        {
            //if (mPerson.Mod.m_IsClosed == true) return;

            decimal hours =  mPerson.TabelHours;

            //decimal hours = mPerson.person.p_type_id == SpecType.ИТР && mPerson.Mod.m_year >= 2023 && mPerson.Mod.m_month > 2
            //    ? 162 + mPerson.AddingHours
            //    : mPerson.TabelHours;

            if(mPerson.person?.p_type_id == SpecType.ИТР && mPerson.Mod.m_year >= 2023 && mPerson.Mod.m_month > 2)
            {
                hours = 162 + mPerson.AddingHours;
                decimal DiffHours = HoursDefault - (mPerson.TabelHours - mPerson.AddingHours);
                if (DiffHours > 0)
                {
                    decimal? oklad = mPerson.md_cat_tarif * 162;
                    decimal? cost_hours = oklad / (HoursDefault - mPerson.AddingHours);
                    oklad -= DiffHours * cost_hours;
                    mPerson.md_Oklad = oklad * mPerson.person.p_stavka;
                }
                else
                    mPerson.md_Oklad = hours * mPerson.md_cat_tarif * mPerson.person.p_stavka;
            }
            else
            {
                mPerson.md_Oklad = mPerson.md_cat_tarif /*mPerson.person.category*/ is null      // установка оклада по часам из тарифа грейда
                    ? 0
                    : (hours * mPerson.md_cat_tarif /*mPerson.person.category.cat_tarif.Value*/ /*+ (mPerson.md_person_achiev / 162 ?? 0)*/) * mPerson.person.p_stavka;
            }



        }

    }
}
