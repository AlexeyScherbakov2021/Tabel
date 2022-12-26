using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models
{
    [Table("Otpusk")]
    public class Otpusk : IEntity
    {
        public Otpusk()
        {
            ListOtpuskPerson = new HashSet<OtpuskPerson>();
        }


        public int id { get; set; }

        [StringLength(20)]
        public string o_number { get; set; }
        public DateTime o_DateCreate { get; set; }
        public int o_year { get; set; }
        public int o_otdelId { get; set; }

        [StringLength(50)]
        public string o_author { get; set; }

        public virtual ICollection<OtpuskPerson> ListOtpuskPerson { get; set; }

    }
}
