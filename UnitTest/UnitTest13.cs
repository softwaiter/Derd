using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class UnitTest13
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

            OrmUtils.EnableDebug(true);
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
            bool ret = OrmUtils.Model("Shopping").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("创建一条Shopping记录，应成功。")]
        public void Test2()
        {
            dynamic newshopping = ModelObject.New("Shopping");
            newshopping.Name = "iPhone 12";
            newshopping.Order = "S202100812085255000";
            bool ret = OrmUtils.Model("Shopping").SetValues(newshopping).Save();
            Assert.IsTrue(ret);
        }

        [Description("修改订单号S202100812085255000d的订单号为: S202100812085255001，应成功。")]
        public void Test3()
        {
            bool bRet = OrmUtils.Model("Shopping").Equals("Order", "S202100812085255000").SetValue("Order", "S202100812085255001").Update();
            Assert.IsTrue(bRet);
        }

        [Description("查询订单号为S202100812085255001的记录，返回记录的商品名称：iPhone 12。")]
        public void Test4()
        {
            dynamic obj = OrmUtils.Model("Shopping").Equals("Order", "S202100812085255001").QueryFirst();
            Assert.AreEqual(obj.Name, "iPhone 12");
        }
    }
}
