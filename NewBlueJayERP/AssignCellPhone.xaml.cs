/* Title:           Assign Cell Phone
 * Date:            3-30-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Assign a Cell phone */

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
using PhonesDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using CellPhoneHistoryDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignCellPhone.xaml
    /// </summary>
    public partial class AssignCellPhone : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CellPhoneHistoryClass TheCellPhoneHistoryClass = new CellPhoneHistoryClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastFourDataSet = new FindCellPhoneByLastFourDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public AssignCellPhone()
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
            txtCellPhoneNumber.Text = "";
            txtCurrentAssignment.Text = "";
            txtLastFour.Text = "";
            txtLastName.Text = "";
            cboSelectEmployee.Items.Clear();

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Assign Cell Phone");
        }

        private void txtLastFour_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intLength;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                strValueForValidation = txtLastFour.Text;
                intLength = strValueForValidation.Length;
                if (intLength == 4)
                {
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Information Entered is not an Integer");
                        return;
                    }

                    TheFindCellPhoneByLastFourDataSet = ThePhoneClass.FindCellPhoneByLastFour(strValueForValidation);

                    intRecordsReturned = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Cell Phone Not Found");
                        return;
                    }

                    txtCellPhoneNumber.Text = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneNumber;
                    txtCurrentAssignment.Text = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].FirstName + " " + TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].LastName;
                    MainWindow.gintPhoneID = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneID;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Assign Cell Phone // Last Four Text Box " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Cell Phone // Last Four Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
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
                if (intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Assign Cell Phone // Last Name Text Box " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Cell Phone // Last name Text Box " + Ex.Message);

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
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strLastFour;
            string strErrorMessage = "";
            int intRecordsReturned = 0;

            try
            {
                strLastFour = txtLastFour.Text;
                if(strLastFour.Length == 4)
                {
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerRange(strLastFour);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Extension Entered is not Numeric\n";
                    }
                    else
                    {
                        TheFindCellPhoneByLastFourDataSet = ThePhoneClass.FindCellPhoneByLastFour(strLastFour);

                        intRecordsReturned = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Cell Phone Entered was not Found\n";
                        }

                        MainWindow.gintPhoneID = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneID;
                    }
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    blnFatalError = ThePhoneClass.UpdateCellPhoneUser(MainWindow.gintPhoneID, MainWindow.gintEmployeeID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheCellPhoneHistoryClass.InsertCellPhoneHistory(MainWindow.gintEmployeeID, MainWindow.gintPhoneID, "CHANGED PHONE USER");

                    if (blnFatalError == true)
                        throw new Exception();

                    TheMessagesClass.InformationMessage("Cell Phone Has Been Assigned");

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Assign Cell Phones // Proces Button " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Cell Phones // Proces Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
