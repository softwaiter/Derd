using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestInitialize]
        public void Init()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();
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
            bool ret = OrmUtils.Model("User").CreateTable();
            Assert.IsTrue(ret);
        }

        [Description("再次创建User模型物理表，因为已存在，所以应该失败")]
        public void Test2()
        {
            try
            {
                bool ret = OrmUtils.Model("User").CreateTable();
                Assert.Fail("User模型物理表已存在，应该失败。");
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.ToLower().Contains("already exists"));
            }
        }

        [Description("使用Force参数第三次创建User模型物理表，应成功。")]
        public void Test3()
        {
            bool ret = OrmUtils.Model("User").CreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("删除Test3测试中创建的User模型物理表，应成功。")]
        public void Test4()
        {
            bool ret = OrmUtils.Model("User").RemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
