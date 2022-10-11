namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Infrastructure;

    [Table("SmenaDay")]
    public partial class SmenaDay : IEntity
    {
        [Key]
        [Column("sd_id")]
        public int id { get; set; }

        public int? sd_SmenaPersonId { get; set; }

        public int sd_Day { get; set; }

        public SmenaKind sd_Kind { get; set; }

        public virtual SmenaPerson SmenaPerson { get; set; }
    }
}
