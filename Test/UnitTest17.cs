using CodeM.Common.Orm;
using CodeM.Common.Tools.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest17
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
        }

        [Description("根据User模型创建物理表，应成功")]
        public void Test1()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("初始化测试数据")]
        public void Test2()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 30;
            newuser.Birthday = new DateTime(1992, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = null;
            bool ret = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret);

            dynamic newuser2 = new DynamicObjectExt();
            newuser2.Name = "huxy";
            newuser2.Age = 23;
            newuser2.Birthday = new DateTime(1999, 4, 29);
            newuser2.Deposit = 10000000.58;
            newuser2.IsAdmin = null;
            bool ret2 = Derd.Model("Person").SetValues(newuser2).Save();
            Assert.IsTrue(ret2);

            dynamic newuser3 = new DynamicObjectExt();
            newuser3.Name = "jisw";
            newuser3.Age = 50;
            newuser3.Birthday = new DateTime(1972, 1, 16);
            newuser3.Deposit = 10000000.58;
            newuser3.IsAdmin = null;
            bool ret3 = Derd.Model("Person").SetValues(newuser3).Save();
            Assert.IsTrue(ret3);

            dynamic newuser4 = new DynamicObjectExt();
            newuser4.Name = "wangss";
            newuser4.Age = 50;
            newuser4.Birthday = new DateTime(1972, 10, 1);
            newuser4.Deposit = 10000000.58;
            newuser4.IsAdmin = null;
            bool ret4 = Derd.Model("Person").SetValues(newuser4).Save();
            Assert.IsTrue(ret4);
        }

        [Description("统计人员的年龄分布情况，统计结果应为3。")]
        public void Test3()
        {
            List<dynamic> result = Derd.Model("Person")
                .GetValue(AggregateType.COUNT, AggregateType.DISTINCT, "Age", "Count")
                .Query();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3, result[0].Count);
        }
    }
}
