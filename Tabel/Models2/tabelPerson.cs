namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Infrastructure;
    using Tabel.Models2;

    [Table("tabelPerson")]
    public partial class TabelPerson : IEntity
    {
        public TabelPerson()
        {
            TabelDays = new HashSet<TabelDay>();
        }

        [Key]
        [Column("tp_Id")]
        public int id { get; set; }

        public int? tp_person_id { get; set; }

        public int tp_tabel_id { get; set; }

        public virtual Personal person { get; set; }

        public virtual WorkTabel tabel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelDay> TabelDays { get; set; }

        [NotMapped]
        public int DaysWeek1 { get; set; }

    }
}
