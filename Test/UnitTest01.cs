using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest01
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

            RemoveOrmTestTable();
        }

        private void RemoveOrmTestTable()
        {
            try
            {
                string sql = "Drop Table orm_test";
                Derd.ExecSql(sql);
            }
            catch
            {
                ;
            }
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

        [Description("加载模型定义，判断是否定义Person模型应返回True")]
        public void Test1()
        {
            Assert.IsTrue(Derd.IsDefind("Person"));
        }

        [Description("获取Person模型，应该返回True")]
        public void Test2()
        {
            Model m = Derd.Model("Person");
            Assert.IsNotNull(m);
            Assert.AreEqual(12, m.PropertyCount);
            Assert.AreEqual<string>(m.Table, "t_person");
        }

        [Description("判断Person模型是否包含Name属性定义，应返回True")]
        public void Test3()
        {
            bool ret = Derd.Model("Person").HasProperty("Name");
            Assert.IsTrue(ret);
        }

        [Description("判断Person模型是否包含Address属性定义，应返回False")]
        public void Test4()
        {
            bool ret = Derd.Model("Person").HasProperty("Address");
            Assert.IsFalse(ret);
        }

        [Description("使用Orm方法直接执行sql语句创建数据表orm_test，应成功。")]
        public void Test5()
        {
            string sql = "Create Table orm_test(id integer primary key, name varchar(64), age int, address varchar(255))";
            int ret = Derd.ExecSql(sql);
            Assert.IsTrue(ret == 0);
        }

        [Description("删除Test3创建的数据表orm_test，应成功。")]
        public void Test6()
        {
            string sql = "Drop Table  orm_test";
            int ret = Derd.ExecSql(sql);
            Assert.IsTrue(ret == 0);
        }

    }
}
