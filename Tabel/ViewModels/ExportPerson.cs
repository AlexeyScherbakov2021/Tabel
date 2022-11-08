using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

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
            tab_number = tabPerson.person.p_tab_number;
            lastname = tabPerson.person.p_lastname;
            name = tabPerson.person.p_name;
            midname = tabPerson.person.p_midname;
            NamePremia = "ПремияДТ";
            Summa = summa;
        }

        public string GetLine(CultureInfo ci)
        {
             return $"{tab_number}," +
                    $"{lastname}," +
                    $"{name}," +
                    $"{midname}," +
                    $"{NamePremia}," +
                    Summa.ToString("0.00",ci);
        }

    }

    internal class FormExport
    {
        public List<ExportPerson> ListExportPerson;

        public FormExport()
        {
            ListExportPerson = new List<ExportPerson>();
        }

        public void ListPersonToListExport(IEnumerable<TabelPerson> ListPersonal, IEnumerable<ModPerson> ListModPerson)
        {
            decimal summa = 0;
            ListExportPerson.Clear();
            foreach(var item in ListPersonal)
            {
                ModPerson mPerson = ListModPerson.FirstOrDefault(it => it.person.id == item.person.id);
                if (mPerson != null)
                {
                    summa = (mPerson.Itogo ?? 0) / item.HoursMonth * (item.OverWork ?? 0);
                }

                ExportPerson p = new ExportPerson(item, summa);
                ListExportPerson.Add(p);
            }

        }
    }

}
