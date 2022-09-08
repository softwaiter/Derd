using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest12
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
        }

        [Description("创建Org模型的物理表，应成功。")]
        public void Test1()
        {
            bool ret = Derd.Model("Org").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [Description("使用ExecProcessor执行CurrentDateTime处理器，并将值代入sql语句，插入一条User记录，应成功。")]
        public void Test2()
        {
            //string value = OrmUtils.ExecProcessor<string>("CurrentDateTime");
            //string sql = string.Format("INSERT INTO t_org(f_code, f_name, f_createtime, f_updatetime) VALUES('mingyue', '明月科技', '{0}', '{1}');", value, value);
            //int ret = OrmUtils.ExecSql(sql);
            //Assert.AreEqual(ret, 1);
        }
    }
}
