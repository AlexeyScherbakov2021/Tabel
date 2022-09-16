using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Infrastructure;

namespace Tabel.Models2
{
    [Table("TabelDay")]
    public partial class TabelDay : Observable, IEntity
    {
        [Key]
        [Column("td_Id")]
        public int id { get; set; }

        public int? td_TabelPersonId { get; set; }

        public int td_Day { get; set; }


        private int? _td_KindId;
        public int? td_KindId 
        { 
            get => _td_KindId;
            set
            {
                if (Set(ref _td_KindId, value))
                    TabelPerson?.UpdateUI();
            }
        }


        private decimal? _td_Hours;
        public decimal? td_Hours 
        { 
            get => _td_Hours; 
            set
            {
                if (Set(ref _td_Hours, value))
                    TabelPerson?.UpdateUI();
            }
        }

        //private typeDay _Kind;
        public virtual typeDay Kind { get; set; }

        //public virtual Personal person { get; set; }

        public virtual TabelPerson TabelPerson { get; set; }

        [NotMapped]
        public int td_Hours2 { get; set; }
    }
}
