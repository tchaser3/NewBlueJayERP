/* Title:           Server Audit Log
 * Date:            10-24-20
 * Author:          Terry Holmes
 * 
 * Description:     This is how to view the information in the Event Log */

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
using DataValidationDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using EmployeeDateEntryDLL;
using System.ComponentModel;
using System.Globalization;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ServerAuditLog.xaml
    /// </summary>
    public partial class ServerAuditLog : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //settting up the data
        FindServerEventLogByNoteKeywordDataSet TheFindServerEventLogByNoteKeywordDataSet = new FindServerEventLogByNoteKeywordDataSet();
        FindServerLogAccessByEmployeeIDDataSet TheFindServerLogAccessByEmployeeIDDataSet = new FindServerLogAccessByEmployeeIDDataSet();

        public ServerAuditLog()
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datTodaysDate = DateTime.Now;
            string strKeyword;
            int intRecordsReturned;

            try
            {
                TheFindServerLogAccessByEmployeeIDDataSet = TheEventLogClass.FindServerLogAccessByEmployeeID(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                intRecordsReturned = TheFindServerLogAccessByEmployeeIDDataSet.FindServerLogAccessByEmmployeeID.Rows.Count;

                if(intRecordsReturned < 0)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Server Audit Log // THERE HAS BEEN AN ATTEMPT TO ACCESS THROUGH ERP");

                    TheMessagesClass.ErrorMessage("ACCESS DENIED, IT HAS BEEN NOTIFIED");
                    throw new Exception();
                }

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Server Audit Log");

                if (blnFatalError == true)
                    throw new Exception();

                //beginning data validation
                strValueForValidation = txtStartDate.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Start Date is not a Date");
                    return;
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);

                    blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datTodaysDate);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after Todays Date");
                        return;
                    }
                }
                strKeyword = txtKeyword.Text;
                if(strKeyword.Length < 3)
                {
                    TheMessagesClass.ErrorMessage("The Search Term is not Long Enough");
                    return;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheFindServerEventLogByNoteKeywordDataSet = TheEventLogClass.FindServerEventLogByNoteKeyword(strKeyword, datStartDate, datTodaysDate);

                dgrResults.ItemsSource = TheFindServerEventLogByNoteKeywordDataSet.FindServerLogByNoteKeyword;

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Server Audit Log // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

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
            txtKeyword.Text = "";
            txtStartDate.Text = "";

           TheFindServerEventLogByNoteKeywordDataSet = TheEventLogClass.FindServerEventLogByNoteKeyword("NOTHING", DateTime.Now, DateTime.Now);           
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
                intRowNumberOfRecords = TheFindServerEventLogByNoteKeywordDataSet.FindServerLogByNoteKeyword.Rows.Count;
                intColumnNumberOfRecords = TheFindServerEventLogByNoteKeywordDataSet.FindServerLogByNoteKeyword.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindServerEventLogByNoteKeywordDataSet.FindServerLogByNoteKeyword.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindServerEventLogByNoteKeywordDataSet.FindServerLogByNoteKeyword.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Server Audit Log // Export To Excel " + ex.Message);

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
