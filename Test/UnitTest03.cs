using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using CodeM.Common.Tools.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest03
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
            Test5();
            Test6();
            Test7();
            Test8();
            Test9();
            Test10();
            Test11();
        }

        [Description("创建User模型的物理表。")]
        public void Test1()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("向User模型的物理表写入一条数据，应成功。")]
        public void Test2()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1980, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = null;
            bool ret = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("向User模型的物理表写入一条数据，Name重复，应失败。")]
        public void Test3()
        {
            try
            {
                bool ret = Derd.Model("Person").SetValue("Name", "wangxm").Save();
                Assert.IsFalse(ret);
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.ToUpper().Contains("UNIQUE") ||
                    exp.Message.ToUpper().Contains("DUPLICATE") ||
                    exp.Message.ToUpper().Contains("ORA-03115"));
            }
        }

        [Description("根据名称更新Test3中写入的数据，修改对应Age为25，Deposit为99999999；应成功。")]
        public void Test4()
        {
            bool ret = Derd.Model("Person").Equals("Name", "wangxm").SetValue("Age", 25).SetValue("Deposit", 99999999).Update();
            Assert.IsTrue(ret);
        }

        [Description("查询Deposit为99999999的用户，应返回1条数据。")]
        public void Test5()
        {
            List<dynamic> result = Derd.Model("Person").Equals("Deposit", 99999999).Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("向User模型的物理表写入一条合法新数据，应成功。")]
        public void Test6()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "jishuwen";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1947, 1, 16);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = true;
            bool ret = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("查看User模型物理表记录条数，应为2。")]
        public void Test7()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangss";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1947, 10, 1);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = true;
            bool ret = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret);

            Assert.IsTrue(Derd.Model("Person").Count() == 3);
            Assert.IsTrue(Derd.Model("Person").GetValue(AggregateType.DISTINCT, "Age").Count() == 2);
        }

        [Description("User模型属性Org真实数据类型应为String，应成功。")]
        public void Test8()
        {
            Property p = Derd.Model("Person").GetProperty("Org");
            Assert.AreEqual<Type>(p.RealType, typeof(string));
        }

        [Description("使用QueryFirst查询第一条用户对象，返回对象名应为wangxm")]
        public void Test9()
        {
            dynamic user = Derd.Model("Person").QueryFirst();
            Assert.AreEqual(user.Name, "wangxm");
        }

        [Description("查询IsAdmin为true的用户，返回对象名应为jishuwen")]
        public void Test10()
        {
            dynamic user = Derd.Model("Person").Equals("IsAdmin", true).QueryFirst();
            Assert.AreEqual(user.Name, "jishuwen");
        }

        [Description("删除Test1测试中创建的User模型物理表，应成功。")]
        public void Test11()
        {
            bool ret = Derd.Model("Person").TryRemoveTable();
            Assert.IsTrue(ret);
        }
    }
}
