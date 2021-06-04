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
using NewToolsDLL;
using ToolHistoryDLL;
using ToolCategoryDLL;
using VehicleMainDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportVehicleAssets.xaml
    /// </summary>
    public partial class ImportVehicleAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssetClass TheAssetClass = new AssetClass();
        ToolsClass TheToolClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //Setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindWaspAssetByAssetIDDataSet TheFindWaspAssetByAssetIDDataSet = new FindWaspAssetByAssetIDDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWaspAssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();
        FindWaspAssetsBySerialNumberDataSet TheFindWaspAssetBySerialNumberDataSet = new FindWaspAssetsBySerialNumberDataSet();
        FindWaspAssetLocationByLocationDataSet TheFindWaspAssetLocationByLocationDataSet = new FindWaspAssetLocationByLocationDataSet();
        ImportWaspToolAssetsDataSet TheImportWaspToolAssetsDataSet = new ImportWaspToolAssetsDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        WaspAssetIDDataSet TheWaspAssetIDDataSet = new WaspAssetIDDataSet();

        string gstrSite;

        public ImportVehicleAssets()
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
            //setting up the variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtEnterLocation.Text = "";
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Site");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectLocation.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectLocation.SelectedIndex = 0;

                TheImportWaspToolAssetsDataSet.importassets.Rows.Clear();

                dgrAssets.ItemsSource = TheImportWaspToolAssetsDataSet.importassets;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Vehicle Assets");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Vehicle Assets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectLocation.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                gstrSite = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;

                if(gstrSite == "CBUS-GROVEPORT")
                {
                    gstrSite = "GROVEPORT";
                }
            }
        }

        private void btnImportExcel_Click(object sender, RoutedEventArgs e)
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
            string strLocation;
            string strSerialNumber;
            string strAssetType;
            string strBJCNumber;
            bool blnFatalError = false;
            string strErrorMessage = "";
            bool blnItemFound;
            int intTransactionID;
            string strModel;
            string strManufacturer;

            try
            {
                TheImportWaspToolAssetsDataSet.importassets.Rows.Clear();

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

                strLocation = txtEnterLocation.Text;
                if(strLocation.Length < 2)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Location is not long Enough\n";
                }
                else
                {
                    TheFindWaspAssetLocationByLocationDataSet = TheAssetClass.FindWaspAssetLocationByLocation(strLocation);

                    intRecordsReturned = TheFindWaspAssetLocationByLocationDataSet.FindWaspAssetLocationByLocation.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "There is no Such Location\n";
                    }
                }
                if(cboSelectLocation.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Site Has Not Been Selected\n";
                }
                if(blnFatalError == true)
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

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    TheWaspAssetIDDataSet = TheAssetClass.GetWaspAssetIDInfo();

                    intAssetID = TheWaspAssetIDDataSet.waspassetid[0].CreatedAssetID;
                    intTransactionID = TheWaspAssetIDDataSet.waspassetid[0].TransactionID;

                    blnFatalError = TheAssetClass.UpdateWaspAssetID(intTransactionID, intAssetID + 1);

                    blnItemFound = false;
                    strAssetType = "UNKNOWN";

                    if (((range.Cells[intCounter, 2] as Excel.Range).Value2) == null)
                    {
                        intCounter = intNumberOfRecords;
                        break;
                    }
                    else
                    {
                        strAssetDescription = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();

                        if (((range.Cells[intCounter, 3] as Excel.Range).Value2) == null)
                        {
                            strSerialNumber = " ";
                        }
                        else
                        {
                            strSerialNumber = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                        }
                        if (((range.Cells[intCounter, 4] as Excel.Range).Value2) == null)
                        {
                            strBJCNumber = " ";
                        }
                        else
                        {
                            strBJCNumber = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                        }

                        if(strSerialNumber.Length > 2)
                        {
                            TheFindWaspAssetBySerialNumberDataSet = TheAssetClass.FindWaspAssetsBySerialNumber(strSerialNumber);

                            intRecordsReturned = TheFindWaspAssetBySerialNumberDataSet.FindWaspAssetsBySerialNumber.Rows.Count;

                            if(intRecordsReturned > 0)
                            {
                                blnItemFound = true;
                            }
                        }
                        if(strBJCNumber.Length > 2)
                        {
                            TheFindWaspAssetByBJCAssetIDDataSet = TheAssetClass.FindWaspAssetByBJCAssetID(strBJCNumber);

                            intRecordsReturned = TheFindWaspAssetByBJCAssetIDDataSet.FindWaspAssetByBJCAssetID.Rows.Count;

                            if (intRecordsReturned > 0)
                            {
                                blnItemFound = true;
                            }
                        }
                    }
                    
                    if(blnItemFound == false)
                    {
                        if(strBJCNumber.Length > 2)
                        {
                            TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(strBJCNumber);

                            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                            if (intRecordsReturned > 0)
                            {
                                strAssetType = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCategory;
                                strAssetDescription = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                            }                            
                        }

                        ImportWaspToolAssetsDataSet.importassetsRow NewAssetRow = TheImportWaspToolAssetsDataSet.importassets.NewimportassetsRow();

                        NewAssetRow.AssetDescription = strAssetDescription;
                        NewAssetRow.AssetID = intAssetID;
                        NewAssetRow.AssetType = strAssetType;
                        NewAssetRow.Location = strLocation;
                        NewAssetRow.Manufacturer = "UNKNOWN";
                        NewAssetRow.Model = "UNKNOWN";
                        NewAssetRow.SerialNumber = strSerialNumber;
                        NewAssetRow.Site = gstrSite;
                        NewAssetRow.WarehouseID = MainWindow.gintWarehouseID;
                        NewAssetRow.BJCAssetID = strBJCNumber;

                        TheImportWaspToolAssetsDataSet.importassets.Rows.Add(NewAssetRow);
                    }
                }

                intNumberOfRecords = TheImportWaspToolAssetsDataSet.importassets.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strAssetDescription = TheImportWaspToolAssetsDataSet.importassets[intCounter].AssetDescription;
                        strAssetType = TheImportWaspToolAssetsDataSet.importassets[intCounter].AssetType;
                        intAssetID = TheImportWaspToolAssetsDataSet.importassets[intCounter].AssetID;
                        strBJCNumber = TheImportWaspToolAssetsDataSet.importassets[intCounter].BJCAssetID;
                        strLocation = TheImportWaspToolAssetsDataSet.importassets[intCounter].Location;
                        strManufacturer = TheImportWaspToolAssetsDataSet.importassets[intCounter].Manufacturer;
                        strModel = TheImportWaspToolAssetsDataSet.importassets[intCounter].Model;
                        strSerialNumber = TheImportWaspToolAssetsDataSet.importassets[intCounter].SerialNumber;

                        blnFatalError = TheAssetClass.InsertWaspAssets(intAssetID, strAssetDescription, strBJCNumber, strAssetType, gstrSite, strLocation, MainWindow.gintWarehouseID, DateTime.Now, strSerialNumber, strManufacturer, strModel);

                        if (blnFatalError == true)
                            throw new Exception();                        
                    }
                }

                PleaseWait.Close();

                dgrAssets.ItemsSource = TheImportWaspToolAssetsDataSet.importassets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Vehicle Assets // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
