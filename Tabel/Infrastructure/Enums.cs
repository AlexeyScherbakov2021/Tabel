using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Infrastructure
{
    public enum SmenaKind: int { None, First, Second,  DayOff, Otpusk }
    public enum TabelKind: int { None, Work, DayOff, Komandir, Otpusk, WorkDayOff, DopOtpusk, Bolen, Obuchenie, NotWork};

}
