using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels;

namespace Tabel.Repository
{
    internal class RepositoryCSV
    {
        private FormExport ListPersons;

        public RepositoryCSV(FormExport form) => ListPersons = form;

        public void SaveFile(string FileName, int Year, int Month)
        {
            var cult = new CultureInfo("ru-RU");
            cult.NumberFormat.NumberDecimalSeparator = ".";

            string line;
            var file = File.CreateText(FileName);

            file.WriteLine($"{Month},{Year}");

            foreach(var item in ListPersons.ListExportPerson)
            {
                line = item.GetLine(cult);
                file.WriteLine(line);
            }
            file.Close();
        }

    }
}
