using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryCSV
    {
        private IEnumerable<TabelPerson> TabelPersons;

        public RepositoryCSV(IEnumerable<TabelPerson> tabel) => TabelPersons = tabel;

        public void SaveFile(string FileName, int Year, int Month)
        {
            string line;
            var file = File.CreateText(FileName);

            file.WriteLine($"{Month},{Year}");

            foreach(var item in TabelPersons)
            {
                line = $"{item.person.p_tab_number}," +
                    $"{item.person.p_lastname}," +
                    $"{item.person.p_name}," +
                    $"{item.person.p_midname}," +
                    $"ПремияДТ," +
                    $"{item.OverWork}";

                file.WriteLine(line);
            }
            file.Close();
        }

    }
}
