using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Infrastructure
{
    public class Months
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public Months(string s, int n)
        {
            Number = n;
            Name = s;
        }

    }
}
