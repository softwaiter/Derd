﻿using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using CodeM.Common.Tools.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest15
    {
        [TestInitialize]
        public void Init()
        {
            OrmUtils.RegisterProcessor("EncryptDeposit", "UnitTest.Processors.EncryptDeposit");
            OrmUtils.RegisterProcessor("DecryptDeposit", "UnitTest.Processors.DecryptDeposit");

            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();

            OrmUtils.RemoveTables();

            OrmUtils.EnableDebug(true);
        }

        [TestMethod]
        public void Test()
        {
            Test1();
        }

        [Description("创建Animal模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("Animal").TryCreateTable(true);
            Assert.IsTrue(ret);

            dynamic newanimal = ModelObject.New("Animal");
            newanimal.Name = "panda";
            newanimal.Feature = new DynamicObjectExt();
            newanimal.Feature.Food = "竹子";
            newanimal.Feature.Color = "黑白";
            newanimal.Feature.Life = 30;
            bool ret2 = OrmUtils.Model("Animal").SetValues(newanimal).Save();
            Assert.IsTrue(ret2);

            dynamic result = OrmUtils.Model("Animal").Equals("Name", "panda").QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreNotEqual("panda", result.Name);
            Assert.AreEqual("PANDA", result.Name);
            Assert.AreEqual("黑白", result.Feature.Color);
            Assert.AreEqual(30, result.Feature.Life);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), result.DiscDate);
        }

    }
}
