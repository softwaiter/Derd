﻿using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UnitTest
{
    [TestClass]
    public class UnitTest4
    {

        [TestInitialize]
        public void Init()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();
        }

        [TestMethod]
        public void Test()
        {
            Thread.Sleep(1000 * 3);

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
        }

        [Description("创建User模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("User").CreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("向User模型的物理表写入一条数据，应成功。")]
        public void Test2()
        {
            dynamic newuser = ModelObject.New("User");
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1980, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = true;
            bool ret = OrmUtils.Model("User").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("查询用户wangxm的信息，并判断年龄是否为18，应为True。")]
        public void Test3()
        {
            dynamic result = OrmUtils.Model("user").GetValue("name", "age").Equals("name", "wangxm").Query();
            bool ret = result[0].age == 18;
            Assert.IsTrue(ret);
        }

        [Description("向User模型的物理表写入一条数据，应成功。")]
        public void Test4()
        {
            dynamic newuser = ModelObject.New("User");
            newuser.Name = "huxinyue";
            newuser.Age = 11;
            newuser.Birthday = new DateTime(1987, 4, 26);
            newuser.Deposit = 99999999;
            newuser.IsAdmin = true;
            bool ret = OrmUtils.Model("User").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("使用Like方法过滤查询用户名中间包含x的用户，返回结果条数应为2。")]
        public void Test5()
        {
            List<dynamic> result = OrmUtils.Model("User").Like("name", "%x%").Query();
            Assert.AreEqual(result.Count, 2);
        }

        [Description("使用Like方法过滤查询用户名以wang开始的用户，返回结果条数应为1。")]
        public void Test6()
        {
            List<dynamic> result = OrmUtils.Model("User").Like("name", "wang%").Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("查询所有Name为Null的数据，返回数量应为0。")]
        public void Test7()
        {
            List<dynamic> result = OrmUtils.Model("User").IsNull("name").Query();
            Assert.AreEqual(result.Count, 0);
        }

        public void Test8()
        {
            List<dynamic> result = OrmUtils.Model("User").IsNotNull("name").Query();
            Assert.AreEqual(result.Count, 2);
        }

        [Description("使用Between方法查询年龄在17-19之间的人，应返回wangxm，其年龄是18。")]
        public void Test9()
        {
            List<dynamic> result = OrmUtils.Model("User").Between("age", 17, 19).Query();
            Assert.AreEqual(result[0].Age, 18);
        }

        [Description("对Name属性进行升序排列查询第1条记录，返回数据的Name值应为huxinyue。")]
        public void Test10()
        {
            List<dynamic> result = OrmUtils.Model("user").Top(1).AscendingSort("Name").Query();
            Assert.AreEqual(result[0].Name, "huxinyue");
        }

        [Description("对Name属性进行升序排列查询第1条记录，返回数据的Name值应为wangxm。")]
        public void Test11()
        {
            List<dynamic> result = OrmUtils.Model("user").Top(1).DescendingSort("Name").Query();
            Assert.AreEqual(result[0].Name, "wangxm");
        }

        [Description("按分页查询，每页1条数据，查询第1页，返回记录条数应为1条。")]
        public void Test12()
        {
            List<dynamic> result = OrmUtils.Model("user").PageSize(1).PageIndex(1).Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("使用Top(1)方法和PageIndex(1).PageSize(1)分别查询，返回结果数量应一致。")]
        public void Test13()
        {
            List<dynamic> result = OrmUtils.Model("user").PageSize(1).PageIndex(1).Query();
            List<dynamic> result2 = OrmUtils.Model("user").Top(1).Query();
            Assert.AreEqual(result.Count, result2.Count);
        }

        [Description("删除Test1测试中创建的User模型物理表，应成功。")]
        public void Test14()
        {
            bool ret = OrmUtils.Model("User").RemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
