/* Title:       Add Phone Extension
 * Date:        3-11-21
 * Author:      Terry Holmes
 * 
 * Description: This is used to add a description */

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
using PhonesDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddPhoneExt.xaml
    /// </summary>
    public partial class AddPhoneExt : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindPhoneByExtensionDataSet TheFindPhoneByExtensionDataSet = new FindPhoneByExtensionDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public AddPhoneExt()
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtDirectNumber.Text = "";
                txtEnterLastName.Text = "";
                txtEnterPhoneExt.Text = "";
                txtMACAddress.Text = "";

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;
                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Phone Ext");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Phone Ext // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;
                if (strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Phone Ext // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            string strDirectNumber;
            string strMACAddress;
            int intPhoneExt = 0;

            try
            {
                strValueForValidation = txtEnterPhoneExt.Text;
                if(strValueForValidation.Length != 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Extension is not the Correct Length Needs to be Four Characters\n";
                }
                else
                {
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Extension is not Numeric\n";
                    }
                    else
                    {
                        intPhoneExt = Convert.ToInt32(strValueForValidation);
                    }
                }
                strDirectNumber = txtDirectNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strDirectNumber);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Direct Number is not a Phone Number\n";
                }
                strMACAddress = txtMACAddress.Text;
                if(strMACAddress.Length < 16)
                {
                    blnFatalError = true;
                    strErrorMessage += "The MAC Address is not Long Enough\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee was not Selected\n";
                }
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Was not Selected\n";
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Phone Ext // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
