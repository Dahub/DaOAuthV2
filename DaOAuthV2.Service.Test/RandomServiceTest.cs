﻿using DaOAuthV2.Service.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class RandomServiceTest
    {
        private IRandomService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new RandomService();
        }

        [TestMethod]
        public void Generate_Random_Int_Should_Generate_Number()
        {
            for (var i = 0; i < 100; i++)
            {
                var result = _service.GenerateRandomInt(5);
                Assert.AreEqual(5, result.ToString().Length);
                Assert.IsTrue(result >= 10000);
                Assert.IsTrue(result <= 99999);
            }
        }

        [TestMethod]
        public void Generate_Random_Int_Should_Generate_Different_Number_Each_Time()
        {
            var old = _service.GenerateRandomInt(5);
            for (var i = 0; i < 10; i++)
            {
                var result = _service.GenerateRandomInt(5);
                Assert.AreNotEqual(result, old);
                old = result;
            }
        }

        [TestMethod]
        public void Generate_Random_String_Should_Generate_String()
        {
            var result = _service.GenerateRandomString(15);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 15);
        }

        [TestMethod]
        public void Generate_Random_String_Should_Generate_Different_String_Each_Time()
        {
            var old = _service.GenerateRandomString(15);
            for (var i = 0; i < 10; i++)
            {
                var result = _service.GenerateRandomString(15);
                Assert.AreNotEqual(result, old);
                old = result;
            }
        }
    }
}
