namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Models;

    [Table("tabelPerson")]
    public partial class TabelPerson : IEntity
    {
        [Key]
        [Column("idTabelPerson")]
        public int id { get; set; }

        public int d_person_id { get; set; }

        public int d_day { get; set; }

        public int? d_hours { get; set; }

        public int? d_type { get; set; }

        public int d_tabel_id { get; set; }

        public virtual Personal personal { get; set; }

        public virtual WorkTabel tabel { get; set; }

        public virtual typeDay typeDay { get; set; }
    }
}
