using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest6
    {
        [TestInitialize]
        public void Init()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();

            OrmUtils.RemoveTables();
        }

        [TestMethod]
        public void Test()
        {
            Test1();
            Test2();
        }

        [Description("创建模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.CreateTables(true);
            Assert.IsTrue(ret);
        }

        [Description("向机构表插入一条数据。")]
        public void Test2()
        {
            dynamic neworg = ModelObject.New("Org");
            neworg.Name = "XX科技";
            bool ret = OrmUtils.Model("Org").SetValues(neworg).Save();
            Assert.IsTrue(ret);
        }

    }
}
