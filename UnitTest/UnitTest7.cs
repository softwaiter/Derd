using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest7
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
        }

        [TestMethod]
        public void Test()
        {
            Test1();
            Test2();
            Test3();
            Test4();
        }

        [Description("创建Organization模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("Org").CreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("创建User模型的物理表。")]
        public void Test2()
        {
            bool ret = OrmUtils.Model("User").CreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("使用事务新建机构和用户，用户新建失败，新建机构信息也回滚")]
        public void Test3()
        {
            dynamic neworg = OrmUtils.Model("Org").NewObject();
            neworg.Name = "事务测试-机构01";

            dynamic newuser = OrmUtils.Model("User").NewObject();
            newuser.Age = 18;

            int trans = OrmUtils.GetTransaction();
            try
            {
                OrmUtils.Model("Org").SetValues(neworg).Save(trans);
                OrmUtils.Model("User").SetValues(newuser).Save(trans);
                OrmUtils.CommitTransaction(trans);
            }
            catch
            {
                OrmUtils.RollbackTransaction(trans);
            }

            List<dynamic> result = OrmUtils.Model("Org").Equals("Name", "事务测试-机构01").Query();
            Assert.AreEqual<int>(result.Count, 0);
        }

        [Description("使用事务新建机构和用户，应成功")]
        public void Test4()
        {
            dynamic neworg = OrmUtils.Model("Org").NewObject();
            neworg.Name = "事务测试-机构01";

            dynamic newuser = OrmUtils.Model("User").NewObject();
            newuser.Name = "事务测试-用户01";

            int trans = OrmUtils.GetTransaction();
            try
            {
                OrmUtils.Model("Org").SetValues(neworg).Save(trans);
                OrmUtils.Model("User").SetValues(newuser).Save(trans);
                OrmUtils.CommitTransaction(trans);
            }
            catch (Exception exp)
            {
                OrmUtils.RollbackTransaction(trans);
            }

            List<dynamic> result = OrmUtils.Model("Org").Equals("Name", "事务测试-机构01").Query();
            Assert.AreEqual<int>(result.Count, 1);

            List<dynamic> result2 = OrmUtils.Model("User").Equals("Name", "事务测试-用户01").Query();
            Assert.AreEqual<int>(result2.Count, 1);
        }
    }
}