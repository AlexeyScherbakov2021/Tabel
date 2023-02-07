using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.ViewModels.DataSummForm
{
    public interface IDataSumm
    {
        void ChangePeriod(int CurrentYear, int CurrentMonth);
        void Save();
        bool ClosingFrom();
    }
}
