/* Title:           Add Admin Productivity
 * Date:            2-5-21
 * Author:          Terry Holmes
 * 
 * Description:     This Class is used for Adding Admin Productivity */

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
using NonProductionProductivityDLL;
using NewEventLogDLL;
using DataValidationDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddAdminProductivity.xaml
    /// </summary>
    public partial class AddAdminProductivity : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        NonProductionProductivityClass TheNonProductivityClass = new NonProductionProductivityClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindSortedNonProductionTaskDataSet TheFindSortedNonProductionTaskDataSet = new FindSortedNonProductionTaskDataSet();

        bool gblnSelfProductivity;

        public AddAdminProductivity()
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
                //clearing controls
                txtEndTime.Text = "";
                txtLastName.Text = "";
                txtStartTime.Text = "";
                txtTransactionDate.Text = Convert.ToString(DateTime.Now);
                txtTransactionNotes.Text = "";
                chkSelfReport.IsChecked = false;

                //clearing combo boxes
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;

                cboSelectTask.Items.Clear();
                cboSelectTask.Items.Add("Select Task");

                TheFindSortedNonProductionTaskDataSet = TheNonProductivityClass.FindSortedNonProductionTask();

                intNumberOfRecords = TheFindSortedNonProductionTaskDataSet.FindSortedNonProductionTask.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectTask.Items.Add(TheFindSortedNonProductionTaskDataSet.FindSortedNonProductionTask[intCounter].WorkTask);
                }

                cboSelectTask.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Admin Productivity // Reset Controls " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void chkSelfReport_Click(object sender, RoutedEventArgs e)
        {
            if(chkSelfReport.IsChecked == true)
            {
                cboSelectEmployee.Visibility = Visibility.Hidden;
                txtLastName.Visibility = Visibility.Hidden;
                gblnSelfProductivity = true;
                MainWindow.gintEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
            }
            else if(chkSelfReport.IsChecked == false)
            {
                cboSelectEmployee.Visibility = Visibility.Visible;
                txtLastName.Visibility = Visibility.Visible;
                gblnSelfProductivity = false;
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

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectTask.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWorkTaskID = TheFindSortedNonProductionTaskDataSet.FindSortedNonProductionTask[intSelectedIndex].WorkTaskID;
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strValueForValidation;
            DateTime datTransactionDate = DateTime.Now;
            string strStartTime;
            string strEndTime;
            decimal decTotalHours;
            string strNotes;

            try
            {
                if(gblnSelfProductivity == false)
                {
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                }
                strValueForValidation = txtTransactionDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date is not a Date\n";
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(strValueForValidation);
                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datTransactionDate, DateTime.Now);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Date Entered is in the Future\n";
                    }
                }
                strStartTime = txtStartTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTime(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Time is not a Time\n";
                }
                strEndTime = txtEndTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTime(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Time is not a Time\n";
                }
                if(cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Is Not Selected\n";
                }
                strNotes = txtTransactionNotes.Text;
                if(strNotes.Length < 1)
                {
                    strNotes = "NO NOTES ENTERED";
                }

                decTotalHours = CalculateTimeSpan(strStartTime, strEndTime);

                if (decTotalHours <= 0)
                {
                    TheMessagesClass.ErrorMessage("The Hours are either 0 or less than 0");
                    return;
                }

                blnFatalError = TheNonProductivityClass.InsertNonProductionProductivity(datTransactionDate, MainWindow.gintEmployeeID, strStartTime, strEndTime, decTotalHours, MainWindow.gintWorkTaskID, strNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Productivity has been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Admin Productivity // Process Expander " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private decimal CalculateTimeSpan(string strStartTime, string strEndTime)
        {
            decimal decTotalHours = 0;
            TimeSpan tspStartTime;
            TimeSpan tspEndTime;
            TimeSpan tspTotalTime;
            decimal decHours;
            decimal decMinutes;
            int intMinutes;

            try
            {
                tspStartTime = TimeSpan.Parse(strStartTime);

                tspEndTime = TimeSpan.Parse(strEndTime);

                tspTotalTime = tspEndTime - tspStartTime;

                decHours = Convert.ToDecimal(tspTotalTime.Hours);
                intMinutes = tspTotalTime.Minutes;
                decMinutes = Convert.ToDecimal(intMinutes) / 60;

                decTotalHours = decHours + decMinutes;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Design // Enter Design WOV Tech Pay // Calculate Time Span " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            return decTotalHours;
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strLastName;

            try
            {
                strLastName = txtLastName.Text;

                if(strLastName.Length > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Admin Productivity // Last Name Text Box " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
