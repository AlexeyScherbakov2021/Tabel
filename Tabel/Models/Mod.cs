namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Mod")]
    public partial class Mod : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mod()
        {
            //ModOtdelSumFPs = new HashSet<ModOtdelSumFP>();
            //ModPersons = new HashSet<ModPerson>();
        }

        [Key]
        [Column("m_Id")]
        public int id { get; set; }

        public int m_otdelId { get; set; }

        public int m_year { get; set; }

        public int m_month { get; set; }

        public int m_author { get; set; }

        public bool? m_IsClosed { get; set; }

        //private decimal? _m_HoursFromFP;

        //[Column(TypeName = "numeric")]
        //public decimal? m_HoursFromFP
        //{
        //    get => _m_HoursFromFP;
        //    set
        //    {
        //        if (Equals(_m_HoursFromFP, value)) return;
        //        _m_HoursFromFP = value;
        //        if (ModPersons is null) return;
        //        foreach (var item in ModPersons)
        //            item.UpdateUI();
        //    }
        //}

        public virtual Otdel otdel { get; set; }

        public virtual User user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<ModPerson> ModPersons { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ModOtdelSumFP> ModOtdelSumFPs { get; set; }

    }
}
