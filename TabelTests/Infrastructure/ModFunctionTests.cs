using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tabel.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using System.Diagnostics;
using Tabel.ViewModels;

namespace Tabel.Infrastructure.Tests
{
    [TestClass()]
    public class ModFunctionTests
    {
        private ModPerson _modPerson;
        private ModPerson _modPersonWorker;


        [TestInitialize] 
        public void Init() 
        {
            _modPerson = new ModPerson()
            {
                //AddingHours = 10,
                md_workHours = 156,
                md_pereWork15 = 2,
                md_pereWork2 = 2,
                md_workOffHours = 2,
                md_absentDays = 2
            };

            _modPerson.person = new Personal()
            {
                p_stavka = 1,
                p_type_id = SpecType.N2,
                p_oklad = 80000
            };


            _modPerson.Mod = new Mod()
            {
                m_year = 2023,
                m_month = 5,
            };



            _modPersonWorker = new ModPerson()
            {
                md_cat_tarif = 245,
                md_workHours = 204,
                md_pereWork15 = 4,
                md_pereWork2 = 4,
                md_workOffHours = 12,
                md_absentDays = 1
            };

            _modPersonWorker.person = new Personal()
            {
                p_stavka = 1,
                p_type_id = SpecType.N1,
            };

            _modPersonWorker.Mod = new Mod()
            {
                m_year = 2023,
                m_month = 5,
            };

            ModFunction.HoursDefault = 184;

        }

        //[TestMethod()]
        //public void ModFunctionTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ModPersonFillingTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SetTarifOffDayTest()
        //{
        //    Assert.Fail();
        //}


        [TestMethod()]
        public void SetOkladTest()
        {
            ModFunction.SetOklad(_modPerson);
            Assert.AreEqual( Math.Round( (_modPerson.md_Oklad ?? 0), 2), 67826.09m);

            ModFunction.SetOklad(_modPersonWorker);
            Assert.AreEqual(_modPersonWorker.md_Oklad, 45080);

        }


        [TestMethod()]
        public void PereWorkTest()
        {
            ModFunction modf = new ModFunction();
            _modPerson.md_cat_tarif = (_modPerson.person.p_oklad ?? 0) / ModFunction.HoursDefault;
            modf.SetPereWork(_modPerson);
            Assert.AreEqual(Math.Round(_modPerson.pereWork15summ ?? 0, 2), 1304.35m);
            Assert.AreEqual(Math.Round(_modPerson.pereWork2summ ?? 0, 2), 1739.13m);

            modf.SetPereWork(_modPersonWorker);
            Assert.AreEqual(Math.Round(_modPersonWorker.pereWork15summ ?? 0, 2), 1470.00m);
            Assert.AreEqual(Math.Round(_modPersonWorker.pereWork2summ ?? 0, 2), 1960.00m);
        }

    }
}