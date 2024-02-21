using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test
{
    [TestClass]
    public class _03_Insert
    {
        [TestInitialize]
        public void Init()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            Derd.ModelPath = modelPath;
            Derd.Load();

            Derd.TryCreateTables(true);

            InitData();
        }

        private void InitData()
        {
            dynamic newperson = new DynamicObjectExt();
            newperson.Name = "zhangsan";
            newperson.Age = 21;
            Derd.Model("Person").SetValues(newperson).Save();
        }

        [TestMethod]
        [Description("向Person模型的物理表写入一条数据，应成功。")]
        public void NormalInsert()
        {
            dynamic newperson = new DynamicObjectExt();
            newperson.Name = "wangxm";
            newperson.Age = 18;
            newperson.Birthday = new DateTime(1980, 6, 14);
            newperson.Deposit = 99999999;
            newperson.IsAdmin = null;
            bool ret = Derd.Model("Person").SetValues(newperson).Save();
            Assert.IsTrue(ret);
        }

        [TestMethod]
        [Description("向Person模型的物理表写入一条数据，Name重复，应失败。")]
        public void DuplicateValue()
        {
            try
            {
                bool ret = Derd.Model("Person").SetValue("Name", "zhangsan").Save();
                Assert.IsFalse(ret);
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.ToUpper().Contains("UNIQUE") ||
                    exp.Message.ToUpper().Contains("DUPLICATE") ||
                    exp.Message.ToUpper().Contains("ORA-00001") ||
                    exp.Message.ToUpper().Contains("唯一约束") ||
                    exp.Message.ToUpper().Contains("唯一性约束"));
            }
        }

        [TestMethod]
        [Description("向Person模型的物理表写入一条数据，NotNull属性Name不赋值，应失败。")]
        public void NotNullProperty()
        {
            try
            {
                bool ret = Derd.Model("Person").SetValue("Age", 18).Save();
                Assert.IsFalse(ret);
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase) ||
                        exp.Message.Contains("非空约束", StringComparison.OrdinalIgnoreCase) ||
                        exp.Message.Contains("无法将 NULL 插入", StringComparison.OrdinalIgnoreCase) ||
                        exp.Message.Contains("违反了非空约束", StringComparison.OrdinalIgnoreCase) ||
                        exp.Message.Contains("不能将值 NULL 插入列", StringComparison.OrdinalIgnoreCase) ||
                        exp.Message.Contains("doesn't have a default value", StringComparison.OrdinalIgnoreCase) ||
                        exp.Message.Contains("cannot be null", StringComparison.OrdinalIgnoreCase));
            }
        }

        [TestMethod]
        [Description("向Person模型的物理表不设置内容保存，应失败。")]
        public void NoDataInsert()
        {
            try
            {
                bool ret = Derd.Model("Person").Save();
                Assert.IsFalse(ret);
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.Contains("未找到要保存的内容"));
            }
        }

        [TestMethod]
        [Description("向Person模型物理表写入一条数据，Birthday属性使用DATE函数赋值，应成功。")]
        public void FunctionValue()
        {
            dynamic newperson = new DynamicObjectExt();
            newperson.Name = "huxy";
            newperson.Age = 18;
            newperson.Birthday = Funcs.DATE(null);
            bool ret = Derd.Model("Person").SetValues(newperson).Save();
            Assert.IsTrue(ret);

            dynamic personObj = Derd.Model("Person").Equals("Name", "huxy").QueryFirst();
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), personObj.Birthday.ToString("yyyy-MM-dd"));
        }

        [TestMethod]
        [Description("向Person模型物理表写入一条数据，不指定Birthday属性，应使用默认值1980/01/01，应成功。")]
        public void DefaultBirthday()
        {
            dynamic newperson = new DynamicObjectExt();
            newperson.Name = "wangss";
            newperson.Age = 18;
            bool ret = Derd.Model("Person").SetValues(newperson).Save();
            Assert.IsTrue(ret);

            dynamic personObj = Derd.Model("Person").Equals("Name", "wangss").QueryFirst();
            Assert.AreEqual("1980/01/01", personObj.Birthday.ToString("yyyy/MM/dd"));
        }

        [TestMethod]
        [Description("向Person模型物理表写入一条数据，为joinInsert设置为false的Id属性赋值，应无效。")]
        public void NotJoinInsert()
        {
            dynamic newperson = new DynamicObjectExt();
            newperson.Id = 100;
            newperson.Name = "manman";
            bool ret = Derd.Model("Person").SetValues(newperson).Save();
            Assert.IsTrue(ret);

            dynamic personObj = Derd.Model("Person").Equals("Name", "manman").QueryFirst();
            Assert.AreNotEqual(100, personObj.Id);
        }

        [TestMethod]
        [Description("使用SqlExpr方法为Email属性赋值，应成功。")]
        public void SetValueBySqlExpr()
        {
            dynamic newperson = new DynamicObjectExt();
            newperson.Id = 100;
            newperson.Name = "liaoliao";
            newperson.Org = "microsoft";
            newperson.Email = PropValues.SqlExpr("'liaoliao@microsoft.com'");
            bool ret = Derd.Model("Person").SetValues(newperson).Save();
            Assert.IsTrue(ret);

            dynamic personObj = Derd.Model("Person").Equals("Name", "liaoliao").QueryFirst();
            Assert.AreEqual("liaoliao@microsoft.com", personObj.Email);
        }
    }
}