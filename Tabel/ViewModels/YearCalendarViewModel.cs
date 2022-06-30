using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views.Admins;

namespace Tabel.ViewModels
{
    internal class YearCalendarViewModel : ViewModel
    {
        private readonly IRepository<WorkCalendar> RepoCal;

        IEnumerable<WorkCalendar> WorkCalendars;


        private Dictionary<int, MonthControl.TypeDays> _exDays1;
        public Dictionary<int, MonthControl.TypeDays> exDays1 { get => _exDays1; set { Set(ref _exDays1, value); } }

        private Dictionary<int, MonthControl.TypeDays> _exDays2;
        public Dictionary<int, MonthControl.TypeDays> exDays2 { get => _exDays2; set { Set(ref _exDays2, value); } }

        private Dictionary<int, MonthControl.TypeDays> _exDays3;
        public Dictionary<int, MonthControl.TypeDays> exDays3 { get => _exDays3; set { Set(ref _exDays3, value); } }

        private Dictionary<int, MonthControl.TypeDays> _exDays4;
        public Dictionary<int, MonthControl.TypeDays> exDays4 { get => _exDays4; set { Set(ref _exDays4, value); } }

        private Dictionary<int, MonthControl.TypeDays> _exDays5;
        public Dictionary<int, MonthControl.TypeDays> exDays5 { get => _exDays5; set { Set(ref _exDays5, value); } }

        
        private Dictionary<int, MonthControl.TypeDays> _exDays6;
        public Dictionary<int, MonthControl.TypeDays> exDays6 { get => _exDays6; set { Set(ref _exDays6, value); } }

        
        private Dictionary<int, MonthControl.TypeDays> _exDays7;
        public Dictionary<int, MonthControl.TypeDays> exDays7 { get => _exDays7; set { Set(ref _exDays7, value); } }


        private Dictionary<int, MonthControl.TypeDays> _exDays8;
        public Dictionary<int, MonthControl.TypeDays> exDays8 { get => _exDays8; set { Set(ref _exDays8, value); } }


        private Dictionary<int, MonthControl.TypeDays> _exDays9;
        public Dictionary<int, MonthControl.TypeDays> exDays9 { get => _exDays9; set { Set(ref _exDays9, value); } }


        private Dictionary<int, MonthControl.TypeDays> _exDays10;
        public Dictionary<int, MonthControl.TypeDays> exDays10 { get => _exDays10; set { Set(ref _exDays10, value); } }


        private Dictionary<int, MonthControl.TypeDays> _exDays11;
        public Dictionary<int, MonthControl.TypeDays> exDays11 { get => _exDays11; set { Set(ref _exDays11, value); } }


        private Dictionary<int, MonthControl.TypeDays> _exDays12;
        public Dictionary<int, MonthControl.TypeDays> exDays12 { get => _exDays12; set { Set(ref _exDays12, value); } }




        List<Dictionary<int, MonthControl.TypeDays>> ListMonthDays; 


        public YearCalendarViewModel()
        {
            RepoCal = new RepositoryMSSQL<WorkCalendar>();

            WorkCalendars = RepoCal.Items.Where(it => it.cal_year == 2022);

            ListMonthDays = new List<Dictionary<int, MonthControl.TypeDays>>
            {
                exDays1, exDays2, exDays3, exDays4, exDays5, exDays6, exDays7, exDays8,
                exDays9, exDays10, exDays11, exDays12
            };



            //Dictionary<int, MonthControl.TypeDays> exDays = new Dictionary<int, MonthControl.TypeDays>();
            //exDays.Add(12, MonthControl.TypeDays.Short);
            //exDays.Add(7, MonthControl.TypeDays.Work);
            //exDays.Add(15, MonthControl.TypeDays.Holyday);
        }

        private void SetDaysForMonth(IEnumerable<WorkCalendar> ListDays)
        {
            foreach(var item in d)
            {
            }

        }

    }
}
