namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Infrastructure;

    [Table("TransDay")]
    public partial class TransDay : Observable, IEntity
    {
        [Key]
        [Column("td_id")]
        public int id { get; set; }

        public int td_TransPersonId { get; set; }

        public int td_Day { get; set; }

        //public int? td_Kind { get; set; }
        private KindTrans? _td_Kind;
        public KindTrans? td_Kind
        {
            get => _td_Kind;
            set
            {
                if (_td_Kind != value)
                {
                    _td_Kind = value;
                    TransPerson?.UpdateUI();
                    OnPropertyChanged(nameof(td_Kind));
                }
            }
        }
        public virtual TransPerson TransPerson { get; set; }

        [NotMapped]
        public bool OffDay { get; set; }

        public string DayString
        {
            get
            {
                DateTime dt = new DateTime(TransPerson.Transport.tr_Year, TransPerson.Transport.tr_Month, td_Day);
                return dt.ToString("d ddd").ToLower();
            }
        }


    }
}
