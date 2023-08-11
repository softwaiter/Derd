using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test
{
    [TestClass]
    public class _18_BatchInsert
    {
        [TestInitialize]
        public void Init()
        {
            Derd.RegisterProcessor("EncryptDeposit", "Test.Processors.EncryptDeposit");
            Derd.RegisterProcessor("DecryptDeposit", "Test.Processors.DecryptDeposit");

            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            Derd.ModelPath = modelPath;
            Derd.Load();

            Derd.TryCreateTables(true);
        }

        [TestMethod]
        public void BatchInsert10000()
        {
            for (int k = 0; k < 10; k++)
            {
                Model m = Derd.Model("Log");
                for (int i = 0; i < 1000; i++)
                {
                    dynamic newlog = new DynamicObjectExt();
                    newlog.Operator = "wangxm";
                    newlog.Product = "大圆Admin";
                    newlog.Module = "微服务列表";
                    newlog.Content = "这是第" + (k * 100 + i + 1) + "条日志";
                    m.SetBatchInsertValues(newlog);
                }
                bool bRet = m.Save();
                Assert.IsTrue(bRet);
            }
        }

        [TestMethod]
        public void BatchInsert100000()
        {
            for (int k = 0; k < 100; k++)
            {
                Model m = Derd.Model("Log");
                for (int i = 0; i < 1000; i++)
                {
                    dynamic newlog = new DynamicObjectExt();
                    newlog.Operator = "wangxm";
                    newlog.Product = "大圆Admin";
                    newlog.Module = "统计分析";
                    newlog.Content = "这是第" + (k * 1000 + i + 1) + "条日志";
                    m.SetBatchInsertValues(newlog);
                }
                bool bRet = m.Save();
                Assert.IsTrue(bRet);
            }
        }

        [TestMethod]
        public void BatchInsert1000000()
        {
            for (int k = 0; k < 1000; k++)
            {
                Model m = Derd.Model("Log");
                for (int i = 0; i < 1000; i++)
                {
                    dynamic newlog = new DynamicObjectExt();
                    newlog.Operator = "wangxm";
                    newlog.Product = "大圆Admin";
                    newlog.Module = "角色管理";
                    newlog.Content = "这是第" + (k * 1000 + i + 1) + "条日志";
                    m.SetBatchInsertValues(newlog);
                }
                bool bRet = m.Save();
                Assert.IsTrue(bRet);
            }
        }
    }
}
