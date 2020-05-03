// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public class Dice
    {
        Random random = new Random();
        
        /// <summary>
        /// Generate a random number depending on number of Dice sides
        /// </summary>
        /// <param name="noOfSides"></param>
        /// <returns></returns>
        public int Roll(int noOfSides)
        {
            return random.Next(1, noOfSides + 1);
        }
    }
}
