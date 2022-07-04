namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TabelPerson : IEntity
    {
        [Key]
        [Column ("idTabelPerson")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        public int d_person_id { get; set; }

        public int d_day { get; set; }

        public int? d_hours { get; set; }

        public int? d_type { get; set; }

        public int? d_tabel_id { get; set; }

        public virtual Personal person { get; set; }

        public virtual WorkTabel tabel { get; set; }
    }
}
