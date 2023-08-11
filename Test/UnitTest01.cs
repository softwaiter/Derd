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
        }

        [TestMethod]
        [Description("加载模型定义，判断是否定义Person模型应返回True")]
        public void Test1()
        {
            Assert.IsTrue(Derd.IsDefind("Person"));
        }

        [TestMethod]
        [Description("获取Person模型，应该返回True")]
        public void Test2()
        {
            Model m = Derd.Model("Person");
            Assert.IsNotNull(m);
            Assert.AreEqual(12, m.PropertyCount);
            Assert.AreEqual<string>(m.Table, "t_person");
        }

        [TestMethod]
        [Description("判断Person模型是否包含Name属性定义，应返回True")]
        public void Test3()
        {
            bool ret = Derd.Model("Person").HasProperty("Name");
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("判断Person模型是否包含Address属性定义，应返回False")]
        public void Test4()
        {
            bool ret = Derd.Model("Person").HasProperty("Address");
            Assert.IsFalse(ret);
        }

        [TestMethod]
        [Description("判断Person模型是否包含Org.Name级联属性定义,应返回True")]
        public void Test5()
        {
            bool ret = Derd.Model("Person").HasProperty("Org.Name");
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("判断Person模型是否包含Org.Address级联属性定义,应返回False")]
        public void Test6()
        {
            bool ret = Derd.Model("Person").HasProperty("Org.Address");
            Assert.IsFalse(ret);
        }
    }
}
