// Albin Karlsson 2019-01-12

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EncounterManager;

namespace EnocunterManagerTests
{
    [TestClass]
    public class DiceTests
    {
        [TestMethod]
        public void TestRoll()
        {
            Dice dice = new Dice();

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(dice.Roll(10) > 0 && dice.Roll(10) < 11);
            }
        }
    }
}
