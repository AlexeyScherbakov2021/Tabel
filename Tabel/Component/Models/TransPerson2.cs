using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Models
{
    public partial class TransPerson : Observable
    {
        [NotMapped]
        public int ItogDays => TransDays.Where(it => it.td_Kind == 1).Count();
        [NotMapped]
        public decimal? Summa => ItogDays * tp_tarif ?? 0;
        [NotMapped]
        public decimal? Itogo => Summa + tp_Kompens ?? 0;

        public void UpdateUI()
        {
            OnPropertyChanged(nameof(ItogDays));
            OnPropertyChanged(nameof(Summa));
            OnPropertyChanged(nameof(Itogo));
        }

    }
}
