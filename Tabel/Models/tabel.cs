namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tabel")]
    public partial class WorkTabel : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkTabel()
        {
            TabelPersons = new HashSet<TabelPerson>();
        }

        [Key]
        [Column("idTabel")]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(30)]
        public string t_number { get; set; }

        public DateTime? t_date_create { get; set; }

        public int t_year { get; set; }

        public int t_month { get; set; }

        public int? t_status { get; set; }

        public int? t_otdel_id { get; set; }

        public virtual Otdel otdel { get; set; }

        [StringLength(180)]
        public string t_author { get; set; }

        public virtual ICollection<TabelPerson> TabelPersons { get; set; }


    }
}
