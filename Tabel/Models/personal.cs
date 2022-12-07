namespace Tabel.Models
{
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
            ModPersons = new HashSet<ModPerson>();
            tabelPersons = new HashSet<TabelPerson>();
            SmenaPersons = new HashSet<SmenaPerson>();
            TransPersons = new HashSet<TransPerson>();
        }

        [Key]
        [Column("idPerson")]
        public int id { get; set; }

        [StringLength(50)]
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

        public bool p_delete { get; set; }

        public decimal p_stavka { get; set; }

        public decimal p_premTarif { get; set; }

        public virtual Category category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModPerson> ModPersons { get; set; }

        public virtual Otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelPerson> tabelPersons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SmenaPerson> SmenaPersons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransPerson> TransPersons { get; set; }

        [NotMapped]
        public string FIO => p_lastname + " " + p_name + " " + p_midname;
        [NotMapped]
        public string ShortFIO => p_lastname + " " +    
            (String.IsNullOrEmpty(p_name) ? ""
              : (p_name[0] + ". " +
                (String.IsNullOrEmpty(p_midname) ? "" : (p_midname[0] + ". "))));

    }
}
