// Albin Karlsson 2019-01-12

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EncounterManager;
using System.Collections.Generic;

namespace EnocunterManagerTests
{
    [TestClass]
    public class MonsterTests
    {
        [TestMethod]
        public void TestCalculateDamageNoDamage()
        {
            List<DamageType> resistances = new List<DamageType>();
            List<DamageType> immunities = new List<DamageType>();
            List<Attack> attacks = new List<Attack>();
            List<MonsterSpecial> monsterSpecials = new List<MonsterSpecial>();
            MonsterType monsterType = MonsterType.Abberation;

            Monster monster = new Monster("", 0, 0, 0, 0, monsterType, resistances, immunities, 0, attacks, monsterSpecials, false, 1);

            int expectedValue = 0;
            int actualValue = monster.CalculateDamage(0, DamageType.Acid);

            Assert.AreEqual(expectedValue, actualValue);
            Console.WriteLine(actualValue);
        }

        [TestMethod]
        public void TestCalculateDamagePositiveDamage()
        {
            List<DamageType> resistances = new List<DamageType>();
            List<DamageType> immunities = new List<DamageType>();
            List<Attack> attacks = new List<Attack>();
            List<MonsterSpecial> monsterSpecials = new List<MonsterSpecial>();
            MonsterType monsterType = MonsterType.Abberation;

            Monster monster = new Monster("", 0, 0, 0, 0, monsterType, resistances, immunities, 0, attacks, monsterSpecials, false, 1);

            int expectedValue = 10;
            int actualValue = monster.CalculateDamage(10, DamageType.Acid);

            Assert.AreEqual(expectedValue, actualValue);
            Console.WriteLine(actualValue);
        }

        [TestMethod]
        public void TestCalculateDamageWithResistanceEvenValue()
        {
            List<DamageType> resistances = new List<DamageType>() { DamageType.Acid };
            List<DamageType> immunities = new List<DamageType>();
            List<Attack> attacks = new List<Attack>();
            List<MonsterSpecial> monsterSpecials = new List<MonsterSpecial>();
            MonsterType monsterType = MonsterType.Abberation;

            Monster monster = new Monster("", 0, 0, 0, 0, monsterType, resistances, immunities, 0, attacks, monsterSpecials, false, 1);

            int expectedValue = 5;
            int actualValue = monster.CalculateDamage(10, DamageType.Acid);

            Assert.AreEqual(expectedValue, actualValue);
            Console.WriteLine(actualValue);
        }

        [TestMethod]
        public void TestCalculateDamageWithResistanceUnevenValue()
        {
            List<DamageType> resistances = new List<DamageType>() { DamageType.Acid };
            List<DamageType> immunities = new List<DamageType>();
            List<Attack> attacks = new List<Attack>();
            List<MonsterSpecial> monsterSpecials = new List<MonsterSpecial>();
            MonsterType monsterType = MonsterType.Abberation;

            Monster monster = new Monster("", 0, 0, 0, 0, monsterType, resistances, immunities, 0, attacks, monsterSpecials, false, 1);

            int expectedValue = 5;
            int actualValue = monster.CalculateDamage(11, DamageType.Acid);

            Assert.AreEqual(expectedValue, actualValue);
            Console.WriteLine(actualValue);
        }

        [TestMethod]
        public void TestCalculateDamageWithImmunity()
        {
            List<DamageType> resistances = new List<DamageType>();
            List<DamageType> immunities = new List<DamageType>() { DamageType.Acid };
            List<Attack> attacks = new List<Attack>();
            List<MonsterSpecial> monsterSpecials = new List<MonsterSpecial>();
            MonsterType monsterType = MonsterType.Abberation;

            Monster monster = new Monster("", 0, 0, 0, 0, monsterType, resistances, immunities, 0, attacks, monsterSpecials, false, 1);

            int expectedValue = 0;
            int actualValue = monster.CalculateDamage(11, DamageType.Acid);

            Assert.AreEqual(expectedValue, actualValue);
            Console.WriteLine(actualValue);
        }

        [TestMethod]
        public void TestSubtractDamage()
        {
            Monster monster = new Monster();

            monster.CurrentHealth = 10;

            Assert.IsTrue(monster.SubtractDamage(5));
        }

        [TestMethod]
        public void TestSubtractDamageKillMonster()
        {
            Monster monster = new Monster();

            monster.CurrentHealth = 10;

            Assert.IsFalse(monster.SubtractDamage(10));
        }
    }
}
