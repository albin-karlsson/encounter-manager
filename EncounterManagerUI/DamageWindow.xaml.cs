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

namespace EncounterManagerUI
{
    /// <summary>
    /// Interaction logic for DamageWindow.xaml
    /// </summary>
    public partial class DamageWindow : Window
    {
        public int Damage { get; set; }

        public DamageWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Check if possible to convert string to int
        /// </summary>
        /// <param name="intToCheck"></param>
        /// <returns></returns>
        private bool CheckInteger(string intToCheck)
        {
            if(int.TryParse(intToCheck, out int tempInt) && tempInt > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the user has entered Damage
        /// Add Damage to Damage property
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(CheckInteger(txtDamage.Text))
            {
                Damage = int.Parse(txtDamage.Text);

                this.Close();
            }
        }
    }
}
