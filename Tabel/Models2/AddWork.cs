namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AddWork
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AddWork()
        {
            ModPersons = new HashSet<ModPerson>();
        }

        [Key]
        public int aw_Id { get; set; }

        [StringLength(150)]
        public string aw_Name { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? aw_Tarif { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModPerson> ModPersons { get; set; }
    }
}
