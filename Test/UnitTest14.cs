using CodeM.Common.Orm;
using CodeM.Common.Orm.Functions;
using CodeM.Common.Tools.DynamicObject;
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
            Derd.RegisterProcessor("EncryptDeposit", "Test.Processors.EncryptDeposit");
            Derd.RegisterProcessor("DecryptDeposit", "Test.Processors.DecryptDeposit");

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
            Test6();
            Test7();
            Test8();
            Test9();
        }

        [Description("创建User模型的物理表。")]
        public void Test1()
        {
            bool ret = Derd.Model("Org").TryCreateTable(true);
            Assert.IsTrue(ret);

            dynamic neworg = new DynamicObjectExt();
            neworg.Code = "xxtech";
            neworg.Name = "XX科技";
            bool ret2 = Derd.Model("Org").SetValues(neworg).Save();
            Assert.IsTrue(ret2);

            bool ret3 = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret3);

            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Org = neworg.Code;
            bool ret4 = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret4);

            dynamic newuser2 = new DynamicObjectExt();
            newuser2.Name = "hxy";
            newuser2.Age = 14;
            newuser2.Org = neworg.Code;
            bool ret5 = Derd.Model("Person").SetValues(newuser2).Save();
            Assert.IsTrue(ret5);

            dynamic newuser3 = new DynamicObjectExt();
            newuser3.Name = "zhangsan";
            newuser3.Age = 18;
            newuser3.Org = neworg.Code;
            bool ret6 = Derd.Model("Person").SetValues(newuser3).Save();
            Assert.IsTrue(ret6);

            dynamic newuser4 = new DynamicObjectExt();
            newuser4.Name = "lisi";
            newuser4.Age = 10;
            newuser4.Org = neworg.Code;
            bool ret7 = Derd.Model("Person").SetValues(newuser4).Save();
            Assert.IsTrue(ret7);
        }

        [Description("使用Distinct去重同年龄的人，应查询得到2人。")]
        public void Test2()
        {
            List<dynamic> result = Derd.Model("Person").GetValue(Aggregate.DISTINCT("Age")).Query();
            Assert.AreEqual(3, result.Count);
        }

        [Description("计算所有人的年龄之和，应为60。")]
        public void Test3()
        {
            dynamic result = Derd.Model("Person").GetValue(Aggregate.SUM("Age")).QueryFirst();
            Assert.AreEqual(60, result.Age);
        }

        [Description("使用COUNT方式获取18岁年龄段的人员数量，应为2人。")]
        public void Test4()
        {
            dynamic result = Derd.Model("Person")
                .Equals("Age", 18)
                .GetValue(Aggregate.COUNT("Id"))
                .QueryFirst();
            Assert.AreEqual(2, result.Id);
        }

        [Description("获取所有人中最大的年龄，应为18。")]
        public void Test5()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Aggregate.MAX("Age"))
                .QueryFirst();
            Assert.IsTrue(18 == result.Age);
        }

        [Description("获取所有人中最小的年龄，应为10。")]
        public void Test6()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Aggregate.MIN("Age"))
                .QueryFirst();
            Assert.IsTrue(10 == result.Age);
        }

        [Description("获取人员平均年龄，应为15。")]
        public void Test7()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Aggregate.AVG("Age"))
                .QueryFirst();
            Assert.IsTrue(15 == result.Age);
        }

        [Description("使用GroupBy计算每个年龄段的人数，应该18岁2人，14岁1人，10岁1人。")]
        public void Test8()
        {
            List<dynamic> result = Derd.Model("Person")
                .GetValue("Age")
                .GetValue(Aggregate.COUNT("Id"), "UserCount")
                .GroupBy("Age")
                .Query();

            dynamic obj18 = result.Find(item => item.Age == 18);
            Assert.IsNotNull(obj18);
            Assert.AreEqual(2, obj18.UserCount);

            dynamic obj14 = result.Find(item => item.Age == 14);
            Assert.IsNotNull(obj14);
            Assert.AreEqual(1, obj14.UserCount);

            dynamic obj10 = result.Find(item => item.Age == 10);
            Assert.IsNotNull(obj10);
            Assert.AreEqual(1, obj10.UserCount);
        }

        [Description("获取关联对象属性，并设置别名，应成功。")]
        public void Test9()
        {
            List<dynamic> result = Derd.Model("Person")
                .GetValue("Org.Code", "OrgCode")
                .GetValue("Name", "Person.Name")
                .GetValue("Org.Name", "Person.OrgName")
                .Query();
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result[0].Has("OrgCode"));
            Assert.IsTrue(result[0].HasPath("Person.Name"));
            Assert.IsTrue(result[0].HasPath("Person.OrgName"));
        }
    }
}
