using CodeM.Common.Orm;
using CodeM.Common.Orm.Functions;
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
        [Description("查询Name等于张大的人，应返1条记录，且Name值为张大")]
        public void QueryByEquals()
        {
            List<dynamic> result = Derd.Model("Person").Equals("Name", "张大").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Name, "张大");
        }

        [TestMethod]
        [Description("查询Name等于王五的人，应返回0条记录")]
        public void QueryByEquals2()
        {
            List<dynamic> result = Derd.Model("Person").Equals("Name", "王五").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期为1949-10-01的人，应返回1条记录")]
        public void QueryByEquals3()
        {
            List<dynamic> result = Derd.Model("Person")
                .Equals(Date.DATE("Birthday"), "1949-10-01")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        [Description("查询人员出生日期时间和所属机构登记日期时间相等的人，应返回张大、李大2条记录")]
        public void QueryByEquals4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Equals(Date.DATE("Birthday"), Date.DATE("Org.RegisterDate"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询人员所属机构为microsoft或ibm的人员，应返回4条记录")]
        public void QueryByEquals5()
        {
            Model m = Derd.Model("Person");
            List<dynamic> result = m
                .Equals("Org.Code", "microsoft")
                .Or(new SubFilter(m).Equals("Org.Code", "ibm"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        [Description("使用1=1常量过滤条件进行查询，应返回所有4条记录")]
        public void QueryByEquals6()
        {
            List<dynamic> result = Derd.Model("Person").And("1=1").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期为1949-10-01的人，应返回张大1条记录")]
        public void QueryByEquals7()
        {
            List<dynamic> result = Derd.Model("Person")
                .Equals(Date.DATE("Birthday"), Date.DATE("1949-10-01"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张大", result[0].Name);
        }

        [TestMethod]
        [Description("查询人员所属机构不等于microsoft的人，应返回李大、李二2条记录")]
        public void QueryByNotEquals()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotEquals("Org", "microsoft")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期不等于1949-10-01的人，应返回张二、李大、李二3条记录")]
        public void QueryByNotEquals2()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotEquals(Date.DATE("Birthday"), "1949-10-01")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
            Assert.AreEqual("李二", result[2].Name);
        }

        [TestMethod]
        [Description("查询人员出生日期时间和所属机构登记日期时间不相等的人，应返回张二、李二2条记录")]
        public void QueryByNotEquals3()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotEquals(Date.DATE("Birthday"), Date.DATE("Org.RegisterDate"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询人员出生日期不等于1949-02-15的人，应返回3条记录")]
        public void QueryByNotEquals4()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotEquals(Date.DATE("Birthday"), Date.DATE("1949-02-15"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [Description("查询年龄大于18的人，应返回3条记录")]
        public void QueryByGt()
        {
            List<dynamic> result = Derd.Model("Person").Gt("Age", 18).Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [Description("查询年龄大于50的人，应返回2条记录")]
        public void QueryByGt2()
        {
            List<dynamic> result = Derd.Model("Person").Gt("Age", 50).Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [Description("查询人员出生日期大于1949-10-01的人员，应返回张二、李二2条记录")]
        public void QueryByGt3()
        {
            List<dynamic> result = Derd.Model("Person")
                .Gt(Date.DATE("Birthday"), "1949-10-01")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询人员出生日期大于1979-03-15的人，应返回张二1条记录")]
        public void QueryByGt4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Gt(Date.DATE("Birthday"), Date.DATE("1979-03-15"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张二", result[0].Name);
        }

        [TestMethod]
        [Description("查询年龄大于等于20的人，应返回张大、李大2条记录")]
        public void QueryByGte()
        {
            List<dynamic> result = Derd.Model("Person").Gte("Age", 20).Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("张大", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄大于等于18的人，应返回4条记录")]
        public void QueryByGte2()
        {
            List<dynamic> result = Derd.Model("Person").Gte("Age", 18).Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期大于等于1979-03-15的人，应返回2条记录")]
        public void QueryByGte3()
        {
            List<dynamic> result = Derd.Model("Person").Gte(Date.DATE("Birthday"), "1979-03-15").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期大于等于1980-07-25的人，应返回张二1条记录")]
        public void QueryByGte4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Gte(Date.DATE("Birthday"), Date.DATE("1980-07-25"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张二", result[0].Name);
        }

        [TestMethod]
        [Description("查询年龄小于20的人，应返回张二、李二2条记录")]
        public void QueryByLt()
        {
            List<dynamic> result = Derd.Model("Person").Lt("Age", "20").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄小于18岁的人，应返回0人")]
        public void QueryByLt2()
        {
            List<dynamic> result = Derd.Model("Person").Lt("Age", "18").Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期小于1949-10-01的人，应返回李大1条记录")]
        public void QueryByLt3()
        {
            List<dynamic> result = Derd.Model("Person")
                .Lt(Date.DATE("Birthday"), "1949-10-01")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("李大", result[0].Name);
        }

        [TestMethod]
        [Description("查询出生日期小于1979-03-15的人，应返回张二、李二2条记录")]
        public void QueryByLt4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Lt(Date.DATE("Birthday"), "1979-03-15")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄小于等于20岁的人，应返回2个人")]
        public void QueryByLte()
        {
            List<dynamic> result = Derd.Model("Person")
                .Lte("Age", 20)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [Description("查询年龄小于等于18岁的人，应返回张二1个人")]
        public void QueryByLte2()
        {
            List<dynamic> result = Derd.Model("Person")
                .Lte("Age", 18)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张二", result[0].Name);
        }

        [TestMethod]
        [Description("查询出生日期小于等于1949-10-01的人，应返回张大、李大2条记录")]
        public void QueryByLte3()
        {
            List<dynamic> result = Derd.Model("Person")
                .Lte(Date.DATE("Birthday"), "1949-10-01")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期小于等于1949-02-15的人，应返回李大1条记录")]
        public void QueryByLte4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Lte(Date.DATE("Birthday"), Date.DATE("1949-02-15"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("李大", result[0].Name);
        }

        [TestMethod]
        [Description("查询名称以张字开始的人员，应返回张大、张二2条记录")]
        public void QueryByLike()
        {
            List<dynamic> result = Derd.Model("Person")
                .Like("Name", "张%")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("张二", result[1].Name);
        }

        [TestMethod]
        [Description("查询名称以二字结尾的人员，应返回张二、李二2条记录")]
        public void QueryByLike2()
        {
            List<dynamic> result = Derd.Model("Person")
                .Like("Name", "%二")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询机构名称包含科技2字的所有人员，应返回4条记录")]
        public void QueryByLike3()
        {
            List<dynamic> result = Derd.Model("Person")
                .Like("Org.Name", "%科技%")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期为15号的人员，应返回李大、李二2条记录")]
        public void QueryByLike4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Like(Date.DATE("Birthday"), "%15")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期匹配1949-10-01的人，应返回张大1条记录")]
        public void QueryByLike5()
        {
            List<dynamic> result = Derd.Model("Person")
                .Like(Date.DATE("Birthday"), Date.DATE("1949-10-01"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张大", result[0].Name);
        }

        [TestMethod]
        [Description("查询姓名结尾不是二的人员，应返回张大、李大2条记录")]
        public void QueryByNotLike()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotLike("Name", "%二")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询不姓李的人员，应返回张大、张二2条记录")]
        public void QueryByNotLike2()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotLike("Name", "李%")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("张二", result[1].Name);
        }

        [TestMethod]
        [Description("查询机构名称不包含软字的所有人员，应返回2条记录")]
        public void QueryByNotLike3()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotLike("Org.Name", "%软%")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [Description("查询出生日期不是1949年的人，应返回张二、李二2条记录")]
        public void QueryByNotLike4()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotLike(Date.DATE("Birthday"), "1949%")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期不是1949-10-01的人员，应返回3条记录")]
        public void QueryByNotLike5()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotLike(Date.DATE("Birthday"), Date.DATE("1949-10-01"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [Description("查询手机号为空的人员，应返回张大、李大2条记录")]
        public void QueryByIsNull()
        {
            List<dynamic> result = Derd.Model("Person")
                .IsNull("Mobile")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询身份证为空的人员，应返回3条记录")]
        public void QueryByIsNull2()
        {
            List<dynamic> result = Derd.Model("Person")
                .IsNull("IDCard")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [Description("查询身份证不为空的人员，应返回李大1条")]
        public void QueryByIsNotNull()
        {
            List<dynamic> result = Derd.Model("Person")
                .IsNotNull("IDCard")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("李大", result[0].Name);
        }

        [TestMethod]
        [Description("查询手机号不为空的人员，应返回张二、李二2条记录")]
        public void QueryByIsNotNull2()
        {
            List<dynamic> result = Derd.Model("Person")
                .IsNotNull("Mobile")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄在50-60之间的人，应返回张大、李大2条记录")]
        public void QueryByBetween()
        {
            List<dynamic> result = Derd.Model("Person")
                .Between("Age", 50, 60)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("张大", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄在18-20之间的人，应返回张二、李二2条记录")]
        public void QueryByBetweeen2()
        {
            List<dynamic> result = Derd.Model("Person")
                .Between("Age", 18, 20)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期在1970-01-01至1980-12-31之间的人，应返回张二、李二2条记录")]
        public void QueryByBetween3()
        {
            List<dynamic> result = Derd.Model("Person")
                .Between(Date.DATE("Birthday"), "1970-01-01", "1980-12-31")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期在1940-01-01至1950-12-31之间的人，应返回张大、李大2条记录")]
        public void QueryByBetween4()
        {
            List<dynamic> result = Derd.Model("Person")
                .Between(Date.DATE("Birthday"), Date.DATE("1940-01-01"), Date.DATE("1950-12-31"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄为18、19、58的人，应返回3条记录")]
        public void QueryByIn()
        {
            List<dynamic> result = Derd.Model("Person")
                .In("Age", 18, 19, 58)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [Description("查找姓名为张大、李二的人，应正确返回2条记录")]
        public void QueryByIn2()
        {
            List<dynamic> result = Derd.Model("Person")
                .In("Name", "张大", "李二")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期为1949-10-01、1949-02-15的人员，应返回张大、李大2条记录")]
        public void QueryByIn3()
        {
            List<dynamic> result = Derd.Model("Person")
                .In(Date.DATE("Birthday"), "1949-10-01", "1949-02-15")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("李大", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期为1979-03-15、1980-07-25的人员，应返回张二、李二2条记录")]
        public void QueryByIn4()
        {
            List<dynamic> result = Derd.Model("Person")
                .In(Date.DATE("Birthday"), Date.DATE("1979-03-15"), Date.DATE("1980-07-25"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张二", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }

        [TestMethod]
        [Description("查询年龄不包含58、60、19的人员，应返回张二1条记录")]
        public void QueryByNotIn()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotIn("Age", 58, 60, 19)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张二", result[0].Name);
        }

        [TestMethod]
        [Description("查询姓名为李大、李二的人，应正确返回2条记录")]
        public void QueryByNotIn2()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotIn("Name", "李大", "李二")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("张大", result[0].Name);
            Assert.AreEqual("张二", result[1].Name);
        }

        [TestMethod]
        [Description("查询出生日期不在1949-10-01、1949-02-15、1979-03-15内的人员，应返回张二1条记录")]
        public void QueryByNotIn3()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotIn(Date.DATE("Birthday"), "1949-10-01", "1949-02-15", "1979-03-15")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("张二", result[0].Name);
        }

        [TestMethod]
        [Description("查询出生日期不在1949-10-01、1980-07-25内的人员，应返回李大、李二2条记录")]
        public void QueryByNotIn4()
        {
            List<dynamic> result = Derd.Model("Person")
                .NotIn(Date.DATE("Birthday"), Date.DATE("1949-10-01"), Date.DATE("1980-07-25"))
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
        }
    }
}
