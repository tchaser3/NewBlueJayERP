/* Title:           Add Cell Phone
 * Date:            3-11-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for adding new cell phones to ERP */

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
using CellPhoneHistoryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddCellPhone.xaml
    /// </summary>
    public partial class AddCellPhone : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CellPhoneHistoryClass TheCellPhoneHistoryClass = new CellPhoneHistoryClass();

        //setting up the data
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastFourDataSet = new FindCellPhoneByLastFourDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();        

        public AddCellPhone()
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
                txtEnterCellNumber.Text = "";
                txtEnterLastName.Text = "";
                txtEquipmentNotes.Text = "";

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

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Cell Phone");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cell Phone // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cell Phones // Enter Last Name Text Box " + Ex.Message);

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

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strCellNumber;
            string strEquipmentNotes;
            int intRecordsReturned;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strLastFour;
            int intPhoneID;

            try
            {
                strCellNumber = txtEnterCellNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strCellNumber);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Cell Phone Number is not the Correct Format\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office was not Selected\n";
                }
                strEquipmentNotes = txtEquipmentNotes.Text;
                if(strEquipmentNotes.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Equipment Notes are not Long Enought\n";
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strLastFour = strCellNumber.Substring(8, 4);

                TheFindCellPhoneByLastFourDataSet = ThePhonesClass.FindCellPhoneByLastFour(strLastFour);

                intRecordsReturned = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Last Four Numbers Of The\nCell Phone Number Exists");
                    return;
                }

                blnFatalError = ThePhonesClass.InsertCellPhone(strCellNumber, MainWindow.gintEmployeeID, MainWindow.gintWarehouseID, strEquipmentNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindCellPhoneByLastFourDataSet = ThePhonesClass.FindCellPhoneByLastFour(strCellNumber);

                intPhoneID = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneID;

                blnFatalError = TheCellPhoneHistoryClass.InsertCellPhoneHistory(MainWindow.gintEmployeeID, intPhoneID, "Phone Was Created");

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Cell Phone Number has been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cell Phone // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
