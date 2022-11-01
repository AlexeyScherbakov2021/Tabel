namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("category")]
    public partial class Category : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            personals = new HashSet<Personal>();
        }

        [Key]
        [Column("idCategory")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cat_tarif { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cat_min_level { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cat_max_level { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cat_prem_tarif { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personal> personals { get; set; }
    }
}
