using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Models2
{
    [Table("Mod")]
    public partial class Mod : IEntity
    {
        public Mod()
        {
            //ModPersons = new HashSet<ModPerson>();
        }

        [Key]
        [Column("m_Id")]
        public int id {get;set;}
        public int m_year { get; set; }
        public int m_month { get; set; }
        public int m_author { get; set; }
        public int m_otdelId { get; set; }
        private decimal? _m_HoursFromFP;
        public decimal? m_HoursFromFP 
        { 
            get => _m_HoursFromFP; 
            set 
            {
                if (Equals(_m_HoursFromFP, value)) return;
                _m_HoursFromFP = value;
                if (ModPersons is null) return;
                foreach(var item in ModPersons)
                    item.UpdateUI();
            } 
        }


        public virtual User Author { get; set; }
        public virtual Otdel Otdel { get; set; }

        public virtual IList<ModPerson> ModPersons { get; set; }  
    }
}
