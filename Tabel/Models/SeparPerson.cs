using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    [Table("SeparPerson")]
    public  class SeparPerson : Observable, IEntity
    {
        public int id { get; set; }
        public int? sp_separId { get; set; }
        public int sp_personalId { get; set; }

        private decimal? _sp_oklad;
        public decimal? sp_oklad { get => _sp_oklad; set { if(Set(ref _sp_oklad, value)) UpdateItogo(); } }

        private decimal? _sp_premia;
        public decimal? sp_premia { get => _sp_premia; set { if (Set(ref _sp_premia, value)) UpdateItogo(); } }

        public virtual Separate Separate { get; set; }
        public virtual Personal person { get; set; }

        [NotMapped]
        public decimal? Itogo => (sp_oklad ?? 0) + (sp_premia ?? 0);

        [NotMapped]
        public decimal? ItogoNDFL => Itogo / 0.87m;

        private void UpdateItogo()
        {
            OnPropertyChanged(nameof(Itogo));
            OnPropertyChanged(nameof(ItogoNDFL));
        }

    }
}
