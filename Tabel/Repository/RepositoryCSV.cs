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
        private string Delimiter = ";";

        public RepositoryCSV(FormExport form) => ListPersons = form;

        public void SaveFile(int Year, int Month)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Файл CSV (*.csv)|*.csv";
            sd.DefaultExt = "Файл CSV (*.csv)|*.csv";

            if (sd.ShowDialog() != true)
                return;

            var cult = new CultureInfo("ru-RU");
            cult.NumberFormat.NumberDecimalSeparator = ".";

            string line;
            var file = File.CreateText(sd.FileName);

            file.WriteLine($"{Month}" + Delimiter + $"{Year}");

            foreach(var item in ListPersons.ListExportPerson)
            {
                line = item.GetLine(cult, Delimiter);
                file.WriteLine(line);
            }
            file.Close();
        }

    }
}
