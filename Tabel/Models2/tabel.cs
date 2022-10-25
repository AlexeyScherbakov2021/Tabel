namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tabel")]
    public partial class tabel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tabel()
        {
            tabelPersons = new HashSet<tabelPerson>();
        }

        [Key]
        public int idTabel { get; set; }

        public int t_otdel_id { get; set; }

        [StringLength(50)]
        public string t_number { get; set; }

        public DateTime? t_date_create { get; set; }

        public int t_year { get; set; }

        public int t_month { get; set; }

        public int? t_status { get; set; }

        [StringLength(180)]
        public string t_author { get; set; }

        public int? t_author_id { get; set; }

        public virtual otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tabelPerson> tabelPersons { get; set; }

        public virtual user user { get; set; }
    }
}
