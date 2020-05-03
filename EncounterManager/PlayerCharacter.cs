// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public class PlayerCharacter : Participant
    {
        public PlayerCharacter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Overridden ToString-method returning the Name property
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
