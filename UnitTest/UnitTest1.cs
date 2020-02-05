using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Init()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();
        }

        [TestMethod]
        [Description("加载模型定义，判断是否定义User模型应返回True")]
        public void LoadModels()
        {
            Assert.IsTrue(OrmUtils.IsDefind("User"));
        }

        [TestMethod]
        [Description("获取User模型，应该返回True")]
        public void GetUserModel()
        {
            Model m = OrmUtils.Model("User");
            Assert.IsNotNull(m);
            Assert.AreEqual<int>(m.PropertyCount, 2);
        }
    }
}
