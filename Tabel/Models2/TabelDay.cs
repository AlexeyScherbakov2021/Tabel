namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TabelDay")]
    public partial class TabelDay
    {
        [Key]
        public int td_Id { get; set; }

        public int td_TabelPersonId { get; set; }

        public int td_Day { get; set; }

        public int td_KindId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? td_Hours { get; set; }

        public virtual tabelPerson tabelPerson { get; set; }

        public virtual typeDay typeDay { get; set; }
    }
}
