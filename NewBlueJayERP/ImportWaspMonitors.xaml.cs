/* Title:           Import Wasp Monitors
 * Date:            7-13-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to get the monitors ready for Wasp */

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
using AssetDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;
using ItAssetsDLL;
using Excel = Microsoft.Office.Interop.Excel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportWaspMonitors.xaml
    /// </summary>
    public partial class ImportWaspMonitors : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssetClass TheAssetClass = new AssetClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedWaspAssetLocationsBySiteDataSet TheFindSortedWaspAssetLocationsBySiteDataSet = new FindSortedWaspAssetLocationsBySiteDataSet();
        WaspAssetIDDataSet TheWaspAssetIDDataSet = new WaspAssetIDDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWaspAssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();
        MonitorsDataSet TheMonitorsDataSet = new MonitorsDataSet();
        WaspAssetForImportDataSet TheWaspAssetForImportDataSet = new WaspAssetForImportDataSet();

        string gstrSite;

        public ImportWaspMonitors()
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
            //this will load the controls
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;
                cboSelectSite.Items.Clear();
                cboSelectSite.Items.Add("Select Site");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectSite.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectSite.SelectedIndex = 0;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Wasp Monitors");

                TheMonitorsDataSet.monitors.Rows.Clear();

                dgrAssets.ItemsSource = TheMonitorsDataSet.monitors;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp Monitors // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //stting up for getting the info
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");

                intSelectedIndex = cboSelectSite.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    gstrSite = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;
                    MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                    if (gstrSite == "CBUS-GROVEPORT")
                    {
                        gstrSite = "GROVEPORT";
                    }

                    TheFindSortedWaspAssetLocationsBySiteDataSet = TheAssetClass.FindSortedAssetLocationsBySite(gstrSite);

                    intNumberOfRecords = TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectLocation.Items.Add(TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intCounter].AssetLocation);
                        }
                    }
                }

                cboSelectLocation.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp Monitors // CBO Site Select " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intAssetID;
            string strAssetDescription;
            string strSerialNumber;
            string strAssetType;
            string strBJCNumber;
            bool blnFatalError = false;
            string strErrorMessage = "";
            bool blnItemFound;
            int intTransactionID;
            string strModel;
            string strManufacturer;
            int intSelectedIndex;

            try
            {


                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intSelectedIndex = cboSelectLocation.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    TheMonitorsDataSet.monitors.Rows.Clear();

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

                    MainWindow.gstrAssetLocation = TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intSelectedIndex].AssetLocation;
                    
                    if (cboSelectLocation.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Site Has Not Been Selected\n";
                    }
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    xlDropOrder = new Excel.Application();
                    xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                    range = xlDropSheet.UsedRange;
                    intNumberOfRecords = range.Rows.Count;
                    intColumnRange = range.Columns.Count;

                    for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        TheWaspAssetIDDataSet = TheAssetClass.GetWaspAssetIDInfo();

                        intAssetID = TheWaspAssetIDDataSet.waspassetid[0].CreatedAssetID;
                        intTransactionID = TheWaspAssetIDDataSet.waspassetid[0].TransactionID;

                        blnFatalError = TheAssetClass.UpdateWaspAssetID(intTransactionID, intAssetID + 1);

                        blnItemFound = false;
                        strAssetType = "UNKNOWN";

                        if (((range.Cells[intCounter, 1] as Excel.Range).Value2) == null)
                        {
                            intCounter = intNumberOfRecords;
                            break;
                        }
                        else
                        {
                            strAssetDescription = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                            strManufacturer = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                            strModel = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                            strSerialNumber = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                            strBJCNumber = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                            strAssetType = Convert.ToString((range.Cells[intCounter, 7] as Excel.Range).Value2).ToUpper();


                            TheFindWaspAssetByBJCAssetIDDataSet = TheAssetClass.FindWaspAssetByBJCAssetID(strBJCNumber);

                            intRecordsReturned = TheFindWaspAssetByBJCAssetIDDataSet.FindWaspAssetByBJCAssetID.Rows.Count;

                            if(intRecordsReturned > 0)
                            {
                                blnItemFound = true;
                            }
                            
                        }

                        if (blnItemFound == false)
                        {
                            MonitorsDataSet.monitorsRow NewMonitorRow = TheMonitorsDataSet.monitors.NewmonitorsRow();

                            NewMonitorRow.AssetID = intAssetID;
                            NewMonitorRow.AssetType = strAssetType;
                            NewMonitorRow.BJCAssetID = strBJCNumber;
                            NewMonitorRow.Item = strAssetDescription;
                            NewMonitorRow.Location = MainWindow.gstrAssetLocation;
                            NewMonitorRow.Manufacturer = strManufacturer;
                            NewMonitorRow.Model = strModel;
                            NewMonitorRow.SerialNumber = strSerialNumber;
                            NewMonitorRow.Site = gstrSite;
                            NewMonitorRow.WarehouseID = MainWindow.gintWarehouseID;

                            TheMonitorsDataSet.monitors.Rows.Add(NewMonitorRow);
                        }
                    }

                    
                }

                PleaseWait.Close();

                dgrAssets.ItemsSource = TheMonitorsDataSet.monitors;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Vehicle Assets // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strItemDescription;
            string strManufacturer;
            string strModel;
            string strSerialNumber;
            int intWarehouse;
            string strBJCAssetID;
            int intAssetID;
            string strAssetCategory;
            string strAssetDescription;
            bool blnFatalError;

            try
            {
                expProcess.IsExpanded = false;
                intNumberOfRecords = TheMonitorsDataSet.monitors.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strItemDescription = TheMonitorsDataSet.monitors[intCounter].Item;
                        strManufacturer = TheMonitorsDataSet.monitors[intCounter].Manufacturer;
                        strModel = TheMonitorsDataSet.monitors[intCounter].Model;
                        strSerialNumber = TheMonitorsDataSet.monitors[intCounter].SerialNumber;
                        intWarehouse = TheMonitorsDataSet.monitors[intCounter].WarehouseID;
                        strBJCAssetID = TheMonitorsDataSet.monitors[intCounter].BJCAssetID;
                        intAssetID = TheMonitorsDataSet.monitors[intCounter].AssetID;
                        strAssetDescription = strItemDescription + " NO. " + strBJCAssetID;
                        strAssetCategory = TheMonitorsDataSet.monitors[intCounter].AssetType;

                        blnFatalError = TheITAssetsClass.InsertITAsset(strItemDescription, strManufacturer, strModel, strSerialNumber, 1, 0, "NO", intWarehouse);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheAssetClass.InsertWaspAssets(intAssetID, strAssetDescription, strBJCAssetID, strAssetCategory, gstrSite, MainWindow.gstrAssetLocation, intWarehouse, DateTime.Now, strSerialNumber, strManufacturer, strModel);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    CreateWaspAssetSheet();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp Monitors // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void CreateWaspAssetSheet()
        {
            int intCounter;
            int intNumberOfRecords;
            string strAssetDecription;
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
                TheWaspAssetForImportDataSet.waspassetforimport.Rows.Clear();

                intNumberOfRecords = TheMonitorsDataSet.monitors.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strAssetDecription = TheMonitorsDataSet.monitors[intCounter].Item + " ID: ";
                        strAssetDecription += TheMonitorsDataSet.monitors[intCounter].BJCAssetID;

                        WaspAssetForImportDataSet.waspassetforimportRow NewAssetRow = TheWaspAssetForImportDataSet.waspassetforimport.NewwaspassetforimportRow();

                        NewAssetRow.AssetDescription = strAssetDecription;
                        NewAssetRow.AssetID = TheMonitorsDataSet.monitors[intCounter].AssetID;
                        NewAssetRow.AssetType = TheMonitorsDataSet.monitors[intCounter].AssetType;
                        NewAssetRow.Location = MainWindow.gstrAssetLocation;
                        NewAssetRow.Manufacturer = TheMonitorsDataSet.monitors[intCounter].Manufacturer;
                        NewAssetRow.Model = TheMonitorsDataSet.monitors[intCounter].Model;
                        NewAssetRow.SerialNumber = TheMonitorsDataSet.monitors[intCounter].BJCAssetID;
                        NewAssetRow.Site = gstrSite;

                        TheWaspAssetForImportDataSet.waspassetforimport.Rows.Add(NewAssetRow);
                    }

                    worksheet = workbook.ActiveSheet;

                    worksheet.Name = "OpenOrders";

                    int cellRowIndex = 1;
                    int cellColumnIndex = 1;
                    intRowNumberOfRecords = TheWaspAssetForImportDataSet.waspassetforimport.Rows.Count;
                    intColumnNumberOfRecords = TheWaspAssetForImportDataSet.waspassetforimport.Columns.Count;

                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWaspAssetForImportDataSet.waspassetforimport.Columns[intColumnCounter].ColumnName;

                        cellColumnIndex++;
                    }

                    cellRowIndex++;
                    cellColumnIndex = 1;

                    //Loop through each row and read value from each column. 
                    for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                    {
                        for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWaspAssetForImportDataSet.waspassetforimport.Rows[intRowCounter][intColumnCounter].ToString();

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
                    TheMessagesClass.InformationMessage("Export Successful");

                    excel.Quit();
                }

                dgrAssets.ItemsSource = TheWaspAssetForImportDataSet.waspassetforimport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp Monitors // Create Wasp Asset Sheet " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
