using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tabel.Component.Models.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.ViewModels;
using Tabel.Infrastructure;

namespace Tabel.Component.Models.Mod.Tests
{
    [TestClass()]
    public class PremiaTests
    {
        private ModPerson _modPerson;

        [TestInitialize]
        public void Init()
        {
            _modPerson = new ModPerson()
            {
                AddingHours = 10,
                md_cat_tarif = 300,
                md_workHours = 180,
                pereWork15summ = 6,
                md_pereWork2 = 6,
                md_workOffDays = 1,
                md_absentDays = 2,
                md_bonus_exec = true,
                md_bonus_max = 5000,
                md_workDays = 31,
                md_tarif_offDay = 2499,
                md_pereWork15 = 10,
                pereWork2summ = 2,
                md_overHours = 4,
                md_premFP = 30,
                md_sumFromFP = 700,
                md_cat_prem_tarif = 185
            };

            _modPerson.person = new Personal()
            {
                p_stavka = 1,
                p_type_id = Infrastructure.SpecType.N1
            };

            _modPerson.premiaBonus = new PremiaBonus(_modPerson);
            _modPerson.premOffDays = new PremOffDays(_modPerson);
            _modPerson.premiaNight = new premiaNight(_modPerson);
            _modPerson.premiaAddWorks = new PremiaAddWorks(_modPerson);

            _modPerson.premiaNight.NightOklad = 60;
            _modPerson.premiaNight.NightHours = 83.5M;

            _modPerson.md_person_achiev = 2000;

            _modPerson.premiaPrize = new PremiaPrize(_modPerson);
            _modPerson.premiaFP = new PremiaFP(_modPerson);
            _modPerson.premiaAddWorks = new PremiaAddWorks(_modPerson);



            //_modPerson.Mod = new Mod()
            //{
            //    m_year = 2023,
            //    m_month = 5,
            //};

        }


        //------------------------------------------------------------------------------------------------------
        [TestMethod()]
        public void CalculationBonusTest()
        {
            ModUCViewModel.BonusProc = 100;

            _modPerson.premiaBonus.Calculation();
            Assert.AreEqual(Math.Round(_modPerson.premiaBonus.Summa.Value, 2), 4677.42m);
        }


        //------------------------------------------------------------------------------------------------------
        [TestMethod()]
        public void CalculationOffDaysTest()
        {
            ModFunction.WorkOffKoeff = 2;
            _modPerson.premOffDays.Calculation();
            Assert.AreEqual(Math.Round(_modPerson.premOffDays.Summa.Value, 2), 4997.53M);
        }

        //------------------------------------------------------------------------------------------------------
        [TestMethod()]
        public void CalculationNightTest()
        {
            ModFunction.NightKoeff = 0.2M;

            _modPerson.premiaNight.Calculation();

            Assert.AreEqual(Math.Round( _modPerson.premiaNight.Summa.Value, 2), 5216.17M);
        }

        //------------------------------------------------------------------------------------------------------
        [TestMethod()]
        public void CalculationPrizeTest()
        {

            _modPerson.premiaPrize.Calculation();

            Assert.AreEqual(Math.Round( _modPerson.premiaPrize.Summa.Value, 2), 2498.77M);
        }

        //------------------------------------------------------------------------------------------------------
        [TestMethod()]
        public void CalculationFPTest()
        {
            _modPerson.premiaFP.Calculation();
            Assert.AreEqual(Math.Round( _modPerson.premiaFP.Summa.Value, 2), 38850M);
        }

        //------------------------------------------------------------------------------------------------------
        [TestMethod()]
        public void CalculationAddWorksTest()
        {
            _modPerson.ListAddWorks = new List<AddWorks>();
            _modPerson.ListAddWorks.Add(new AddWorks() { aw_Tarif = 20000, aw_IsRelateHours = true } );
            _modPerson.ListAddWorks.Add(new AddWorks() { aw_Tarif = 3000, aw_IsRelateHours = false } );

            _modPerson.premiaAddWorks.Calculation();
            Assert.AreEqual(Math.Round( _modPerson.premiaAddWorks.Summa.Value, 2), 23709.68M);
        }


    }
}