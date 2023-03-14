namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("smena")]
    public partial class Smena : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Smena()
        {
            SmenaPerson = new HashSet<SmenaPerson>();
        }

        [Key]
        [Column("sm_Id")]
        public int id { get; set; }

        [StringLength(20)]
        public string sm_Number { get; set; }

        public DateTime sm_DateCreate { get; set; }

        public int sm_Month { get; set; }

        public int sm_Year { get; set; }

        public int sm_UserId { get; set; }

        public int sm_OtdelId { get; set; }

        public bool? sm_IsClosed { get; set; }

        public virtual Otdel otdel { get; set; }

        public virtual User user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SmenaPerson> SmenaPerson { get; set; }
    }
}
