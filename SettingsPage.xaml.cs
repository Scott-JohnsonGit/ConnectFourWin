using Connect4;
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

namespace ConnectFourWin
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Window
    {
        /// <summary>
        /// Rules selected by player(s)
        /// </summary>
        public SpecialRules SelectedRules { get { return _selectedRules; } }
        /// <summary>
        /// Rules selected by player(s)
        /// </summary>
        private SpecialRules _selectedRules = SpecialRules.None;
        /// <summary>
        /// Initalize page
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Unchecks all other radio buttons when one is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameModeChecked(object sender, RoutedEventArgs e)
        {
            RadioButton checkedButton = sender as RadioButton;
            _selectedRules = (SpecialRules)Grid.GetColumn(checkedButton);
            foreach (var objects in GameModeGrid.Children)
            {
                // Check to make sure its a radio button
                RadioButton radio;
                try
                {
                    radio = (RadioButton)objects;
                }
                // Skip any non radio buttons
                catch
                {
                    continue;
                }
                // Uncheck any non clicked buttons
                if (radio != checkedButton)
                {
                    radio.IsChecked = false;
                }
            }
        }
        /// <summary>
        /// Player finished selecting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
