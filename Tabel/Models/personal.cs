namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("personal")]
    public partial class Personal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Personal()
        {
            tabels = new HashSet<Tabel>();
        }

        [Key]
        public int idPerson { get; set; }

        [StringLength(20)]
        public string p_tab_number { get; set; }

        [StringLength(50)]
        public string p_lastname { get; set; }

        [StringLength(50)]
        public string p_name { get; set; }

        [StringLength(50)]
        public string p_midname { get; set; }

        [StringLength(150)]
        public string p_profeesion { get; set; }

        public int? p_otdel_id { get; set; }

        public int? p_cat_id { get; set; }

        public virtual Category category { get; set; }

        public virtual Otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tabel> tabels { get; set; }
    }
}
