using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("GenChargMonth")]
    public partial class GenChargMonth : IEntity
    {
        [Key]
        [Column("gm_Id")]
        public int id { get; set; }

        public int gm_GenId { get; set; }
        public int gm_Month { get; set; }
        public int gm_Year { get; set; }
        public decimal? gm_Value { get; set; }

        public virtual GeneralCharges GenCarhge { get; set; }
    }
}
