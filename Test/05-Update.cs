using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test
{
    [TestClass]
    public class _05_Update
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
            dynamic newOrg = new DynamicObjectExt();
            newOrg.Code = "microsoft";
            newOrg.Name = "微软科技有限公司";
            newOrg.RegisterDate = new DateTime(1949, 10, 1, 10, 10, 10);
            Derd.Model("Org").SetValues(newOrg).Save();

            dynamic newOrg2 = new DynamicObjectExt();
            newOrg2.Code = "ibm";
            newOrg2.Name = "IBM科技有限公司";
            newOrg2.RegisterDate = new DateTime(1949, 2, 15, 1, 1, 1);
            Derd.Model("Org").SetValues(newOrg2).Save();

            dynamic newPerson = new DynamicObjectExt();
            newPerson.Name = "张大";
            newPerson.Age = 60;
            newPerson.Deposit = 10000;
            newPerson.Birthday = new DateTime(1949, 10, 1, 10, 10, 10);
            newPerson.Org = "microsoft";
            newPerson.IsAdmin = true;
            Derd.Model("Person").SetValues(newPerson).Save();

            dynamic newPerson2 = new DynamicObjectExt();
            newPerson2.Name = "张二";
            newPerson2.Age = 18;
            newPerson2.Deposit = 500;
            newPerson2.Birthday = new DateTime(1980, 7, 25, 8, 8, 8);
            newPerson2.Org = "microsoft";
            newPerson2.Mobile = "13600000000";
            Derd.Model("Person").SetValues(newPerson2).Save();

            dynamic newPerson3 = new DynamicObjectExt();
            newPerson3.Name = "李大";
            newPerson3.Age = 58;
            newPerson3.Deposit = 9000;
            newPerson3.Birthday = new DateTime(1949, 2, 15, 10, 10, 10);
            newPerson3.Org = "ibm";
            newPerson3.IDCard = "130602199907250951";
            newPerson3.IsAdmin = true;
            Derd.Model("Person").SetValues(newPerson3).Save();

            dynamic newPerson4 = new DynamicObjectExt();
            newPerson4.Name = "李二";
            newPerson4.Age = 19;
            newPerson4.Deposit = 1000;
            newPerson4.Birthday = new DateTime(1979, 3, 15, 6, 6, 6);
            newPerson4.Org = "ibm";
            newPerson4.Mobile = "13800000000";
            Derd.Model("Person").SetValues(newPerson4).Save();
        }

        [TestMethod]
        [Description("修改microsoft公司Name属性为“微软科技”，应成功。")]
        public void UpdateMicrosoftName()
        {
            bool bRet = Derd.Model("Org")
                .Equals("Code", "microsoft")
                .SetValue("Name", "微软科技")
                .Update();
            Assert.IsTrue(bRet);

            dynamic orgObj = Derd.Model("Org").Equals("Code", "microsoft").QueryFirst();
            Assert.AreEqual("微软科技", orgObj.Name);
        }

        [TestMethod]
        [Description("更新所有人员的年龄为18，不指定允许修改所有数据参数，触发安全策略，应失败。")]
        public void UpdateAllPersonNameFailed()
        {
            try
            {
                Derd.Model("Person").SetValue("Age", 18).Update();
            }
            catch (Exception exp)
            {
                Assert.IsTrue(exp.Message.Contains("未设置更新的条件范围。"));
            }
        }

        [TestMethod]
        [Description("更新所有人员的年龄为18，指定允许修改所有数据参数，应成功。")]
        public void UpdateAllPersonNameSuccessful()
        {
            bool bRet = Derd.Model("Person").SetValue("Age", 18).Update(true);
            Assert.IsTrue(bRet);
        }

        [TestMethod]
        [Description("修改张大的CreateTime属性值为1949-10-01 12:00:00，因为joinUpdate为false，应失败。")]
        public void UpdateJoinUpdateProperty()
        {
            DateTime createTime = new DateTime(1949, 10, 1, 12, 0, 0);

            bool bRet = Derd.Model("Person")
                .Equals("Name", "张大")
                .SetValue("CreateTime", createTime)
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "张大").QueryFirst();
            Assert.AreNotEqual(createTime, personObj.CreateTime);
        }

        [TestMethod]
        [Description("使用DATE函数修改李大的Birthday属性为当前日期，应成功。")]
        public void UpdateBirthdayByDateFunction()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "李大")
                .SetValue("Birthday", Funcs.DATE(null))
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "李大").QueryFirst();
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), personObj.Birthday.ToString("yyyy-MM-dd"));
        }

        [TestMethod]
        [Description("使用SelfAdd方法为张大增加一岁，应成功；查询其年龄应为61。")]
        public void UpdateWithSelfAdd()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "张大")
                .SetValue("Age", PropValues.SelfAdd())
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "张大").QueryFirst();
            Assert.AreEqual(61, personObj.Age);
        }

        [TestMethod]
        [Description("使用SelfSubtract方法为李二减去5岁，应成功；查询其年龄应为14。")]
        public void UpdateWithSelfSubtract()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "李二")
                .SetValue("Age", PropValues.SelfSubtract(5))
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "李二").QueryFirst();
            Assert.AreEqual(14, personObj.Age);
        }

        [TestMethod]
        [Description("使用SelfMultiply方法为张二的存款扩大10倍，应成功；查询其存款应为5000。")]
        public void UpdateWithSelfMultiply()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "张二")
                .SetValue("Deposit", PropValues.SelfMultiply(10))
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "张二").QueryFirst();
            Assert.AreEqual(5000, personObj.Deposit);
        }

        [TestMethod]
        [Description("使用SelfDivide方法为李二的存款缩小2倍，应成功；查询其存款应为500。")]
        public void UpdateWithSelfDivide()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "李二")
                .SetValue("Deposit", PropValues.SelfDivide(2))
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "李二").QueryFirst();
            Assert.AreEqual(500, personObj.Deposit);
        }

        [TestMethod]
        [Description("使用SqlExpr方法修改李大的身份证号为100000000000000000，应成功。")]
        public void UpdateWithSqlExpr()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "李大")
                .SetValue("IDCard", PropValues.SqlExpr("100000000000000000"))
                .Update();
            Assert.IsTrue(bRet);

            dynamic personObj = Derd.Model("Person").Equals("Name", "李大").QueryFirst();
            Assert.AreEqual("100000000000000000", personObj.IDCard);
        }
    }
}