using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
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
            Derd.RegisterProcessor("EncryptDeposit", "Test.Processors.EncryptDeposit");
            Derd.RegisterProcessor("DecryptDeposit", "Test.Processors.DecryptDeposit");

            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            Derd.ModelPath = modelPath;
            Derd.Load();

            Derd.RemoveTables();
        }

        [TestMethod]
        public void Test()
        {
            Test1();
            Test2();
        }

        [Description("创建Animal模型的物理表。")]
        public void Test1()
        {
            bool ret = Derd.Model("Animal").TryCreateTable(true);
            Assert.IsTrue(ret);

            dynamic newanimal = new DynamicObjectExt();
            newanimal.Name = "panda";
            newanimal.Feature = new DynamicObjectExt();
            newanimal.Feature.Food = "竹子";
            newanimal.Feature.Color = "黑白";
            newanimal.Feature.Life = 30;
            bool ret2 = Derd.Model("Animal").SetValues(newanimal).Save();
            Assert.IsTrue(ret2);

            dynamic result = Derd.Model("Animal").Equals("Name", "panda").QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreNotEqual("panda", result.Name);
            Assert.AreEqual("PANDA", result.Name);
            Assert.AreEqual("黑白", result.Feature.Color);
            Assert.AreEqual(30, result.Feature.Life);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), result.DiscDate);
        }

        [Description("创建Animal模型的物理表。")]
        public void Test2()
        {
            bool ret = Derd.Model("Animal").TryCreateTable(true);
            Assert.IsTrue(ret);

            dynamic newanimal = new DynamicObjectExt();
            newanimal.Name = "panda";
            newanimal.Feature = "{\"Food\":\"竹子\",\"Color\":\"黑白\",\"Life\":30}";
            bool ret2 = Derd.Model("Animal").SetValues(newanimal).Save();
            Assert.IsTrue(ret2);

            dynamic result = Derd.Model("Animal").Equals("Name", "panda").QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreNotEqual("panda", result.Name);
            Assert.AreEqual("PANDA", result.Name);
            Assert.AreEqual("黑白", result.Feature.Color);
            Assert.AreEqual(30, result.Feature.Life);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), result.DiscDate);
        }
    }
}
