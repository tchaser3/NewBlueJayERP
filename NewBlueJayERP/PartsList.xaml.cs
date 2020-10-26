/* Title:           Parts List
 * Date:            10-19-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to print off a parts list */

using InspectionsDLL;
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
using NewPartNumbersDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using InventoryDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for PartsList.xaml
    /// </summary>
    public partial class PartsList : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindPartsListReportDataSet TheFindPartsListReportDataSet = new FindPartsListReportDataSet();
        PartsListReportDataSet ThePartsListReportDataSet = new PartsListReportDataSet();
        FindMasterPartListPartByPartIDDataSet TheFindMasterPartListByPartIDDataSet = new FindMasterPartListPartByPartIDDataSet();

        public PartsList()
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
            int intPartID;
            int intRecordsReturned;
            string strOldPartNumber;
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Parts List");

                if (blnFatalError == true)
                    throw new Exception();

                ThePartsListReportDataSet.partlistreport.Rows.Clear();

                TheFindPartsListReportDataSet = TheInventoryClass.FindPartsListReport();

                intNumberOfRecords = TheFindPartsListReportDataSet.FindPartsListReport.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intPartID = TheFindPartsListReportDataSet.FindPartsListReport[intCounter].PartID;

                    strOldPartNumber = "NONE FOUND";

                    TheFindMasterPartListByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                    intRecordsReturned = TheFindMasterPartListByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        strOldPartNumber = TheFindMasterPartListByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                    }

                    PartsListReportDataSet.partlistreportRow NewPartRow = ThePartsListReportDataSet.partlistreport.NewpartlistreportRow();

                    NewPartRow.JDEPartNumber = TheFindPartsListReportDataSet.FindPartsListReport[intCounter].JDEPartNumber;
                    NewPartRow.OldPartNumber = strOldPartNumber;
                    NewPartRow.PartDescription = TheFindPartsListReportDataSet.FindPartsListReport[intCounter].PartDescription;
                    NewPartRow.PartID = intPartID;
                    NewPartRow.PartNumber = TheFindPartsListReportDataSet.FindPartsListReport[intCounter].PartNumber;

                    ThePartsListReportDataSet.partlistreport.Rows.Add(NewPartRow);
                }

                dgrResult.ItemsSource = ThePartsListReportDataSet.partlistreport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Parts List // Reset Controls " + Ex.Message);

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
                intRowNumberOfRecords = ThePartsListReportDataSet.partlistreport.Rows.Count;
                intColumnNumberOfRecords = ThePartsListReportDataSet.partlistreport.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = ThePartsListReportDataSet.partlistreport.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = ThePartsListReportDataSet.partlistreport.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Parts List // Export To Excel " + ex.Message);

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
