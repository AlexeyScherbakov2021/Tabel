namespace Tabel.Models
{
    using global::Tabel.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("personal")]
    public partial class Personal : IEntity 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Personal()
        {
            TabelPersons = new HashSet<TabelPerson>();
        }

        [Key]
        [Column ("idPerson")]
        public int id { get; set; }

        [StringLength(20)]
        public string p_tab_number { get; set; }

        [StringLength(50)]
        public string p_lastname { get; set; }

        [StringLength(50)]
        public string p_name { get; set; }

        [StringLength(50)]
        public string p_midname { get; set; }

        [StringLength(150)]
        public string p_profession { get; set; }

        public int? p_otdel_id { get; set; }

        public int? p_cat_id { get; set; }

        public virtual Category category { get; set; }

        public virtual Otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelPerson> TabelPersons { get; set; }
    }
}
