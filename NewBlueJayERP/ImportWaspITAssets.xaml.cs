/* Title:           Import Wasp IT Assets
 * Date:            5-21-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for importing the IT Assets to record the Asset ID */

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
using AssetDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportWaspITAssets.xaml
    /// </summary>
    public partial class ImportWaspITAssets : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        AssetClass TheAssetClass = new AssetClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        FindWaspAssetByAssetIDDataSet TheFindWASPAssetByAssetIDDataSet = new FindWaspAssetByAssetIDDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWASPAssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();
        ImportWASPITAssetsDataSet TheImportWASPITAssetsDataSet = new ImportWASPITAssetsDataSet();
        FindWaspAssetsBySerialNumberDataSet TheFindWaspAssetsBySerialNumberDataSet = new FindWaspAssetsBySerialNumberDataSet();
        FindWarehouseByWarehouseNameDataSet TheFindWarehouseByWarehouseNameDataSet = new FindWarehouseByWarehouseNameDataSet();

        public ImportWaspITAssets()
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
            TheImportWASPITAssetsDataSet.importassets.Rows.Clear();

            dgrITAssets.ItemsSource = TheImportWASPITAssetsDataSet.importassets;
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
            int intAssetID;
            string strAssetID;
            string strAssetDescription;
            string strAssetType;
            string strSite;
            string strLocation;
            string strSerialNumber;
            string strManufacturer;
            string strModel;
            int intRecordsReturned;
            bool blnItemFound;
            int intWarehouseID;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportWASPITAssetsDataSet.importassets.Rows.Clear();

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

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnItemFound = false;
                    strAssetID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    intAssetID = Convert.ToInt32(strAssetID);
                    strAssetDescription = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strAssetType = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strSite = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                    strLocation = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                    strSerialNumber = Convert.ToString((range.Cells[intCounter, 13] as Excel.Range).Value2).ToUpper();
                    strManufacturer = Convert.ToString((range.Cells[intCounter, 15] as Excel.Range).Value2).ToUpper();
                    strModel = Convert.ToString((range.Cells[intCounter, 16] as Excel.Range).Value2).ToUpper();

                    if(strSite == "GROVEPORT")
                    {
                        strSite = "CBUS-GROVEPORT";
                    }

                    TheFindWarehouseByWarehouseNameDataSet = TheEmployeeClass.FindWarehouseByWarehouseName(strSite);

                    intWarehouseID = TheFindWarehouseByWarehouseNameDataSet.FindWarehouseByWarehouseName[0].EmployeeID;

                    TheFindWASPAssetByAssetIDDataSet = TheAssetClass.FindWaspAssetByAssetID(intAssetID);

                    intRecordsReturned = TheFindWASPAssetByAssetIDDataSet.FindWaspAssetByAssetID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnItemFound = true;
                    }
                    else if(intRecordsReturned < 1)
                    {
                        TheFindWaspAssetsBySerialNumberDataSet = TheAssetClass.FindWaspAssetsBySerialNumber(strSerialNumber);

                        intRecordsReturned = TheFindWaspAssetsBySerialNumberDataSet.FindWaspAssetsBySerialNumber.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            blnItemFound = true;
                        }
                    }

                    if(blnItemFound == false)
                    {
                        ImportWASPITAssetsDataSet.importassetsRow NewAssetRow = TheImportWASPITAssetsDataSet.importassets.NewimportassetsRow();

                        NewAssetRow.AssetDescription = strAssetDescription;
                        NewAssetRow.AssetID = intAssetID;
                        NewAssetRow.AssetType = strAssetType;
                        NewAssetRow.Location = strLocation;
                        NewAssetRow.Manufacturer = strManufacturer;
                        NewAssetRow.Model = strModel;
                        NewAssetRow.SerialNumber = strSerialNumber;
                        NewAssetRow.Site = strSite;
                        NewAssetRow.WarehouseID = intWarehouseID;

                        TheImportWASPITAssetsDataSet.importassets.Rows.Add(NewAssetRow);
                    }

                }

                dgrITAssets.ItemsSource = TheImportWASPITAssetsDataSet.importassets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp IT Assets // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intAssetID;
            string strAssetDescription;
            string strAssetType;
            string strSite;
            string strLocation;
            string strSerialNumber;
            string strManufacturer;
            string strModel;
            int intWarehouseID;
            DateTime datTransactionDate;
            bool blnFatalError = false;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheImportWASPITAssetsDataSet.importassets.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intAssetID = TheImportWASPITAssetsDataSet.importassets[intCounter].AssetID;
                    strAssetType = TheImportWASPITAssetsDataSet.importassets[intCounter].AssetType;
                    strAssetDescription = TheImportWASPITAssetsDataSet.importassets[intCounter].AssetDescription;
                    strSite = TheImportWASPITAssetsDataSet.importassets[intCounter].Site;
                    strLocation = TheImportWASPITAssetsDataSet.importassets[intCounter].Location;
                    strSerialNumber = TheImportWASPITAssetsDataSet.importassets[intCounter].SerialNumber;
                    strManufacturer = TheImportWASPITAssetsDataSet.importassets[intCounter].Manufacturer;
                    strModel = TheImportWASPITAssetsDataSet.importassets[intCounter].Model;
                    datTransactionDate = DateTime.Now;
                    intWarehouseID = TheImportWASPITAssetsDataSet.importassets[intCounter].WarehouseID;

                    blnFatalError = TheAssetClass.InsertWaspAssets(intAssetID, strAssetDescription, strSerialNumber, strAssetType, strSite, strLocation, intWarehouseID, datTransactionDate, strSerialNumber, strManufacturer, strModel);

                    if (blnFatalError == true)
                        throw new Exception();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp IT Assets // Proces Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
    }
}
