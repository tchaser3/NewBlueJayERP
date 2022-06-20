/* Title:           Add Work Task
 * Date:            3-2-21
 * Author:          Terry Holmes
 * 
 * Description:     This is the way to add a Work Task*/

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
using WorkTaskDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;
using NewEventLogDLL;
using DepartmentDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddWorkTask.xaml
    /// </summary>
    public partial class AddWorkTask : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up for the data
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindDepartmentByNameDataSet TheFindDepartmentByNameDataSet = new FindDepartmentByNameDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByTaskKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();

        //setting up global variables
        string gstrDepartment;
        int gintDepartment;
        int gintBusinesLineID;
        int gintWorkTaskID;

        public AddWorkTask()
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
            //setting up local variables
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtEnterWorkTask.Text = "";

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");
                cboSelectDepartment.Items.Add("Aerial");
                cboSelectDepartment.Items.Add("Underground");
                cboSelectDepartment.Items.Add("Both");
                cboSelectDepartment.SelectedIndex = 0;

                //loading up the business line combo box
                cboSelectBusinesLine.Items.Clear();
                cboSelectBusinesLine.Items.Add("Select Business Line");

                //loading up the data
                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinesLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinesLine.SelectedIndex = 0;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Work Task");

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Add Work Task // Reset Controls " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Work Task // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectBusinesLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectBusinesLine.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintBusinesLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSelectDepartment.SelectedIndex > 0)
            {
                gstrDepartment = cboSelectDepartment.SelectedItem.ToString().ToUpper();
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strWorkTask;
            string strCode;
            int intRecordsReturned;
            bool blnFatalError = false;
            string strErrorMessage = "";
            decimal decCost;
            int intEmployeeID;
            DateTime datTransactionDate = DateTime.Now;

            try
            {
                decCost = 0;
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                strWorkTask = txtEnterWorkTask.Text;
                if(strWorkTask.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task is not Long Enough\n";
                }
                else
                {
                    strCode = strWorkTask.Substring(0, 7);

                    TheFindWorkTaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strCode);

                    intRecordsReturned = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Code is Already Been Used\n";
                    }
                }
                if (cboSelectBusinesLine.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Business Line was not Selected\n";
                } 
                if (cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department was not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheWorkTaskClass.InsertWorkTask(strWorkTask, decCost);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindWorkTaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                gintWorkTaskID = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;

                if(gstrDepartment == "AERIAL")
                {
                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrDepartment);

                    gintDepartment = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinesLineID, gintDepartment, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(gstrDepartment == "UNDERGROUND")
                {
                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrDepartment);

                    gintDepartment = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinesLineID, gintDepartment, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(gstrDepartment == "BOTH")
                {
                    gstrDepartment = "AERIAL";
                    
                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrDepartment);

                    gintDepartment = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinesLineID, gintDepartment, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    gstrDepartment = "UNDERGROUND";

                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrDepartment);

                    gintDepartment = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinesLineID, gintDepartment, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Work Task Has Been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Add Work Task // Process Button " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Work Task // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
