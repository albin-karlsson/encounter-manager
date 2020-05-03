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
    /// Interaction logic for AddSpecialWindow.xaml
    /// </summary>
    public partial class AddMonsterSpecialWindow : Window
    {
        public delegate void MonsterSpecialCreatedEventHandler(object sender, EventArgs e);
        public event MonsterSpecialCreatedEventHandler MonsterSpecialCreated;

        public string Name { get; set; }
        public string Text { get; set; }
        public MonsterSpecial NewMonsterSpecial { get; set; }

        public AddMonsterSpecialWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Check if all inputs are corent
        /// Assign inputs to properties
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputs())
            {
                AssignInputs();
                CreateMonsterSpecial();
                this.Close();
            }
            else
            {
                MessageBox.Show("Check your inputs!", "Error");
            }
        }

        /// <summary>
        /// Check if Special has been added
        /// </summary>
        /// <returns></returns>
        private bool CheckInputs()
        {
            if(string.IsNullOrEmpty(txtSpecial.Text) || string.IsNullOrEmpty(txtName.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Assign inputs to properties
        /// </summary>
        private void AssignInputs()
        {
            Name = txtName.Text;
            Text = txtSpecial.Text;
        }

        /// <summary>
        /// Create new MonsterSpecial object from assigned properties
        /// </summary>
        private void CreateMonsterSpecial()
        {
            MonsterSpecial monsterSpecialObj;

            monsterSpecialObj = new MonsterSpecial(Name, Text);

            NewMonsterSpecial = monsterSpecialObj;

            OnMonsterSpecialCreated(EventArgs.Empty);
        }

        /// <summary>
        /// Raise MonsterSpecial created event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMonsterSpecialCreated(EventArgs e)
        {
            MonsterSpecialCreated?.Invoke(this, e); // Raise event! 
        }
    }
}
