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
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();
        }

        [TestMethod]
        [Description("����ģ�Ͷ��壬�ж��Ƿ���Userģ��Ӧ����True")]
        public void T1_LoadModels()
        {
            Assert.IsTrue(OrmUtils.IsDefind("User"));
        }

        [TestMethod]
        [Description("��ȡUserģ�ͣ�Ӧ�÷���True")]
        public void T2_GetUserModel()
        {
            Model m = OrmUtils.Model("User");
            Assert.IsNotNull(m);
            Assert.AreEqual<int>(m.PropertyCount, 3);
        }

        [TestMethod]
        public void T3_ExecuteSql()
        {
            string sql = "Create Table test(id int not null primary key, name varchar(64), age int, address varchar(255))";
            OrmUtils.ExecSql(sql);
        }

        [TestMethod]
        public void T4_DropTable()
        {
            string sql = "Drop Table test";
            OrmUtils.ExecSql(sql);
        }

    }
}