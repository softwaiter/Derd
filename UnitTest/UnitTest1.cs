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
        [Description("加载模型定义，判断是否定义User模型应返回True")]
        public void T1_LoadModels()
        {
            Console.WriteLine("T1_LoadModels");
            Assert.IsTrue(OrmUtils.IsDefind("User"));
        }

        [TestMethod]
        [Description("获取User模型，应该返回True")]
        public void T2_GetUserModel()
        {
            Console.WriteLine("T2_GetUserModel");
            Model m = OrmUtils.Model("User");
            Assert.IsNotNull(m);
            Assert.AreEqual<int>(m.PropertyCount, 4);
        }

        [TestMethod]
        public void T3_ExecuteSql()
        {
            Console.WriteLine("T3_ExecuteSql");
            string sql = "Create Table test(id integer primary key, name varchar(64), age int, address varchar(255))";
            OrmUtils.ExecSql(sql);
        }

        [TestMethod]
        public void T4_DropTable()
        {
            Console.WriteLine("T4_DropTable");
            string sql = "Drop Table test";
            OrmUtils.ExecSql(sql);
        }

    }
}
