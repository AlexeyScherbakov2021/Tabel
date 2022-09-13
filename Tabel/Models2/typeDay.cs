namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("typeDay")]
    public partial class typeDay : IEntity

    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public typeDay()
        {
            tabelPersons = new HashSet<TabelPerson>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("idTypeDay")]
        public int id { get; set; }

        [StringLength(3)]
        public string t_name { get; set; }

        [StringLength(200)]
        public string t_desc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelPerson> tabelPersons { get; set; }
    }
}
