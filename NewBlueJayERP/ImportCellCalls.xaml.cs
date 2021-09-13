/* Title:           Import Cell Calls
 * Date:            9-10-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import the cell calls */

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
using CellPhoneCallsDLL;
using PhonesDLL;
using DataValidationDLL;
using NewEventLogDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportCellCalls.xaml
    /// </summary>
    public partial class ImportCellCalls : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        CellPhoneCallsDataSet TheCellPhoneCallsDataSet = new CellPhoneCallsDataSet();
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastFourDataSet = new FindCellPhoneByLastFourDataSet();
        FindCellPhoneCallForVerificationDataSet TheFindCellPhoneCallForVerificationDataSet = new FindCellPhoneCallForVerificationDataSet();

        public ImportCellCalls()
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
        private void  ResetControls()
        {
            TheCellPhoneCallsDataSet.cellphonecalls.Rows.Clear();

            dgrCellCalls.ItemsSource = TheCellPhoneCallsDataSet.cellphonecalls;

            TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Import Cell Calls " );

        }

        private void expImportExcel_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strCellNumber;
            string strLastFour;
            int intPhoneID;
            int intEmployeeID;
            string strFirstName;
            string strLastName;
            DateTime datTransactionDate;
            string strDestination;
            int intCallMinutes;
            string strCallTime;
            string strTransactionNumber;
            string strTransactionDate;
            string strCallMinutes;
            int intRecordsReturned;
            double douDate;
            double douTime;

            try
            {
                expImportExcel.IsExpanded = false;
                TheCellPhoneCallsDataSet.cellphonecalls.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 19000; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strCellNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strDestination = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strTransactionDate = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strCallMinutes = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                    strTransactionNumber = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                    strCallTime = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();

                    strLastFour = strCellNumber.Substring(8, 4);

                    if(strLastFour != "5546")
                    {
                        TheFindCellPhoneByLastFourDataSet = ThePhonesClass.FindCellPhoneByLastFour(strLastFour);

                        intRecordsReturned = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            TheMessagesClass.ErrorMessage(strCellNumber + " Cell Number Does Not Exist");

                            return;
                        }

                        intPhoneID = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneID;
                        intEmployeeID = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].EmployeeID;
                        strFirstName = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].FirstName;
                        strLastName = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].LastName;

                        douDate = Convert.ToDouble(strTransactionDate);

                        douTime = Convert.ToDouble(strCallTime);

                        douDate = douDate + douTime;

                        datTransactionDate = DateTime.FromOADate(douDate);



                        strCallTime = "";

                        intCallMinutes = Convert.ToInt32(strCallMinutes);

                        CellPhoneCallsDataSet.cellphonecallsRow NewCallRow = TheCellPhoneCallsDataSet.cellphonecalls.NewcellphonecallsRow();

                        NewCallRow.CallMinutes = intCallMinutes;
                        NewCallRow.CallTime = strCallTime;
                        NewCallRow.CellNumber = strCellNumber;
                        NewCallRow.Destination = strDestination;
                        NewCallRow.EmployeeID = intEmployeeID;
                        NewCallRow.FirstName = strFirstName;
                        NewCallRow.LastName = strLastName;
                        NewCallRow.PhoneID = intPhoneID;
                        NewCallRow.TransactionDate = datTransactionDate;
                        NewCallRow.TransactionNumber = strTransactionNumber;

                        TheCellPhoneCallsDataSet.cellphonecalls.Rows.Add(NewCallRow);
                    }
                }

                dgrCellCalls.ItemsSource = TheCellPhoneCallsDataSet.cellphonecalls;
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Cell Calls // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intPhoneID;
            int intEmployeeID;
            DateTime datTransactionDate;
            string strDestination;
            int intCallMinutes;
            string strCallTime;
            string strTransactionNumber;
            int intRecordsReturned;

            

            try
            {
                expProcessImport.IsExpanded = false;

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intNumberOfRecords = TheCellPhoneCallsDataSet.cellphonecalls.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intPhoneID = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].PhoneID;
                        intEmployeeID = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].EmployeeID;
                        datTransactionDate = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].TransactionDate;
                        strDestination = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].Destination;
                        intCallMinutes = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].CallMinutes;
                        strCallTime = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].CallTime;
                        strTransactionNumber = TheCellPhoneCallsDataSet.cellphonecalls[intCounter].TransactionNumber;

                        TheFindCellPhoneCallForVerificationDataSet = TheCellPhoneCallsClass.FindCellPhoneCallForVerification(intPhoneID, datTransactionDate, intCallMinutes, strCallTime, strTransactionNumber);

                        intRecordsReturned = TheFindCellPhoneCallForVerificationDataSet.FindCellPhoneCallForVerification.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            blnFatalError = TheCellPhoneCallsClass.InsertCellPhoneCall(intPhoneID, intEmployeeID, datTransactionDate, strDestination, intCallMinutes, strCallTime, strTransactionNumber);

                            if(blnFatalError == true)
                            {
                                throw new Exception();
                            }
                        }
                    }
                }

                PleaseWait.Close();

                TheMessagesClass.InformationMessage("All Calls Have Been Imported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Cell Calls // Process Import " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

           
        }
    }
}
