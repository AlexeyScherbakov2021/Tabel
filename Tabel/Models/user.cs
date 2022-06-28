namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User : IEntity
    {
        [Key]
        [Column ("idUser")]
        public int id { get; set; }

        [StringLength(30)]
        public string u_login { get; set; }

        [StringLength(30)]
        public string u_pass { get; set; }

        public int? u_role { get; set; }

        public int? u_otdel_id { get; set; }

        [StringLength(150)]
        public string u_fio { get; set; }
    }
}
