/* Title:           Manually Add Fuel PIN
 * Date:            5-27-20
 * Author:          Terry Holmes
 * 
 * Description:     This used to add a card number manually */

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
using FuelCardDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ManuallyAddFuelPIN.xaml
    /// </summary>
    public partial class ManuallyAddFuelPIN : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        FuelCardClass TheFuelCardClass = new FuelCardClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindFuelCardEmployeeDataSet TheFindFuelCardEmployeeDataSet = new FindFuelCardEmployeeDataSet();
        FindEmployeeActiveFuelCardNumberDataSet TheFindEmployeeActiveFuelCardNumberDataSet = new FindEmployeeActiveFuelCardNumberDataSet();

        public ManuallyAddFuelPIN()
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
            this.Visibility = Visibility.Hidden;
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
            txtEnterPIN.Text = "";
            txtLastName.Text = "";
            cboSelectEmployee.Items.Clear();
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Manually Add Fuel PIN // Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                TheFindEmployeeActiveFuelCardNumberDataSet = TheFuelCardClass.FindEmployeeActiveFuelCardNumber(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindEmployeeActiveFuelCardNumberDataSet.FindEmployeeActiveFuelCardNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Employee Has a Fuel Card PIN");
                    return;
                }
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up variables
            int intFuelPIN = 0;
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            int intRecordsReturned;

            try
            {
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strValueForValidation = txtEnterPIN.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Fuel PIN is not Numeric\n";
                }
                else
                {
                    intFuelPIN = Convert.ToInt32(strValueForValidation);

                    if((intFuelPIN < 1000) || (intFuelPIN > 9999))
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Fuel PIN is out Range\n";
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindEmployeeActiveFuelCardNumberDataSet = TheFuelCardClass.FindEmployeeActiveFuelCardNumber(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindEmployeeActiveFuelCardNumberDataSet.FindEmployeeActiveFuelCardNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Already Has a Fuel Card PIN");
                    return;
                }

                TheFindFuelCardEmployeeDataSet = TheFuelCardClass.FindFuelCardEmployee(intFuelPIN);

                intRecordsReturned = TheFindFuelCardEmployeeDataSet.FindFuelCardEmployee.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Fuel Card PIN Has Already Been Used");
                    return;
                }

                blnFatalError = TheFuelCardClass.InsertFuelCard(MainWindow.gintEmployeeID, intFuelPIN);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Fuel PIN has been entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Manually Add Fuel PIN // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
