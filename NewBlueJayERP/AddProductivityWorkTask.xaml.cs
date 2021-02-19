/* Title:       Add Productivity Work Task
 * Date:        2-16-21
 * Author:      Terry Holmes
 * 
 * Description: This is used to add productivity work tasks */

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
using NewEventLogDLL;
using DepartmentDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddProductivityWorkTask.xaml
    /// </summary>
    public partial class AddProductivityWorkTask : Window
    {
        //setting up the classes
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindProductivityWorkTaskByWorkTaskIDDataSet TheFindProductivityWorkTaskByWorkTaskIDDataSet = new FindProductivityWorkTaskByWorkTaskIDDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindSortedProductionTypesDataSet TheFindSortedProductionTypesDataSet = new FindSortedProductionTypesDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByTaskKeyWordDataSet = new FindWorkTaskByTaskKeywordDataSet();

        //setting up global variables
        int gintBusinessLineID;
        int gintDepartmentID;
        int gintWorkTaskID;
        bool gblnAllBusinessLines;
        bool gblnAllDepartments;

        public AddProductivityWorkTask()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                txtEnterTask.Text = "";
                cboSelectTask.Items.Clear();
                cboSelectTask.Items.Add("Select Task");
                cboSelectTask.SelectedIndex = 0;

                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;
                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.Items.Add("All");
                cboSelectBusinessLine.SelectedIndex = 0;

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");

                TheFindSortedProductionTypesDataSet = TheDepartmentClass.FindSortedProductionTypes();

                intNumberOfRecords = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intCounter].Department);
                }

                cboSelectDepartment.Items.Add("All");
                cboSelectDepartment.SelectedIndex = 0;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Productivity Work Task ");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Productivity Work Task // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strWorkTask = txtEnterTask.Text;
                if(strWorkTask.Length > 2)
                {
                    TheFindWorkTaskByTaskKeyWordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);
                    cboSelectTask.Items.Clear();
                    cboSelectTask.Items.Add("Select Work Task");

                    intNumberOfRecords = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Work Task was not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectTask.Items.Add(TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
                    }

                    cboSelectTask.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Productivity Work Task // Enter Task Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strBusinessLine;

            intSelectedIndex = cboSelectBusinessLine.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                strBusinessLine = cboSelectBusinessLine.SelectedItem.ToString();

                if(strBusinessLine == "All")
                {
                    gblnAllBusinessLines = true;
                }
                else
                {
                    gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;

                    gblnAllBusinessLines = false;
                }
            }
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectTask.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintWorkTaskID = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strDepartment;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                strDepartment = cboSelectDepartment.SelectedItem.ToString();

                if(strDepartment == "All")
                {
                    gblnAllDepartments = true;
                }
                else
                {
                    gintDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intSelectedIndex].DepartmentID;

                    gblnAllDepartments = false;
                }
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intDepartmentCounter;
            int intDepartmentNumberOfRecords;
            int intBOLCounter;
            int intBOLNumberOfRecords;
            int intDepartmentID;
            int intBusinessLineID;

            try
            {
                if(cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task Was Not Selected\n";
                }
                if(cboSelectBusinessLine.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Business Line Was Not Selected\n";
                }
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intDepartmentNumberOfRecords = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes.Rows.Count;
                intBOLNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                if((gblnAllDepartments == false) && (gblnAllBusinessLines == false))
                {
                    blnFatalError = TheWorkTaskClass.InsertProductivityWorkTask(gintWorkTaskID, gintBusinessLineID, gintDepartmentID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if((gblnAllDepartments == false) && (gblnAllBusinessLines == true))
                {
                    for(intBOLCounter = 0; intBOLCounter < intBOLNumberOfRecords; intBOLCounter++)
                    {
                        intBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intBOLCounter].DepartmentID;

                        blnFatalError = TheWorkTaskClass.InsertProductivityWorkTask(gintWorkTaskID, intBusinessLineID, gintDepartmentID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
                else if((gblnAllDepartments == true) && (gblnAllBusinessLines == false))
                {
                    for(intDepartmentCounter = 0; intDepartmentCounter < intDepartmentNumberOfRecords; intDepartmentCounter++)
                    {
                        intDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intDepartmentCounter].DepartmentID;

                        blnFatalError = TheWorkTaskClass.InsertProductivityWorkTask(gintWorkTaskID, gintBusinessLineID, intDepartmentID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
                else if ((gblnAllDepartments == true) && (gblnAllBusinessLines == true))
                {
                    for (intBOLCounter = 0; intBOLCounter < intBOLNumberOfRecords; intBOLCounter++)
                    {
                        intBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intBOLCounter].DepartmentID;

                        for(intDepartmentCounter = 0; intDepartmentCounter < intDepartmentNumberOfRecords; intDepartmentCounter++)
                        {
                            intDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intDepartmentCounter].DepartmentID;

                            blnFatalError = TheWorkTaskClass.InsertProductivityWorkTask(gintWorkTaskID, intBusinessLineID, intDepartmentID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Information has been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Productivity Work Task // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
