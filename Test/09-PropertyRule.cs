using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test
{
    [TestClass]
    public class _09_PropertyRule
    {
        [TestInitialize]
        public void Init()
        {
            Derd.RegisterProcessor("EncryptDeposit", "Test.Processors.EncryptDeposit");
            Derd.RegisterProcessor("DecryptDeposit", "Test.Processors.DecryptDeposit");
            Derd.RegisterProcessor("OrgExists", "Test.Processors.OrgExists");

            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            Derd.ModelPath = modelPath;
            Derd.Load();

            Derd.TryCreateTables(true);
        }

        /// <summary>
        /// Null值或空值的属性不进行Rule检查
        /// </summary>
        [TestMethod]
        public void NullProperty()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1980, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = null;
            bool ret = Derd.Model("Person").SetValues(newuser, true).Save();
            Assert.IsTrue(ret);
        }

        /// <summary>
        /// 设置Email属性值为"Hello World"，使用配置的Email模式检查，应报异常。
        /// </summary>
        [TestMethod]
        public void EmailPatternPropertyFailed()
        {
            try
            {
                dynamic newuser = new DynamicObjectExt();
                newuser.Name = "wangxm";
                newuser.Age = 18;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = null;
                newuser.Email = "Hello World";
                Derd.Model("Person").SetValues(newuser, true).Save();
                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 设置Email属性值为"hello@126.com"，使用配置的Email模式检查，应成功。
        /// </summary>
        [TestMethod]
        public void EmailPatternPropertySuccessful()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1980, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = null;
            newuser.Email = "hello@126.com";
            bool bRet = Derd.Model("Person").SetValues(newuser, true).Save();
            Assert.IsTrue(bRet);
        }

        /// <summary>
        /// 设置IDCard属性值为“130123456789054321”,
        /// </summary>
        [TestMethod]
        public void IDCardPatternFailed()
        {
            try
            {
                dynamic newuser = new DynamicObjectExt();
                newuser.Name = "wangxm";
                newuser.Age = 18;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = null;
                newuser.IDCard = "130123456789054321";
                bool bRet = Derd.Model("Person").SetValues(newuser, true).Save();
                Assert.Fail();
            }
            catch (Exception exp)
            {
                Assert.AreEqual("无效的身份证号码", exp.Message);
            }
        }

        /// <summary>
        /// 设置Mobile属性值为“1361234567”，使用配置的Regex正则表达式检查，应报异常。
        /// </summary>
        [TestMethod]
        public void MobileRegexFailed()
        {
            try
            {
                dynamic newuser = new DynamicObjectExt();
                newuser.Name = "wangxm";
                newuser.Age = 18;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = null;
                newuser.Mobile = "1361234567";
                Derd.Model("Person").SetValues(newuser, true).Save();
                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 设置Mobile属性值为“13612345678”，使用配置的Regex正则表达式检查，应成功。
        /// </summary>
        [TestMethod]
        public void MobileRegexSuccessful()
        {
            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1980, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = null;
            newuser.Mobile = "13612345678";
            bool bRet = Derd.Model("Person").SetValues(newuser, true).Save();
            Assert.IsTrue(bRet);
        }

        /// <summary>
        /// 设置Org属性值为不存在的Org编码“NotFound”，使用配置的Validation处理器检查，应报异常。
        /// </summary>
        [TestMethod]
        public void OrgValidationFailed()
        {
            try
            {
                dynamic newuser = new DynamicObjectExt();
                newuser.Name = "wangxm";
                newuser.Age = 18;
                newuser.Birthday = new DateTime(1980, 6, 14);
                newuser.Deposit = 10000000.58;
                newuser.IsAdmin = null;
                newuser.Org = "NotFound";
                Derd.Model("Person").SetValues(newuser, true).Save();
                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 设置Org属性值为已有的Org编码“XXTech”，使用配置的Validation处理器检查，应成功。
        /// </summary>
        [TestMethod]
        public void OrgValidationSuccessful()
        {
            dynamic neworg = new DynamicObjectExt();
            neworg.Code = "XXTech";
            neworg.Name = "XX科技";
            bool ret = Derd.Model("Org").SetValues(neworg).Save();
            Assert.IsTrue(ret);

            dynamic newuser = new DynamicObjectExt();
            newuser.Name = "wangxm";
            newuser.Age = 18;
            newuser.Birthday = new DateTime(1980, 6, 14);
            newuser.Deposit = 10000000.58;
            newuser.IsAdmin = null;
            newuser.Org = "XXTech";
            ret = Derd.Model("Person").SetValues(newuser, true).Save();
            Assert.IsTrue(ret);
        }

        [TestCleanup]
        public void Uninit()
        {
            Derd.RemoveTables();
        }
    }
}
