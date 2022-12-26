namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tabel.Infrastructure;

    [Table("personal")]
    public partial class Personal : Observable, IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Personal()
        {
            ModPersons = new HashSet<ModPerson>();
            tabelPersons = new HashSet<TabelPerson>();
            SmenaPersons = new HashSet<SmenaPerson>();
            TransPersons = new HashSet<TransPerson>();
            SeparPersons = new HashSet<SeparPerson>();
            OtpuskPersons = new HashSet<OtpuskPerson>();
        }

        [Key]
        [Column("idPerson")]
        public int id { get; set; }

        private string _p_tab_number;
        [StringLength(50)]
        public string p_tab_number { get => _p_tab_number; set { Set(ref _p_tab_number, value); } }


        private string _p_lastname;
        [StringLength(50)]
        public string p_lastname { get => _p_lastname; set { Set(ref _p_lastname, value); } }

        private string _p_name;
        [StringLength(50)]
        public string p_name { get => _p_name; set { Set(ref _p_name, value); } }

        private string _p_midname;
        [StringLength(50)]
        public string p_midname { get => _p_midname; set { Set(ref _p_midname, value); } }

        private string _p_profession;
        [StringLength(150)]
        public string p_profession { get => _p_profession; set { Set(ref _p_profession, value); } }

        private int? _p_otdel_id;
        public int? p_otdel_id { get => _p_otdel_id; set { Set(ref _p_otdel_id, value); } }


        private int? _p_cat_id;
        public int? p_cat_id { get => _p_cat_id; set { Set(ref _p_cat_id, value);  } }

        private bool _p_delete;
        public bool p_delete { get => _p_delete; set { Set(ref _p_delete, value); } }

        private decimal _p_stavka;
        public decimal p_stavka { get => _p_stavka; set { Set(ref _p_stavka, value); } }

        private decimal? _p_premTarif;
        public decimal? p_premTarif { get => _p_premTarif; set { Set(ref _p_premTarif, value); } }

        private SpecType _p_type_id;
        public SpecType p_type_id { get => _p_type_id; set { Set(ref _p_type_id, value); } }

        public virtual Category category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModPerson> ModPersons { get; set; }

        public virtual Otdel otdel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TabelPerson> tabelPersons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SmenaPerson> SmenaPersons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransPerson> TransPersons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeparPerson> SeparPersons { get; set;}
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OtpuskPerson> OtpuskPersons { get; set;}

        [NotMapped]
        public string FIO => p_lastname + " " + p_name + " " + p_midname;
        [NotMapped]
        public string ShortFIO => p_lastname + " " +    
            (String.IsNullOrEmpty(p_name) ? ""
              : (p_name[0] + ". " +
                (String.IsNullOrEmpty(p_midname) ? "" : (p_midname[0] + ". "))));

        [NotMapped]
        public decimal? MidOklad => 162 * category?.cat_tarif * p_stavka;

        [NotMapped]
        public decimal? MidPrem => 162 * p_premTarif * p_stavka;

        [NotMapped]
        public decimal? MidItog => MidOklad + MidPrem;

        [NotMapped]
        public decimal? MidNdfl => MidItog / 0.87m;


    }
}
