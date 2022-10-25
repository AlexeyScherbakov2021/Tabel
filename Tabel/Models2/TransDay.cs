namespace Tabel.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TransDay")]
    public partial class TransDay
    {
        [Key]
        public int td_Id { get; set; }

        public int td_TransPersonId { get; set; }

        public int td_Day { get; set; }

        public int? td_Kind { get; set; }

        public virtual TransPerson TransPerson { get; set; }
    }
}
