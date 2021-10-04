/* Title:           Import Cell Messaging
 * Date:            9-29-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import messages information */

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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using DataValidationDLL;
using NewEventLogDLL;
using CellPhoneCallsDLL;
using PhonesDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportCellMessaging.xaml
    /// </summary>
    public partial class ImportCellMessaging : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastFourDataSet = new FindCellPhoneByLastFourDataSet();
        FindCellPhoneMessagesForValidationDataSet TheFindCellPhoneMessagesForValidationDataSet = new FindCellPhoneMessagesForValidationDataSet();
        ImportCellMessagesDataSet TheImportCellMessagesDataSet = new ImportCellMessagesDataSet();

        public ImportCellMessaging()
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
            TheImportCellMessagesDataSet.importcellmessages.Rows.Clear();

            dgrCellMessages.ItemsSource = TheImportCellMessagesDataSet.importcellmessages;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Cell Messages");
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
            string strTransactionDate;
            DateTime datTransactionDate;
            string strTransactionNumber;
            string strMessageDirection;
            string strMessageType;
            int intRecordsReturned;
            double douDate;

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportCellMessagesDataSet.importcellmessages.Rows.Clear();

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
                    strCellNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strTransactionDate = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                    strTransactionNumber = Convert.ToString((range.Cells[intCounter, 7] as Excel.Range).Value2).ToUpper();
                    strMessageDirection = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();
                    strMessageType = Convert.ToString((range.Cells[intCounter, 10] as Excel.Range).Value2).ToUpper();

                    strLastFour = strCellNumber.Substring(8, 4);

                    if (strLastFour != "5546")
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

                        datTransactionDate = DateTime.FromOADate(douDate);

                        TheFindCellPhoneMessagesForValidationDataSet = TheCellPhoneCallsClass.FindCellPhoneMessagesForValidation(intPhoneID, intEmployeeID, datTransactionDate, strTransactionNumber, strMessageDirection, strMessageType);

                        intRecordsReturned = TheFindCellPhoneMessagesForValidationDataSet.FindCellPhoneMessagesForValidation.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            ImportCellMessagesDataSet.importcellmessagesRow NewMessageRow = TheImportCellMessagesDataSet.importcellmessages.NewimportcellmessagesRow();

                            NewMessageRow.EmployeeID = intEmployeeID;
                            NewMessageRow.FirstName = strFirstName;
                            NewMessageRow.LastName = strLastName;
                            NewMessageRow.MessageDirection = strMessageDirection;
                            NewMessageRow.MessageType = strMessageType;
                            NewMessageRow.PhoneID = intPhoneID;
                            NewMessageRow.PhoneNumber = strCellNumber;
                            NewMessageRow.TransactionDate = datTransactionDate;
                            NewMessageRow.TransactionNumber = strTransactionNumber;
                            
                            TheImportCellMessagesDataSet.importcellmessages.Rows.Add(NewMessageRow);
                        }
                    }
                }

                dgrCellMessages.ItemsSource = TheImportCellMessagesDataSet.importcellmessages;
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Cell Message // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intPhoneID;
            int intEmployeeID;
            DateTime datTransactionDate;
            string strTransactionNumber;
            string strMessageDirection;
            string strMessageType;
            bool blnFatalError = false;

            try
            {
                expProcessImport.IsExpanded = false;

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intNumberOfRecords = TheImportCellMessagesDataSet.importcellmessages.Rows.Count;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intPhoneID = TheImportCellMessagesDataSet.importcellmessages[intCounter].PhoneID;
                        intEmployeeID = TheImportCellMessagesDataSet.importcellmessages[intCounter].EmployeeID;
                        datTransactionDate = TheImportCellMessagesDataSet.importcellmessages[intCounter].TransactionDate;
                        strTransactionNumber = TheImportCellMessagesDataSet.importcellmessages[intCounter].TransactionNumber;
                        strMessageDirection = TheImportCellMessagesDataSet.importcellmessages[intCounter].MessageDirection;
                        strMessageType = TheImportCellMessagesDataSet.importcellmessages[intCounter].MessageType;

                        blnFatalError = TheCellPhoneCallsClass.InsertCellPhoneMessages(intPhoneID, intEmployeeID, datTransactionDate, strTransactionNumber, strMessageDirection, strMessageType);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                PleaseWait.Close();

                TheMessagesClass.InformationMessage("Data Has Been Imported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Cell Messages // Process Import " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
