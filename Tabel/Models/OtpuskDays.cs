//using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    [Table("OtpuskDays")]
    public class OtpuskDays : Observable, IEntity
    {
        //public OtpuskDays() { }

        public int id { get; set; }

        private DateTime _StartDate;
        public DateTime od_StartDate { get => _StartDate; set { if(Set(ref _StartDate, value)) OnPropertyChanged(nameof(ToolTipName)); } }

        private DateTime _EndDate;
        public DateTime od_EndDate { get => _EndDate; set { if(Set(ref _EndDate, value)) OnPropertyChanged(nameof(ToolTipName)); } }

        public int? od_otpuskPersonId { get; set; }

        public virtual OtpuskPerson otpuskPerson { get; set; }
        
        public int CountDay => od_EndDate.DayOfYear - od_StartDate.DayOfYear + 1;
        
        public string ToolTipName => od_StartDate.ToString("d") + " - " + od_EndDate.ToString("d");


    }
}
