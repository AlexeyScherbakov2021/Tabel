namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TransDay")]
    public partial class TransDay : IEntity
    {
        [Key]
        [Column("td_id")]
        public int id { get; set; }

        public int td_TransPersonId { get; set; }

        public int td_Day { get; set; }

        //public int? td_Kind { get; set; }
        private int? _td_Kind;
        public int? td_Kind
        {
            get => _td_Kind;
            set
            {
                if (_td_Kind != value)
                {
                    _td_Kind = value;
                    TransPerson?.UpdateUI();
                }
            }
        }



        public virtual TransPerson TransPerson { get; set; }
    }
}
