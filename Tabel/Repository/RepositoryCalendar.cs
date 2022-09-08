using System;
using System.Collections.Generic;
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
            return db.calendars.Select(it => it.cal_date.Value.Year).Distinct().OrderBy(o => o);
        }

        //---------------------------------------------------------------------------------------------
        // получение списка измененных дней в году
        //---------------------------------------------------------------------------------------------
        public IEnumerable<WorkCalendar> GetDaysForYear(int year)
        {
            return db.calendars.Where(it => it.cal_date.Value.Year == year).OrderBy(o => o.cal_date);
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
                wc.cal_type = (int)item.Type;
                ListWC.Add(wc);
            }

            List<WorkCalendar> oldList = db.calendars.Where(it => it.cal_date.Value.Year == Year).ToList();

            //db.calendars.SqlQuery("delete from calendar where cal_date.Year=@year" );
            db.calendars.RemoveRange(oldList);

            db.calendars.AddRange(ListWC);
            db.SaveChanges();

            return true;
        }

    }
}
