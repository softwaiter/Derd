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
        [Description("根据User模型创建物理表，应成功")]
        public void T1_CreateUserTable()
        {
            bool ret = OrmUtils.Model("User").CreateTable();
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("再次创建User模型物理表，因为已存在，所以应该失败")]
        public void T2_CreateUserTableAgain()
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

        [TestMethod]
        [Description("使用Force参数第三次创建User模型物理表，应成功；接着进行删除，应成功。")]
        public void T3_CreateUserTableThirdTimeWithForceParameter()
        {
            bool ret = OrmUtils.Model("User").CreateTable(true);
            Assert.IsTrue(ret);

            ret = OrmUtils.Model("User").RemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
