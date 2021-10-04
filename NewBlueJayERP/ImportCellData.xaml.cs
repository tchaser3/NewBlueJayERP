/* Title:           Import Cell Data
 * Date:            9-28-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import data */

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
    /// Interaction logic for ImportCellData.xaml
    /// </summary>
    public partial class ImportCellData : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        ImportCellPhoneDataDataSet TheImportCellPhoneDataDataSet = new ImportCellPhoneDataDataSet();
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastFourDataSet = new FindCellPhoneByLastFourDataSet();
        FindCellPhoneDataValidationDataSet TheFindCellPhoneDataValidationDataSet = new FindCellPhoneDataValidationDataSet();

        public ImportCellData()
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
            TheImportCellPhoneDataDataSet.importcellphonedata.Rows.Clear();

            dgrCellData.ItemsSource = TheImportCellPhoneDataDataSet.importcellphonedata;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Import Cell Data ");
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
            string strGigaBytesUsed;
            decimal decGigaBytesUsed;
            int intRecordsReturned;
            double douDate;

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportCellPhoneDataDataSet.importcellphonedata.Rows.Clear();

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
                    strGigaBytesUsed = Convert.ToString((range.Cells[intCounter, 11] as Excel.Range).Value2).ToUpper();

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

                        decGigaBytesUsed = Convert.ToDecimal(strGigaBytesUsed);

                        TheFindCellPhoneDataValidationDataSet = TheCellPhoneCallsClass.FindCellPhoneDataValidation(intPhoneID, intEmployeeID, datTransactionDate, decGigaBytesUsed);

                        intRecordsReturned = TheFindCellPhoneDataValidationDataSet.FindCellPhoneDataValidation.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            ImportCellPhoneDataDataSet.importcellphonedataRow NewDataRow = TheImportCellPhoneDataDataSet.importcellphonedata.NewimportcellphonedataRow();

                            NewDataRow.EmployeeID = intEmployeeID;
                            NewDataRow.FirstName = strFirstName;
                            NewDataRow.GigaByteUsed = decGigaBytesUsed;
                            NewDataRow.LastName = strLastName;
                            NewDataRow.PhoneID = intPhoneID;
                            NewDataRow.PhoneNumber = strCellNumber;
                            NewDataRow.TransactionDate = datTransactionDate;

                            TheImportCellPhoneDataDataSet.importcellphonedata.Rows.Add(NewDataRow);
                        }
                    }
                }

                dgrCellData.ItemsSource = TheImportCellPhoneDataDataSet.importcellphonedata;
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Cell Data // Import Excel  " + Ex.Message);

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
            decimal decGigaBytesUsed;
            bool blnFatalError = false;

            try
            {
                expProcessImport.IsExpanded = false;

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intNumberOfRecords = TheImportCellPhoneDataDataSet.importcellphonedata.Rows.Count;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intPhoneID = TheImportCellPhoneDataDataSet.importcellphonedata[intCounter].PhoneID;
                        intEmployeeID = TheImportCellPhoneDataDataSet.importcellphonedata[intCounter].EmployeeID;
                        datTransactionDate = TheImportCellPhoneDataDataSet.importcellphonedata[intCounter].TransactionDate;
                        decGigaBytesUsed = TheImportCellPhoneDataDataSet.importcellphonedata[intCounter].GigaByteUsed;

                        blnFatalError = TheCellPhoneCallsClass.InsertCellPhoneData(intPhoneID, intEmployeeID, datTransactionDate, decGigaBytesUsed);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Cell Data // Process Import " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
