﻿using CodeM.Common.Orm;
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

        [Description("判断User模型对应的物理表是否已创建，应返回false")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("User").TableExists();
            Assert.IsFalse(ret);
        }

        [Description("根据User模型创建物理表，应成功")]
        public void Test2()
        {
            bool ret = OrmUtils.Model("User").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("判断User模型对应的物理表是否已创建，应返回true")]
        public void Test3()
        {
            bool ret = OrmUtils.Model("User").TableExists();
            Assert.IsTrue(ret);
        }

        [Description("删除测试中创建的User模型物理表，应成功。")]
        public void Test4()
        {
            bool ret = OrmUtils.Model("User").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}