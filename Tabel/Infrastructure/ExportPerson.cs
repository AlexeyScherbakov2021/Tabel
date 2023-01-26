using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models.Mod;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.ModViewModel;

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

        public ExportPerson(ModPerson tabPerson, decimal summa)
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

            RepositoryMSSQL<ModPerson> repoModPerson = new RepositoryMSSQL<ModPerson>();
            var db = repoModPerson.GetDB();
            List<ModPerson> ListModPerson = repoModPerson.Items
                .AsNoTracking()
                .Where(it => it.Mod.m_year == SelectYear && it.Mod.m_month == SelectMonth)
                .OrderBy(o => o.person.p_lastname)
                .ToList();

            ModFunction mf = new ModFunction(db, SelectYear, SelectMonth);
            mf.ModPersonFilling(ListModPerson);

            ListExportPerson.Clear();

            foreach(var item in ListModPerson)
            {
                //decimal PremiaItogo = item.premiaBonus.GetPremia()
                //    + item.premiaFP.GetPremia()
                //    + item.premiaAddWorks.GetPremia()
                //    + item.premiStimul.GetPremia()
                //    + item.premiaOtdel.GetPremia()
                //    + item.premiaTransport.GetPremia()
                //    + item.premiaPrize.GetPremia();

                if (item.PremiaItogo > 0
                    && !String.IsNullOrEmpty(item.person.p_tab_number)
                    && IsAllDigits(item.person.p_tab_number))
                {
                    ExportPerson p = new ExportPerson(item, item.PremiaItogo.Value);
                    ListExportPerson.Add(p);
                }
            }
        }


        //------------------------------------------------------------------------------------------
        // Проверка строки на наличие только цифр
        //------------------------------------------------------------------------------------------
        private bool IsAllDigits(string StringWithNumber)
        {
            foreach(Char c in StringWithNumber.Trim())
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }


        //------------------------------------------------------------------------------------------
        // Перенос данных в структуры для экспорта для Внетарифов
        //------------------------------------------------------------------------------------------
        public void ListPersonSeparToListExport( int SelectYear, int SelectMonth)
        {

            RepositoryMSSQL<TabelPerson> repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
            IEnumerable<TabelPerson> ListTabelPerson = repoTabelPerson.Items
                .AsNoTracking()
                .Where(it => it.tabel.t_year == SelectYear && it.tabel.t_month == SelectMonth && it.tabel.otdel.ot_itr == 10)
                .OrderBy(o => o.person.p_lastname)
                .ThenBy(o => o.person.p_name);

            var db = repoTabelPerson.GetDB();

            RepositoryMSSQL<SeparPerson> repoSeparPerson = new RepositoryMSSQL<SeparPerson>(db);
            List<SeparPerson> ListModPerson = repoSeparPerson.Items
                .AsNoTracking()
                .Where(it => it.Separate.s_year == SelectYear && it.Separate.s_month == SelectMonth)
                .ToList();



            //decimal summa = 0;
            ListExportPerson.Clear();
            //foreach(var item in ListTabelPerson)
            //{
            //    SeparPerson sPerson = ListModPerson.FirstOrDefault(it => it.person.id == item.person.id);

            //    if (sPerson != null)
            //    {
            //        sPerson.Oklad = sPerson.person.category is null 
            //            ? 0 
            //            : sPerson.TabelHours * item.person.category.cat_tarif.Value * sPerson.person.p_stavka;

            //        //summa = mPerson.Oklad / item.HoursMonth * (item.OverWork ?? 0) * 2;

            //        if (sPerson.Oklad > 0 
            //            && !String.IsNullOrEmpty( sPerson.person.p_tab_number) 
            //            && IsAllDigits(sPerson.person.p_tab_number))
            //        {
            //            ExportPerson p = new ExportPerson(item, sPerson.Oklad.Value);
            //            ListExportPerson.Add(p);
            //        }
            //    }

            //}

        }
    }

}
