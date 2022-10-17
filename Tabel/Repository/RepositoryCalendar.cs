using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.MonthPanel;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryCalendar : RepositoryMSSQL<WorkCalendar>
    {
        //---------------------------------------------------------------------------------------------
        // получение списка использованных годов
        //---------------------------------------------------------------------------------------------
        public IEnumerable<int> GetYears()
        {
            return db.calendars.Select(it => it.cal_date.Year).Distinct().OrderBy(o => o);
        }

        //---------------------------------------------------------------------------------------------
        // получение списка измененных дней в году
        //---------------------------------------------------------------------------------------------
        public IEnumerable<WorkCalendar> GetDaysForYear(int year)
        {
            return db.calendars.Where(it => it.cal_date.Year == year).OrderBy(o => o.cal_date);
        }

        //---------------------------------------------------------------------------------------------
        // запись измененных дней в году
        //---------------------------------------------------------------------------------------------
        public bool SaveDays(List<MonthDays> listDays, int Year)
        {
            List<WorkCalendar> ListWC = new List<WorkCalendar>();

            foreach(var item in listDays)
            {
                WorkCalendar wc = new WorkCalendar();
                wc.cal_date = new DateTime(Year, item.Month, item.Day);
                wc.cal_type = item.Type;
                ListWC.Add(wc);
            }

            List<WorkCalendar> oldList = db.calendars.Where(it => it.cal_date.Year == Year).ToList();

            //db.calendars.SqlQuery("delete from calendar where cal_date.Year=@year" );
            db.calendars.RemoveRange(oldList);

            db.calendars.AddRange(ListWC);
            db.SaveChanges();

            return true;
        }



        //---------------------------------------------------------------------------------------------
        // Получние дней месяца с типом дня
        //---------------------------------------------------------------------------------------------
        public List<(int Day, TypeDays KindDay, decimal Hours)> GetListDays(int year, int month)
        {
            List<(int Day, TypeDays KindDay, decimal Hours)> list = new List<(int, TypeDays, decimal)>();

            IEnumerable<WorkCalendar> cal = db.calendars.AsNoTracking().Where(it => it.cal_date.Year == year
                    && it.cal_date.Month == month);


            DateTime StartDay = new DateTime(year, month, 1);

            for (DateTime IndexDate = StartDay; IndexDate.Month == month; IndexDate = IndexDate.AddDays(1))
            {
                WorkCalendar ChangeDay = cal.FirstOrDefault(it => it.cal_date == IndexDate);
                TypeDays typeDay;
                int hours = 0;

                if (ChangeDay != null)
                {
                    typeDay = ChangeDay.cal_type;
                    switch (typeDay)
                    {
                        case TypeDays.Holyday:
                            hours = 0;
                            break;
                        case TypeDays.Work:
                            hours = 8;
                            break;
                        case TypeDays.ShortWork:
                            hours = 7;
                            break;
                    }
                }
                else
                {
                    if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        typeDay =TypeDays.Holyday;
                        hours = 0;
                    }
                    else
                    {
                        typeDay = TypeDays.Work;
                        hours = 8;
                    }
                }


                //(int, TypeDays, decimal) newDay = (IndexDate.Day, typeDay, hours);
                list.Add((IndexDate.Day, typeDay, hours));

            }

            return list;
        }


    }
}
