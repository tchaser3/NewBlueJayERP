/* Title:           Import Vendors
 * Date:            6-6-20
 * Author:          Terry Holmes
 * 
 * Description:     This is for the importing of Vendors */

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
using VendorsDLL;
using NewEventLogDLL;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportVendors.xaml
    /// </summary>
    public partial class ImportVendors : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindVendorByVendorNameDataSet TheFindVendorByVendorNameDataSet = new FindVendorByVendorNameDataSet();
        ImportVendorsDataSet TheImportVendorsDataSet = new ImportVendorsDataSet();

        public ImportVendors()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            TheImportVendorsDataSet.importvendors.Rows.Clear();

            dgrVendors.ItemsSource = TheImportVendorsDataSet.importvendors;

            expProcessImport.IsEnabled = false;
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
            string strVendorName;
            string strPhoneNumber;
            string strContactName;
            int intRecordsReturned;

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportVendorsDataSet.importvendors.Rows.Clear();

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
                    strVendorName = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strPhoneNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strContactName = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();

                    TheFindVendorByVendorNameDataSet = TheVendorsClass.FindVendorByVendorName(strVendorName);

                    intRecordsReturned = TheFindVendorByVendorNameDataSet.FindVendorByVendorName.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        ImportVendorsDataSet.importvendorsRow NewVendorRow = TheImportVendorsDataSet.importvendors.NewimportvendorsRow();

                        NewVendorRow.ContactName = strContactName;
                        NewVendorRow.PhoneNumber = strPhoneNumber;
                        NewVendorRow.VendorName = strVendorName;

                        TheImportVendorsDataSet.importvendors.Rows.Add(NewVendorRow);
                    }
                }

                dgrVendors.ItemsSource = TheImportVendorsDataSet.importvendors;
                expProcessImport.IsEnabled = true;

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Vendors // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            string strVendorName;
            string strContactName;
            string strPhoneNumber;

            try
            {
                intNumberOfRecords = TheImportVendorsDataSet.importvendors.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strVendorName = TheImportVendorsDataSet.importvendors[intCounter].VendorName;
                        strContactName = TheImportVendorsDataSet.importvendors[intCounter].ContactName;
                        strPhoneNumber = TheImportVendorsDataSet.importvendors[intCounter].PhoneNumber;

                        blnFatalError = TheVendorsClass.InsertNewVendor(strVendorName, strContactName, strPhoneNumber);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    TheMessagesClass.InformationMessage("The Vendors Have Been Imported");

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Vendor // Process Import " + Ex.Message);
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
