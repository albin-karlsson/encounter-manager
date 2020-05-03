// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public class Monster : Participant
    {
        public override int TotalHealth { get; set; }
        public override int CurrentHealth { get; set; }
        public override List<DamageType> Resistances { get; set; }
        public override List<DamageType> Immunities { get; set; }
        public override List<Attack> Attacks { get; set; }
        public override int PassivePerception { get; set; }
        public override int ArmorClass { get; set; }
        public override int Speed { get; set; }
        public int DexterityModifier { get; set; }
        public override MonsterType Type { get; set; }
        public override List<MonsterSpecial> MonsterSpecials { get; set; }
        public override bool Multiattack { get; set; }
        public override int ID { get; set; }
        public override List<Attack> RemainingAttacks { get; set; } = new List<Attack>();

        public Monster()
        {

        }

        /// <summary>
        /// Constructor setting properties
        /// </summary>
        /// <param name="name"></param>
        /// <param name="totalHealth"></param>
        /// <param name="armorClass"></param>
        /// <param name="passivePerception"></param>
        /// <param name="dexterityModifier"></param>
        /// <param name="type"></param>
        /// <param name="resistances"></param>
        /// <param name="immunities"></param>
        /// <param name="speed"></param>
        /// <param name="attacks"></param>
        /// <param name="monsterSpecials"></param>
        /// <param name="multiattack"></param>
        /// <param name="id"></param>
        public Monster(string name, int totalHealth, int armorClass, int passivePerception, int dexterityModifier, MonsterType type, List<DamageType> resistances, List<DamageType> immunities, int speed, List<Attack> attacks, List<MonsterSpecial> monsterSpecials, bool multiattack, int id)
        {
            Name = name;
            TotalHealth = totalHealth;
            ArmorClass = armorClass;
            PassivePerception = passivePerception;
            DexterityModifier = dexterityModifier;
            Type = type;
            Resistances = resistances;
            Immunities = immunities;
            Speed = speed;
            Attacks = attacks;
            MonsterSpecials = monsterSpecials;
            Multiattack = multiattack;
            ID = id;
            CurrentHealth = totalHealth;
        }

        /// <summary>
        /// Calculate Damage amount from damage and DamageType
        /// Taking into consideration Resistances and Immunities
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="damageType"></param>
        /// <returns></returns>
        public int CalculateDamage(int damage, DamageType damageType)
        {
            double totalDamage = 0d;

            if(Resistances.Contains(damageType))
            {
                totalDamage = Math.Floor((double)damage / 2d);
            }
            else if (Immunities.Contains(damageType))
            {

            }
            else
            {
                totalDamage = damage;
            }

            return Convert.ToInt32(totalDamage);
        }

        /// <summary>
        /// Subtract total Damage from Health
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public bool SubtractDamage(int damage)
        {
            if(CurrentHealth <= damage) // monster will be killed
            {
                CurrentHealth = 0;
                return false;
            }
            else
            {
                CurrentHealth -= damage;
                return true;
            }
        }

        /// <summary>
        /// Return Name and ID
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} #{ID}";
        }
    }
}
