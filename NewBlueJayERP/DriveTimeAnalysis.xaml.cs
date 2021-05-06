/* Title:           Drive Time Analysis
 * Date:            5-6-21
 * Author:          Terry Holmes
 * 
 * Description:     This used for Drive Time Analysis */

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
using EmployeeProjectAssignmentDLL;
using Microsoft.Win32;
using NewEmployeeDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for DriveTimeAnalysis.xaml
    /// </summary>
    public partial class DriveTimeAnalysis : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        FindEmployeeProjectAssignmentDriveTimeHoursDataSet TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet = new FindEmployeeProjectAssignmentDriveTimeHoursDataSet();
        FindEmployeeProjectAssignmentForComparisonDataSet TheFindEmployeeProjectAssignmentForComparisonDataSet = new FindEmployeeProjectAssignmentForComparisonDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        EmployeeDriveTimeDataSet TheEmployeeDriveTimeDataSet = new EmployeeDriveTimeDataSet();

        public DriveTimeAnalysis()
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
            int intEmployeeID;
            int intProjectID;
            DateTime datTransactionDate;
            bool blnFatalError = false;
            int intRecordsReturned;
            int intManagerID;
            string strManagerName;
            int intSecondCounter;
            decimal decReportHours;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                TheEmployeeDriveTimeDataSet.employeedrivetime.Rows.Clear();

                TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProjectAssignmentDriveTimeHours();

                intNumberOfRecords = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].EmployeeID;
                        intProjectID = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].ProjectID;
                        datTransactionDate = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].TransactionDate;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                        intManagerID = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].ManagerID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                        strManagerName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                        strManagerName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                        TheFindEmployeeProjectAssignmentForComparisonDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProjectAssignmentForComparison(intEmployeeID, intProjectID, datTransactionDate);

                        intRecordsReturned = TheFindEmployeeProjectAssignmentForComparisonDataSet.FindEmployeeProjectAssignmentforComparison.Rows.Count;

                        if(intRecordsReturned < 1)
                        {              
                            EmployeeDriveTimeDataSet.employeedrivetimeRow NewEmployeeRow = TheEmployeeDriveTimeDataSet.employeedrivetime.NewemployeedrivetimeRow();

                            NewEmployeeRow.BlueJayProjectID = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].AssignedProjectID;
                            NewEmployeeRow.CustomerProjectID = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].CustomerAssignedID;
                            NewEmployeeRow.FirstName = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].FirstName;
                            NewEmployeeRow.LastName = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].LastName;
                            NewEmployeeRow.Manager = strManagerName;
                            NewEmployeeRow.ProjectName = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].ProjectName;
                            NewEmployeeRow.TotalHours = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].TotalHours;
                            NewEmployeeRow.TransactionDate = datTransactionDate;

                            TheEmployeeDriveTimeDataSet.employeedrivetime.Rows.Add(NewEmployeeRow);
                        }
                        else if(intRecordsReturned > 0)
                        {
                            decReportHours = 0;

                            for(intSecondCounter = 0; intSecondCounter < intRecordsReturned; intSecondCounter++)
                            {
                                decReportHours += TheFindEmployeeProjectAssignmentForComparisonDataSet.FindEmployeeProjectAssignmentforComparison[intSecondCounter].TotalHours;
                            }

                            if(decReportHours == 0)
                            {
                                EmployeeDriveTimeDataSet.employeedrivetimeRow NewEmployeeRow = TheEmployeeDriveTimeDataSet.employeedrivetime.NewemployeedrivetimeRow();

                                NewEmployeeRow.BlueJayProjectID = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].AssignedProjectID;
                                NewEmployeeRow.CustomerProjectID = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].CustomerAssignedID;
                                NewEmployeeRow.FirstName = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].FirstName;
                                NewEmployeeRow.LastName = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].LastName;
                                NewEmployeeRow.Manager = strManagerName;
                                NewEmployeeRow.ProjectName = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].ProjectName;
                                NewEmployeeRow.TotalHours = TheFindEmployeeProjectAssignmentDriveTimeHoursDataSet.FindEmployeeProjectAssignmentDriveTimeHours[intCounter].TotalHours;
                                NewEmployeeRow.TransactionDate = datTransactionDate;

                                TheEmployeeDriveTimeDataSet.employeedrivetime.Rows.Add(NewEmployeeRow);
                            }
                        }
                    }
                }

                dgrResults.ItemsSource = TheEmployeeDriveTimeDataSet.employeedrivetime;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Drive Time Analysis " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
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
                intRowNumberOfRecords = TheEmployeeDriveTimeDataSet.employeedrivetime.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeDriveTimeDataSet.employeedrivetime.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeDriveTimeDataSet.employeedrivetime.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeDriveTimeDataSet.employeedrivetime.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Drive Time Analysis // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
