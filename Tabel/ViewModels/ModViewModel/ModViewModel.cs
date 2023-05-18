using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    public abstract class ModViewModel : ViewModel
    {
        protected Otdel _SelectedOtdel;
        protected int _SelectMonth;
        protected int _SelectYear;
        protected readonly BaseModel db;

        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public ModViewModel(BaseModel ctx)
        {
            db = ctx;
        }



        //-------------------------------------------------------------------------------------------------------
        // Изменение списка сотрудников
        //-------------------------------------------------------------------------------------------------------
        public abstract void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel);

        //-------------------------------------------------------------------------------------------------------
        // Добавление сотрудника
        //-------------------------------------------------------------------------------------------------------
        public abstract void AddPersons(ICollection<ModPerson> listPerson);


    }
}
