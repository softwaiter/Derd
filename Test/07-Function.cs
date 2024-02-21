using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    [TestClass]
    public class _07_Function
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
            newOrg.Code = "Microsoft";
            newOrg.Name = "微软科技有限公司";
            newOrg.RegisterDate = new DateTime(1949, 10, 1, 10, 10, 10);
            Derd.Model("Org").SetValues(newOrg).Save();

            dynamic newOrg2 = new DynamicObjectExt();
            newOrg2.Code = "Ibm";
            newOrg2.Name = "IBM科技有限公司";
            newOrg2.RegisterDate = new DateTime(1949, 2, 15, 1, 1, 1);
            Derd.Model("Org").SetValues(newOrg2).Save();

            dynamic newPerson = new DynamicObjectExt();
            newPerson.Name = "张大";
            newPerson.Age = 60;
            newPerson.Deposit = 10000;
            newPerson.Birthday = new DateTime(1949, 10, 1, 10, 10, 10);
            newPerson.Org = "Microsoft";
            Derd.Model("Person").SetValues(newPerson).Save();

            dynamic newPerson2 = new DynamicObjectExt();
            newPerson2.Name = "张二";
            newPerson2.Age = 18;
            newPerson2.Deposit = 500;
            newPerson2.Birthday = new DateTime(1980, 7, 25, 8, 8, 8);
            newPerson2.Org = "Microsoft";
            newPerson2.Mobile = "13600000000";
            Derd.Model("Person").SetValues(newPerson2).Save();

            dynamic newPerson3 = new DynamicObjectExt();
            newPerson3.Name = "李大";
            newPerson3.Age = 58;
            newPerson3.Deposit = 9000;
            newPerson3.Birthday = new DateTime(1949, 2, 15, 10, 10, 10);
            newPerson3.Org = "Ibm";
            newPerson3.IDCard = "130602199907250951";
            Derd.Model("Person").SetValues(newPerson3).Save();

            dynamic newPerson4 = new DynamicObjectExt();
            newPerson4.Name = "李二";
            newPerson4.Age = 19;
            newPerson4.Deposit = 1000;
            newPerson4.Birthday = new DateTime(1979, 3, 15, 6, 6, 6);
            newPerson4.Org = "Ibm";
            newPerson4.Mobile = "13800000000";
            Derd.Model("Person").SetValues(newPerson4).Save();

            dynamic newPerson5 = new DynamicObjectExt();
            newPerson5.Name = "李三";
            newPerson5.Age = 16;
            newPerson5.Deposit = 500;
            newPerson5.Birthday = new DateTime(1984, 9, 28, 8, 8, 8);
            newPerson5.Org = "Ibm";
            newPerson5.Mobile = "13900000000";
            Derd.Model("Person").SetValues(newPerson5).Save();
        }

        [TestMethod]
        [Description("统计各个机构人员的平均年龄，应返回2条记录；且Ibm员工为38.5，Microsoft员工为39")]
        public void AVG()
        {
            List<dynamic> result = Derd.Model("Person")
                .GetValue("Org", "Org")
                .GetValue(Funcs.AVG("Age"), "Age")
                .GroupBy("Org")
                .AscendingSort("Org")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(31, (double)result[0].Age);
            Assert.AreEqual(39, (double)result[1].Age);
        }

        [TestMethod]
        [Description("统计各个机构人员数量，应返回2条记录；且Ibm员工数为3，Microsoft员工数为2")]
        public void COUNT()
        {
            List<dynamic> result = Derd.Model("Person")
                .GetValue("Org", "Org")
                .GetValue(Funcs.COUNT("Org"), "Count")
                .GroupBy("Org")
                .AscendingSort("Org")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, (double)result[0].Count);
            Assert.AreEqual(2, (double)result[1].Count);
        }

        [TestMethod]
        [Description("查询张二的出生日期，应为1980-07-25 08:08:08")]
        public void DATETIME()
        {
            dynamic person = Derd.Model("Person")
                .Equals("Name", "张二")
                .GetValue(Funcs.DATETIME("Birthday"), "Birthday")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual("1980-07-25 08:08:08", person.Birthday);
        }

        [TestMethod]
        [Description("查询张二的出生日期（不包括时间），应为1980-07-25")]
        public void DATE()
        {
            dynamic person = Derd.Model("Person")
                .Equals("Name", "张二")
                .GetValue(Funcs.DATE("Birthday"), "Birthday")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual("1980-07-25", person.Birthday);
        }

        [TestMethod]
        [Description("查询张二的出生时间（不包括日期），应为08:08:08")]
        public void TIME()
        {
            dynamic person = Derd.Model("Person")
                .Equals("Name", "张二")
                .GetValue(Funcs.TIME("Birthday"), "Birthday")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual("08:08:08", person.Birthday);
        }

        [TestMethod]
        [Description("通过人员表统计机构数量，应返回统计数量为2")]
        public void DISTINCT()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Funcs.COUNT(Funcs.DISTINCT("Org")), "Count")
                .QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [Description("通过人员表统计机构数量，应返回统计数量为2")]
        public void DISTINCT2()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Funcs.COUNT(Funcs.DISTINCT("Org.Code")), "Count")
                .QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [Description("获取人员表中的最大年龄，应返回年龄值为60")]
        public void MAX()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Funcs.MAX("Age"), "Age")
                .QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreEqual((int)60, (int)result.Age);
        }

        [TestMethod]
        [Description("获取人员表中的最小年龄，应返回年龄值为16")]
        public void Min()
        {
            dynamic result = Derd.Model("Person")
                .GetValue(Funcs.MIN("Age"), "Age")
                .QueryFirst();
            Assert.IsNotNull(result);
            Assert.AreEqual((int)16, (int)result.Age);
        }

        [TestMethod]
        [Description("统计各个机构人员的年龄总和，应返回2条记录；且Microsoft的为78，Ibm的为93")]
        public void SUM()
        {
            List<dynamic> result = Derd.Model("Person")
                .GetValue("Org", "Org")
                .GetValue(Funcs.SUM("Age"), "Age")
                .GroupBy("Org")
                .AscendingSort("Org")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(93, result[0].Age);
            Assert.AreEqual(78, result[1].Age);
        }

        [TestMethod]
        [Description("查询姓名第二个字是“三”的人，应返回一条记录，且姓名为李三")]
        public void SUBSTR()
        {
            List<dynamic> result = Derd.Model("Person")
                .Equals(Funcs.SUBSTR("Name", 2, 1), "三")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("李三", result[0].Name);
        }

        [TestMethod]
        [Description("查询机构名长度为3的所有人员，应返回李大、李二、李三3条记录")]
        public void LENGTH()
        {
            List<dynamic> result = Derd.Model("Person")
                .Equals(Funcs.LENGTH("Org"), 3)
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("李大", result[0].Name);
            Assert.AreEqual("李二", result[1].Name);
            Assert.AreEqual("李三", result[2].Name);
        }

        [TestMethod]
        [Description("查询所有机构编码的大写形式，应为IBM、MICROSOFT")]
        public void UPPER()
        {
            List<dynamic> result = Derd.Model("Org")
                .GetValue(Funcs.UPPER("Code"), "Code")
                .AscendingSort("Code")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("IBM", result[0].Code);
            Assert.AreEqual("MICROSOFT", result[1].Code);
        }

        [TestMethod]
        [Description("查询所有机构编码的小写形式，应为ibm、microsoft")]
        public void LOWER()
        {
            List<dynamic> result = Derd.Model("Org")
                .GetValue(Funcs.LOWER("Code"), "Code")
                .AscendingSort("Code")
                .Query();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("ibm", result[0].Code);
            Assert.AreEqual("microsoft", result[1].Code);
        }

        [TestMethod]
        [Description("使用LTRIM函数计算“ Hello World ”，应返回“Hello World ”")]
        public void LTRIM()
        {
            dynamic person = Derd.Model("Person")
                .GetValue(Funcs.LTRIM(" Hello World "), "NewField")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual("Hello World ", person.NewField);
        }

        [TestMethod]
        [Description("使用RTRIM函数计算“ Hello World ”，应返回“ Hello World”")]
        public void RTRIM()
        {
            dynamic person = Derd.Model("Person")
                .GetValue(Funcs.RTRIM(" Hello World "), "NewField")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual(" Hello World", person.NewField);
        }

        [TestMethod]
        [Description("使用TRIM函数计算“ Hello World ”，应返回“Hello World”")]
        public void TRIM()
        {
            dynamic person = Derd.Model("Person")
                .GetValue(Funcs.TRIM(" Hello World "), "NewField")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual("Hello World", person.NewField);
        }

        [TestMethod]
        [Description("使用ABS函数计算-1.23，应返回1.23")]
        public void ABS()
        {
            dynamic person = Derd.Model("Person")
                .GetValue(Funcs.ABS(-1.23), "NewField")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual((double)1.23, (double)person.NewField);
        }

        [TestMethod]
        [Description("使用ROUND函数计算12.345，并指定精度为2，应返回12.35")]
        public void ROUND()
        {
            dynamic person = Derd.Model("Person")
                .GetValue(Funcs.ROUND(12.345, 2), "NewField")
                .QueryFirst();
            Assert.IsNotNull(person);
            Assert.AreEqual((double)12.35, (double)person.NewField);
        }
    }
}
