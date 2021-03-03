/* Title:           Assign Work Task Busines Line
 * Date:            3-1-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Assign Work Task Business Line */

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
using DepartmentDLL;
using WorkTaskDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignWorkTaskBusinessLine.xaml
    /// </summary>
    public partial class AssignWorkTaskBusinessLine : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByTaskKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindDepartmentByNameDataSet TheFindDepartmentByNameDataSet = new FindDepartmentByNameDataSet();
        FindWorkTaskDepartmentByWorkTaskDataSet TheFindWorkTaskDepartmentByWorkTaskDataSet = new FindWorkTaskDepartmentByWorkTaskDataSet();
        WorkTaskAssignedDataSet TheWorkTaskAssignedDataSet = new WorkTaskAssignedDataSet();
        FindDepartmentByDepartmentIDDataSet TheFindDepartmentByDepartmentIDDataSet = new FindDepartmentByDepartmentIDDataSet();
        FindWorkTaskDepartmentWorkTaskMatchDataSet TheFindWorkTaskDepartmentWorkTaskMatchDataSet = new FindWorkTaskDepartmentWorkTaskMatchDataSet();

        //setting up global variables
        string gstrFunction;
        int gintBusinessLineID;

        public AssignWorkTaskBusinessLine()
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtEnterTaskCode.Text = "";
                txtWorkTask.Text = "";
                cboSelectTask.Items.Clear();
                cboSelectTask.Items.Add("Select Task");
                cboSelectTask.SelectedIndex = 0;

                //loading up first combo box
                cboSelectFunction.Items.Clear();
                cboSelectFunction.Items.Add("Select Function");
                cboSelectFunction.Items.Add("Aerial");
                cboSelectFunction.Items.Add("Underground");
                cboSelectFunction.Items.Add("Both");
                cboSelectFunction.SelectedIndex = 0;

                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.SelectedIndex = 0;

                TheWorkTaskAssignedDataSet.worktaskassigned.Rows.Clear();

                dgrWorkTasks.ItemsSource = TheWorkTaskAssignedDataSet.worktaskassigned;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Assign Work Task Business Line ");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Work Task Business Line // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedInndex;
            int intCounter;
            int intNumberOfRecords;
            int intDepartmentID;
            string strFunction;

            try
            {
                intSelectedInndex = cboSelectTask.SelectedIndex - 1;

                if (intSelectedInndex > -1)
                {
                    MainWindow.gintWorkTaskID = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedInndex].WorkTaskID;

                    TheWorkTaskAssignedDataSet.worktaskassigned.Rows.Clear();

                    txtWorkTask.Text = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedInndex].WorkTask;

                    TheFindWorkTaskDepartmentByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskDepartmentByWorkTask(MainWindow.gintWorkTaskID);

                    intNumberOfRecords = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            intDepartmentID = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask[intCounter].DepartmentID;

                            TheFindDepartmentByDepartmentIDDataSet = TheDepartmentClass.FindDepartmentByDepartmentID(intDepartmentID);

                            strFunction = TheFindDepartmentByDepartmentIDDataSet.FindDepartmentByDepartmentID[0].Department;

                            WorkTaskAssignedDataSet.worktaskassignedRow NewTaskRow = TheWorkTaskAssignedDataSet.worktaskassigned.NewworktaskassignedRow();

                            NewTaskRow.BusinessLine = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask[intCounter].BusinessLine;
                            NewTaskRow.Function = strFunction;
                            NewTaskRow.WorkTask = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask[intCounter].WorkTask;
                            NewTaskRow.WorkTaskID = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask[intCounter].WorkTaskID;
                            NewTaskRow.TransactionID = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask[intCounter].TransactionID;

                            TheWorkTaskAssignedDataSet.worktaskassigned.Rows.Add(NewTaskRow);
                        }

                        dgrWorkTasks.ItemsSource = TheWorkTaskAssignedDataSet.worktaskassigned;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Work Task Business Line // Select Task Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectFunction.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrFunction = cboSelectFunction.SelectedItem.ToString().ToUpper();
            }
        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectBusinessLine.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;
            }
        }

        private void txtEnterTaskCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strWorkTask;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strWorkTask = txtEnterTaskCode.Text;

                if(strWorkTask.Length > 2)
                {
                    TheFindWorkTaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                    cboSelectTask.Items.Clear();
                    cboSelectTask.Items.Add("Select Task");

                    intNumberOfRecords = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Task Was Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectTask.Items.Add(TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
                    }

                    cboSelectTask.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Work Task Business Lines // Enter Task Code Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will add the files
            int intDepartmentID;
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intEmployeeID;
            DateTime datTransactionDate = DateTime.Now;
            bool blnBoth;

            try
            {
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                if(cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Was Not Selected\n";
                }
                if(cboSelectBusinessLine.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Businesss Line Was Not Selected\n";
                }
                if(cboSelectFunction.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Function Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                
                if(gstrFunction == "AERIAL")
                {
                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrFunction);

                    intDepartmentID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(MainWindow.gintWorkTaskID, gintBusinessLineID, intDepartmentID, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(gstrFunction == "UNDERGROUND")
                {
                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrFunction);

                    intDepartmentID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(MainWindow.gintWorkTaskID, gintBusinessLineID, intDepartmentID, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(gstrFunction == "BOTH")
                {
                    gstrFunction = "AERIAL";

                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrFunction);

                    intDepartmentID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(MainWindow.gintWorkTaskID, gintBusinessLineID, intDepartmentID, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    gstrFunction = "UNDERGROUND";

                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrFunction);

                    intDepartmentID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(MainWindow.gintWorkTaskID, gintBusinessLineID, intDepartmentID, intEmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Information Has Been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Work Task Business Line // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
