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
using System.Windows.Shapes;
using EncounterManager;
using EncounterManagerDAL;

namespace EncounterManagerUI
{
    /// <summary>
    /// Interaction logic for AddParticipantWindow.xaml
    /// </summary>
    public partial class AddParticipantWindow : Window
    {
        bool monster;

        public delegate void ParticipantChangedEventHandler(object sender, EventArgs e);
        public event ParticipantChangedEventHandler ParticipantChanged;

        public delegate void ParticipantCreatedEventHandler(object sender, EventArgs e);
        public event ParticipantCreatedEventHandler ParticipantCreated;

        public string Name { get; set; }
        public int ArmorClass { get; set; }
        public int HealthPoints { get; set; }
        public int DexterityModifier { get; set; }
        public int PassivePerception { get; set; }
        public int Speed { get; set; }
        public int Amount { get; set; } = 1;
        public bool Multiattack { get; set; }
        public MonsterType Type { get; set; }
        public List<DamageType> Resistances { get; set; } = new List<DamageType>();
        public List<DamageType> Immunities { get; set; } = new List<DamageType>();
        public List<Participant> NewParticipants { get; set; } = new List<Participant>();
        public List<Attack> Attacks { get; set; } = new List<Attack>();
        public List<MonsterSpecial> MonsterSpecials { get; set; } = new List<MonsterSpecial>();


        public AddParticipantWindow()
        {
            InitializeComponent();

            AddEnums();
        }

        // EVENTS

        /// <summary>
        /// Add Participant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(CheckInputs())
            {
                AssignInputs();
                CreateParticipant();
                this.Close();
            }
            else
            {
                MessageBox.Show("Check your inputs!", "Error");
            }

        }

        /// <summary>
        /// Radio button checked for "Monster"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoMonster_Checked(object sender, RoutedEventArgs e)
        {
            monster = true;

            grpMonster.IsEnabled = true;
        }

        /// <summary>
        /// Radio button checked for "Player"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoPlayer_Checked(object sender, RoutedEventArgs e)
        {
            monster = false;

            grpMonster.IsEnabled = false;

            OnParticipantChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Raise Participant changed event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnParticipantChanged(EventArgs e) // Methods raising events should be protected virtual void
        {
            ParticipantChanged?.Invoke(this, e); // Raise event! 
        }

        /// <summary>
        /// Raise Participant created event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnParticipantCreated(EventArgs e)
        {
            ParticipantCreated?.Invoke(this, e); // Raise event! 
        }

        /// <summary>
        ///  Add selected Immunity to the UI Resistances list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbResistances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!lstResistances.Items.Contains(cmbResistances.SelectedItem))
            {
                lstResistances.Items.Add(cmbResistances.SelectedItem);
            }
        }

        /// <summary>
        /// Add selected Immunity to the UI Immunities list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbImmunities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!lstImmunities.Items.Contains(cmbImmunities.SelectedItem))
            {
                lstImmunities.Items.Add(cmbImmunities.SelectedItem);
            }
        }

        /// <summary>
        /// Remove Resistance from UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveResistance_Click(object sender, RoutedEventArgs e)
        {
            lstResistances.Items.RemoveAt(lstResistances.Items.IndexOf(lstResistances.SelectedItem));
        }

        /// <summary>
        /// Remove Immunity from UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveImmunity_Click(object sender, RoutedEventArgs e)
        {
            lstImmunities.Items.RemoveAt(lstImmunities.Items.IndexOf(lstImmunities.SelectedItem));
        }

        /// <summary>
        /// Add Attack to Attacks list
        /// Check if Attack is Trigger
        /// Update the UI Attacks list
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnAttackCreated(object source, EventArgs e)
        {
            AddAttackToList((AddAttackWindow)source);
            CheckForTriggerAttacks();
            UpdateAttacksUI();
        }

        /// <summary>
        /// Open Add Attack window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAttack_Click(object sender, RoutedEventArgs e)
        {
            AddAttackWindow addAttack = new AddAttackWindow();
            this.ParticipantChanged += addAttack.OnParticipantChanged;
            addAttack.AttackCreated += this.OnAttackCreated;
            addAttack.Show();

            addAttack.AddTriggeredBy(this);
        }

        /// <summary>
        /// Remove Attack from UI and Attacks list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveAttack_Click(object sender, RoutedEventArgs e)
        {
            if (lstAttacks.SelectedIndex > -1)
            {
                Attack attack = (Attack)lstAttacks.SelectedItem;
                List<Attack> attacksToRemove = new List<Attack>();

                if (attack.Trigger)
                {
                    foreach (Attack a in Attacks)
                    {
                        if (a.TriggeredBy != null && a.TriggeredBy.Name == attack.Name)
                        {
                            attacksToRemove.Add(a);
                        }
                    }
                }

                attacksToRemove.Add(attack);

                if(Attacks.Any(a => a.Name == attack.Name))
                {
                    attacksToRemove.Add(attack);
                }

                foreach (Attack a in attacksToRemove)
                {
                    Attacks.Remove(a);
                    UpdateAttacksUI();
                }
            }
        }

        /// <summary>
        /// Start XMLSerialzation to save a Monster object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveMonster_Click(object sender, RoutedEventArgs e)
        {
            XMLSerialization xmlSerialization = new XMLSerialization();

            if (CheckInputs())
            {
                AssignInputs();
                Monster monsterToSave = new Monster(Name, HealthPoints, ArmorClass, PassivePerception, DexterityModifier, Type, Resistances, Immunities, Speed, Attacks, MonsterSpecials, Multiattack, 0);

                xmlSerialization.SaveMonster(monsterToSave);
            }
            else
            {
                MessageBox.Show("Check your inputs!", "Error");
            }
        }

        /// <summary>
        /// Start XMLSerialization loading a Monster object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadMonster_Click(object sender, RoutedEventArgs e)
        {
            XMLSerialization xmlSerialization = new XMLSerialization();

            xmlSerialization.LoadMonster();

            Monster loadedMonster = xmlSerialization.Monster;

            ClearMonsterStats();

            if(loadedMonster != null)
            {
                DisplayMonsterStats(loadedMonster);
            }
        }

        /// <summary>
        /// If the user clicks to add Special
        /// Show AddMonsterSpecial window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSpecial_Click(object sender, RoutedEventArgs e)
        {
            AddMonsterSpecialWindow addMonsterSpecial = new AddMonsterSpecialWindow();
            addMonsterSpecial.MonsterSpecialCreated += this.OnMonsterSpecialCreated;

            addMonsterSpecial.Show();
        }

        /// <summary>
        /// If adding the MonsterSpecial to the MonsterSpecials list was successfull
        /// Update the MonsterSpecials UI
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnMonsterSpecialCreated(object source, EventArgs e)
        {
            if(AddMonsterSpecialToList((AddMonsterSpecialWindow)source))
            {
                UpdateMonsterSpecialsUI();
            }
        }

        /// <summary>
        /// Remove selected MonsterSpecial from MonsterSpecials
        /// Remove it also from the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveSpecial_Click(object sender, RoutedEventArgs e)
        {
            if(lstMonsterSpecials.SelectedIndex > -1)
            {
                MonsterSpecials.Remove((MonsterSpecial)lstMonsterSpecials.SelectedItem);
                lstMonsterSpecials.Items.Remove(lstMonsterSpecials.SelectedItem);
            }
        }

        /// <summary>
        /// Check of uncheck check box
        /// Set Multiattack accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkMultiattack_Changed(object sender, RoutedEventArgs e)
        {
            if((bool)chkMultiattack.IsChecked)
            {
                chkMultiattack.IsChecked = true;
                Multiattack = true;
            }
            else
            {
                chkMultiattack.IsChecked = false;
                Multiattack = false;
            }
        }

        // CLEAR

        /// <summary>
        /// Clear all text boxes and UI lists
        /// </summary>
        private void ClearMonsterStats()
        {
            txtAC.Text = string.Empty;
            txtDEX.Text = string.Empty;
            txtHP.Text = string.Empty;
            txtName.Text = string.Empty;
            txtPP.Text = string.Empty;
            txtSpeed.Text = string.Empty;
            cmbMonsterTypes.SelectedIndex = -1;
            lstResistances.Items.Clear();
            lstImmunities.Items.Clear();
            lstAttacks.Items.Clear();
            lstMonsterSpecials.Items.Clear();

            MonsterSpecials.Clear();
            Attacks.Clear();
        }

        // CHECK

        /// <summary>
        /// Create a list of bools and add all checked inputs to it
        /// </summary>
        /// <returns></returns>
        private bool CheckInputs()
        {
            List<bool> inputs = new List<bool>();

            inputs.Add(CheckString(txtName.Text));

            if (monster)
            {
                inputs.Add(CheckInteger(txtAC.Text));
                inputs.Add(CheckInteger(txtDEX.Text));
                inputs.Add(CheckInteger(txtHP.Text));
                inputs.Add(CheckInteger(txtSpeed.Text));
                inputs.Add(CheckInteger(txtPP.Text));
                inputs.Add(CheckComboBox(cmbMonsterTypes));

                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    inputs.Add(CheckInteger(txtAmount.Text));
                }
            }

            if (inputs.Contains(false))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// Check if string is null or empty
        /// </summary>
        /// <param name="stringToCheck"></param>
        /// <returns></returns>
        private bool CheckString(string stringToCheck)
        {
            if(String.IsNullOrEmpty(stringToCheck))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Check if possible to convert string to int
        /// </summary>
        /// <param name="intToCheck"></param>
        /// <returns></returns>
        private bool CheckInteger(string intToCheck)
        {
            return int.TryParse(intToCheck, out int tempInt);
        }

        /// <summary>
        /// Check if an item is selected from the combo box
        /// </summary>
        /// <param name="comboBox"></param>
        /// <returns></returns>
        private bool CheckComboBox(ComboBox comboBox)
        {
            return comboBox.SelectedIndex > -1;
        }

        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        private int ConvertToInt(string stringToConvert)
        {
            return int.Parse(stringToConvert);
        }

        /// <summary>
        /// Check if valid list selection
        /// </summary>
        /// <param name="listBox"></param>
        /// <returns></returns>
        private bool CheckSelection(ListBox listBox)
        {
            return listBox.SelectedIndex > -1;
        }

        /// <summary>
        /// Check if Attack is triggered by any attack
        /// If yes
        /// Add this attack as a Trigger
        /// </summary>
        private void CheckForTriggerAttacks()
        {
            foreach(Attack attack in Attacks)
            {
                if (attack.TriggeredBy != null)
                {
                    Attack triggerAttack = attack.TriggeredBy;

                    triggerAttack.Trigger = true;
                }
            }
        }

        // UPDATE

        /// <summary>
        /// Clear the UI Attacks list
        /// For each Attack the Monster has
        /// Add it to the Attacks list
        /// </summary>
        private void UpdateAttacksUI()
        {
            lstAttacks.Items.Clear();

            foreach (Attack attack in Attacks)
            {
                lstAttacks.Items.Add(attack);
            }
        }

        /// <summary>
        /// Clear the MonsterSpecials list
        /// For each MonsterSpecial the Monster has
        /// Add it to the MonsterSpecials list
        /// </summary>
        private void UpdateMonsterSpecialsUI()
        {
            lstMonsterSpecials.Items.Clear();

            foreach (MonsterSpecial monsterSpecial in MonsterSpecials)
            {
                lstMonsterSpecials.Items.Add(monsterSpecial);
            }
        }

        /// <summary>
        /// Display input in text boxes and lists
        /// </summary>
        /// <param name="monsterToDisplay"></param>
        private void DisplayMonsterStats(Monster monsterToDisplay)
        {
            txtAC.Text = monsterToDisplay.ArmorClass.ToString();
            txtDEX.Text = monsterToDisplay.DexterityModifier.ToString();
            txtHP.Text = monsterToDisplay.TotalHealth.ToString();
            txtName.Text = monsterToDisplay.Name;
            txtPP.Text = monsterToDisplay.PassivePerception.ToString();
            txtSpeed.Text = monsterToDisplay.PassivePerception.ToString();
            cmbMonsterTypes.SelectedValue = monsterToDisplay.Type;
            chkMultiattack.IsChecked = monsterToDisplay.Multiattack;

            foreach (DamageType resistance in monsterToDisplay.Resistances)
            {
                lstResistances.Items.Add(resistance);
                Resistances.Add(resistance);
            }

            foreach (DamageType immunity in monsterToDisplay.Immunities)
            {
                lstImmunities.Items.Add(immunity);
                Immunities.Add(immunity);
            }

            foreach (Attack attack in monsterToDisplay.Attacks)
            {
                lstAttacks.Items.Add(attack);
                Attacks.Add(attack);
            }

            foreach (MonsterSpecial monsterSpecial in monsterToDisplay.MonsterSpecials)
            {
                lstMonsterSpecials.Items.Add(monsterSpecial);
                MonsterSpecials.Add(monsterSpecial);
            }
        }

        // ASSIGN

        /// <summary>
        /// Assign the checked inputs to the properties
        /// </summary>
        private void AssignInputs()
        {
            Name = txtName.Text;

            if(monster)
            {
                ArmorClass = ConvertToInt(txtAC.Text);
                HealthPoints = ConvertToInt(txtHP.Text);
                DexterityModifier = ConvertToInt(txtDEX.Text);
                PassivePerception = ConvertToInt(txtPP.Text);
                Type = (MonsterType)cmbMonsterTypes.SelectedItem;
                Resistances = lstResistances.Items.Cast<DamageType>().ToList();
                Immunities = lstImmunities.Items.Cast<DamageType>().ToList();
                Speed = ConvertToInt(txtSpeed.Text);

                foreach(Attack attack in lstAttacks.Items)
                {
                    if(!Attacks.Contains(attack))
                    {
                        Attacks.Add(attack);
                    }
                }
                foreach(MonsterSpecial monsterSpecial in lstMonsterSpecials.Items)
                {
                    if(!MonsterSpecials.Contains(monsterSpecial))
                    {
                        MonsterSpecials.Add(monsterSpecial);
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    Amount = ConvertToInt(txtAmount.Text);
                }
            }
        }

        /// <summary>
        /// If the user has selected Monster
        /// Crate new Monster
        /// Add this Monster to the NewParticipants list
        /// If the user has selected Player
        /// Add this Player
        /// </summary>
        private void CreateParticipant()
        {
            Participant participantObj;

            if (monster)
            {
                for (int i = 1; i <= Amount; i++)
                {
                    participantObj = new Monster(Name, HealthPoints, ArmorClass, PassivePerception, DexterityModifier, Type, Resistances, Immunities, Speed, Attacks, MonsterSpecials, Multiattack, i);

                    NewParticipants.Add(participantObj);
                }
            }
            else
            {
                participantObj = new PlayerCharacter(Name);

                NewParticipants.Add(participantObj);
            }

            OnParticipantCreated(EventArgs.Empty);
        }

        /// <summary>
        /// For however many times
        /// Add a new Attack to the Attacks list
        /// </summary>
        /// <param name="addAttackWindow"></param>
        private void AddAttackToList(AddAttackWindow addAttackWindow)
        {
            for (int i = 0; i < addAttackWindow.MultiAttackAmount; i++)
            {
                Attacks.Add(addAttackWindow.NewAttack);
            }
        }

        /// <summary>
        /// Check if MonsterSpecial is unique
        /// Set NewMonsterSpecial
        /// </summary>
        /// <param name="addMonsterSpecialWindow"></param>
        /// <returns></returns>
        private bool AddMonsterSpecialToList(AddMonsterSpecialWindow addMonsterSpecialWindow)
        {
            if (!MonsterSpecials.Any(x => x.Name.ToUpper() == addMonsterSpecialWindow.Name.ToUpper()))
            {
                MonsterSpecials.Add(addMonsterSpecialWindow.NewMonsterSpecial);
                return true;
            }
            else
            {
                MessageBox.Show("A Special with that name already exists.", "Message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }

        // OTHER METHODS

        /// <summary>
        /// Add Enums to combo boxes
        /// </summary>
        private void AddEnums()
        {
            foreach (var monsterType in Enum.GetValues(typeof(MonsterType)))
            {
                cmbMonsterTypes.Items.Add(monsterType);
            }

            foreach (var damageType in Enum.GetValues(typeof(DamageType)))
            {
                cmbResistances.Items.Add(damageType);
                cmbImmunities.Items.Add(damageType);
            }
        }

    }
}
