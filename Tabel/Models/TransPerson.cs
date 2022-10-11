namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TransPerson")]
    public partial class TransPerson : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransPerson()
        {
            TransDays = new HashSet<TransDay>();
        }

        [Key]
        [Column("tp_Id")]
        public int id { get; set; }

        public int tp_PersonId { get; set; }

        public int tp_TranspId { get; set; }

        //public decimal? tp_tarif { get; set; }
        private decimal? _tp_tarif;
        [Column(TypeName = "numeric")]
        public decimal? tp_tarif
        {
            get => _tp_tarif;
            set
            {
                if (_tp_tarif != value)
                {
                    _tp_tarif = value;
                    UpdateUI();
                }

            }
        }


        //public decimal? tp_Kompens { get; set; }
        private decimal? _tp_Kompens;
        [Column(TypeName = "numeric")]
        public decimal? tp_Kompens
        {
            get => _tp_Kompens;
            set
            {
                if (_tp_Kompens != value)
                {
                    _tp_Kompens = value;
                    UpdateUI();
                }

            }
        }


        public virtual Personal person { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransDay> TransDays { get; set; }

        public virtual Transport Transport { get; set; }
    }
}
