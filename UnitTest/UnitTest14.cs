using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest14
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
            Test5();
            Test6();
            Test7();
            Test8();
        }

        [Description("创建User模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("User").TryCreateTable(true);
            Assert.IsTrue(ret);

            dynamic newuser = ModelObject.New("User");
            newuser.Name = "wangxm";
            newuser.Age = 18;
            bool ret2 = OrmUtils.Model("User").SetValues(newuser).Save();
            Assert.IsTrue(ret2);

            dynamic newuser2 = ModelObject.New("User");
            newuser2.Name = "hxy";
            newuser2.Age = 14;
            bool ret3 = OrmUtils.Model("User").SetValues(newuser2).Save();
            Assert.IsTrue(ret3);

            dynamic newuser3 = ModelObject.New("User");
            newuser3.Name = "zhangsan";
            newuser3.Age = 18;
            bool ret4 = OrmUtils.Model("User").SetValues(newuser3).Save();
            Assert.IsTrue(ret4);

            dynamic newuser4 = ModelObject.New("User");
            newuser4.Name = "lisi";
            newuser4.Age = 10;
            bool ret5 = OrmUtils.Model("User").SetValues(newuser4).Save();
            Assert.IsTrue(ret5);
        }

        [Description("使用Distinct去重同年龄的人，应查询得到2人。")]
        public void Test2()
        {
            List<dynamic> result = OrmUtils.Model("User").GetValue(Model.AggregateType.DISTINCT, "Age").Query();
            Assert.AreEqual(3, result.Count);
        }

        [Description("计算所有人的年龄之和，应为60。")]
        public void Test3()
        {
            dynamic result = OrmUtils.Model("User").GetValue(Model.AggregateType.SUM, "Age").QueryFirst();
            Assert.AreEqual(60, result.Age);
        }

        [Description("使用COUNT方式获取18岁年龄段的人员数量，应为2人。")]
        public void Test4()
        {
            dynamic result = OrmUtils.Model("User")
                .Equals("Age", 18)
                .GetValue(Model.AggregateType.COUNT, "Id")
                .QueryFirst();
            Assert.AreEqual(2, result.Id_Count);
        }

        [Description("获取所有人中最大的年龄，应为18。")]
        public void Test5()
        {
            dynamic result = OrmUtils.Model("User")
                .GetValue(Model.AggregateType.MAX, "Age")
                .QueryFirst();
            Assert.AreEqual(18, result.Age);
        }

        [Description("获取所有人种最小的年龄，应为10。")]
        public void Test6()
        {
            dynamic result = OrmUtils.Model("User")
                .GetValue(Model.AggregateType.MIN, "Age")
                .QueryFirst();
            Assert.AreEqual(10, result.Age);
        }

        [Description("获取人员平均年龄，应为15。")]
        public void Test7()
        {
            dynamic result = OrmUtils.Model("User")
                .GetValue(Model.AggregateType.AVG, "Age")
                .QueryFirst();
            Assert.AreEqual(15, result.Age);
        }

        [Description("使用GroupBy计算每个年龄段的人数，应该18岁2人，14岁1人，10岁1人。")]
        public void Test8()
        {
            List<dynamic> result = OrmUtils.Model("User")
                .GetValue("Age")
                .GetValue(Model.AggregateType.COUNT, "Id")
                .GroupBy("Age")
                .Query();

            dynamic obj18 = result.Find(item => item.Age == 18);
            Assert.IsNotNull(obj18);
            Assert.AreEqual(2, obj18.Id_Count);

            dynamic obj14 = result.Find(item => item.Age == 14);
            Assert.IsNotNull(obj14);
            Assert.AreEqual(1, obj14.Id_Count);

            dynamic obj10 = result.Find(item => item.Age == 10);
            Assert.IsNotNull(obj10);
            Assert.AreEqual(1, obj10.Id_Count);
        }
    }
}
