namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tabelPerson")]
    public partial class TabelPerson : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TabelPerson()
        {
            TabelDays = new HashSet<TabelDay>();
        }

        [Key]
        [Column("tp_Id")]
        public int id { get; set; }

        public int tp_person_id { get; set; }

        public int tp_tabel_id { get; set; }

        public virtual Personal person { get; set; }

        public virtual WorkTabel tabel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelDay> TabelDays { get; set; }
    }
}
