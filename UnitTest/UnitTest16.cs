using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest16
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
            newshopping.Code = "iPhone 12";
            newshopping.Name = "苹果12手机";
            newshopping.Order = "S202100812085255000";
            bool ret = OrmUtils.Model("Shopping").SetValues(newshopping).Save();
            Assert.IsTrue(ret);
        }

        [Description("使用DATE函数，返回记录创建时间，应为当前日期的yyyy-MM-dd格式字符串。")]
        public void Test3()
        {
            dynamic result = OrmUtils.Model("Shopping")
                .Equals("Code", "iPhone 12")
                .GetValue(FunctionType.DATE, "CreateTime")
                .QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), result.CreateTime);
        }

        [Description("将CreateTime转化为yyyy-MM-dd格式，并以CDate别名输出。")]
        public void Test4()
        {
            dynamic result = OrmUtils.Model("Shopping")
                .Equals("Code", "iPhone 12")
                .GetValue(FunctionType.DATE, "CreateTime", "CDate")
                .QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), result.CDate);
        }
    }
}
