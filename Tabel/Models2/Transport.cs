namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Transport")]
    public partial class Transport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transport()
        {
            TransPersons = new HashSet<TransPerson>();
        }

        [Key]
        public int tr_Id { get; set; }

        [StringLength(20)]
        public string tr_Number { get; set; }

        public DateTime tr_DateCreate { get; set; }

        public int tr_Month { get; set; }

        public int tr_Year { get; set; }

        public int tr_UserId { get; set; }

        public int tr_OtdelId { get; set; }

        public virtual otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransPerson> TransPersons { get; set; }

        public virtual user user { get; set; }
    }
}
