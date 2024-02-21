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
            Test5();
        }

        [Description("获取当前数据库版本，版本控制未启动，应返回0")]
        public void Test1()
        {
            int ver = Derd.GetVersion();
            Assert.AreEqual(ver, 0);
        }

        [Description("启动数据库版本控制，应成功")]
        public void Test2()
        {
            bool enabled = Derd.IsVersionControlEnabled();
            Assert.IsFalse(enabled);

            Derd.EnableVersionControl();
            enabled = Derd.IsVersionControlEnabled();
            Assert.IsTrue(enabled);
        }

        [Description("设置新版本号为1，应成功")]
        public void Test3()
        {
            bool ret = Derd.SetVersion("/", 1);
            Assert.IsTrue(ret);
        }

        [Description("设置新版本号为0，应失败")]
        public void Test4()
        {
            bool ret = Derd.SetVersion("/", 0);
            Assert.IsFalse(ret);
        }

        [Description("查询当前版本号，应返回1")]
        public void Test5()
        {
            int ver = Derd.GetVersion();
            Assert.AreEqual(ver, 1);
        }

    }
}
