/* Title:           Import Employee Punches
 * Date:            12-17-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import employee punches*/

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
using NewEmployeeDLL;
using EmployeeTimeClockEntriesDLL;
using DataValidationDLL;
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportEmployeePunches.xaml
    /// </summary>
    public partial class ImportEmployeePunches : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeTimeClockEntriesClass TheEmployeeTimeClockEntriesClass = new EmployeeTimeClockEntriesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        ImportedPunchesDataSet TheImportPunchesDataSet = new ImportedPunchesDataSet();
        FindEmployeeByPayIDDataSet TheFindEmployeebyPayIDDataSet = new FindEmployeeByPayIDDataSet();
        FindEmployeeTimeCardEntriesDataSet TheFindEmployeeTimeCardEntriesDataSet = new FindEmployeeTimeCardEntriesDataSet();

        public ImportEmployeePunches()
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
            TheImportPunchesDataSet.punches.Rows.Clear();

            dgrResults.ItemsSource = TheImportPunchesDataSet.punches;

            expProcess.IsEnabled = false;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Employee Punches");
        }

        private void expImportPunches_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intPayID = 0;
            string strValueForValidation;
            bool blnFatalError;
            bool blnNextRecord;
            DateTime datPunchDate = DateTime.Now;
            string strPunchStatus;

            try
            {
                expImportPunches.IsExpanded = false;
                TheImportPunchesDataSet.punches.Rows.Clear();

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

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnNextRecord = true;
                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);
                    if (strValueForValidation == "")
                    {
                        blnNextRecord = false;
                    }
                    else
                    {
                        strValueForValidation = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);
                        blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                        if (blnFatalError == true)
                        {
                            blnNextRecord = false;
                        }
                        else
                        {
                            intPayID = Convert.ToInt32(strValueForValidation);
                        }
                        strValueForValidation = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2);
                        //double douMyDate = Convert.ToDouble(strValueForValidation);

                        //DateTime datMyDate = DateTime.FromOADate(douMyDate);

                        blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("Date is not a Date");
                            return;
                        }
                        else
                        {
                            datPunchDate = Convert.ToDateTime(strValueForValidation);
                        }
                        
                       
                        
                        strPunchStatus = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2);
                        if (strPunchStatus == "")
                        {
                            blnNextRecord = false;
                        }
                        if (blnNextRecord == true)
                        {
                            TheFindEmployeebyPayIDDataSet = TheEmployeeClass.FindEmployeeByPayID(intPayID);

                            ImportedPunchesDataSet.punchesRow NewPunchRow = TheImportPunchesDataSet.punches.NewpunchesRow();

                            NewPunchRow.EmployeeID = TheFindEmployeebyPayIDDataSet.FindEmployeeByPayID[0].EmployeeID;
                            NewPunchRow.FirstName = TheFindEmployeebyPayIDDataSet.FindEmployeeByPayID[0].FirstName;
                            NewPunchRow.LastName = TheFindEmployeebyPayIDDataSet.FindEmployeeByPayID[0].LastName;
                            NewPunchRow.PayID = intPayID;
                            NewPunchRow.PunchStatus = strPunchStatus.ToUpper();
                            NewPunchRow.PunchTime = datPunchDate;

                            TheImportPunchesDataSet.punches.Rows.Add(NewPunchRow);
                        }
                    }
                }

                expProcess.IsEnabled = true;
                PleaseWait.Close();
                dgrResults.ItemsSource = TheImportPunchesDataSet.punches;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Import Excel Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intRecordCounter;
            bool blnTransactionProcessed;
            bool blnFatalError;
            int intEmployeeID;
            int intPayID;
            DateTime datPunchTime;
            string strPunchStatus;
            DateTime datStartDate;
            DateTime datEndDate;

            try
            {
                expProcess.IsExpanded = false;
                intNumberOfRecords = TheImportPunchesDataSet.punches.Rows.Count - 1;

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheImportPunchesDataSet.punches[intCounter].EmployeeID;
                    intPayID = TheImportPunchesDataSet.punches[intCounter].PayID;
                    datPunchTime = TheImportPunchesDataSet.punches[intCounter].PunchTime;
                    strPunchStatus = TheImportPunchesDataSet.punches[intCounter].PunchStatus;
                    datStartDate = TheDateSearchClass.RemoveTime(datPunchTime);
                    datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);
                    blnTransactionProcessed = false;

                    TheFindEmployeeTimeCardEntriesDataSet = TheEmployeeTimeClockEntriesClass.FindEmployeeTimeCardEntries(intEmployeeID, datStartDate, datEndDate);

                    intRecordsReturned = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries.Rows.Count - 1;

                    if (intRecordsReturned > -1)
                    {
                        for (intRecordCounter = 0; intRecordCounter <= intRecordsReturned; intRecordCounter++)
                        {
                            if (strPunchStatus == TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intRecordCounter].PunchStatus)
                            {
                                if (datPunchTime == TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intRecordCounter].PunchTime)
                                {
                                    blnTransactionProcessed = true;
                                }
                            }
                        }
                    }

                    if (blnTransactionProcessed == false)
                    {
                        blnFatalError = TheEmployeeTimeClockEntriesClass.InsertEmployeeTimeClockEntry(intEmployeeID, intPayID, datPunchTime, strPunchStatus);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                }

                PleaseWait.Close();

                TheMessagesClass.InformationMessage("The Time Card Entries have been Imported");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Process Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
