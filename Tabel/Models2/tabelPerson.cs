namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tabelPerson")]
    public partial class tabelPerson
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tabelPerson()
        {
            TabelDays = new HashSet<TabelDay>();
        }

        [Key]
        public int tp_Id { get; set; }

        public int tp_person_id { get; set; }

        public int tp_tabel_id { get; set; }

        public virtual personal personal { get; set; }

        public virtual tabel tabel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelDay> TabelDays { get; set; }
    }
}
