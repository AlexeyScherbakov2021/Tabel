using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;

namespace Tabel.Models2
{
    public partial class TabelPerson : Observable
    {
        //[NotMapped]
        public int DaysWeek1 => ((IEnumerable<TabelDay>) TabelDays).Count(s => s.td_KindId == 1  && s.td_Day <= 15);

        public int DaysWeek2 => ((IEnumerable<TabelDay>)TabelDays).Count(s => s.td_KindId == 1  && s.td_Day > 15);

        public decimal HoursWeek1 => ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Day <= 15).Sum(s => s.td_Hours.Value);

        public decimal HoursWeek2 => ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Day > 15 ).Sum(s => s.td_Hours.Value);

        public int DaysMonth => DaysWeek1 + DaysWeek2;

        public decimal HoursMonth => HoursWeek1 + HoursWeek2;


        public decimal? WorkedHours1
        {
            get
            {
                return ((IEnumerable<TabelDay>)TabelDays).Select(it => it.td_Hours.Value).Sum() - WorkedOffHours;

            }
        }
        public decimal? WorkedHours15
        {
            get
            {
                int summa = 0;
                List<decimal> HoursBig = ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Hours.Value > 8).Select(s => s.td_Hours.Value - 8).ToList();
                foreach(int i in HoursBig)
                {
                    summa += i > 2 ? 2 : i;
                }
                return summa;
            }
        }
        
        public decimal WorkedHours2
        {
            get
            {
                int summa = 0;
                List<decimal> HoursBig = ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Hours.Value > 10).Select(s => s.td_Hours.Value - 10).ToList();
                foreach (int i in HoursBig)
                {
                    summa += i;
                }
                return summa;

            }
        }


        [NotMapped]
        public decimal? WorkedOffDays => ((IEnumerable<TabelDay>) TabelDays).Count(it => it.td_KindId == 5);
        public decimal? WorkedOffHours => ((IEnumerable<TabelDay>) TabelDays).Where(it => it.td_KindId == 5).Sum(s => s.td_Hours.Value);


        public void UpdateUI()
        {

            OnPropertyChanged("DaysWeek1");
            OnPropertyChanged("DaysWeek2");
            OnPropertyChanged("HoursWeek1");
            OnPropertyChanged("HoursWeek2");
            OnPropertyChanged("DaysMonth");
            OnPropertyChanged("HoursMonth");
            OnPropertyChanged("WorkedHours1");
            OnPropertyChanged("WorkedHours15");
            OnPropertyChanged("WorkedHours2");
            OnPropertyChanged("WorkedOffDays");
            OnPropertyChanged("WorkedOffHours");
        }



        public void SetCalendarTypeDays()
        {
            RepositoryCalendar repo = new RepositoryCalendar();
            IEnumerable<WorkCalendar> cal = repo.Items.AsNoTracking().Where(it => it.cal_date.Year == tabel.t_year 
                    && it.cal_date.Month == tabel.t_month);

            // выставление выходных дней для списка
            List<TabelDay> days = TabelDays.ToList();
            DateTime dt = new DateTime(tabel.t_year, tabel.t_month, 1);
            while(dt.Month == tabel.t_month)
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                    days[dt.Day - 1].CalendarTypeDay = TypeDays.Holyday;
                else
                    days[dt.Day - 1].CalendarTypeDay = TypeDays.Work;
                dt = dt.AddDays(1);
            }

            // корректировка измененных дней
            foreach(var item in cal)
            {
                days[item.cal_date.Day - 1].CalendarTypeDay = item.cal_type;
            }

        }


    }
}
