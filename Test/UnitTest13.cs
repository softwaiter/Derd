using CodeM.Common.Orm;
using CodeM.Common.Tools.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest13
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

        [Description("创建Shopping模型的物理表，应成功。")]
        public void Test1()
        {
            bool ret = Derd.Model("Shopping").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("创建一条Shopping记录，应成功。")]
        public void Test2()
        {
            dynamic newshopping = new DynamicObjectExt();
            newshopping.Code = "iPhone 12";
            newshopping.Name = "苹果12手机";
            newshopping.Order = "S202100812085255000";
            bool ret = Derd.Model("Shopping").SetValues(newshopping).Save();
            Assert.IsTrue(ret);
        }

        [Description("修改订单号S202100812085255000d的订单号为: S202100812085255001，应成功。")]
        public void Test3()
        {
            bool bRet = Derd.Model("Shopping").Equals("Order", "S202100812085255000").SetValue("Order", "S202100812085255001").Update();
            Assert.IsTrue(bRet);
        }

        [Description("查询订单号为S202100812085255001的记录，返回记录的商品编码：iPhone 12。")]
        public void Test4()
        {
            dynamic obj = Derd.Model("Shopping").Equals("Order", "S202100812085255001").QueryFirst();
            Assert.AreEqual(obj.Code, "iPhone 12");
        }
    }
}
