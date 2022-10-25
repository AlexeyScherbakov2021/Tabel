namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SmenaDay")]
    public partial class SmenaDay
    {
        [Key]
        public int sd_Id { get; set; }

        public int? sd_SmenaPersonId { get; set; }

        public int sd_Day { get; set; }

        public int sd_Kind { get; set; }

        public virtual SmenaPerson SmenaPerson { get; set; }
    }
}
