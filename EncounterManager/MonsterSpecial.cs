// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public class MonsterSpecial
    {
        public string Name { get; set; }
        public string Text { get; set; }

        public MonsterSpecial()
        {

        }

        /// <summary>
        /// Constructor assigning properties
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public MonsterSpecial(string name, string text)
        {
            Name = name;
            Text = text;
        }

        /// <summary>
        /// Overrridden ToString-method returning Name property
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}";
        }

        /// <summary>
        /// Return Text property
        /// </summary>
        /// <returns></returns>
        public string GetInfoMessage()
        {
            return Text;
        }
    }
}
