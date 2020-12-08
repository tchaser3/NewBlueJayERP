/* Title:           Import Prices
 * Date:            11-24-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import the prices */

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
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportPrices.xaml
    /// </summary>
    public partial class ImportPrices : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        PartPriceImportDataSet ThePartPriceImportDataSet = new PartPriceImportDataSet();

        public ImportPrices()
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
            ThePartPriceImportDataSet.partpriceimport.Rows.Clear();

            dgrParts.ItemsSource = ThePartPriceImportDataSet.partpriceimport;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Prices");
        }

        private void expImportParts_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strPartID;
            int intPartID;
            string strPartNumber;
            string strJDEPartNumber;
            string strPartDescription;
            string strPartPrice;
            decimal decPartPrice;

            try
            {
                ThePartPriceImportDataSet.partpriceimport.Rows.Clear();

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
                    strPartID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    intPartID = Convert.ToInt32(strPartID);
                    strPartNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strJDEPartNumber = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strPartDescription = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                    strPartPrice = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                    decPartPrice = Convert.ToDecimal(strPartPrice);

                    decPartPrice = Math.Round(decPartPrice, 2);

                    PartPriceImportDataSet.partpriceimportRow NewPartRow = ThePartPriceImportDataSet.partpriceimport.NewpartpriceimportRow();

                    NewPartRow.PartID = intPartID;
                    NewPartRow.PartNumber = strPartNumber;
                    NewPartRow.JDEPartNumber = strJDEPartNumber;
                    NewPartRow.PartDescription = strPartDescription;
                    NewPartRow.PartPrice = decPartPrice;

                    ThePartPriceImportDataSet.partpriceimport.Rows.Add(NewPartRow);
                }

                dgrParts.ItemsSource = ThePartPriceImportDataSet.partpriceimport;

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Prices // Import Parts  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expUpdatePrices_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intPartID;
            decimal decPartPrice;
            bool blnFatalError = false;
            double douPartPrice;

            try
            {
                intNumberOfRecords = ThePartPriceImportDataSet.partpriceimport.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intPartID = ThePartPriceImportDataSet.partpriceimport[intCounter].PartID;
                        decPartPrice = ThePartPriceImportDataSet.partpriceimport[intCounter].PartPrice;
                        douPartPrice = Convert.ToDouble(decPartPrice);

                        blnFatalError = ThePartNumberClass.UpdatePricePrice(intPartID, douPartPrice);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Prices have been Updated");
            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Prices // Update Prices Expander " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }
    }
}
