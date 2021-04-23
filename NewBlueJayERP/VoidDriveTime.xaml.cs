/* Title:           Void Drive Time
 * Date:            4-19-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Void Drive Time */

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
using EmployeeProjectAssignmentDLL;
using NewEmployeeDLL;
using ProjectMatrixDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VoidDriveTime.xaml
    /// </summary>
    public partial class VoidDriveTime : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        FindDriveTimeForVoidingDataSet TheFindDriveTimeForVoidingDataSet = new FindDriveTimeForVoidingDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        DriveTimeForVoidingDataSet TheDriveTimeForVoidingDataSet = new DriveTimeForVoidingDataSet();

        public VoidDriveTime()
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
            txtDate.Text = "";
            txtEnterLastName.Text = "";
            txtProjectID.Text = "";

            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");

            cboSelectEmployee.SelectedIndex = 0;

            TheDriveTimeForVoidingDataSet.drivetimeforvoiding.Rows.Clear();

            dgrResults.ItemsSource = TheDriveTimeForVoidingDataSet.drivetimeforvoiding;
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strLastName = "";

            try
            {
                strLastName = txtEnterLastName.Text;
                if (strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Drive Time // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Drive Time // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expFindItems_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strValueForValidation;
            string strProjectID;
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;
            int intProjectID = 0;
            int intCounter;
            int intNumberForRecords;

            try
            {
                expFindItems.IsExpanded = false;

                TheDriveTimeForVoidingDataSet.drivetimeforvoiding.Rows.Clear();

                if (cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Has Not Been Selected\n";
                }
                strProjectID = txtProjectID.Text;
                if(strProjectID.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project ID Is Not Long Enough\n";
                }
                else
                {
                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        intProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    }
                    else if(intRecordsReturned < 1)
                    {
                        TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strProjectID);

                        intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            intProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                        }
                        else if(intRecordsReturned < 1)
                        {
                            strErrorMessage += "The Project Was Not Found\n";
                        }
                    }
                }
                strValueForValidation = txtDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Entered is not a Date\n";
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindDriveTimeForVoidingDataSet = TheEmployeeProjectAssignmentClass.FindDriveTimeForVoiding(datTransactionDate, MainWindow.gintEmployeeID, intProjectID);

                intNumberForRecords = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding.Rows.Count;

                if(intNumberForRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberForRecords; intCounter++)
                    {
                        DriveTimeForVoidingDataSet.drivetimeforvoidingRow NewDriveTime = TheDriveTimeForVoidingDataSet.drivetimeforvoiding.NewdrivetimeforvoidingRow();

                        NewDriveTime.FirstName = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding[intCounter].FirstName;
                        NewDriveTime.LastName = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding[intCounter].LastName;
                        NewDriveTime.ProjectID = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding[intCounter].CustomerAssignedID;
                        NewDriveTime.TransactionDate = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding[intCounter].TransactionDate;
                        NewDriveTime.TransactionID = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding[intCounter].TransactionID;
                        NewDriveTime.VoidTransactions = false;
                        NewDriveTime.WorkTask = TheFindDriveTimeForVoidingDataSet.FindDriveTimeForVoiding[intCounter].WorkTask;

                        TheDriveTimeForVoidingDataSet.drivetimeforvoiding.Rows.Add(NewDriveTime);
                    }
                }

                dgrResults.ItemsSource = TheDriveTimeForVoidingDataSet.drivetimeforvoiding;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Drive Time // Find Items Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expVoidItems_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intTransactionID;
            decimal decTotalHours = 0;
            bool blnFatalError = false;

            try
            {
                expVoidItems.IsExpanded = false;

                intNumberOfRecords = TheDriveTimeForVoidingDataSet.drivetimeforvoiding.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheDriveTimeForVoidingDataSet.drivetimeforvoiding[intCounter].VoidTransactions == true)
                        {
                            intTransactionID = TheDriveTimeForVoidingDataSet.drivetimeforvoiding[intCounter].TransactionID;

                            blnFatalError = TheEmployeeProjectAssignmentClass.UpdateEmployeeLaborHours(intTransactionID, decTotalHours);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Selected Transactions Have Been Voided");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Drive Time // Void Items Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
