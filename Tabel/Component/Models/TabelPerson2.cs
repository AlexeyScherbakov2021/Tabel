using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;

namespace Tabel.Models
{
    public partial class TabelPerson : Observable
    {
        //[NotMapped]
        public int DaysWeek1 => ((IEnumerable<TabelDay>) TabelDays).Count(s => s.td_KindId == 1  && s.td_Day <= 15);

        public int DaysWeek2 => ((IEnumerable<TabelDay>)TabelDays).Count(s => s.td_KindId == 1  && s.td_Day > 15);

        public decimal HoursWeek1 => ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Day <= 15).Sum(s => s.WhiteHours);

        public decimal HoursWeek2 => ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Day > 15 ).Sum(s => s.WhiteHours);

        public int DaysMonth => DaysWeek1 + DaysWeek2;

        public decimal HoursMonth => HoursWeek1 + HoursWeek2;


        public decimal? WorkedHours1
        {
            get
            {
                return ((IEnumerable<TabelDay>)TabelDays).Select(it => it.WhiteHours).Sum() - WorkedOffHours;

            }
        }
        public decimal? WorkedHours15
        {
            get
            {
                decimal? summa = 0;
                List<decimal> HoursBig = ((IEnumerable<TabelDay>)TabelDays)
                    .Where(it => it.WhiteHours > 8 && it.CalendarTypeDay != TypeDays.Holyday)
                    .Select(s => s.WhiteHours - 8).ToList();
                foreach(decimal i in HoursBig)
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
                decimal? summa = 0;
                //summa = TabelDays?.Sum(it => it.td_Hours2);

                List<decimal> HoursBig = ((IEnumerable<TabelDay>)TabelDays)
                    .Where(it => it.WhiteHours > 10 && it.CalendarTypeDay != TypeDays.Holyday)
                    .Select(s => s.WhiteHours - 10).ToList();

                foreach (decimal i in HoursBig)
                {
                    summa += i;
                }
                return summa ?? 0;

            }
        }


        [NotMapped]
        public decimal? WorkedOffDays => ((IEnumerable<TabelDay>) TabelDays).Count(it => it.td_KindId == 5);
        public decimal? WorkedOffHours => ((IEnumerable<TabelDay>) TabelDays)
            .Where(it => it.td_KindId == 5)
            .Sum(s => s.WhiteHours);

        [NotMapped]
        public decimal? OverWork => TabelDays?.Sum(it => it.td_Hours2);

        [NotMapped]
        public TabelDay PrevDay;        // количество часов в последний день предыдущего месяца
        [NotMapped]
        public int PrevPermWorkCount;       // количество дней непрерывной работы в последние дни месяца
        [NotMapped]
        public decimal? CalcHours { get; set; } // рассчитанные часы для 1С


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


    }
}
