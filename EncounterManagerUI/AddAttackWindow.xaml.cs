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

namespace EncounterManagerUI
{
    /// <summary>
    /// Interaction logic for AddAttackWindow.xaml
    /// </summary>
    public partial class AddAttackWindow : Window
    {
        public delegate void AttackCreatedEventHandler(object sender, EventArgs e);
        public event AttackCreatedEventHandler AttackCreated;

        public string Name { get; set; }
        public int NoOfDice { get; set; }
        public int NoOfSides { get; set; }
        public int DamageModifier { get; set; }
        public int ToHit { get; set; }
        public string Special { get; set; }
        public DamageType Type { get; set; }
        public Attack NewAttack { get; set; }
        public int MultiAttackAmount { get; set; } = 1;
        public bool Trigger { get; set; }
        public Attack TriggeredBy { get; set; }

        public AddAttackWindow()
        {
            InitializeComponent();
            AddEnums();
        }

        /// <summary>
        /// Add enums to combo boxes
        /// </summary>
        private void AddEnums()
        {
            foreach (var damageType in Enum.GetValues(typeof(DamageType)))
            {
                cmbType.Items.Add(damageType);
            }
        }

        /// <summary>
        /// Add possible attack to make into Triggers
        /// </summary>
        /// <param name="participant"></param>
        public void AddTriggeredBy(AddParticipantWindow participant)
        {
            foreach (Attack attack in participant.Attacks)
            {
                if(!cmbTriggeredBy.Items.Contains(attack))
                {
                    cmbTriggeredBy.Items.Add(attack);
                }
            }
        }

        // EVENTS

        /// <summary>
        /// When Add button is pressed
        /// Check if inputs are valid
        /// Assign inputs to properties
        /// Create new Attack from assigned properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(CheckInputs())
            {
                AssignInputs();
                CreateAttack();
                this.Close();
            }
            else
            {
                MessageBox.Show("Check your inputs!", "Error");
            }
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnParticipantChanged(object source, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Raise on Attack created event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAttackCreated(EventArgs e)
        {
            AttackCreated?.Invoke(this, e); // Raise event! 
        }

        //CHECK

        /// <summary>
        /// Create a list of bools and add all checked inputs to it
        /// </summary>
        /// <returns></returns>
        private bool CheckInputs()
        {
            List<bool> inputs = new List<bool>();

            inputs.Add(CheckString(txtName.Text));
            inputs.Add(CheckInteger(txtNoOfDice.Text));
            inputs.Add(CheckInteger(txtNoOfSides.Text));
            inputs.Add(CheckInteger(txtDamageModifier.Text));
            inputs.Add(CheckInteger(txtToHit.Text));
            inputs.Add(CheckComboBox(cmbType));

            if(!string.IsNullOrEmpty(txtMultiAttackAmount.Text))
            {
                inputs.Add(CheckInteger(txtMultiAttackAmount.Text));
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
        /// Check if an item is selected from the combo box
        /// </summary>
        /// <param name="comboBox"></param>
        /// <returns></returns>
        private bool CheckComboBox(ComboBox comboBox)
        {
            return comboBox.SelectedIndex > -1;
        }

        /// <summary>
        /// Check if string is null or empty
        /// </summary>
        /// <param name="stringToCheck"></param>
        /// <returns></returns>
        private bool CheckString(string stringToCheck)
        {
            if (String.IsNullOrEmpty(stringToCheck))
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


        // CONVERT

        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        private int ConvertToInt(string stringToConvert)
        {
            return int.Parse(stringToConvert);
        }

        // ASSIGN

        /// <summary>
        /// Assign the checked inputs to the properties
        /// </summary>
        private void AssignInputs()
        {
            Name = txtName.Text;
            NoOfDice = ConvertToInt(txtNoOfDice.Text);
            NoOfSides = ConvertToInt(txtNoOfSides.Text);
            DamageModifier = ConvertToInt(txtDamageModifier.Text);
            ToHit = ConvertToInt(txtToHit.Text);
            Type = (DamageType)cmbType.SelectedItem;

            if(cmbTriggeredBy.SelectedIndex > -1)
            {
                TriggeredBy = (Attack)cmbTriggeredBy.SelectedItem;
            }

            if(!string.IsNullOrEmpty(txtMultiAttackAmount.Text))
            {
                MultiAttackAmount = ConvertToInt(txtMultiAttackAmount.Text);
            }

            if (!string.IsNullOrEmpty(txtSpecial.Text))
            {
                Special = txtSpecial.Text;
            }
        }

        // CREATE

        /// <summary>
        /// Create new Attack object from assigned properties
        /// </summary>
        private void CreateAttack()
        {
            Attack attackObj;

            attackObj = new Attack(Name, NoOfDice, NoOfSides, DamageModifier, ToHit, Special, Type, Trigger, TriggeredBy);

            NewAttack = attackObj;

            OnAttackCreated(EventArgs.Empty);
        }
    }
}
