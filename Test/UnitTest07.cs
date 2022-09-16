using CodeM.Common.Orm;
using CodeM.Common.Tools.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest07
    {
        [TestInitialize]
        public void Init()
        {
            Derd.RegisterProcessor("EncryptDeposit", "UnitTest.Processors.EncryptDeposit");
            Derd.RegisterProcessor("DecryptDeposit", "UnitTest.Processors.DecryptDeposit");

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

        [Description("创建Organization模型的物理表。")]
        public void Test1()
        {
            bool ret = Derd.Model("Org").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("创建User模型的物理表。")]
        public void Test2()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("使用事务新建机构和用户，用户新建失败，新建机构信息也回滚")]
        public void Test3()
        {
            dynamic neworg = new DynamicObjectExt();
            neworg.Code = "Trans_Org";
            neworg.Name = "事务测试-机构01";

            dynamic newuser = new DynamicObjectExt();
            newuser.Age = 18;

            int trans = Derd.GetTransaction();
            try
            {
                Derd.Model("Org").SetValues(neworg).Save(trans);
                Derd.Model("Person").SetValues(newuser).Save(trans);
                Derd.CommitTransaction(trans);
            }
            catch
            {
                Derd.RollbackTransaction(trans);
            }

            List<dynamic> result = Derd.Model("Org").Equals("Name", "事务测试-机构01").Query();
            Assert.AreEqual<int>(result.Count, 0);
        }

        [Description("使用事务新建机构和用户，应成功")]
        public void Test4()
        {
            dynamic neworg = new DynamicObjectExt();
            neworg.Code = "Trans_Org";
            neworg.Name = "事务测试-机构01";

            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "事务测试-用户01";

            int trans = Derd.GetTransaction();
            try
            {
                Derd.Model("Org").SetValues(neworg).Save(trans);
                Derd.Model("Person").SetValues(newuser).Save(trans);
                Derd.CommitTransaction(trans);
            }
            catch
            {
                Derd.RollbackTransaction(trans);
            }

            List<dynamic> result = Derd.Model("Org").Equals("Name", "事务测试-机构01").Query();
            Assert.AreEqual<int>(result.Count, 1);

            List<dynamic> result2 = Derd.Model("Person").Equals("Name", "事务测试-用户01").Query();
            Assert.AreEqual<int>(result2.Count, 1);
        }
    }
}