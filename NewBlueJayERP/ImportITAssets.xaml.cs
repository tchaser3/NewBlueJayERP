/* Title:           Import IT Assets
 * Date:            3-15-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import IT Assets */

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
using NewEventLogDLL;
using NewEmployeeDLL;
using ItAssetsDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportITAssets.xaml
    /// </summary>
    public partial class ImportITAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindWarehouseByWarehouseNameDataSet TheFindWarehouseByWarehouseNameDataSet = new FindWarehouseByWarehouseNameDataSet();
        FindITAssetBySerialNumberDataSet TheFindITAssetBySerialNumberDataSet = new FindITAssetBySerialNumberDataSet();
        FindITAssetsByItemIDDataSet TheFindITAssetByItemIDDataSet = new FindITAssetsByItemIDDataSet();
        ImportITAssetsDataSet TheImportITAssetsDataSet = new ImportITAssetsDataSet();

        public ImportITAssets()
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

        private void expImportExcel_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            string strItemID;
            int intItemID = 0;
            string strItem;
            string strManufacturer;
            string strModel;
            string strSerialNumber;
            string strQuantity;
            int intQuantity = 0;
            string strWarehouse;
            string strAssetNotes;
            bool blnFatalError = false;
            int intWarehouseID;

            try
            {
                TheImportITAssetsDataSet.importitassets.Rows.Clear();

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

                for (intCounter = 5; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strItemID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();

                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strItemID);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Item ID is not Numeric at Count " + Convert.ToString(intCounter));
                        return;
                    }
                    else
                    {
                        intItemID = Convert.ToInt32(strItemID);
                    }
                    strItem = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strManufacturer = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strModel = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                    strSerialNumber = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                    strQuantity = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strQuantity);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Quantity is not Numeric");
                        return;
                    }
                    else
                    {
                        intQuantity = Convert.ToInt32(strQuantity);
                    }
                    strWarehouse = Convert.ToString((range.Cells[intCounter, 7] as Excel.Range).Value2).ToUpper();
                    TheFindWarehouseByWarehouseNameDataSet = TheEmployeeClass.FindWarehouseByWarehouseName(strWarehouse);
                    intWarehouseID = TheFindWarehouseByWarehouseNameDataSet.FindWarehouseByWarehouseName[0].EmployeeID;
                    strAssetNotes = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();

                    ImportITAssetsDataSet.importitassetsRow NewItAsset = TheImportITAssetsDataSet.importitassets.NewimportitassetsRow();

                    NewItAsset.Item = strItem;
                    NewItAsset.ItemID = intItemID;
                    NewItAsset.Location = strWarehouse;
                    NewItAsset.Manufacturer = strManufacturer;
                    NewItAsset.Model = strModel;
                    NewItAsset.Notes = strAssetNotes;
                    NewItAsset.Quantity = intQuantity;
                    NewItAsset.SerialNumber = strSerialNumber;
                    NewItAsset.WarehouseID = intWarehouseID;

                    TheImportITAssetsDataSet.importitassets.Rows.Add(NewItAsset);
                }

                dgrAssets.ItemsSource = TheImportITAssetsDataSet.importitassets;

             
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import IT Assets // Import Excel  " + Ex.Message);

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
            //setting back to default
            TheFindWarehouseByWarehouseNameDataSet = TheEmployeeClass.FindWarehouseByWarehouseName("");

            TheImportITAssetsDataSet.importitassets.Rows.Clear();

            dgrAssets.ItemsSource = TheImportITAssetsDataSet.importitassets;
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intItemID;
            string strItem;
            string strManufacturer;
            string strModel;
            string strSerialNumber;
            int intQuantity;
            decimal decValue = 0;
            string strUpgrades = "NONE";
            int intWarehouseID;
            int intRecordsReturned;

            try
            {
                intNumberOfRecords = TheImportITAssetsDataSet.importitassets.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intItemID = TheImportITAssetsDataSet.importitassets[intCounter].ItemID;
                    strItem = TheImportITAssetsDataSet.importitassets[intCounter].Item;
                    strManufacturer = TheImportITAssetsDataSet.importitassets[intCounter].Manufacturer;
                    strModel = TheImportITAssetsDataSet.importitassets[intCounter].Model;
                    strSerialNumber = TheImportITAssetsDataSet.importitassets[intCounter].SerialNumber;
                    intQuantity = TheImportITAssetsDataSet.importitassets[intCounter].Quantity;
                    intWarehouseID = TheImportITAssetsDataSet.importitassets[intCounter].WarehouseID;

                    if(intItemID > 1000)
                    {
                        blnFatalError = TheITAssetsClass.UpdateITAssetLocation(intItemID, intWarehouseID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else
                    {
                        TheFindITAssetBySerialNumberDataSet = TheITAssetsClass.FindITAssetBySerialNumber(strSerialNumber);

                        intRecordsReturned = TheFindITAssetByItemIDDataSet.FindITAssetByItemID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            blnFatalError = TheITAssetsClass.UpdateITAssetLocation(intItemID, intWarehouseID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        else if(intRecordsReturned < 1)
                        {
                            blnFatalError = TheITAssetsClass.InsertITAsset(strItem, strManufacturer, strModel, strSerialNumber, intQuantity, decValue, strUpgrades, intWarehouseID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }                    
                    
                }

                TheMessagesClass.InformationMessage("The Records have been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import IT Assets // Process Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
