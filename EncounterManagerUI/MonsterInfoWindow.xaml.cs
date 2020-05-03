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
    /// Interaction logic for MonsterInfoWindow.xaml
    /// </summary>
    public partial class MonsterInfoWindow : Window
    {
        public MonsterInfoWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// If the selected Monster has any MonsterSpecials
        /// For each MonsterSpecial
        /// Add it to the UI Info list
        /// Also if any Attack has a Special
        /// Add Attack to list
        /// </summary>
        /// <param name="activeParticipant"></param>
        public void DisplayInfo(Participant activeParticipant)
        {
            if(activeParticipant.MonsterSpecials.Any())
            {
                foreach (MonsterSpecial monsterSpecial in activeParticipant.MonsterSpecials)
                {
                    if(!lstInfo.Items.Contains(monsterSpecial))
                    {
                        lstInfo.Items.Add(monsterSpecial);
                    }    
                }
            }
        
            foreach (Attack attack in activeParticipant.Attacks)
            {
                if (!string.IsNullOrEmpty(attack.Special) && !lstInfo.Items.Contains(attack))
                {
                    lstInfo.Items.Add(attack);
                }
            }
        }

        /// <summary>
        /// When the User clicks on a different MonsterSpecial or Attack in the UI Info list
        /// If its a MonsterSpecial
        /// Add MonsterSpecial info message to the info label
        /// If its an Attack
        /// Add the Attack's Special to the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstInfo.SelectedItem is MonsterSpecial)
            {
                MonsterSpecial monsterSpecial = (MonsterSpecial)lstInfo.SelectedItem;
                lblInfo.Text = monsterSpecial.GetInfoMessage();
            }
            else
            {
                Attack attack = (Attack)lstInfo.SelectedItem;
                lblInfo.Text = attack.Special;
            }
        }
    }
}
