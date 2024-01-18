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
    /// Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
        /// <summary>
        /// User submitted username
        /// </summary>
        public string Username = "Guest";
        /// <summary>
        /// Create window and select text in username box
        /// </summary>
        /// <param name="num">Current user number</param>
        public LoginWin(int num)
        {
            InitializeComponent();
            Loaded += (sender, e) => { UsernameBox.Focus(); UsernameBox.SelectAll(); };
            UserIdentifier.Content += (num + 1).ToString();
        }
        /// <summary>
        /// The enter key is hit in the username box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsernameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
            }
        }
        /// <summary>
        /// Sends username information to program after window closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            // If text is left empty use guest account
            if (UsernameBox.Text == "")
            {
                Username = "Guest";
            }
            else
            {
                Username = UsernameBox.Text;
            }
        }
    }
}
