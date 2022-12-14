using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class SeparUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private bool IsModify;


        private readonly BaseModel db;

        public SeparUCViewModel()
        {
            IsModify = false;
        }

        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
        }

        public bool ClosingFrom()
        {
            return IsModify;
        }

        public void SaveForm()
        {
        }
    }
}
