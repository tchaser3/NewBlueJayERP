/* Title:           Sorted Work Task Report
 * Date:            10-20-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to do the Sorted Work Task Report*/

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
using WorkTaskDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using EmployeeDateEntryDLL;
using DepartmentDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for SortedWorkTaskReport.xaml
    /// </summary>
    public partial class SortedWorkTaskReport : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();

        //setting up the data
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSEt = new FindSortedCustomerLinesDataSet();
        FindWorkTaskByDepartmentDataSet TheFindWorkTaskByDepartmentDataSet = new FindWorkTaskByDepartmentDataSet();
        DepartmentWorkTaskDataSet TheDepartmentWorkTaskDataSet = new DepartmentWorkTaskDataSet();
        FindWorkTaskImportByWorkTaskIDDataSet TheFindWorkTaskImportByWorkTaskIDDataSet = new FindWorkTaskImportByWorkTaskIDDataSet();
        FindWorkTaskDepartmentForBusinessLineDataSet TheFindWorkTaskDepartmentForBusinessLineDataSet = new FindWorkTaskDepartmentForBusinessLineDataSet();
        FindDepartmentByDepartmentIDDataSet TheFindDepartmentByDepartmentIDDataSet = new FindDepartmentByDepartmentIDDataSet();

        public SortedWorkTaskReport()
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
                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Sorted Work Task Report");

                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");

                TheFindSortedCustomerLinesDataSEt = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSEt.FindSortedCustomerLines.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSEt.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.SelectedIndex = 0;

                TheFindWorkTaskByDepartmentDataSet = TheWorkTaskClass.FindWorkTaskByDepartment(-1);

                dgrWorkTasks.ItemsSource = TheDepartmentWorkTaskDataSet.worktask;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Sorted Work Task Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheDepartmentWorkTaskDataSet.worktask.Rows.Count;
                intColumnNumberOfRecords = TheDepartmentWorkTaskDataSet.worktask.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheDepartmentWorkTaskDataSet.worktask.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheDepartmentWorkTaskDataSet.worktask.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Sorted Work Task Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }

        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intDepartmentID;
            int intCounter;
            int intNumberOfNumber;
            int intWorkTaskID;
            int intBusinessLineID;
            string strDescription = "";
            string strDepartment = "";
            string strWorkTask;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = cboSelectBusinessLine.SelectedIndex - 1;

                TheDepartmentWorkTaskDataSet.worktask.Rows.Clear();

                if(intSelectedIndex > -1)
                {
                    intBusinessLineID = TheFindSortedCustomerLinesDataSEt.FindSortedCustomerLines[intSelectedIndex].DepartmentID;

                    TheFindWorkTaskByDepartmentDataSet = TheWorkTaskClass.FindWorkTaskByDepartment(intBusinessLineID);

                    intNumberOfNumber = TheFindWorkTaskByDepartmentDataSet.FindWorkTaskByDepartment.Rows.Count;

                    if(intNumberOfNumber > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfNumber; intCounter++)
                        {
                            intWorkTaskID = TheFindWorkTaskByDepartmentDataSet.FindWorkTaskByDepartment[intCounter].WorkTaskID;
                            strWorkTask = TheFindWorkTaskByDepartmentDataSet.FindWorkTaskByDepartment[intCounter].WorkTask;

                            TheFindWorkTaskImportByWorkTaskIDDataSet = TheWorkTaskClass.FindWorkTaskImportByWorkTaskID(intWorkTaskID);

                            intRecordsReturned = TheFindWorkTaskImportByWorkTaskIDDataSet.FindWorkTaskImportByWorkTaskID.Rows.Count;

                            if(intRecordsReturned == 0)
                            {
                                strDescription = "";
                            }
                            else if(intRecordsReturned > 0)
                            {
                                strDescription = TheFindWorkTaskImportByWorkTaskIDDataSet.FindWorkTaskImportByWorkTaskID[0].ItemDescription;
                            }

                            TheFindWorkTaskDepartmentForBusinessLineDataSet = TheWorkTaskClass.FindWorkTaskDepartmentForBusienssLine(intBusinessLineID, intWorkTaskID);

                            intRecordsReturned = TheFindWorkTaskDepartmentForBusinessLineDataSet.FindWorkTaskDepartmentForBusinessLine.Rows.Count;

                            if(intRecordsReturned > 1)
                            {
                                strDepartment = "BOTH";
                            }
                            else if(intRecordsReturned == 1)
                            {
                                intDepartmentID = TheFindWorkTaskDepartmentForBusinessLineDataSet.FindWorkTaskDepartmentForBusinessLine[0].DepartmentID;

                                TheFindDepartmentByDepartmentIDDataSet = TheDepartmentClass.FindDepartmentByDepartmentID(intDepartmentID);

                                strDepartment = TheFindDepartmentByDepartmentIDDataSet.FindDepartmentByDepartmentID[0].Department;
                            }

                            DepartmentWorkTaskDataSet.worktaskRow NewWorkTask = TheDepartmentWorkTaskDataSet.worktask.NewworktaskRow();

                            NewWorkTask.Department = strDepartment;
                            NewWorkTask.Description = strDescription;
                            NewWorkTask.WorkTask = strWorkTask;

                            TheDepartmentWorkTaskDataSet.worktask.Rows.Add(NewWorkTask);
                        }
                    }

                    dgrWorkTasks.ItemsSource = TheDepartmentWorkTaskDataSet.worktask;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Sorted Work Task Report // Select Business Line Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
