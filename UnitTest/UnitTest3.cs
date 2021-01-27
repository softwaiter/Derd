using CodeM.Common.Orm;
using CodeM.Common.Orm.Serialize;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest3
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
            Test7();
        }

        [Description("创建User模型的物理表。")]
        public void Test1()
        {
            bool ret = OrmUtils.Model("User").TryCreateTable(true);
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

        [Description("向User模型的物理表写入一条数据，Name重复，应失败。")]
        public void Test3()
        {
            try
            {
                bool ret = OrmUtils.Model("User").SetValue("Name", "wangxm").Save();
                Assert.IsFalse(ret);
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.ToUpper().Contains("UNIQUE") || exp.Message.ToUpper().Contains("DUPLICATE"));
            }
        }

        [Description("根据名称更新Test3中写入的数据，修改对应Age为25，Deposit为99999999；应成功。")]
        public void Test4()
        {
            bool ret = OrmUtils.Model("User").Equals("Name", "wangxm").SetValue("Age", 25).SetValue("Deposit", 99999999).Update();
            Assert.IsTrue(ret);
        }

        [Description("向User模型的物理表写入一条合法新数据，应成功。")]
        public void Test5()
        {
            dynamic newuser = ModelObject.New("User");
            newuser.Name = "jishuwen";
            newuser.Age = 100;
            newuser.Birthday = new DateTime(1947, 1, 16);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = true;
            bool ret = true;    
            OrmUtils.Model("User").SetValues(newuser).Save();
            Assert.IsTrue(ret);
        }

        [Description("查看User模型物理表记录条数，应为2。")]
        public void Test6()
        {
            Assert.IsTrue(OrmUtils.Model("User").Count() == 2);
        }

        [Description("删除Test1测试中创建的User模型物理表，应成功。")]
        public void Test7()
        {
            bool ret = OrmUtils.Model("User").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
