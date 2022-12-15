using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models.Mod;
using Tabel.Models;
using Tabel.Repository;

namespace Tabel.ViewModels
{
    internal class ExportPerson
    {
        private string tab_number;
        private string lastname;
        private string name;
        private string midname;
        public string NamePremia;
        private decimal Summa;

        public ExportPerson(TabelPerson tabPerson, decimal summa)
        {
            tab_number = tabPerson.person.p_tab_number?.Trim();
            lastname = tabPerson.person.p_lastname?.Trim();
            name = tabPerson.person.p_name?.Trim();
            midname = tabPerson.person.p_midname?.Trim();
            NamePremia = "ПрмДТ";
            Summa = summa;
        }

        public string GetLine(CultureInfo ci, string Delim)
        {
             return $"{tab_number}" + Delim +
                    $"{lastname}" + Delim +
                    $"{name}" + Delim +
                    $"{midname}" + Delim +
                    $"{NamePremia}" + Delim +
                    Summa.ToString("0.00",ci);
        }

    }

    internal class FormExport
    {
        public List<ExportPerson> ListExportPerson;

        //------------------------------------------------------------------------------------------
        // конструктор
        //------------------------------------------------------------------------------------------
        public FormExport()
        {
            ListExportPerson = new List<ExportPerson>();
        }


        //------------------------------------------------------------------------------------------
        // Перенос данных в структуры для экспорта
        //------------------------------------------------------------------------------------------
        public void ListPersonToListExport( int SelectYear, int SelectMonth, decimal? bonusProc )
        {

            RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
            IEnumerable<TabelPerson> ListTabelPerson = repoTabelPerson.Items
                .AsNoTracking()
                .Where(it => it.tabel.t_year == SelectYear && it.tabel.t_month == SelectMonth)
                .OrderBy(o => o.person.p_lastname)
                .ThenBy(o => o.person.p_name);

            var db = repoTabelPerson.GetDB();

            RepositoryMSSQL<ModPerson> repoModPerson = new RepositoryMSSQL<ModPerson>(db);
            List<ModPerson> ListModPerson = repoModPerson.Items
                .AsNoTracking()
                .Where(it => it.Mod.m_year == SelectYear && it.Mod.m_month == SelectMonth)
                .ToList();

            RepositoryMSSQL<TransPerson> repoTransPerson = new RepositoryMSSQL<TransPerson>(db);
            List<TransPerson> ListTransPerson = repoTransPerson.Items
                .AsNoTracking()
                .Where(it => it.Transport.tr_Year == SelectYear && it.Transport.tr_Month == SelectMonth)
                .ToList();


            decimal summa = 0;
            ListExportPerson.Clear();
            foreach(var item in ListTabelPerson)
            {
                ModPerson mPerson = ListModPerson.FirstOrDefault(it => it.person.id == item.person.id);
                TransPerson tPerson = ListTransPerson.FirstOrDefault(it => it.person.id == item.person.id);

                if (mPerson != null)
                {
                    mPerson.TabelHours = item.HoursMonth;
                    mPerson.TabelDays= item.DaysMonth;
                    //mPerson.TabelAbsent = item.
                    mPerson.OverHours = item.OverWork.Value;
                    mPerson.Oklad = mPerson.person.category is null 
                        ? 0 
                        : mPerson.TabelHours * item.person.category.cat_tarif.Value * mPerson.person.p_stavka;
                    mPerson.TransportPremia = tPerson?.Summa;
                    mPerson.premiaBonus.BonusForAll = bonusProc;
                    summa = mPerson.PremiaItogo.Value;

                    //summa = mPerson.Oklad / item.HoursMonth * (item.OverWork ?? 0) * 2;

                    if (summa > 0 && !String.IsNullOrEmpty( mPerson.person.p_tab_number))
                    {
                        ExportPerson p = new ExportPerson(item, summa);
                        ListExportPerson.Add(p);
                    }
                }

            }

        }


        //------------------------------------------------------------------------------------------
        // Перенос данных в структуры для экспорта для Внетарифов
        //------------------------------------------------------------------------------------------
        public void ListPersonSeparToListExport( int SelectYear, int SelectMonth)
        {

            RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
            IEnumerable<TabelPerson> ListTabelPerson = repoTabelPerson.Items
                .AsNoTracking()
                .Where(it => it.tabel.t_year == SelectYear && it.tabel.t_month == SelectMonth && it.tabel.otdel.ot_itr == 1)
                .OrderBy(o => o.person.p_lastname)
                .ThenBy(o => o.person.p_name);

            var db = repoTabelPerson.GetDB();

            RepositoryMSSQL<SeparPerson> repoSeparPerson = new RepositoryMSSQL<SeparPerson>(db);
            List<SeparPerson> ListModPerson = repoSeparPerson.Items
                .AsNoTracking()
                .Where(it => it.Separate.s_year == SelectYear && it.Separate.s_month == SelectMonth)
                .ToList();



            decimal summa = 0;
            ListExportPerson.Clear();
            foreach(var item in ListTabelPerson)
            {
                SeparPerson sPerson = ListModPerson.FirstOrDefault(it => it.person.id == item.person.id);

                if (sPerson != null)
                {
                    sPerson.TabelHours = item.HoursMonth;
                    sPerson.TabelDays= item.DaysMonth;
                    sPerson.Oklad = sPerson.person.category is null 
                        ? 0 
                        : sPerson.TabelHours * item.person.category.cat_tarif.Value * sPerson.person.p_stavka;

                    //summa = mPerson.Oklad / item.HoursMonth * (item.OverWork ?? 0) * 2;

                    if (sPerson.Oklad > 0 && !String.IsNullOrEmpty( sPerson.person.p_tab_number))
                    {
                        ExportPerson p = new ExportPerson(item, sPerson.Oklad.Value);
                        ListExportPerson.Add(p);
                    }
                }

            }

        }
    }

}
