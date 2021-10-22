/* Title:           Server Security Report
 * Date:            10-22-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show the server report */

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
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using NewEmployeeDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ServerSercurityReport.xaml
    /// </summary>
    public partial class ServerSercurityReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindServerEventLogSecurityAccessDataSet TheFindServerEventLogSecurityAccessDataSet = new FindServerEventLogSecurityAccessDataSet();
        FindServerEventLogSecurityAccessByKeywordDataSet TheFindServerEventLogSecurityAccessByKeywordDataSet = new FindServerEventLogSecurityAccessByKeywordDataSet();
        EventlLogSecurityDataSet TheEventLogSecurityDataSet = new EventlLogSecurityDataSet();

        public ServerSercurityReport()
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
            //setting up the variables
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strLogonName;
            string strItemAccessed;
            string strEventNotes;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

                TheFindServerEventLogSecurityAccessDataSet = TheEventLogClass.FindServerEventLogSecurityAccess();

                intNumberOfRecords = TheFindServerEventLogSecurityAccessDataSet.FindServerEventLogSecurityAccess.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        datTransactionDate = TheFindServerEventLogSecurityAccessDataSet.FindServerEventLogSecurityAccess[intCounter].TransactionDate;
                        strLogonName = "Just Beginging";
                        strItemAccessed = "Date Goes Here";
                        strEventNotes = TheFindServerEventLogSecurityAccessDataSet.FindServerEventLogSecurityAccess[intCounter].EventNotes;

                        char[] delims = new[] { '\n', '\t', '\r' };
                        string []strNewItems = strEventNotes.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        strLogonName = strNewItems[5];
                        strItemAccessed = strNewItems[16];

                        EventlLogSecurityDataSet.eventlogsecurityRow NewEventRow = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                        NewEventRow.TransactionDate = datTransactionDate;
                        NewEventRow.LogonName = strLogonName;
                        NewEventRow.ItemAccessed = strItemAccessed;

                        TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventRow);
                    }
                }

                dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Server Security Report");

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Event Log Security // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strKeyword;
            string strValueForValidation;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strLogonName;
            string strItemAccessed;
            string strEventNotes;
            DateTime datStartDate = DateTime.Now;
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

                strValueForValidation = txtEnterDate.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    strErrorMessage += "The Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strKeyword = txtEnterKeyword.Text;
                if(strKeyword.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Keyword is to Short\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheFindServerEventLogSecurityAccessByKeywordDataSet = TheEventLogClass.FindServerEventLogSecurityByKeyword(strKeyword, datStartDate, DateTime.Now);

                intNumberOfRecords = TheFindServerEventLogSecurityAccessByKeywordDataSet.FindServerEventLogSercurityAccessByKeyword.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        datTransactionDate = TheFindServerEventLogSecurityAccessByKeywordDataSet.FindServerEventLogSercurityAccessByKeyword[intCounter].TransactionDate;
                        strLogonName = "Just Beginging";
                        strItemAccessed = "Date Goes Here";
                        strEventNotes = TheFindServerEventLogSecurityAccessByKeywordDataSet.FindServerEventLogSercurityAccessByKeyword[intCounter].EventNotes;

                        
                        char[] delims = new[] { '\n', '\t', '\r' };
                        string[] strNewItems = strEventNotes.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        strLogonName = strNewItems[5];
                        strItemAccessed = strNewItems[16];

                        EventlLogSecurityDataSet.eventlogsecurityRow NewEventRow = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                        NewEventRow.TransactionDate = datTransactionDate;
                        NewEventRow.LogonName = strLogonName;
                        NewEventRow.ItemAccessed = strItemAccessed;

                        TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventRow);
                                                
                    }
                }

                dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Server Security Report // Search Button " + Ex.Message);

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
                intRowNumberOfRecords = TheEventLogSecurityDataSet.eventlogsecurity.Rows.Count;
                intColumnNumberOfRecords = TheEventLogSecurityDataSet.eventlogsecurity.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEventLogSecurityDataSet.eventlogsecurity.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEventLogSecurityDataSet.eventlogsecurity.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Server Security Report // Export To Excel " + ex.Message);

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
