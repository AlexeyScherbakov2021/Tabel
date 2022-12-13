namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Infrastructure;

    [Table("SmenaDay")]
    public partial class SmenaDay : Observable, IEntity
    {
        [Key]
        [Column("sd_id")]
        public int id { get; set; }

        public int? sd_SmenaPersonId { get; set; }

        public int sd_Day { get; set; }

        private SmenaKind _sd_Kind;
        public SmenaKind sd_Kind { get => _sd_Kind; set { Set(ref _sd_Kind, value); } }

        public virtual SmenaPerson SmenaPerson { get; set; }

        [NotMapped]
        public bool OffDay { get; set; }

        public string DayString
        {
            get
            {
                DateTime dt = new DateTime(SmenaPerson.smena.sm_Year, SmenaPerson.smena.sm_Month, sd_Day);
                return dt.ToString("d ddd").ToLower();
            }
        }


    }
}
