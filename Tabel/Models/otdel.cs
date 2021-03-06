namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Otdel : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Otdel()
        {
            //subOtdels = new HashSet<Otdel>();
            personals = new HashSet<Personal>();
            tabels = new HashSet<WorkTabel>();
            users = new HashSet<User>();

        }

        [Key]
        [Column ("idOtdel")]
        public int id { get; set; }

        [StringLength(100)]
        public string ot_name { get; set; }

        public int? ot_parent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Otdel> subOtdels { get; set; }

        public virtual Otdel parent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personal> personals { get; set; }
        public virtual ICollection<User> users { get; set; }

        public virtual ICollection<WorkTabel> tabels { get; set; }

    }
}
