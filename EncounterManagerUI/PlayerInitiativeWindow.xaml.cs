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
    /// Interaction logic for PlayerInitiativeWindow.xaml
    /// </summary>
    public partial class PlayerInitiativeWindow : Window
    {
        public int Initiative { get; set; }

        public PlayerInitiativeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Run method displaying which Player to add Initiative for
        /// </summary>
        /// <param name="player"></param>
        public PlayerInitiativeWindow(Participant player) : this()
        {
            DisplayPlayerName(player);
        }

        /// <summary>
        /// Display Player name in label
        /// </summary>
        /// <param name="player"></param>
        private void DisplayPlayerName(Participant player)
        {
            lblPlayerName.Content = player.Name;
        }

        /// <summary>
        /// When the user clicks OK
        /// Check if the user has entered an Initiative
        /// Add that Initiative to Initiative property
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(CheckInteger(txtPlayerInitiative.Text))
            {
                Initiative = int.Parse(txtPlayerInitiative.Text);
                this.Close();
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
    }
}
