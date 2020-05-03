// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public abstract class Participant
    {
        public int Initiative { get; set; }
        public string Name { get; set; }
        public virtual int TotalHealth { get; set; }
        public virtual int CurrentHealth { get; set; }
        public virtual List<DamageType> Resistances { get; set; }
        public virtual List<DamageType> Immunities { get; set; }
        public virtual List<Attack> Attacks { get; set; } = new List<Attack>();
        public virtual int PassivePerception { get; set; }
        public virtual int Speed { get; set; }
        public virtual int ArmorClass { get; set; }
        public virtual MonsterType Type { get; set; }
        public virtual List<MonsterSpecial> MonsterSpecials { get; set; } = new List<MonsterSpecial>();
        public virtual bool Multiattack { get; set; }
        public virtual int ID { get; set; }
        public virtual List<Attack> RemainingAttacks { get; set; }

        public abstract override string ToString();

        /// <summary>
        /// Return Iniative, Name, and ID of Participant
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        public string GetInitiativeString(Participant participant)
        {
            if(participant is Monster)
            {
                return String.Format("{0}   {1} #{2}",
                Initiative, Name, ID);
            }
            else
            {
                return String.Format("{0}   {1}",
                Initiative, Name);
            }


        }
    }
}
