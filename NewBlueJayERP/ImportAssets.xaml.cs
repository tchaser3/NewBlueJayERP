/* Title:           Import Assets
 * Date:            7-6-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import the assets */

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
using DataValidationDLL;
using AssetDLL;
using NewToolsDLL;
using Excel = Microsoft.Office.Interop.Excel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportAssets.xaml
    /// </summary>
    public partial class ImportAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        AssetClass TheAssetClass = new AssetClass();
        ToolsClass TheToolsClass = new ToolsClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindWaspAssetLocationBySiteDataSet TheFindWaspAssetLocationBySiteDataSet = new FindWaspAssetLocationBySiteDataSet(); 
        FindWaspAssetByAssetIDDataSet TheFindWaspAssetByAssetIDDataSet = new FindWaspAssetByAssetIDDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWaspAssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();
        FindWaspAssetsBySerialNumberDataSet TheFindWaspAssetBySerialNumberDataSet = new FindWaspAssetsBySerialNumberDataSet();
        FindWaspAssetLocationByLocationDataSet TheFindWaspAssetLocationByLocationDataSet = new FindWaspAssetLocationByLocationDataSet();
        ImportWaspToolAssetsDataSet TheImportWaspToolAssetsDataSet = new ImportWaspToolAssetsDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        WaspAssetIDDataSet TheWaspAssetIDDataSet = new WaspAssetIDDataSet();

        string gstrSite;

        public ImportAssets()
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Site");
                cboSelectAssetLocation.Items.Clear();
                cboSelectAssetLocation.Items.Add("Select Location");

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectLocation.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectLocation.SelectedIndex = 0;
                cboSelectAssetLocation.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Assets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }


        private void cboSelectLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;

            try
            {
                cboSelectAssetLocation.Items.Clear();
                cboSelectAssetLocation.Items.Add("Select Location");

                intSelectedIndex = cboSelectLocation.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gstrSite = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;

                    if(gstrSite == "CBUS-GROVEPORT")
                    {
                        gstrSite = "GROVEPORT";
                    }

                    TheFindWaspAssetLocationBySiteDataSet = TheAssetClass.FindWaspAssetLocationBySite(gstrSite);

                    intNumberOfRecords = TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectAssetLocation.Items.Add(TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite[intCounter].AssetLocation);
                        }
                    }

                    cboSelectAssetLocation.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Assets // Site Combo Box Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectAssetLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

                intSelectedIndex = cboSelectAssetLocation.SelectedIndex - 1;

                if(intSelectedIndex > -1)
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

                    MainWindow.gstrAssetLocation = TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite[intSelectedIndex].AssetLocation;
                    if (MainWindow.gstrAssetLocation.Length < 2)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Location is not long Enough\n";
                    }
                    else
                    {
                        TheFindWaspAssetLocationByLocationDataSet = TheAssetClass.FindWaspAssetLocationByLocation(MainWindow.gstrAssetLocation);

                        intRecordsReturned = TheFindWaspAssetLocationByLocationDataSet.FindWaspAssetLocationByLocation.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "There is no Such Location\n";
                        }
                    }
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

                            if (strSerialNumber.Length > 2)
                            {
                                TheFindWaspAssetBySerialNumberDataSet = TheAssetClass.FindWaspAssetsBySerialNumber(strSerialNumber);

                                intRecordsReturned = TheFindWaspAssetBySerialNumberDataSet.FindWaspAssetsBySerialNumber.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    blnItemFound = true;
                                }
                            }
                            if (strBJCNumber.Length > 2)
                            {
                                TheFindWaspAssetByBJCAssetIDDataSet = TheAssetClass.FindWaspAssetByBJCAssetID(strBJCNumber);

                                intRecordsReturned = TheFindWaspAssetByBJCAssetIDDataSet.FindWaspAssetByBJCAssetID.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    blnItemFound = true;
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            if (strBJCNumber.Length > 2)
                            {
                                TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strBJCNumber);

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
                            NewAssetRow.Location = MainWindow.gstrAssetLocation;
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

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            strAssetDescription = TheImportWaspToolAssetsDataSet.importassets[intCounter].AssetDescription;
                            strAssetType = TheImportWaspToolAssetsDataSet.importassets[intCounter].AssetType;
                            intAssetID = TheImportWaspToolAssetsDataSet.importassets[intCounter].AssetID;
                            strBJCNumber = TheImportWaspToolAssetsDataSet.importassets[intCounter].BJCAssetID;
                            strManufacturer = TheImportWaspToolAssetsDataSet.importassets[intCounter].Manufacturer;
                            strModel = TheImportWaspToolAssetsDataSet.importassets[intCounter].Model;
                            strSerialNumber = TheImportWaspToolAssetsDataSet.importassets[intCounter].SerialNumber;

                            blnFatalError = TheAssetClass.InsertWaspAssets(intAssetID, strAssetDescription, strBJCNumber, strAssetType, gstrSite, MainWindow.gstrAssetLocation, MainWindow.gintWarehouseID, DateTime.Now, strSerialNumber, strManufacturer, strModel);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
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
