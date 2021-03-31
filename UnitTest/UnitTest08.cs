using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest08
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

        [Description("根据User模型创建物理表，应成功")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("User").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("插入10000条数据备用")]
        public void Test2()
        {
            Random r = new Random();
            for (int i = 0; i < 10000; i++)
            {
                dynamic newuser = ModelObject.New("User");
                newuser.Name = "User" + (i + 1);
                newuser.Age = r.Next(18, 150);
                newuser.Birthday = new DateTime(1980, 1, 16);
                newuser.Deposit = r.Next(10000000, 999999999);
                newuser.IsAdmin = true;
                OrmUtils.Model("User").SetValues(newuser).Save();
            }
        }

        [Description("使用In方法查询用户名为User1、User5、User32的用户，应成功返回3条数据。")]
        public void Test3()
        {
            List<dynamic> result = OrmUtils.Model("User").In("Name", "User1", "User5", "User32").AscendingSort("Name").Query();
            Assert.AreEqual<int>(result.Count, 3);
            Assert.AreEqual<string>(result[2].Name, "User5");
        }

        [Description("使用NotIn方法查询用户名不是User1、User5、User32的用户，应成功返回9997条数据。")]
        public void Test4()
        {
            List<dynamic> result = OrmUtils.Model("User").NotIn("Name", "User1", "User5", "User32").AscendingSort("Name").Query();
            Assert.AreEqual<int>(result.Count, 9997);
            Assert.AreEqual<string>(result[0].Name, "User10");
        }
    }
}