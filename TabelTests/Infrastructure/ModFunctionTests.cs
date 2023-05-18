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


        [TestInitialize] 
        public void Init() 
        {
            _modPerson = new ModPerson()
            {
                AddingHours = 10,
                md_cat_tarif = 400,
                md_workHours = 162,
                md_pereWork15 = 2,
                md_pereWork2 = 2,
                md_workOffDays = 2,
                md_absentDays = 2
            };

            _modPerson.person = new Personal()
            {
                p_stavka = 1,
                p_type_id = Infrastructure.SpecType.ИТР
            };


            _modPerson.Mod = new Mod()
            {
                m_year = 2023,
                m_month = 5,
            };

            ModFunction.HoursDefault = 160;

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
            //Trace.WriteLine("Tracing from test");
            //Console.WriteLine(@"Тест 'TestConsoleWriteLine' успешно пройден");
            Assert.AreEqual(_modPerson.md_Oklad, 61344);
        }

        [TestMethod()]
        public void SetOkladWorkerTest()
        {
            _modPerson.AddingHours = 0;
            _modPerson.person.p_type_id = SpecType.Рабочий;
            _modPerson.md_cat_tarif = 300;
            _modPerson.md_workHours = 180;
            _modPerson.md_pereWork15 = 10;
            _modPerson.md_pereWork2 = 2;
            _modPerson.md_workOffDays = 1;
            ModFunction.SetOklad(_modPerson);
            Assert.AreEqual(_modPerson.md_Oklad, 48000);
        }

        [TestMethod()]
        public void Pere15Test()
        {
            ModFunction modf = new ModFunction();
            modf.SetPereWork(_modPerson);

            Assert.AreEqual(_modPerson.pereWork15summ, 1200m);

        }

        [TestMethod()]
        public void Pere2Test()
        {
            ModFunction modf = new ModFunction();
            modf.SetPereWork(_modPerson);

            Assert.AreEqual(_modPerson.pereWork2summ, 1600m);

        }
    }
}