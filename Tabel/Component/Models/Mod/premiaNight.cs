using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;

namespace Tabel.Component.Models.Mod
{
    public class premiaNight : BasePremia
    {
        //public decimal? NightOklad => person?.category?.cat_tarif * 0.2m;

        //private decimal? _NightHours;
        //public decimal? NightHours
        //{
        //    get => _NightHours;
        //    set
        //    {
        //        if (_NightHours == value) return;
        //        _NightHours = value;
        //    }
        //}

        public decimal? NightOklad { get; set; }
        public decimal? NightHours { get; set; }


        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public premiaNight(ModPerson person) : base(person)
        {
        }

        //-------------------------------------------------------------------------------------------------------
        // Инициализация
        //-------------------------------------------------------------------------------------------------------
        public override void Initialize(int SmenaId)
        {
            RepositoryMSSQL<SmenaPerson> repoSmena = new RepositoryMSSQL<SmenaPerson>();// AllRepo.GetRepoSmenaPerson();
            var pers = repoSmena.Items.FirstOrDefault(it => it.sp_SmenaId == SmenaId && it.sp_PersonId == model.md_personalId);
            if (pers != null)
            {
                NightHours = pers.SmenaDays.Count(s => s.sd_Kind == SmenaKind.Second) * 4.5m;
                NightOklad = model.person?.category?.cat_tarif * 0.2m;
                Summa = NightOklad * NightHours;
            }
        }



        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
        }


    }
}
