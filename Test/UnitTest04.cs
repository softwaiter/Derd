using CodeM.Common.Orm;
using CodeM.Common.Tools.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest04
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
            Test12();
            Test13();
            Test14();
            Test15();
            Test16();
            Test17();
            Test18();
            Test19();
            Test20();
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
            newuser.IsAdmin = true;
            bool ret = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("查询用户wangxm的信息，并判断年龄是否为18，应为True。")]
        public void Test3()
        {
            List<dynamic> result = Derd.Model("person").GetValue("name", "age").Equals("name", "wangxm").Query();
            Assert.IsTrue(result[0].age == 18);
        }

        [Description("向User模型的物理表写入一条数据，应成功。")]
        public void Test4()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "huxinyue";
            newuser.Age = 11;
            newuser.Birthday = new DateTime(1987, 4, 26);
            newuser.Deposit = 99999999;
            newuser.IsAdmin = true;
            bool ret = Derd.Model("Person").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("使用In操作查询用户，应返回对应数据条数。")]
        public void Test5()
        {
            List<dynamic> result = Derd.Model("Person").In("Name", "wangxm", "huxinyue").Query();
            Assert.AreEqual(result.Count, 2);
            List<dynamic> result2 = Derd.Model("Person").In("Name", "").Query();
            Assert.AreEqual(result2.Count, 0);
        }

        [Description("查询返回的对象属性应为模型定义时指定的属性。")]
        public void Test6()
        {
            List<dynamic> result = Derd.Model("Person").Equals("Name", "huxinyue").Top(1).Query();
            Assert.AreEqual(result.Count, 1);
            Assert.IsInstanceOfType(result[0].Name, typeof(string));
            Assert.IsInstanceOfType(result[0].Age, typeof(UInt16));
            Assert.IsInstanceOfType(result[0].Birthday, typeof(DateTime));
            Assert.IsInstanceOfType(result[0].Deposit, typeof(decimal));
            Assert.IsInstanceOfType(result[0].IsAdmin, typeof(bool));
        }

        [Description("查找age大于11的用户，返回结果条数应为1。")]
        public void Test7()
        {
            List<dynamic> result = Derd.Model("Person").Gt("age", 11).Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("查找age大于等于11的用户，返回结果条数应为2。")]
        public void Test8()
        {
            List<dynamic> result = Derd.Model("Person").Gte("age", 11).Query();
            Assert.AreEqual(result.Count, 2);
        }

        [Description("查找age小于18的用户，返回结果条数应为1。")]
        public void Test9()
        {
            List<dynamic> result = Derd.Model("Person").Lt("age", 18).Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("查找age小于等于18的用户，返回结果条数应为2。")]
        public void Test10()
        {
            List<dynamic> result = Derd.Model("Person").Lte("age", 18).Query();
            Assert.AreEqual(result.Count, 2);
        }

        [Description("使用Like方法过滤查询用户名中间包含x的用户，返回结果条数应为2。")]
        public void Test11()
        {
            List<dynamic> result = Derd.Model("Person").Like("name", "%x%").Query();
            Assert.AreEqual(result.Count, 2);
        }

        [Description("使用Like方法过滤查询用户名以wang开始的用户，返回结果条数应为1。")]
        public void Test12()
        {
            List<dynamic> result = Derd.Model("Person").Like("name", "wang%").Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("查询所有Name为Null的数据，返回数量应为0。")]
        public void Test13()
        {
            List<dynamic> result = Derd.Model("Person").IsNull("name").Query();
            Assert.AreEqual(result.Count, 0);
        }

        [Description("查询所有Name不为Null的数据，返回数量应为2。")]
        public void Test14()
        {
            List<dynamic> result = Derd.Model("Person").IsNotNull("name").Query();
            Assert.AreEqual(result.Count, 2);
        }

        [Description("使用Between方法查询年龄在17-19之间的人，应返回wangxm，其年龄是18。")]
        public void Test15()
        {
            List<dynamic> result = Derd.Model("Person").Between("age", 17, 19).Query();
            Assert.AreEqual(result[0].Age, 18);
        }

        [Description("对Name属性进行升序排列查询第1条记录，返回数据的Name值应为huxinyue。")]
        public void Test16()
        {
            List<dynamic> result = Derd.Model("person").Top(1).AscendingSort("Name").Query();
            Assert.AreEqual(result[0].Name, "huxinyue");
        }

        [Description("对Name属性进行升序排列查询第1条记录，返回数据的Name值应为wangxm。")]
        public void Test17()
        {
            List<dynamic> result = Derd.Model("person").Top(1).DescendingSort("Name").Query();
            Assert.AreEqual(result[0].Name, "wangxm");
        }

        [Description("按分页查询，每页1条数据，查询第1页，返回记录条数应为1条。")]
        public void Test18()
        {
            List<dynamic> result = Derd.Model("person").PageSize(1).PageIndex(1).Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("使用Top(1)方法和PageIndex(1).PageSize(1)分别查询，返回结果数量应一致。")]
        public void Test19()
        {
            List<dynamic> result = Derd.Model("person").PageSize(1).PageIndex(1).Query();
            List<dynamic> result2 = Derd.Model("person").Top(1).Query();
            Assert.AreEqual(result.Count, result2.Count);
        }

        [Description("删除Test1测试中创建的User模型物理表，应成功。")]
        public void Test20()
        {
            bool ret = Derd.Model("Person").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
