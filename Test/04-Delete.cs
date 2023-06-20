using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    [TestClass]
    public class _04_Delete
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
        [Description("删除姓名为张大的人，应成功，查询剩余3条记录")]
        public void DeleteByEquals()
        {
            bool bRet = Derd.Model("Person")
                .Equals("Name", "张大")
                .Delete();
            Assert.IsTrue(bRet);

            long count = Derd.Model("Person")
                .Count();
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        [Description("删除所属机构不是IBM科技有限公司的人员，应成功，查询剩余李大、李二2条记录")]
        public void DeleteByNotEquals()
        {
            bool bRet = Derd.Model("Person")
                .NotEquals("Org.Name", "IBM科技有限公司")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("删除年龄大于50岁的人，应成功，查询剩余张二、李二2条记录")]
        public void DeleteByGt()
        {
            bool bRet = Derd.Model("Person")
                .Gt("Age", 50)
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("删除年龄大于等于18岁的人，应成功，查询剩余0条记录")]
        public void DeleteByGte()
        {
            bool bRet = Derd.Model("Person")
                .Gte("Age", 18)
                .Delete();
            Assert.IsTrue(bRet);

            long count = Derd.Model("Person").Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        [Description("删除年龄小于60岁的人，应成功，查询剩余张大1条记录")]
        public void DeleteByLt()
        {
            bool bRet = Derd.Model("Person")
                .Lt("Age", 60)
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张大", result[0].Name);
        }

        [TestMethod]
        [Description("删除年龄小于等于60岁的人，应成功，查询剩余0条记录")]
        public void DeleteByLte()
        {
            bool bRet = Derd.Model("Person")
                .Lte("Age", 60)
                .Delete();
            Assert.IsTrue(bRet);

            long count = Derd.Model("Person").Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        [Description("删除手机号为空的人员，应成功，查询剩余张二、李二2条记录")]
        public void DeleteByIsNull()
        {
            bool bRet = Derd.Model("Person")
                .IsNull("Mobile")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("删除手机号不为空的人员，应成功，查询剩余张大、李大2条记录")]
        public void DeleteByIsNotNull()
        {
            bool bRet = Derd.Model("Person")
                .IsNotNull("Mobile")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("删除所有姓张的人，应成功，查询剩余李大、李二2条记录")]
        public void DeleteByLike()
        {
            bool bRet = Derd.Model("Person")
                .Like("Name", "张%")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("删除所有不姓张的人，应成功，查询剩余张大、张二2条记录")]
        public void DeleteByNotLike()
        {
            bool bRet = Derd.Model("Person")
                .NotLike("Name", "张%")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("张二", result[1].Name);
        }

        [TestMethod]
        [Description("删除出生日期在1970-01-01至1980-12-31之间的人员，应成功，查询剩余张大、李大2条记录")]
        public void DeleteByBetween()
        {
            bool bRet = Derd.Model("Person")
                .Between(Funcs.DATE("Birthday"), "1970-01-01", "1980-12-31")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("删除年龄为18、19、58的人员，应成功，查询剩余张大1条记录")]
        public void DeleteByIn()
        {
            bool bRet = Derd.Model("Person")
                .In("Age", 18, 19, 58)
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张大", result[0].Name);
        }

        [TestMethod]
        [Description("删除姓名不是李二的人，应成功，查询剩余李二1条记录")]
        public void DeleteByNotIn()
        {
            bool bRet = Derd.Model("Person")
                .NotIn("Name", "李二")
                .Delete();
            Assert.IsTrue(bRet);

            List<dynamic> result = Derd.Model("Person").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("李二", result[0].Name);
        }

        [TestMethod]
        [Description("删除所有人员，应成功，查询剩余0条记录")]
        public void DeleteAll()
        {
            bool bRet = Derd.Model("Person").Delete(true);
            Assert.IsTrue(bRet);

            long count = Derd.Model("Person").Count();
            Assert.AreEqual(0, count);
        }
    }
}
