namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ModPerson")]
    public partial class ModPerson
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModPerson()
        {
            AddWorks = new HashSet<AddWork>();
        }

        [Key]
        public int md_Id { get; set; }

        public int? md_modId { get; set; }

        public int md_personalId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_hoursFP { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_premFP { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_prem1_tarif { get; set; }

        public bool md_bonus_exec { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_bonus_max { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_bonus_proc { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_prem_otdel { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_prem_otdel_proc { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? md_tarif_offDay { get; set; }

        public virtual Mod Mod { get; set; }

        public virtual personal personal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddWork> AddWorks { get; set; }
    }
}
