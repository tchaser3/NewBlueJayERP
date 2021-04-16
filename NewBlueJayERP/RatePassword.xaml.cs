/* Title:           RatePassword
 * Date:            4-14-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to enter a password */

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
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for RatePassword.xaml
    /// </summary>
    public partial class RatePassword : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        int gintTimesTried = 0;

        public RatePassword()
        {
            InitializeComponent();
        }

        private void pbxEnterPassword_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strPassword;

            strPassword = pbxEnterPassword.Password;

            
            if(strPassword == "BJC#1exterminate!")
            {
                MainWindow.gblnPasswordWorked = true;

                this.Close();
            }
            else
            {
                gintTimesTried++;

                if(gintTimesTried == 3)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "The User Has Failed to Enter the Rate Password");

                    TheMessagesClass.ErrorMessage("You Have Failed to enter right Password, the Window Will Close");

                    this.Close();
                }
                else
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There is a Failed Attempt to Enter Add Labor Rate");

                    TheMessagesClass.ErrorMessage("You Have not Enter The Correct Password");

                    pbxEnterPassword.Clear();
                }
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnPasswordWorked = false;
        }
    }
}
