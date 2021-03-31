using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UnitTest
{
    [TestClass]
    public class UnitTest06
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
        }

        [Description("创建模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.TryCreateTables(true);
            Assert.IsTrue(ret);
        }

        [Description("向机构表插入一条数据，CreateTime和UpdateTime值应一致。")]
        public void Test2()
        {
            dynamic neworg = ModelObject.New("Org");
            neworg.Code = "XXTech";
            neworg.Name = "XX科技";
            bool ret = OrmUtils.Model("Org").SetValues(neworg).Save();
            Assert.IsTrue(ret);

            List<dynamic> result = OrmUtils.Model("Org").Equals("Name", "XX科技").Top(1).Query();
            Assert.IsTrue(result[0].CreateTime.ToString("G") == result[0].UpdateTime.ToString("G"));
        }

        [Description("修改XX科技为YY科技，由于defaultValue只在新建时起作用，因此UpdateTime不变，和CreateTime一致。")]
        public void Test3()
        {
            OrmUtils.Model("Org").Equals("Name", "XX科技").SetValue("Name", "YY科技").Update();
            List<dynamic> result = OrmUtils.Model("Org").Equals("Name", "YY科技").Top(1).Query();
            Assert.IsTrue(result[0].CreateTime.ToString("G") == result[0].UpdateTime.ToString("G"));
        }

        [Description("为机构YY科技增加一个用户，应成功。")]
        public void Test4()
        {
            List<dynamic> result = OrmUtils.Model("Org").Equals("Name", "YY科技").Top(1).Query();

            dynamic newuser = ModelObject.New("User");
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Org = result[0].Code;
            newuser.Deposit = 99999999;
            newuser.IsAdmin = true;
            bool ret = OrmUtils.Model("User").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("查询用户wangxm的Deposit属性，应为99999999。")]
        public void Test5()
        {
            List<dynamic> result = OrmUtils.Model("User").Equals("Name", "wangxm").Top(1).Query();
            Assert.AreEqual(result[0].Deposit, 99999999);
        }

        [Description("修改用户wangxm的年龄为20，beforeSave处理的作用，UpdateTime应更新，此时大于CreateTime。")]
        public void Test6()
        {
            Thread.Sleep(3000);

            bool ret = OrmUtils.Model("User").Equals("Name", "wangxm").SetValue("Age", 20).Update();
            Assert.IsTrue(ret);

            List<dynamic> result = OrmUtils.Model("User").Equals("Name", "wangxm").Top(1).Query();
            Assert.IsTrue(result[0].UpdateTime > result[0].CreateTime);
        }
    }
}
