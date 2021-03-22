/* Title:           Edit Phone Ext
 * Date:            3-22-2021
 * Author:          Terry Holmes
 * 
 * Description:     This is used to for editing a phone extension */ 

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
using PhonesDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditPhoneExt.xaml
    /// </summary>
    public partial class EditPhoneExt : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //loading data
        FindPhoneByExtensionDataSet TheFindPhoneByExtensionDataSet = new FindPhoneByExtensionDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeebyEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        public EditPhoneExt()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessagesClass.LaunchEmail();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            Visibility = Visibility.Hidden;
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
            bool blnFatalError = false;

            try
            {
                txtDirectNumber.Text = "";
                txtEnterLastName.Text = "";
                txtEnterPhoneExt.Text = "";
                txtMACAddress.Text = "";

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;

                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Phone Ext");

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Phone Ext // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterPhoneExt_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            int intPhoneExtension = 0;
            string strDirectNumber;

            try
            {
                strValueForValidation = txtEnterPhoneExt.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Extension is not Numeric\n";
                }
                else
                {
                    intPhoneExtension = Convert.ToInt32(strValueForValidation);
                }
                strDirectNumber = txtDirectNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strDirectNumber);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Direct Number is not a Phone Number\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = ThePhoneClass.UpdatePhone(MainWindow.gintTransactionID, intPhoneExtension, MainWindow.gintWarehouseID, MainWindow.gintEmployeeID, strDirectNumber);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Phone Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Phone Ext // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
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

        private void expFindExtension_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strValueForValidation;
            int intPhoneExt = 0;
            int intRecordsReturned;
            int intEmployeeID;
            string strLastName;
            int intWarehouseID;
            int intSelectedIndex = 0;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                expFindExtension.IsExpanded = false;
                strValueForValidation = txtEnterPhoneExt.Text;
                if (strValueForValidation.Length == 4)
                {
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Extension Entered is not Numeric");
                        return;
                    }
                    else if (blnFatalError == false)
                    {
                        intPhoneExt = Convert.ToInt32(strValueForValidation);

                        TheFindPhoneByExtensionDataSet = ThePhoneClass.FindPhoneByExtension(intPhoneExt);

                        intRecordsReturned = TheFindPhoneByExtensionDataSet.FindPhoneByExtension.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            TheMessagesClass.ErrorMessage("The Phone Extension Does Not Exist");
                            return;
                        }

                        txtDirectNumber.Text = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].DirectNumber;
                        txtMACAddress.Text = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].MACAddress;
                        intEmployeeID = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].EmployeeID;
                        strLastName = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].LastName;
                        intWarehouseID = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].WarehouseID;
                        MainWindow.gintTransactionID = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].TransactionID;

                        //setting up the employee combo box
                        cboSelectEmployee.Items.Clear();
                        cboSelectEmployee.Items.Add("Select Employee");

                        TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                        intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);

                            if (intEmployeeID == TheComboEmployeeDataSet.employees[intCounter].EmployeeID)
                            {
                                intSelectedIndex = intCounter + 1;
                            }
                        }

                        cboSelectEmployee.SelectedIndex = intSelectedIndex;

                        intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            if (intWarehouseID == TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                            {
                                intSelectedIndex = intCounter + 1;
                            }
                        }

                        cboSelectOffice.SelectedIndex = intSelectedIndex;
                    }
                }
                else if (strValueForValidation.Length > 4)
                {
                    TheMessagesClass.ErrorMessage("There Are To Many Characters");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Phone Ext // Find Extension Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;
                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Was Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Phone Ext // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
