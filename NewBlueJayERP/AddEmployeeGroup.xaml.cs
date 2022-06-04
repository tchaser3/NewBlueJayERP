/* Title:           Add Employee Group
 * Date:            12-15-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for adding an employee group */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddEmployeeGroup.xaml
    /// </summary>
    public partial class AddEmployeeGroup : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindEmployeeGroupByGroupNameDataSet TheFindEmployeeGroupByGroupNameDataSet = new FindEmployeeGroupByGroupNameDataSet();

        public AddEmployeeGroup()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessagesClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEmployeeGroupName.Text = "";
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables;
            bool blnFatalError = false;
            string strEmployeeGroup;
            int intRecordsReturned;

            try
            {
                strEmployeeGroup = txtEmployeeGroupName.Text;

                //data validation
                if(strEmployeeGroup.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Employee Group is not Long Enough");
                    return;
                }

                //checking to see if the group existed already
                TheFindEmployeeGroupByGroupNameDataSet = TheEmployeeClass.FindEmployeeGroupByName(strEmployeeGroup);

                intRecordsReturned = TheFindEmployeeGroupByGroupNameDataSet.FindEmployeeGroupByGroupName.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Group Already Exists");
                    return;
                }

                blnFatalError = TheEmployeeClass.CreateEmployeeGroup(strEmployeeGroup);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Added Employee Group " + strEmployeeGroup);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Employee Group has been Created");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Employee Groups // Process Button " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
