namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tabel")]
    public partial class Tabel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tabel()
        {
            days = new HashSet<Day>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idTabel { get; set; }

        [StringLength(30)]
        public string t_number { get; set; }

        public DateTime? t_date_create { get; set; }

        public int? t_year { get; set; }

        public int? t_month { get; set; }

        public int? t_person_id { get; set; }

        public int? t_days_id { get; set; }

        public int? t_status { get; set; }

        [StringLength(180)]
        public string t_author { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Day> days { get; set; }

        public virtual Personal personal { get; set; }
    }
}
