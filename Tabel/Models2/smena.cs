namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("smena")]
    public partial class smena
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public smena()
        {
            SmenaPersons = new HashSet<SmenaPerson>();
        }

        [Key]
        public int sm_Id { get; set; }

        [StringLength(20)]
        public string sm_Number { get; set; }

        public DateTime sm_DateCreate { get; set; }

        public int sm_Month { get; set; }

        public int sm_Year { get; set; }

        public int sm_UserId { get; set; }

        public int sm_OtdelId { get; set; }

        public virtual otdel otdel { get; set; }

        public virtual user user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SmenaPerson> SmenaPersons { get; set; }
    }
}
