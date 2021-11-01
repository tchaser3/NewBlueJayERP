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
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindServerEventLogForReportsByDateRangeDataSet TheFindServerEventLogForReportsbyDateRangeDataSet = new FindServerEventLogForReportsByDateRangeDataSet();
        FindServerEventLogForReportsByUserDataSet TheFindServerEventLogForReportsByUserDataSet = new FindServerEventLogForReportsByUserDataSet();
        FindServerEventLogForReportsByItemDataSet TheFindServerEventLogForReportsByItemDataSet = new FindServerEventLogForReportsByItemDataSet();
        EventlLogSecurityDataSet TheEventLogSecurityDataSet = new EventlLogSecurityDataSet();

        int gintSelectedIndex;
        DateTime gdatStartDate;
        DateTime gdatEndDate;

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
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

                txtEndDate.Text = "";
                txtStartDate.Text = "";
                txtEnterKeyWord.Text = "";
                cboReportType.Items.Clear();
                cboReportType.Items.Add("Select Report Type");
                cboReportType.Items.Add("Date Search Report");
                cboReportType.Items.Add("User Report");
                cboReportType.Items.Add("Item Search");
                cboReportType.SelectedIndex = 0;

                datStartDate = DateTime.Now;
                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);
                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 1);

                TheFindServerEventLogForReportsbyDateRangeDataSet = TheEventLogClass.FindServerEventLogForReportsByDateRange(datStartDate, datEndDate);

                intNumberOfRecords = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        datTransactionDate = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange[intCounter].TransactionDate;
                        strLogonName = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange[intCounter].LogonName;
                        strItemAccessed = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange[intCounter].ItemAccessed;

                        EventlLogSecurityDataSet.eventlogsecurityRow NewEventEntry = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                        NewEventEntry.TransactionDate = datTransactionDate;
                        NewEventEntry.LogonName = strLogonName;
                        NewEventEntry.ItemAccessed = strItemAccessed;

                        TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventEntry);
                    }
                }

                dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;

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
            string strKeyword = "";
            string strValueForValidation;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strLogonName;
            string strItemAccessed;
            string strEventNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intSecondCounter;
            bool blnThereIsAProblem;

            try
            {
                TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

                //data validation
                if(cboReportType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Report Type Was Not Chosen\n";
                }
                if(gintSelectedIndex > 1)
                {
                    strKeyword = txtEnterKeyWord.Text;
                    if(strKeyword.Length < 3)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Word Entered is not Long Enough\n";
                    }
                }
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
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
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if (gintSelectedIndex == 1)
                {
                    TheFindServerEventLogForReportsbyDateRangeDataSet = TheEventLogClass.FindServerEventLogForReportsByDateRange(gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            datTransactionDate = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange[intCounter].TransactionDate;
                            strItemAccessed = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange[intCounter].ItemAccessed;
                            strLogonName = TheFindServerEventLogForReportsbyDateRangeDataSet.FindServerEventLogForReportsByDateRange[intCounter].LogonName;

                            EventlLogSecurityDataSet.eventlogsecurityRow NewEventRow = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                            NewEventRow.ItemAccessed = strItemAccessed;
                            NewEventRow.LogonName = strLogonName;
                            NewEventRow.TransactionDate = datTransactionDate;

                            TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventRow);
                        }
                    }
                }
                else if (gintSelectedIndex == 2)
                {
                    TheFindServerEventLogForReportsByUserDataSet = TheEventLogClass.FindServerEventLogForReportsByUser(strKeyword, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindServerEventLogForReportsByUserDataSet.FindServerEventLogForReportsByUser.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            datTransactionDate = TheFindServerEventLogForReportsByUserDataSet.FindServerEventLogForReportsByUser[intCounter].TransactionDate;
                            strItemAccessed = TheFindServerEventLogForReportsByUserDataSet.FindServerEventLogForReportsByUser[intCounter].ItemAccessed;
                            strLogonName = TheFindServerEventLogForReportsByUserDataSet.FindServerEventLogForReportsByUser[intCounter].LogonName;

                            EventlLogSecurityDataSet.eventlogsecurityRow NewEventRow = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                            NewEventRow.ItemAccessed = strItemAccessed;
                            NewEventRow.LogonName = strLogonName;
                            NewEventRow.TransactionDate = datTransactionDate;

                            TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventRow);
                        }
                    }
                }
                else if (gintSelectedIndex == 3)
                {
                    TheFindServerEventLogForReportsByItemDataSet = TheEventLogClass.FindServerEventLogForReportsByItem(strKeyword, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindServerEventLogForReportsByItemDataSet.FindServerEventLogForReportsByItem.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            datTransactionDate = TheFindServerEventLogForReportsByItemDataSet.FindServerEventLogForReportsByItem[intCounter].TransactionDate;
                            strItemAccessed = TheFindServerEventLogForReportsByItemDataSet.FindServerEventLogForReportsByItem[intCounter].ItemAccessed;
                            strLogonName = TheFindServerEventLogForReportsByItemDataSet.FindServerEventLogForReportsByItem[intCounter].LogonName;

                            EventlLogSecurityDataSet.eventlogsecurityRow NewEventRow = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                            NewEventRow.ItemAccessed = strItemAccessed;
                            NewEventRow.LogonName = strLogonName;
                            NewEventRow.TransactionDate = datTransactionDate;

                            TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventRow);
                        }
                    }
                }
                

                dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;

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

        private void expEmailReport_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress = "tholmes@bluejaycommunications.com";
            string strHeader;
            string strMessage;
            DateTime datPayDate = DateTime.Now;
            bool blnFatalError = false;

            try
            {
                expEmailReport.IsExpanded = false;
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intNumberOfRecords = TheEventLogSecurityDataSet.eventlogsecurity.Rows.Count;

                strHeader = "Server File Access Report Prepared on " + Convert.ToString(datPayDate);

                strMessage = "<h1>Server File Access Report Prepared on " + Convert.ToString(datPayDate) + "</h1>";
                strMessage += "<p>               </p>";
                strMessage += "<p>               </p>";
                strMessage += "<table>";
                strMessage += "<tr>";
                strMessage += "<td><b>Transaction Date</b></td>";
                strMessage += "<td><b>Logon Name</b></td>";
                strMessage += "<td><b>Item Accessed</b></td>";
                strMessage += "</tr>";
                strMessage += "<p>               </p>";

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strMessage += "<tr>";
                        strMessage += "<td>" + Convert.ToString(TheEventLogSecurityDataSet.eventlogsecurity[intCounter].TransactionDate) + "</td>";
                        strMessage += "<td>" + TheEventLogSecurityDataSet.eventlogsecurity[intCounter].LogonName + "</td>";
                        strMessage += "<td>" + TheEventLogSecurityDataSet.eventlogsecurity[intCounter].ItemAccessed + "</td>";
                        strMessage += "</tr>";
                        strMessage += "<p>               </p>";
                    }
                }

                strMessage += "</table>";

                blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Server Security Report // Email Report Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gintSelectedIndex = cboReportType.SelectedIndex;

            if(gintSelectedIndex == 1)
            {
                lblEnterKeyWord.Visibility = Visibility.Hidden;
                txtEnterKeyWord.Visibility = Visibility.Hidden;
                btnSearch.IsEnabled = true;
            }
            else if(gintSelectedIndex == 2)
            {
                lblEnterKeyWord.Visibility = Visibility.Visible;
                lblEnterKeyWord.Content = "Enter User";
                txtEnterKeyWord.Visibility = Visibility.Visible;
                btnSearch.IsEnabled = true;
            }
            else if(gintSelectedIndex == 3)
            {
                lblEnterKeyWord.Visibility = Visibility.Visible;
                lblEnterKeyWord.Content = "Enter Item";
                txtEnterKeyWord.Visibility = Visibility.Visible;
                btnSearch.IsEnabled = true;
            }
            else
            {
                lblEnterKeyWord.Visibility = Visibility.Hidden;
                txtEnterKeyWord.Visibility = Visibility.Hidden;
                btnSearch.IsEnabled = false;
            }
        }
    }
}
