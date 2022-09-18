using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest10
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
            Test3();
            Test4();
        }

        [Description("判断User模型对应的物理表是否已创建，应返回false")]
        public void Test1()
        {
            bool ret = Derd.Model("Person").TableExists();
            Assert.IsFalse(ret);
        }

        [Description("根据User模型创建物理表，应成功")]
        public void Test2()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("判断User模型对应的物理表是否已创建，应返回true")]
        public void Test3()
        {
            bool ret = Derd.Model("Person").TableExists();
            Assert.IsTrue(ret);
        }

        [Description("删除测试中创建的User模型物理表，应成功。")]
        public void Test4()
        {
            bool ret = Derd.Model("Person").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}