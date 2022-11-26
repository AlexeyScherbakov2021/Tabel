using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.Views;

namespace Tabel.Component.Models.Mod
{
    public class PremiaTransport : BasePremia
    {
        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaTransport(ModPerson person) : base(person)
        {

        }

        //-------------------------------------------------------------------------------------------------------
        // Инициализация
        //-------------------------------------------------------------------------------------------------------
        public override void Initialize(int TransportId)
        {
            RepositoryMSSQL<TransPerson> repoTransPerson = new RepositoryMSSQL<TransPerson>();// AllRepo.GetRepoTransPerson();
            var pers = repoTransPerson.Items.FirstOrDefault(it => it.tp_TranspId == TransportId && it.tp_PersonId == model.md_personalId);
            Summa = pers?.Summa;
        }


        //-------------------------------------------------------------------------------------------------------
        // расчет премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
        }

    }
}
