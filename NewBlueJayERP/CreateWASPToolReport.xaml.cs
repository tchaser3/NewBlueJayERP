/* Title:           Create WASP Tool Report
 * Date:            4-23-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for creating a WASP Tool Report */

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
using NewToolsDLL;
using ToolHistoryDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateWASPToolReport.xaml
    /// </summary>
    public partial class CreateWASPToolReport : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        FindActiveToolsDataSet TheFindActiveToolsDataSet = new FindActiveToolsDataSet();
        FindToolHistoryByToolKeyDataSet TheFindToolHistoryByToolKeyDataSet = new FindToolHistoryByToolKeyDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        WASPToolsDataSet TheWASPToolsDataSet = new WASPToolsDataSet();
        WASPToolsDataSet TheNoWASPTransactionsDataSet = new WASPToolsDataSet();

        public CreateWASPToolReport()
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
            lblActiveTools.Content = "Active Tools";

            TheFindActiveToolsDataSet = TheToolsClass.FindActiveTools();

            dgrTools.ItemsSource = TheFindActiveToolsDataSet.FindActiveTools;
        }

        private void expCreateReport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;
            int intRecordsReturned;
            int intToolKey;
            DateTime datEndDate = DateTime.Now;
            DateTime datStartDate = DateTime.Now;
            string strWarehouse;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheFindActiveToolsDataSet.FindActiveTools.Rows.Count;

                TheWASPToolsDataSet.wasptools.Rows.Clear();

                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 365);

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intToolKey = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolKey;

                        TheFindToolHistoryByToolKeyDataSet = TheToolHistoryClass.FindToolHistoryByToolKey(datStartDate, datEndDate, intToolKey);

                        intRecordsReturned = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            WASPToolsDataSet.wasptoolsRow NewToolRow = TheWASPToolsDataSet.wasptools.NewwasptoolsRow();

                            intWarehouseID = TheFindActiveToolsDataSet.FindActiveTools[intCounter].CurrentLocation;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);

                            strWarehouse = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;

                            NewToolRow.FirstName = TheFindActiveToolsDataSet.FindActiveTools[intCounter].FirstName;
                            NewToolRow.LastName = TheFindActiveToolsDataSet.FindActiveTools[intCounter].LastName;
                            NewToolRow.ToolCategory = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolCategory;
                            NewToolRow.ToolDescription = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolDescription;
                            NewToolRow.ToolID = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolID;
                            NewToolRow.ToolKey = intToolKey;
                            NewToolRow.ToolNotes = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolNotes;
                            NewToolRow.Site = strWarehouse;
                            NewToolRow.TransactionDate = TheFindActiveToolsDataSet.FindActiveTools[intCounter].TransactionDate;

                            TheWASPToolsDataSet.wasptools.Rows.Add(NewToolRow);
                        }
                        else if(intRecordsReturned < 1)
                        {
                            WASPToolsDataSet.wasptoolsRow NewNoTransaction = TheNoWASPTransactionsDataSet.wasptools.NewwasptoolsRow();

                            intWarehouseID = TheFindActiveToolsDataSet.FindActiveTools[intCounter].CurrentLocation;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);

                            strWarehouse = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;

                            NewNoTransaction.FirstName = TheFindActiveToolsDataSet.FindActiveTools[intCounter].FirstName;
                            NewNoTransaction.LastName = TheFindActiveToolsDataSet.FindActiveTools[intCounter].LastName;
                            NewNoTransaction.ToolCategory = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolCategory;
                            NewNoTransaction.ToolDescription = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolDescription;
                            NewNoTransaction.ToolID = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolID;
                            NewNoTransaction.ToolKey = intToolKey;
                            NewNoTransaction.ToolNotes = TheFindActiveToolsDataSet.FindActiveTools[intCounter].ToolNotes;
                            NewNoTransaction.Site = strWarehouse;
                            NewNoTransaction.TransactionDate = TheFindActiveToolsDataSet.FindActiveTools[intCounter].TransactionDate;

                            TheNoWASPTransactionsDataSet.wasptools.Rows.Add(NewNoTransaction);
                        }
                    }
                }

                dgrTools.ItemsSource = TheNoWASPTransactionsDataSet.wasptools;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create WASP Tool Report // Create Report Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                blnFatalError = ExportUsedTools();

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = ExportedNotUsedTools();

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create WASP Tool Report // Export To Excel Tool Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();

        }
        private bool ExportUsedTools()
        {
            bool blnFatalError = false;
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

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheWASPToolsDataSet.wasptools.Rows.Count;
                intColumnNumberOfRecords = TheWASPToolsDataSet.wasptools.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWASPToolsDataSet.wasptools.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWASPToolsDataSet.wasptools.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create WASP Tool Report // Export Used Tools " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }


            return blnFatalError;
        }
        private bool ExportedNotUsedTools()
        {
            bool blnFatalError = false;
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

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheNoWASPTransactionsDataSet.wasptools.Rows.Count;
                intColumnNumberOfRecords = TheNoWASPTransactionsDataSet.wasptools.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheNoWASPTransactionsDataSet.wasptools.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheNoWASPTransactionsDataSet.wasptools.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create WASP Tool Report // Export Not Used Tools " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }


            return blnFatalError;
        }
    }
}
