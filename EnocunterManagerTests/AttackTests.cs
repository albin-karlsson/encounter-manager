// Albin Karlsson 2019-01-12

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EncounterManager;

namespace EnocunterManagerTests
{
    [TestClass]
    public class AttackTests
    {
        [TestMethod]
        public void TestRollForAttackWithNoToHit()
        {
            Attack attack = new Attack();

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(attack.RollForAttack(0) > 0 && attack.RollForAttack(0) < 21);
            }
        }

        [TestMethod]
        public void TestRollForAttackWithPositiveToHit()
        {
            Attack attack = new Attack();

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(attack.RollForAttack(10) > 10 && attack.RollForAttack(10) < 31);
            }
        }

        [TestMethod]
        public void TestRollForAttackWithNegativeToHit()
        {
            Attack attack = new Attack();

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(attack.RollForAttack(-10) > -10 && attack.RollForAttack(-10) < 11);
            }
        }
    }
}
