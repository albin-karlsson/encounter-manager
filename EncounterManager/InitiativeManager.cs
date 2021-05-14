// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterManager
{
    public class InitiativeManager
    {
        Dice dice = new Dice();
        public bool InitiativeRolled { get; set; }

        public List<Participant> InitiativeList { get; set; } = new List<Participant>();

        /// <summary>
        /// Remove the first element from the initiative list and add it to the end
        /// </summary>
        public void NextInitiative()
        {
            InitiativeList.Add(InitiativeList[0]);
            InitiativeList.RemoveAt(0);
        }

        /// <summary>
        /// Roll initiative for monsters and return their calculated total initiative
        /// </summary>
        /// <param name="participantToRollFor"></param>
        public void RollForInitiative(Monster participantToRollFor)
        {
            int rolledInitiative = dice.Roll(20);

            participantToRollFor.Initiative = rolledInitiative + participantToRollFor.DexterityModifier;
        }

        /// <summary>
        /// Sort initative list by descending initative
        /// </summary>
        /// <param name="listToSort"></param>
        /// <returns></returns>
        public List<Participant> SortInitiative(List<Participant> listToSort)
        {
            List<Participant> sortedList = listToSort.OrderByDescending(x => x.Initiative).ToList();

            InitiativeList = sortedList;

            return sortedList;
        }
    }
}
