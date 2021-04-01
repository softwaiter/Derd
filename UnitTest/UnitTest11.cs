using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest11
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

        [Description("获取当前数据库版本，版本控制未启动，应返回-1")]
        public void Test1()
        {
            int ver = OrmUtils.GetVersion();
            Assert.AreEqual(ver, -1);
        }

        [Description("启动数据库版本控制，应成功")]
        public void Test2()
        {
            OrmUtils.EnableVersionControl();
            int ver = OrmUtils.GetVersion();
            Assert.AreEqual(ver, 0);
        }

        [Description("设置新版本号为1，应成功")]
        public void Test3()
        {
            bool ret = OrmUtils.SetVersion(1);
            Assert.IsTrue(ret);
        }

        [Description("设置新版本号为0，应失败")]
        public void Test4()
        {
            bool ret = OrmUtils.SetVersion(0);
            Assert.IsFalse(ret);
        }

        [Description("查询当前版本号，应返回1")]
        public void Test5()
        {
            int ver = OrmUtils.GetVersion();
            Assert.AreEqual(ver, 1);
        }

    }
}
