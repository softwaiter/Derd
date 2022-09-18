using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest02
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

        [Description("根据User模型创建物理表，应成功")]
        public void Test1()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("再次创建User模型物理表，因为已存在，所以应该失败")]
        public void Test2()
        {
            bool ret = Derd.Model("Person").TryCreateTable();
            Assert.IsFalse(ret);
        }

        [Description("使用Force参数第三次创建User模型物理表，应成功。")]
        public void Test3()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("删除Test3测试中创建的User模型物理表，应成功。")]
        public void Test4()
        {
            bool ret = Derd.Model("Person").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
