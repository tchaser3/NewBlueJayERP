/* Title:           Assign Fuel Cards
 * Date:            5-18-20
 * Author:          Terry Holmes
 * 
 * Description:     This will create a random 4 digit number */

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
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateFuelCardNumber.xaml
    /// </summary>
    public partial class CreateFuelCardNumber : Window
    {
        //setting up the classes;
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        FuelCardClass TheFuelCardClass = new FuelCardClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //Setting up the data
        FindFuelCardEmployeeDataSet TheFindFuelCardEmployeeDataSet = new FindFuelCardEmployeeDataSet();
        FindEmployeeActiveFuelCardNumberDataSet TheFindEmployeeActiveFuelCardNumberDataSet = new FindEmployeeActiveFuelCardNumberDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        int gintFuelCardNumber;

        public CreateFuelCardNumber()
        {
            InitializeComponent();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Was Not Found");
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Fuel Card Number // Last Name Text Box " + Ex.Message);
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    TheFindEmployeeActiveFuelCardNumberDataSet = TheFuelCardClass.FindEmployeeActiveFuelCardNumber(MainWindow.gintEmployeeID);

                    intRecordsReturned = TheFindEmployeeActiveFuelCardNumberDataSet.FindEmployeeActiveFuelCardNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Already has a Fuel Card Number, Please Edit the Number");
                        return;
                    }

                    gintFuelCardNumber = RandomNumber();

                    txtFuelCardNumber.Text = Convert.ToString(gintFuelCardNumber);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Fuel Card Number // Select Employee Combo Box " + Ex.Message);
            }
        }
        private int RandomNumber()
        {
            int intFuelCardNumber = -1;
            int intRecordsReturned = 1;

            while (intRecordsReturned > 0)
            {
                Random FuelCardNumber = new Random();
                intFuelCardNumber = FuelCardNumber.Next(1000, 9999);

                TheFindFuelCardEmployeeDataSet = TheFuelCardClass.FindFuelCardEmployee(intFuelCardNumber);

                intRecordsReturned = TheFindFuelCardEmployeeDataSet.FindFuelCardEmployee.Rows.Count;
            }           

            return intFuelCardNumber;
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtFuelCardNumber.Text = "";
            txtLastName.Text = "";
            cboSelectEmployee.Items.Clear();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheFuelCardClass.InsertFuelCard(MainWindow.gintEmployeeID, gintFuelCardNumber);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Number Has Been Inserted");

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Create Fuel Card Number ");

                if (blnFatalError == true)
                    throw new Exception();

                ResetControls();
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Fuel Card Number // Process Button " + Ex.Message);
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
