/* Title:           Import Phone Class
 * Date:            9-15-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for Importing Phone Calls */

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
using EmployeeDateEntryDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportPhoneCalls.xaml
    /// </summary>
    public partial class ImportPhoneCalls : Window
    {
        //setting up the classes
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        ImportedPhoneCallsDataSet TheImportedPhoneCallsDataSet = new ImportedPhoneCallsDataSet();
        FindPhoneForImportDataSet TheFindPhoneForImportDataSet = new FindPhoneForImportDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindPhoneCallsForVerificationDataSet TheFindPhoneCallsForVerificationDataSet = new FindPhoneCallsForVerificationDataSet();

        public ImportPhoneCalls()
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
            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Phone Calls");

            TheImportedPhoneCallsDataSet.importedphonecalls.Rows.Clear();

            dgrPhoneCalls.ItemsSource = TheImportedPhoneCallsDataSet.importedphonecalls;
        }

        private void expImportCalls_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strCallType;
            string strExtension;
            int intExtension;
            string strDialedDigits;
            string strStartTime;
            DateTime datStartTime;
            string strCallDuration;            
            int intPhoneID;
            int intEmployeeID;
            string strFirstName;
            string strLastName;
            int intRecordsReturned;
            double douDate;
            bool blnIsNotNumeric;
            double douTime;

            try
            {
                expImportCalls.IsExpanded = false;
                TheImportedPhoneCallsDataSet.importedphonecalls.Rows.Clear();

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

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strCallType = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strExtension = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strDialedDigits = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                    strStartTime = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                    strCallDuration = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();

                    blnIsNotNumeric = TheDataValidationClass.VerifyIntegerData(strExtension);

                    if (blnIsNotNumeric == false)
                    {
                        intExtension = Convert.ToInt32(strExtension);

                        TheFindPhoneForImportDataSet = ThePhonesClass.FindPhoneForImport(intExtension);

                        intRecordsReturned = TheFindPhoneForImportDataSet.FindPhoneForImport.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            intPhoneID = TheFindPhoneForImportDataSet.FindPhoneForImport[0].TransactionID;
                            intEmployeeID = TheFindPhoneForImportDataSet.FindPhoneForImport[0].EmployeeID;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                            strFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                            strLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                            douDate = Convert.ToDouble(strStartTime);

                            datStartTime = DateTime.FromOADate(douDate);

                            douTime = Convert.ToDouble(strCallDuration);

                            TimeSpan tspInterval = TimeSpan.FromDays(douTime);

                            strCallDuration = tspInterval.ToString();

                            ImportedPhoneCallsDataSet.importedphonecallsRow NewCallRow = TheImportedPhoneCallsDataSet.importedphonecalls.NewimportedphonecallsRow();

                            NewCallRow.CallDuration = strCallDuration;
                            NewCallRow.CallType = strCallType;
                            NewCallRow.DialedDigits = strDialedDigits;
                            NewCallRow.PhoneExtension = intExtension;
                            NewCallRow.EmployeeID = intEmployeeID;
                            NewCallRow.FirstName = strFirstName;
                            NewCallRow.LastName = strLastName;
                            NewCallRow.PhoneID = intPhoneID;
                            NewCallRow.StartTime = datStartTime;

                            TheImportedPhoneCallsDataSet.importedphonecalls.Rows.Add(NewCallRow);
                        }                       
                        
                    }
                }

                dgrPhoneCalls.ItemsSource = TheImportedPhoneCallsDataSet.importedphonecalls;
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Phone Calls // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        static string GenTimeSpanFromMinutes(double minutes)
        {
            // Create a TimeSpan object and TimeSpan string from 
            // a number of minutes.
            TimeSpan interval = TimeSpan.FromMinutes(minutes);
            string timeInterval = interval.ToString();

            // Pad the end of the TimeSpan string with spaces if it 
            // does not contain milliseconds.
            int pIndex = timeInterval.IndexOf(':');
            pIndex = timeInterval.IndexOf('.', pIndex);
            if (pIndex < 0) timeInterval += "        ";

            return timeInterval;
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strCallType;
            int intPhoneID;
            int intEmployeeID;
            string strDialedDigits;
            DateTime datStartTime;
            string strCallDuration;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                expProcessImport.IsExpanded = false;
                intNumberOfRecords = TheImportedPhoneCallsDataSet.importedphonecalls.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strCallType = TheImportedPhoneCallsDataSet.importedphonecalls[intCounter].CallType;
                        intPhoneID = TheImportedPhoneCallsDataSet.importedphonecalls[intCounter].PhoneID;
                        intEmployeeID = TheImportedPhoneCallsDataSet.importedphonecalls[intCounter].EmployeeID;
                        strDialedDigits = TheImportedPhoneCallsDataSet.importedphonecalls[intCounter].DialedDigits;
                        datStartTime = TheImportedPhoneCallsDataSet.importedphonecalls[intCounter].StartTime;
                        strCallDuration = TheImportedPhoneCallsDataSet.importedphonecalls[intCounter].CallDuration;

                        TheFindPhoneCallsForVerificationDataSet = TheCellPhoneCallsClass.FindPhoneCallsForVerification(strDialedDigits, datStartTime, strCallDuration);

                        intRecordsReturned = TheFindPhoneCallsForVerificationDataSet.FindPhoneCallsForVerification.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            blnFatalError = TheCellPhoneCallsClass.InsertPhoneCalls(strCallType, intPhoneID, intEmployeeID, strDialedDigits, datStartTime, strCallDuration);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        
                    }
                }

                TheMessagesClass.InformationMessage("Phone Numbers Have Been Imported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Phone Calls // Process Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
