﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.ViewModels.Base
{
    public interface IBaseUCViewModel
    {
        void OtdelChanged(Otdel SelectOtdel, int Year, int Month);

        bool ClosingFrom();

        void SaveForm();
    }
}
