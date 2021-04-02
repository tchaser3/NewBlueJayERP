/* Title:           Assign Phone Extension
 * Date:            3-30-21
 * Author:          Terry Holmes
 * 
 * Description:     This is for assigning a Phone Extension */

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
using DataValidationDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using PhonesDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignPhoneExtension.xaml
    /// </summary>
    public partial class AssignPhoneExtension : Window
    {
        //setting classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindPhoneByExtensionDataSet TheFindPhoneByExtensionDataSet = new FindPhoneByExtensionDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public AssignPhoneExtension()
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
            txtCurrentAssignment.Text = "";
            txtEnterExtension.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Assign Phone Extension");
        }

        private void txtEnterExtension_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            int intLength;
            int intRecordsReturned;
            int intExtension;

            try
            {
                strValueForValidation = txtEnterExtension.Text;
                intLength = strValueForValidation.Length;
                if (intLength == 4)
                {
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Value Entered is not an Integer");
                        return;
                    }

                    intExtension = Convert.ToInt32(strValueForValidation);

                    TheFindPhoneByExtensionDataSet = ThePhoneClass.FindPhoneByExtension(intExtension);

                    intRecordsReturned = TheFindPhoneByExtensionDataSet.FindPhoneByExtension.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Extension Entered Does Not Exist");
                        return;
                    }

                    MainWindow.gintTransactionID = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].TransactionID;

                    txtCurrentAssignment.Text = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].FullName;
                }
                else if(intLength > 4)
                {
                    TheMessagesClass.ErrorMessage("Invalid Extension");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Phone Extension // Extension Text box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                if (intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if (intNumberOfRecords == -1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Phone Extension // Last Name Text Box Change Event " + Ex.Message);

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

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                if (txtEnterExtension.Text.Length != 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Extension is not the Correct Format\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = ThePhoneClass.UpdateEmployeePhone(MainWindow.gintTransactionID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Extension has been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Phone Extension // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
