namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("calendar")]
    public partial class calendar
    {
        [Key]
        public int idCal { get; set; }

        public int? cal_year { get; set; }

        [Column(TypeName = "date")]
        public DateTime cal_date { get; set; }

        public int cal_type { get; set; }
    }
}
