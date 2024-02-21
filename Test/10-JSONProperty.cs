using CodeM.Common.Orm;
using CodeM.Common.Tools.DynamicObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test
{
    [TestClass]
    public class _10_JSONProperty
    {
        [TestInitialize]
        public void Init()
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\models");
            Derd.ModelPath = modelPath;
            Derd.Load();

            Derd.TryCreateTables(true);
        }

        [TestMethod]
        public void WriteJsonValue()
        {
            dynamic newPlayer = new DynamicObjectExt();
            newPlayer.Type = 0;
            newPlayer.Skill = new DynamicObjectExt();
            newPlayer.Skill.Walk = 1;
            newPlayer.Skill.Run = 0.8;
            newPlayer.Skill.Attack = 50;
            newPlayer.Skill.MagicAttack = 80;
            newPlayer.Skill.Blood = 100;
            bool ret = Derd.Model("Player").SetValues(newPlayer).Save();
            Assert.IsTrue(ret);
        }

        [TestMethod]
        public void ReadJsonValue()
        {
            dynamic newPlayer = new DynamicObjectExt();
            newPlayer.Type = 0;
            newPlayer.Skill = new DynamicObjectExt();
            newPlayer.Skill.Walk = 1;
            newPlayer.Skill.Run = 0.8;
            newPlayer.Skill.Attack = 50;
            newPlayer.Skill.MagicAttack = 80;
            newPlayer.Skill.Blood = 100;
            Derd.Model("Player").SetValues(newPlayer).Save();

            dynamic obj = Derd.Model("Player").Equals("Type", 0).QueryFirst();
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, obj.Type);
            Assert.AreEqual(1, obj.Skill.Walk);
            Assert.AreEqual(100, obj.Skill.Blood);
        }

        [TestCleanup]
        public void Uninit()
        {
            Derd.RemoveTables();
        }
    }
}
