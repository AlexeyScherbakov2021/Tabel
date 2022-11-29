﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    internal abstract class ModViewModel : ViewModel
    {
        protected Otdel _SelectedOtdel;
        protected int _SelectMonth;
        protected int _SelectYear;
        protected readonly BaseModel db;

        public ModViewModel(BaseModel ctx)
        {
            db = ctx;
        }


        public abstract void ChangeListPerson(ObservableCollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel);
    }
}