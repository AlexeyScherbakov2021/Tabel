using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryCalendar : RepositoryMSSQL<WorkCalendar>
    {
        public IEnumerable<int> GetYears()
        {
            return db.calendars.Select(it => it.cal_date.Value.Year).Distinct().OrderBy(o => o);
        }

        public IEnumerable<WorkCalendar> GetDaysForYear(int year)
        {
            return db.calendars.Where(it => it.cal_date.Value.Year == year).OrderBy(o => o.cal_date);
        }
    }
}
