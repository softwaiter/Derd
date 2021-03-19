using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest9
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
            Test5();
        }

        [Description("创建User模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("User").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("向User模型的物理表写入一条数据，生日属性写入test字符串，应失败。")]
        public void Test2()
        {
            bool ret;
            try
            {
                dynamic newuser = ModelObject.New("User");
                newuser.Name = "wangxm";
                newuser.Age = 18;
                newuser.Birthday = "test";
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = true;
                ret = OrmUtils.Model("User").SetValues(newuser).Save();
            }
            catch
            {
                ret = false;
            }
            Assert.IsFalse(ret);
        }

        [Description("向User模型的物理表写入一条数据，年龄属性写入-1，应失败。")]
        public void Test3()
        {
            bool ret;
            try
            {
                dynamic newuser = ModelObject.New("User");
                newuser.Name = "wangxm";
                newuser.Age = -1;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = true;
                ret = OrmUtils.Model("User").SetValues(newuser).Save();
            }
            catch
            {
                ret = false;
            }
            Assert.IsFalse(ret);
        }

        [Description("向User模型的物理表写入一条数据，年龄属性写入250，应失败。")]
        public void Test4()
        {
            bool ret;
            try
            {
                dynamic newuser = ModelObject.New("User");
                newuser.Name = "wangxm";
                newuser.Age = 250;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = true;
                ret = OrmUtils.Model("User").SetValues(newuser).Save();
            }
            catch
            {
                ret = false;
            }
            Assert.IsFalse(ret);
        }

        [Description("删除Test1测试中创建的User模型物理表，应成功。")]
        public void Test5()
        {
            bool ret = OrmUtils.Model("User").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
