namespace Tabel.Models2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SmenaPerson")]
    public partial class SmenaPerson : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SmenaPerson()
        {
            SmenaDays = new HashSet<SmenaDay>();
        }

        [Key]
        [Column("sp_Id")]
        public int id { get; set; }

        public int sp_PersonId { get; set; }

        public int sp_SmenaId { get; set; }

        public int? sp_Group { get; set; }

        public virtual Personal personal { get; set; }

        public virtual Smena smena { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SmenaDay> SmenaDays { get; set; }

        [NotMapped]
        public IList SelectedDays { get; set; }
        [NotMapped]
        public int SelectIndex { get; set; }
    }
}
