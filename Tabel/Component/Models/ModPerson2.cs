using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models;
using Tabel.Component.Models.Mod;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    public partial class ModPerson : Observable
    {
        [NotMapped]
        public PremiaFP premiaFP { get; set; }

        [NotMapped]
        public PremiaBonus premiaBonus { get; set; }

        [NotMapped]
        public PremiaKvalif premiaKvalif { get; set; }

        [NotMapped]
        public PremiaOtdel premiaOtdel { get; set; }

        [NotMapped]
        public PremOffDays premOffDays { get; set; }

        [NotMapped]
        public PremiaAddWorks premiaAddWorks { get; set; }

        [NotMapped]
        public PremiaTransport premiaTrnasport { get; set; }

        [NotMapped]
        public premiaNight premiaNight { get; set; }


        public ModPerson()
        {
            premiaFP = new PremiaFP(this);
            premiaBonus = new PremiaBonus(this);
            premiaKvalif = new PremiaKvalif(this);
            premiaOtdel = new PremiaOtdel(this);
            premOffDays = new PremOffDays(this);
            premiaAddWorks = new PremiaAddWorks(this);
            premiaTrnasport = new PremiaTransport(this);
            premiaNight = new premiaNight(this);
        }


        [NotMapped]
        public decimal TabelHours { get; set; }
        [NotMapped]
        public int TabelDays { get; set; }
        [NotMapped]
        public int TabelAbsent { get; set; } = 0;
        [NotMapped]
        public decimal Oklad { get; set; }

        [NotMapped]
        public decimal? TabelWorkOffDay { get; set; }


        public decimal? Itogo => Oklad
            + premiaFP.GetPremia()
            + premiaBonus.GetPremia()
            + premiaKvalif.GetPremia()
            + premiaOtdel.GetPremia()
            + premiaNight.GetPremia()
            + premOffDays.GetPremia()
            + premiaTrnasport.GetPremia()
            + premiaAddWorks.GetPremia();

        //public void UpdateUI()
        //{
        //    OnPropertyChanged(nameof(Itogo));
        //}

    }
}
