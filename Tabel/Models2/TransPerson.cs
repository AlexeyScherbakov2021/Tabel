namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TransPerson")]
    public partial class TransPerson
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransPerson()
        {
            TransDays = new HashSet<TransDay>();
        }

        [Key]
        public int tp_Id { get; set; }

        public int tp_PersonId { get; set; }

        public int tp_TranspId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? tp_tarif { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? tp_Kompens { get; set; }

        public virtual personal personal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransDay> TransDays { get; set; }

        public virtual Transport Transport { get; set; }
    }
}
