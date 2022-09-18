using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest08
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
        }

        [Description("根据User模型创建物理表，应成功")]
        public void Test1()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("插入10000条数据备用")]
        public void Test2()
        {
            Random r = new Random();
            for (int i = 0; i < 10000; i++)
            {
                dynamic newuser = new DynamicObjectExt();
                newuser.Name = "Person" + (i + 1);
                newuser.Age = r.Next(18, 150);
                newuser.Birthday = new DateTime(1980, 1, 16);
                newuser.Deposit = r.Next(10000000, 999999999);
                newuser.IsAdmin = true;
                Derd.Model("Person").SetValues(newuser).Save();
            }
        }

        [Description("使用In方法查询用户名为Person1、Person5、Person32的用户，应成功返回3条数据。")]
        public void Test3()
        {
            List<dynamic> result = Derd.Model("Person").In("Name", "Person1", "Person5", "Person32").AscendingSort("Name").Query();
            Assert.AreEqual<int>(result.Count, 3);
            Assert.AreEqual<string>(result[2].Name, "Person5");
        }

        [Description("使用NotIn方法查询用户名不是Person1、Person5、Person32的用户，应成功返回9997条数据。")]
        public void Test4()
        {
            List<dynamic> result = Derd.Model("Person").NotIn("Name", "Person1", "Person5", "Person32").AscendingSort("Name").Query();
            Assert.AreEqual<int>(result.Count, 9997);
            Assert.AreEqual<string>(result[0].Name, "Person10");
        }

        [Description("根据年龄分类统计各年龄段人数，返回人数最多的5各年龄段。")]
        public void Test5()
        {
            List<dynamic> result = Derd.Model("Person")
                .GroupBy("Age")
                .GetValue(AggregateType.COUNT, "Id", "Id_Count")
                .GetValue("Age")
                .DescendingSort("Id_Count")
                .Top(5)
                .Query();
            Assert.AreEqual<int>(result.Count, 5);
        }
    }
}