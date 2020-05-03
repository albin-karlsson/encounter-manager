// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public class Attack
    {
        Dice dice = new Dice();

        public string Name { get; set; }
        public int Damage { get; set; }
        public DamageType Type { get; set; }
        public int DamageModifier { get; set; }
        public int NoOfDice { get; set; }
        public int NoOfSides { get; set; }
        public int ToHit { get; set; }
        public string Special { get; set; }
        public bool Trigger { get; set; } = false;
        public Attack TriggeredBy { get; set; }

        public Attack()
        {

        }

        /// <summary>
        /// Constructor setting properties
        /// </summary>
        /// <param name="name"></param>
        /// <param name="noOfDice"></param>
        /// <param name="noOfSides"></param>
        /// <param name="damageModifier"></param>
        /// <param name="toHit"></param>
        /// <param name="special"></param>
        /// <param name="type"></param>
        /// <param name="trigger"></param>
        /// <param name="triggeredBy"></param>
        public Attack(string name, int noOfDice, int noOfSides, int damageModifier, int toHit, string special, DamageType type, bool trigger, Attack triggeredBy)
        {
            Name = name;
            NoOfDice = noOfDice;
            NoOfSides = noOfSides;
            DamageModifier = damageModifier;
            ToHit = toHit;
            Special = special;
            Type = type;
            Trigger = trigger;
            TriggeredBy = triggeredBy;
        }

        /// <summary>
        /// Roll Attack Dice
        /// Adding the Monster's ToHit value
        /// </summary>
        /// <param name="toHit"></param>
        /// <returns></returns>
        public int RollForAttack(int toHit)
        {
            int rolledToHit = dice.Roll(20);
            int totalToHit = rolledToHit + toHit;

            return totalToHit;
        }

        /// <summary>
        /// Calculate total Damage
        /// Taking into consideration a natural 20
        /// </summary>
        /// <param name="natural20"></param>
        /// <returns></returns>
        public int RollDamageDice(bool natural20)
        {
            int totalRolledDamage = 0;
            int totalDamage = 0;

            for (int i = 0; i < NoOfDice; i++)
            {
                int rolledDamage = dice.Roll(NoOfSides);

                totalRolledDamage += rolledDamage;
            }

            if(natural20)
            {
                totalDamage = (totalRolledDamage * 2) + DamageModifier;
            }
            else
            {
                totalDamage = totalRolledDamage + DamageModifier;
            }

            return totalDamage;
        }

        /// <summary>
        /// Check if Attack has a Special
        /// </summary>
        /// <returns></returns>
        public bool CheckForSpecial()
        {
            if(string.IsNullOrEmpty(Special))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Return Attack Name and if Trigger or is triggered by something
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} ({NoOfDice}d{NoOfSides}+{DamageModifier})";
        }
    }
}
