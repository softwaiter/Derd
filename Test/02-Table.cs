using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test
{
    [TestClass]
    public class _02_Table
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            Derd.ModelPath = modelPath;
            Derd.Load();

            Derd.RemoveTables();
        }

        [TestMethod]
        [Description("根据User模型创建物理表，应成功")]
        public void CreateUserTable()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("再次创建User模型物理表，表已存在，不做任何操作直接返回成功")]
        public void CreateUserTableAgin()
        {
            bool ret = Derd.Model("Person").TryCreateTable();
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("使用Force参数第三次创建User模型物理表，应成功。")]
        public void CreateUserAginByForceParameter()
        {
            bool ret = Derd.Model("Person").TryCreateTable(true);
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("删除CreateUserAginByForceParameter测试中创建的User模型物理表，应成功。")]
        public void DropUserTable()
        {
            bool ret = Derd.Model("Person").TryRemoveTable();
            Assert.IsTrue(ret);
        }

    }
}
