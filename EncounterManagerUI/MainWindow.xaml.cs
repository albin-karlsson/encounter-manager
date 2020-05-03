// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EncounterManager;

namespace EncounterManagerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParticipantManager participantManager = new ParticipantManager();
        InitiativeManager initiativeManager = new InitiativeManager();

        public bool Advantage { get; set; }
        public bool Disadvantage { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            AddEnums();
        }

        // EVENTS

        /// <summary>
        /// When the user clicks to add a new participant...
        /// Open the Add Participant window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            AddParticipantWindow addParticipant = new AddParticipantWindow();
            addParticipant.ParticipantCreated += OnParticipantCreated;
            addParticipant.Show();
        }

        /// <summary>
        /// When this event is called...
        /// Add the new Participant to the Participants-list in ParticipantManager
        /// Enable the button to roll for initiative
        /// Update the Participants UI list
        /// Add Remaining Attacks to all Participants (checking first if they already have them)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnParticipantCreated(object source, EventArgs e)
        {
            AddNewParticipantsToLists((AddParticipantWindow)source);
            EnableRollingForInitiative();
            UpdateParticipantsUI();
            InitializeRemainingAttacksForAllMonsters();
        }

        /// <summary>
        /// Add Participant to the Participants list in ParticipantManager
        /// </summary>
        /// <param name="source"></param>
        private void AddNewParticipantsToLists(AddParticipantWindow source)
        {
            foreach(Participant participant in source.NewParticipants)
            {
                participantManager.Participants.Add(participant);
            }
        }

        /// <summary>
        /// When the selected is changed in the UI Participants list
        /// Clear the UI Attack list
        /// Populate the UI Attack list with Attacks from Remaining Attacks
        /// Display other Monster stats in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstParticipants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstParticipants.SelectedIndex > -1)
            {
                ClearAttackList();
                PopulateAttackListFromRemainingAttacks();
                DisplayActive();
            }
        }

        /// <summary>
        /// When the user clicks to roll for Initiative
        /// Create new Initiative list
        /// Select the first index from the Initiative list in the UI Participants list
        /// Reset all Remaining Attacks for each Monster
        /// Clear UI Attack list for selected Monster
        /// Populate the UI Attack list from Remaining Attacks list
        /// Display other Monster stats in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRollForInitiative_Click(object sender, RoutedEventArgs e)
        {
            CreateInitiativeList();
            InitializeInitiativeUI();
            SelectCurrentParticipant();
            InitializeRemainingAttacksForAllMonsters();
            ClearAttackList();
            PopulateAttackListFromRemainingAttacks();
            DisplayActive();
        }

        /// <summary>
        /// When the user clicks new turn
        /// Jump one step in the Initaitive list
        /// Jump one step in the UI Initiative list
        /// Select the first index in the Initiative list
        /// Reset Remaining Attacks for all Monsters
        /// Clear the Attack list for the selected Monster
        /// Populate the Attack list from Remaining Attacks
        /// Display other monster stats in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            initiativeManager.NextInitiative();
            UpdateCurrentInitiativeUI();
            SelectCurrentParticipant();
            InitializeRemainingAttacksForAllMonsters();
            ClearAttackList();
            PopulateAttackListFromRemainingAttacks();
            DisplayActive();
        }

        /// <summary>
        /// Attack logic
        /// When the user clicks on Attack!
        /// If the monster does not have Multiattack, remove all Remaining Attacks
        /// Roll a hit dice
        /// Check if hit, miss, or natural 20
        /// Remove Remaining Attacks
        /// Clear the Attack list
        /// Populate the Attack list with Attacks from Remaining Attacks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAttack_Click(object sender, RoutedEventArgs e)
        {
            if(lstAttacks.SelectedIndex > -1)
            {
                Participant activeParticipant = (Participant)lstParticipants.SelectedItem;
                Attack attack = (Attack)lstAttacks.SelectedItem;

                int toHit = RollToHit(attack);

                activeParticipant.RemainingAttacks.Remove(attack);

                if (!activeParticipant.Multiattack)
                {
                    activeParticipant.RemainingAttacks.Clear();
                }

                if (toHit - attack.ToHit == 20) // NATURAL 20
                {
                    int damage = attack.RollDamageDice(true);

                    AddAttacksIfTrigger(activeParticipant, attack);

                    MessageBox.Show($"{activeParticipant} rolls a natural 20! The damage dealt to the target is {damage} {attack.Type}.", attack.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (MessageBox.Show($"{activeParticipant}'s attack roll with {attack.Name} is {toHit}. Does it hit?", "To hit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int damage = attack.RollDamageDice(false);

                    AddAttacksIfTrigger(activeParticipant, attack);

                    MessageBox.Show($"{activeParticipant} deals {damage} {attack.Type} damage to the target!", attack.Name);

                    if (attack.CheckForSpecial())
                    {
                        MessageBox.Show($"{attack.Special}", attack.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

                if (activeParticipant.RemainingAttacks.Count <= 0)
                {
                    btnAttack.IsEnabled = false;
                    DisableAdvantageAndDisadvantage();
                }
            }
            else
            {
                MessageBox.Show("Select an attack first.", "Message");
            }

            ClearAttackList();
            PopulateAttackListFromRemainingAttacks();
        }

        /// <summary>
        /// Add all unique Trigger Attacks to a list
        /// If the Attack is a trigger
        /// Check which Attacks are triggered by the attack
        /// Add these attacks to Remaining Attacks
        /// </summary>
        /// <param name="activeParticipant"></param>
        /// <param name="attack"></param>
        private void AddAttacksIfTrigger(Participant activeParticipant, Attack attack)
        {
            List<Attack> triggerAttacksList = activeParticipant.Attacks.Where(p => p.Trigger).ToList();
            List<Attack> uniqueTriggerAttacksList = new List<Attack>();

            foreach (Attack a in triggerAttacksList)
            {
                if (uniqueTriggerAttacksList.Any(x => x.Name == a.Name))
                {
                    // do nothing
                }
                else
                {
                    uniqueTriggerAttacksList.Add(a);
                }
            }

            if (attack.Trigger)
            {
                foreach (Attack x in uniqueTriggerAttacksList)
                {
                    foreach(Attack y in activeParticipant.Attacks)
                    {
                        if (y.TriggeredBy != null && y.TriggeredBy.Name == x.Name)
                        {
                            activeParticipant.RemainingAttacks.Add(y);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Run TakeDamage method with DamageType Slashing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSlashing_Click(object sender, RoutedEventArgs e)
        {
            if(chkMagic.IsChecked == false)
            {
                TakeDamage(DamageType.Slashing);
            }
            else
            {
                TakeDamage(DamageType.MagicSlashing);
            }
        }

        /// <summary>
        /// Run TakeDamage method with DamageType Piercing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPiercing_Click(object sender, RoutedEventArgs e)
        {
            if (chkMagic.IsChecked == false)
            {
                TakeDamage(DamageType.Piercing);
            }
            else
            {
                TakeDamage(DamageType.MagicPiercing);
            }
        }

        /// <summary>
        /// Run TakeDamage method with DamageType Bludgeoning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBludgeoning_Click(object sender, RoutedEventArgs e)
        {
            if (chkMagic.IsChecked == false)
            {
                TakeDamage(DamageType.Bludgeoning);
            }
            else
            {
                TakeDamage(DamageType.MagicBludgeoning);
            }
        }

        /// <summary>
        /// Run TakeDamage method with selected DamageType
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDamageOK_Click(object sender, RoutedEventArgs e)
        {
            if(lstDamageType.SelectedIndex > -1)
            {
                TakeDamage((DamageType)lstDamageType.SelectedItem);
            }
        }

        /// <summary>
        /// Display the Monster Info window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMonsterInfo_Click(object sender, RoutedEventArgs e)
        {
            MonsterInfoWindow monsterInfoWindow = new MonsterInfoWindow();

            monsterInfoWindow.Show();

            Participant activeParticipant = (Participant)lstParticipants.SelectedItem;

            monsterInfoWindow.DisplayInfo(activeParticipant);
        }

        /// <summary>
        /// Close application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Run a method clearing and resetting everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            ClearAndReset();
        }

        private void chkAdvantage_Changed(object sender, RoutedEventArgs e)
        {
            if((bool)chkAdvantage.IsChecked)
            {
                chkDisadvantage.IsChecked = false;
                Advantage = true;
                Disadvantage = false;
            }
            else
            {
                chkAdvantage.IsChecked = false;
                Advantage = false;
            }
        }

        private void chkDisadvantage_Changed(object sender, RoutedEventArgs e)
        {
            if ((bool)chkDisadvantage.IsChecked)
            {
                chkAdvantage.IsChecked = false;
                Disadvantage = true;
                Advantage = false;
            }
            else
            {
                chkDisadvantage.IsChecked = false;
                Disadvantage = false;
            }
        }

        private void btnRechargeAttacks_Click(object sender, RoutedEventArgs e)
        {
            InititalizeRemainingAttacksForSelectedMonster();
            ClearAttackList();
            PopulateAttackListFromRemainingAttacks();
            EnableAttackListAndButton();
        }

        // DISPLAY

        /// <summary>
        /// Clear UI Initiative list
        /// For each Participant
        /// Add Participant to UI Initiative list
        /// Run method updating who has the current Initiative in the UI
        /// </summary>
        private void InitializeInitiativeUI()
        {
            lstInitiative.Items.Clear();

            foreach (Participant participant in initiativeManager.InitiativeList)
            {
                lstInitiative.Items.Add(participant.GetInitiativeString(participant));
            }

            UpdateCurrentInitiativeUI();
        }

        /// <summary>
        /// Update who has the current Initiative in the UI
        /// Enable the Next Turn button if it was disabled
        /// </summary>
        private void UpdateCurrentInitiativeUI()
        {
            lblCurrentInitiative.Content = initiativeManager.InitiativeList[0];

            if (btnNextTurn.IsEnabled == false)
            {
                btnNextTurn.IsEnabled = true;
            }
        }

        /// <summary>
        /// Run methods enabling the groups Monster and Taking Damage and displaying the Monster UI
        /// </summary>
        private void DisplayActive()
        {
            Participant activeParticipant = (Participant)lstParticipants.SelectedItem;

            if (activeParticipant is Monster)
            {
                EnableMonsterAndTakingDamage();

                if(activeParticipant.RemainingAttacks.Count > 0)
                {
                    EnableAttackListAndButton();
                }

                DisplayMonsterUI(activeParticipant);
            }
            else
            {
                ClearMonsterUI();
            }
        }

        /// <summary>
        /// Display all monster stats except Attacks
        /// </summary>
        /// <param name="activeParticipant"></param>
        private void DisplayMonsterUI(Participant activeParticipant)
        {
            lblName.Content = $"{activeParticipant.Name} {activeParticipant.ID}";
            lblType.Content = activeParticipant.Type;
            lblTotalHP.Content = activeParticipant.TotalHealth;
            lblArmorClass.Content = activeParticipant.ArmorClass;
            lblSpeed.Content = activeParticipant.Speed;
            lblPassivePerception.Content = activeParticipant.PassivePerception;
            lblCurrentHP.Content = activeParticipant.CurrentHealth;
        }

        /// <summary>
        /// Clear the UI Participants list
        /// For each Participant add them to the UI Participants list
        /// </summary>
        private void UpdateParticipantsUI()
        {
            lstParticipants.Items.Clear();

            foreach (Participant participant in participantManager.Participants)
            {
                lstParticipants.Items.Add(participant);
            }
        }

        // CLEAR

        /// <summary>
        /// Clear the Monster UI
        /// </summary>
        private void ClearMonsterUI()
        {
            lblName.Content = "";
            lblType.Content = "";
            lblTotalHP.Content = "";
            lblCurrentHP.Content = "";
            lblArmorClass.Content = "";
            lblSpeed.Content = "";
            lblPassivePerception.Content = "";
            lstAttacks.Items.Clear();
        }

        /// <summary>
        /// Clear the Current Initiative label
        /// </summary>
        private void ClearCurrentInitiative()
        {
            lblCurrentInitiative.Content = string.Empty;
        }

        /// <summary>
        /// Crate new empty ParticipantManager and InitiativeManager objets
        /// </summary>
        private void ResetInitiativeAndParticipants()
        {
            ParticipantManager participantManager = new ParticipantManager();
            InitiativeManager initiativeManager = new InitiativeManager();
        }

        /// <summary>
        /// Run methods disabling and clearing everything
        /// The Monster UI
        /// UI and ParticipantManager and InitiativeManager lists
        /// Disable the Rolling for Initiative button
        /// Disable the groups Monster and Taking Damage
        /// Reset the ParticipantManager and InitiativeManager objects
        /// </summary>
        private void ClearAndReset()
        {
            ClearMonsterUI();
            ClearCurrentInitiative();
            ClearLists();
            DisableRollingForInitiative();
            DisableMonsterAndTakingDamage();
            ResetInitiativeAndParticipants();
        }

        /// <summary>
        /// Clear UI lists and Participants list and Initiative list
        /// </summary>
        private void ClearLists()
        {
            lstInitiative.Items.Clear();
            lstParticipants.Items.Clear();
            participantManager.Participants.Clear();
            initiativeManager.InitiativeList.Clear();
        }

        /// <summary>
        /// Clear the UI Attack list
        /// </summary>
        private void ClearAttackList()
        {
            lstAttacks.Items.Clear();
        }

        // ENABLE AND DISABLE

        /// <summary>
        /// Enable the group Initiative and the Roll for Initiative button
        /// </summary>
        private void EnableRollingForInitiative()
        {
            grpInitiative.IsEnabled = true;
            btnRollForInitiative.IsEnabled = true;
        }

        /// <summary>
        /// Disable the group Initiative and the Roll for Initiative button
        /// </summary>
        private void DisableRollingForInitiative()
        {
            grpInitiative.IsEnabled = false;
            btnRollForInitiative.IsEnabled = false;
        }

        /// <summary>
        /// Enable the group Taking Damage and the group Monster
        /// </summary>
        private void EnableMonsterAndTakingDamage()
        {
                grpTakingDamage.IsEnabled = true;
                grpMonster.IsEnabled = true;
        }

        /// <summary>
        /// Enable the UI Attack list and the button Attack
        /// </summary>
        private void EnableAttackListAndButton()
        {
            lstAttacks.IsEnabled = true;
            btnAttack.IsEnabled = true;
        }

        /// <summary>
        /// Disable the groups Monster and Taking Damage
        /// </summary>
        private void DisableMonsterAndTakingDamage()
        {
            grpTakingDamage.IsEnabled = false;
            grpMonster.IsEnabled = false;
        }

        private void DisableAdvantageAndDisadvantage()
        {
            chkAdvantage.IsChecked = false;
            chkDisadvantage.IsChecked = false;
        }

        // OTHER METHODS

        /// <summary>
        /// Populate Damage Type list with Damage Types
        /// </summary>
        private void AddEnums()
        {
            List<DamageType> commonDamageTypes = new List<DamageType>();
            commonDamageTypes.Add(DamageType.Bludgeoning);
            commonDamageTypes.Add(DamageType.Slashing);
            commonDamageTypes.Add(DamageType.Piercing);
            commonDamageTypes.Add(DamageType.MagicBludgeoning);
            commonDamageTypes.Add(DamageType.MagicSlashing);
            commonDamageTypes.Add(DamageType.MagicPiercing);

            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                if (commonDamageTypes.Contains(damageType))
                {
                    continue;
                }
                else
                {
                    lstDamageType.Items.Add(damageType);
                }
            }
        }

        /// <summary>
        /// Calculate Damage
        /// If Monster has enough HP left
        /// Display new HP
        /// Otherwise kill the Monster by running a method removing it from lists
        /// </summary>
        /// <param name="damageType"></param>
        private void TakeDamage(DamageType damageType)
        {
            int damage = 0;

            DamageWindow damageWindow = new DamageWindow();
            damageWindow.ShowDialog();

            Monster monsterTakingDamage = (Monster)lstParticipants.SelectedItem;

            damage = monsterTakingDamage.CalculateDamage(damageWindow.Damage, damageType);

            if(monsterTakingDamage.SubtractDamage(damage))
            {
                DisplayMonsterUI(monsterTakingDamage);
            }
            else
            {
                KillMonster(monsterTakingDamage);
            }
        }

        /// <summary>
        /// Notify the user the Monster was killed
        /// Remove the Monster from Participants list and Initiative list
        /// Check if there are any Monsters left
        /// If there are,  Update the Initative and the Participants UI lists
        /// If not, clear and reset everything
        /// </summary>
        /// <param name="monsterTakingDamage"></param>
        private void KillMonster(Monster monsterTakingDamage)
        {
            MessageBox.Show($"{monsterTakingDamage} was killed! How do you want to do this?", "Monster killed", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            participantManager.Participants.Remove(monsterTakingDamage);
            initiativeManager.InitiativeList.Remove(monsterTakingDamage);

            if (!CheckIfMonstersLeft())
            {
                MessageBox.Show("All monsters are dead!", "Message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ClearAndReset();
            }
            else
            {
                InitializeInitiativeUI();
                UpdateParticipantsUI();
                //DisplayActive();
            }
        }

        /// <summary>
        /// Returns true if there are Monsters left and False if not
        /// </summary>
        /// <returns></returns>
        private bool CheckIfMonstersLeft()
        {
            if (participantManager.Participants.Any(x => x is Monster))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add the active Monster's Remaining Attacks to the UI Attack list
        /// </summary>
        private void PopulateAttackListFromRemainingAttacks()
        {
            Participant activeParticipant = (Participant)lstParticipants.SelectedItem;

            if(activeParticipant is Monster)
            {
                foreach (Attack attack in activeParticipant.RemainingAttacks)
                {
                    lstAttacks.Items.Add(attack);
                }
            }
        }

        /// <summary>
        /// Add Remaining Attacks to all Monsters
        /// </summary>
        private void InitializeRemainingAttacksForAllMonsters()
        {
            foreach (Participant participant in participantManager.Participants)
            {
                if(participant is Monster)
                {
                    participant.RemainingAttacks.Clear();

                    foreach(Attack attack in participant.Attacks)
                    {
                        if(participant.RemainingAttacks.Count < participant.Attacks.Count && attack.TriggeredBy == null)
                        {
                            participant.RemainingAttacks.Add(attack);
                        }
                    }
                }
            }
        }

        private void InititalizeRemainingAttacksForSelectedMonster()
        {
            Participant selectedParticipant = (Participant)lstParticipants.SelectedItem;

            if(selectedParticipant is Monster)
            {
                selectedParticipant.RemainingAttacks.Clear();

                foreach (Attack attack in selectedParticipant.Attacks)
                {
                    if (selectedParticipant.RemainingAttacks.Count < selectedParticipant.Attacks.Count && attack.TriggeredBy == null)
                    {
                        selectedParticipant.RemainingAttacks.Add(attack);
                    }
                }
            }
        }

        /// <summary>
        /// Select the first index of the Initiative list in the Participants UI
        /// </summary>
        private void SelectCurrentParticipant()
        {
            lstParticipants.SelectedItem = initiativeManager.InitiativeList[0];
        }

        /// <summary>
        /// For each Participant
        /// If the Participant is a monster roll for Initiative
        /// Else, show a dialog for the user to put in Initative
        /// </summary>
        private void CreateInitiativeList()
        {
            foreach (Participant participant in participantManager.Participants)
            {
                if (participant is Monster)
                {
                    initiativeManager.RollForInitiative((Monster)participant);
                }
                else
                {
                    PlayerInitiativeWindow playerInitiativeWindow = new PlayerInitiativeWindow(participant);
                    playerInitiativeWindow.ShowDialog();
                    participant.Initiative = playerInitiativeWindow.Initiative;
                }
            }

            initiativeManager.SortInitiative(participantManager.Participants);
        }

        private int RollToHit(Attack attack)
        {
            int toHit = 0;

            if (Advantage)
            {
                List<int> attackRolls = new List<int>();

                for (int i = 0; i < 2; i++)
                {
                    attackRolls.Add(attack.RollForAttack(attack.ToHit));
                }

                var sortedAttackRolls = attackRolls.OrderByDescending(x => x).ToList();

                toHit = sortedAttackRolls[0];
            }
            else if (Disadvantage)
            {
                List<int> attackRolls = new List<int>();

                for (int i = 0; i < 2; i++)
                {
                    attackRolls.Add(attack.RollForAttack(attack.ToHit));
                }

                var sortedAttackRolls = attackRolls.OrderBy(x => x).ToList();

                toHit = sortedAttackRolls[0];
            }
            else
            {
                toHit = attack.RollForAttack(attack.ToHit);
            }

            return toHit;
        }
    }
}
