using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    [TestClass]
    public class _06_Query
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
            newOrg.Name = "微软";
            newOrg.RegisterDate = new DateTime(1949, 10, 1, 10, 10, 10);
            Derd.Model("Org").SetValues(newOrg).Save();

            dynamic newOrg2 = new DynamicObjectExt();
            newOrg2.Code = "ibm";
            newOrg2.Name = "IBM";
            newOrg2.RegisterDate = new DateTime(1949, 2, 15, 1, 1, 1);
            Derd.Model("Org").SetValues(newOrg2).Save();

            dynamic newPerson = new DynamicObjectExt();
            newPerson.Name = "张三";
            newPerson.Age = 18;
            newPerson.Deposit = 10000;
            newPerson.Birthday = new DateTime(1949, 10, 1, 10, 10, 10);
            newPerson.Org = "microsoft";
            Derd.Model("Person").SetValues(newPerson).Save();

            dynamic newPerson2 = new DynamicObjectExt();
            newPerson2.Name = "李四";
            newPerson2.Age = 21;
            newPerson2.Deposit = 1000;
            newPerson2.Birthday = new DateTime(1949, 2, 15, 10, 10, 10);
            newPerson2.Org = "ibm";
            Derd.Model("Person").SetValues(newPerson2).Save();
        }

        //[TestMethod]
        //[Description("查询Name等于张三的记录，应返1条记录，且记录Name值为张三")]
        //public void QueryByEquals()
        //{
        //    List<dynamic> result = Derd.Model("Person").Equals("Name", "张三").Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.Count);
        //    Assert.AreEqual(result[0].Name, "张三");
        //}

        //[TestMethod]
        //[Description("查询Name等于王五的记录，应返回0条记录")]
        //public void QueryByEquals2()
        //{
        //    List<dynamic> result = Derd.Model("Person").Equals("Name", "王五").Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(0, result.Count);
        //}

        [TestMethod]
        [Description("使用1=1作为查询条件查询Person记录，应返回2条记录")]
        public void QueryByEquals3()
        {
            List<dynamic> result = Derd.Model("Person").Equals("Org.Name", "1").Query();
            Assert.IsNotNull(result);
            //Assert.AreEqual(1, result.Count);
        }

        //[TestMethod]
        //[Description("查询人员出生日期时间和所属机构登记日期时间相等的记录，应返回一条记录")]
        //public void QueryByEquals3()
        //{
        //    List<dynamic> result = Derd.Model("Person").Equals("Birthday", "Org.RegisterDate").Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.Count);
        //}

        //[TestMethod]
        //[Description("查询年龄大于18的记录，应返回1条记录")]
        //public void QueryByGt()
        //{
        //    List<dynamic> result = Derd.Model("Person").Gt("Age", 18).Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.Count);
        //}

        //[TestMethod]
        //[Description("查询年龄大于10的记录，应返回2条记录")]
        //public void QueryByGt2()
        //{
        //    List<dynamic> result = Derd.Model("Person").Gt("Age", 10).Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.Count);
        //}

        //[TestMethod]
        //[Description("查询年龄小于20的记录，应返回1条记录")]
        //public void QueryByLt()
        //{
        //    List<dynamic> result = Derd.Model("Person").Lt("Age", "20").Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.Count);
        //}

        //[TestMethod]
        //[Description("查询年龄小于30的记录，应返回2条记录")]
        //public void QueryByLt2()
        //{
        //    List<dynamic> result = Derd.Model("Person").Lt("Age", "30").Query();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.Count);
        //}
    }
}
