namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            smenas = new HashSet<Smena>();
        }

        [Key]
        [Column("idUser")]
        public int id { get; set; }

        [StringLength(50)]
        public string u_login { get; set; }

        [StringLength(50)]
        public string u_pass { get; set; }

        public int? u_role { get; set; }

        public int? u_otdel_id { get; set; }

        [StringLength(150)]
        public string u_fio { get; set; }

        public virtual Otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Smena> smenas { get; set; }
    }
}
