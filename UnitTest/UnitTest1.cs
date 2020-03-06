using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
        }

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
            Test1();
            Test2();
            Test3();
            Test4();
        }

        [Description("加载模型定义，判断是否定义User模型应返回True")]
        public void Test1()
        {
            Assert.IsTrue(OrmUtils.IsDefind("User"));
        }

        [Description("获取User模型，应该返回True")]
        public void Test2()
        {
            Model m = OrmUtils.Model("User");
            Assert.IsNotNull(m);
            Assert.AreEqual<string>(m.Table, "t_user");
        }

        [Description("使用Orm方法直接执行sql语句创建数据表orm_test")]
        public void Test3()
        {
            string sql = "Create Table orm_test(id integer primary key, name varchar(64), age int, address varchar(255))";
            bool ret = OrmUtils.ExecSql(sql);
            Assert.IsTrue(ret);
        }

        [Description("删除Test3创建的数据表orm_test")]
        public void Test4()
        {
            string sql = "Drop Table orm_test";
            bool ret = OrmUtils.ExecSql(sql);
            Assert.IsTrue(ret);
        }

    }
}
