using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models2;

namespace Tabel.Models2
{
    public partial class TabelPerson : Observable
    {
        //[NotMapped]
        public int DaysWeek1 => ((IEnumerable<TabelDay>) TabelDays).Count(s => s.td_KindId == 1  && s.td_Day <= 15);

        public int DaysWeek2 => ((IEnumerable<TabelDay>)TabelDays).Count(s => s.td_KindId == 1  && s.td_Day > 15);

        public int HoursWeek1 => ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Day <= 15).Sum(s => s.td_Hours);

        public int HoursWeek2 => ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Day > 15 ).Sum(s => s.td_Hours);

        public int DaysMonth => DaysWeek1 + DaysWeek2;

        public int HoursMonth => HoursWeek1 + HoursWeek2;


        public int WorkedHours1
        {
            get
            {
                return ((IEnumerable<TabelDay>)TabelDays).Select(it => it.td_Hours).Sum() - WorkedOffDays;

            }
        }
        public int WorkedHours15
        {
            get
            {
                int summa = 0;
                List<int> HoursBig = ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Hours > 8).Select(s => s.td_Hours - 8).ToList();
                foreach(int i in HoursBig)
                {
                    summa += i > 2 ? 2 : i;
                }
                return summa;
            }
        }
        
        public int WorkedHours2
        {
            get
            {
                int summa = 0;
                List<int> HoursBig = ((IEnumerable<TabelDay>)TabelDays).Where(it => it.td_Hours > 10).Select(s => s.td_Hours - 10).ToList();
                foreach (int i in HoursBig)
                {
                    summa += i;
                }
                return summa;

            }
        }
       
        
        public int WorkedOffDays => ((IEnumerable<TabelDay>) TabelDays).Where(it => it.td_KindId == 5).Sum(s => s.td_Hours);


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
        }


        public void SetCalendarHours()
        {

        }


    }
}
