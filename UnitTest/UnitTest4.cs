using CodeM.Common.Orm;
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
            Test8();
            //Test7();
        }

        public void Test8()
        {
            int i = 10000;
            while (i > 0)
            {
                dynamic newuser = ModelObject.New("User");
                newuser.Name = "wangxm" + i;
                newuser.Age = 18;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = true;
                OrmUtils.Model("User").SetValues(newuser).Save();

                i--;
            }
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

        [Description("按分页查询，每页1条数据，查询第1页，返回记录条数应为1条。")]
        public void Test5()
        {
            List<ModelObject> result = OrmUtils.Model("user").PageSize(1).PageIndex(1).Query();
            Assert.AreEqual(result.Count, 1);
        }

        [Description("使用Top(1)方法和PageIndex(1).PageSize(1)分别查询，返回结果数量应一致。")]
        public void Test6()
        {
            List<ModelObject> result = OrmUtils.Model("user").PageSize(1).PageIndex(1).Query();
            List<ModelObject> result2 = OrmUtils.Model("user").Top(1).Query();
            Assert.AreEqual(result.Count, result2.Count);
        }

        [Description("删除Test1测试中创建的User模型物理表，应成功。")]
        public void Test7()
        {
            bool ret = OrmUtils.Model("User").RemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
