using CodeM.Common.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LoadModels()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");

            OrmUtils.ModelPath = modelPath;
            OrmUtils.Load();
            Assert.IsTrue(OrmUtils.IsDefind("User"));
        }
    }
}
