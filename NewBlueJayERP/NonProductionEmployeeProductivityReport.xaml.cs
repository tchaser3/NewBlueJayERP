/* Title:           Non-Production Employee Productivity Report
 * Date:            2-9-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to capture non-production employees productivity */

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
using DataValidationDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for NonProductionEmployeeProductivityReport.xaml
    /// </summary>
    public partial class NonProductionEmployeeProductivityReport : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        NonProductionProductivityClass TheNonProductionProductivityClass = new NonProductionProductivityClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindTotalNonProductionProductivityOverDateRangeDataSet TheFindTotalNonProductionProductivityOverDateRange = new FindTotalNonProductionProductivityOverDateRangeDataSet();
        ReportedNonProductionProductivityDataSet TheReportedNonProdcuctionProductivityDataSet = new ReportedNonProductionProductivityDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        public NonProductionEmployeeProductivityReport()
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
            txtEndDate.Text = "";
            txtStartDate.Text = "";

            TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.Rows.Clear();

            dgrEmployees.ItemsSource = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity;
        }

        private void expCreateReport_Expanded(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            int intCounter;
            int intNumberOfRecords;
            int intManagerID;
            string strManager;

            try
            {
                //clearing data set
                TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.Rows.Clear();

                expCreateReport.IsExpanded = false;
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                TheFindTotalNonProductionProductivityOverDateRange = TheNonProductionProductivityClass.FindTotalNonProductionProductivityOverDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intManagerID = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange[intCounter].ManagerID;                        

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                        strManager = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                        strManager += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                        ReportedNonProductionProductivityDataSet.reportednonproductionproductivityRow NewEmployeeRow = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.NewreportednonproductionproductivityRow();

                        NewEmployeeRow.Department = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange[intCounter].Department;
                        NewEmployeeRow.FirstName = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange[intCounter].FirstName;
                        NewEmployeeRow.HomeOffice = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange[intCounter].HomeOffice;
                        NewEmployeeRow.LastName = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange[intCounter].LastName;
                        NewEmployeeRow.Manager = strManager;
                        NewEmployeeRow.TotalHours = TheFindTotalNonProductionProductivityOverDateRange.FindTotalNonProducductionProductivityOverDateRange[intCounter].TotalHours;

                        TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.Rows.Add(NewEmployeeRow);
                    }
                }

                dgrEmployees.ItemsSource = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Non-Production Employee Productivity Report // Create Report Expander " + Ex.Message);

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
                intRowNumberOfRecords = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.Rows.Count;
                intColumnNumberOfRecords = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheReportedNonProdcuctionProductivityDataSet.reportednonproductionproductivity[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Non Production Employee Productivity Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void dgrEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetailedNonProductionEmployeeProductivity DetailedNonProductionEmployeeProductivity = new DetailedNonProductionEmployeeProductivity();
            DetailedNonProductionEmployeeProductivity.ShowDialog();
        }
    }
}
