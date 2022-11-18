using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models.Mod;
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
                    mPerson.TabelHours = item.HoursMonth;
                    mPerson.OverHours = item.OverWork.Value;
                    mPerson.Oklad = mPerson.person.category is null 
                        ? 0 
                        : mPerson.TabelHours * item.person.category.cat_tarif.Value * mPerson.person.p_stavka;
                    PremiaPrize prem = new PremiaPrize(mPerson);
                    prem.Calculation();
                    summa = prem.GetPremia().Value;
                    //summa = mPerson.Oklad / item.HoursMonth * (item.OverWork ?? 0) * 2;

                    if (summa > 0)
                    {
                        ExportPerson p = new ExportPerson(item, summa);
                        ListExportPerson.Add(p);
                    }
                }

            }

        }
    }

}
