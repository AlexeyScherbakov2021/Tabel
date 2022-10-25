namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Mod")]
    public partial class Mod
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mod()
        {
            ModOtdelSumFPs = new HashSet<ModOtdelSumFP>();
            ModPersons = new HashSet<ModPerson>();
        }

        [Key]
        public int m_Id { get; set; }

        public int m_otdelId { get; set; }

        public int m_year { get; set; }

        public int m_month { get; set; }

        public int m_author { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? m_HoursFromFP { get; set; }

        public virtual otdel otdel { get; set; }

        public virtual user user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModOtdelSumFP> ModOtdelSumFPs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModPerson> ModPersons { get; set; }
    }
}
