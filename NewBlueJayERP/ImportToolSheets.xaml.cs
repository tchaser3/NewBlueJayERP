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
using Excel = Microsoft.Office.Interop.Excel;
using NewToolsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportToolSheets.xaml
    /// </summary>
    public partial class ImportToolSheets : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssetClass TheAssetClass = new AssetClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ToolsClass TheToolClass = new ToolsClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedWaspAssetLocationsBySiteDataSet TheFindSortedWaspAssetLocationsBySiteDataSet = new FindSortedWaspAssetLocationsBySiteDataSet();
        FindWaspAssetsByLocationDataSet TheFindWaspAssetsByLocationDataSet = new FindWaspAssetsByLocationDataSet();
        EmployeeToolAssetDataSet TheEmployeeToolAssetDataSet = new EmployeeToolAssetDataSet();
        WaspAssetIDDataSet TheWaspAssetIDDataSet = new WaspAssetIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindWaspAssetsBySerialNumberDataSet TheFindWaspAssetBySerialNumberDataSet = new FindWaspAssetsBySerialNumberDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWaspAssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();

        public static string gstrSite;
        public static string gstrBJCAssetID;
        public static string gstrToolDescription;
        public static string gstrSerialNumber;
        public static string gstrToolCategory;
        public static string gstrLastName;
        public static string gstrLocation;

        public ImportToolSheets()
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
                cboSelectLocation.SelectedIndex = 0;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;
                cboSelectSite.Items.Clear();
                cboSelectSite.Items.Add("Select Site");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectSite.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectSite.SelectedIndex = 0;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Select Wasp Assets");

                TheEmployeeToolAssetDataSet.employeetoolassets.Rows.Clear();

                dgrAssets.ItemsSource = TheEmployeeToolAssetDataSet.employeetoolassets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Tool Sheets // Reset Controls " + Ex.Message);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Tool Sheets // CBO Site Select " + Ex.Message);

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
            string strFirstName;
            string strLastName;
            bool blnFatalError = false;
            bool blnItemFound;
            int intTransactionID;
            int intSelectedIndex;
            int intEmployeeID;

            try
            {

                intSelectedIndex = cboSelectLocation.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    TheEmployeeToolAssetDataSet.employeetoolassets.Rows.Clear();

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


                    gstrLocation = TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intSelectedIndex].AssetLocation;

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
                            strAssetDescription = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();

                            if (((range.Cells[intCounter, 2] as Excel.Range).Value2) == null)
                            {
                                strBJCNumber = " ";
                            }
                            else
                            {
                                strBJCNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                            }
                            if (((range.Cells[intCounter, 5] as Excel.Range).Value2) == null)
                            {
                                strSerialNumber = " ";
                            }
                            else
                            {
                                strSerialNumber = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                            }
                            if (((range.Cells[intCounter, 4] as Excel.Range).Value2) == null)
                            {
                                strAssetType = " ";
                            }
                            else
                            {
                                strAssetType = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                            }

                            strFirstName = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();
                            strLastName = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();

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
                                TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(strBJCNumber);

                                intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    strAssetType = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCategory;
                                    strAssetDescription = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                                }
                            }
                            
                        }

                        TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                        EmployeeToolAssetDataSet.employeetoolassetsRow NewToolRow = TheEmployeeToolAssetDataSet.employeetoolassets.NewemployeetoolassetsRow();

                        NewToolRow.AssetID = intAssetID;
                        NewToolRow.BJCAssetID = strBJCNumber;
                        NewToolRow.EmployeeID = TheComboEmployeeDataSet.employees[0].EmployeeID;
                        NewToolRow.FirstName = strFirstName;
                        NewToolRow.LastName = strLastName;
                        NewToolRow.Office = gstrSite;
                        NewToolRow.ToolLocation = gstrLocation;
                        NewToolRow.ToolCategory = strAssetType;
                        NewToolRow.ToolDescription = strAssetDescription;

                        TheEmployeeToolAssetDataSet.employeetoolassets.Rows.Add(NewToolRow);
                    }

                    xlDropOrder.Quit();
                    PleaseWait.Close();
                    
                }        
                
                
                dgrAssets.ItemsSource = TheEmployeeToolAssetDataSet.employeetoolassets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Vehicle Assets // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrAssets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell AssetID;
            DataGridCell BJCAssetID;
            DataGridCell ToolDescription;
            DataGridCell ToolCategory;
            DataGridCell SerialNumber;
            DataGridCell LastName;
            string strAssetID;

            try
            {
                if (dgrAssets.SelectedIndex > -1)
                {

                    //setting local variable
                    dataGrid = dgrAssets;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    AssetID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    BJCAssetID = (DataGridCell)dataGrid.Columns[1].GetCellContent(selectedRow).Parent;
                    ToolDescription = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    ToolCategory = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    SerialNumber = (DataGridCell)dataGrid.Columns[4].GetCellContent(selectedRow).Parent;
                    LastName = (DataGridCell)dataGrid.Columns[9].GetCellContent(selectedRow).Parent;
                    strAssetID = ((TextBlock)AssetID.Content).Text;
                    gstrBJCAssetID = ((TextBlock)BJCAssetID.Content).Text;
                    gstrToolDescription = ((TextBlock)ToolDescription.Content).Text;
                    gstrToolCategory = ((TextBlock)ToolCategory.Content).Text;
                    gstrSerialNumber = ((TextBlock)SerialNumber.Content).Text;
                    gstrLastName = ((TextBlock)LastName.Content).Text;

                    //find the record
                    MainWindow.gintAssetID = Convert.ToInt32(strAssetID);

                    AssignToolAsset AssignToolAsset = new AssignToolAsset();
                    AssignToolAsset.ShowDialog();

                    intNumberOfRecords = TheEmployeeToolAssetDataSet.employeetoolassets.Rows.Count;

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(MainWindow.gintAssetID == TheEmployeeToolAssetDataSet.employeetoolassets[intCounter].AssetID)
                        {
                            TheEmployeeToolAssetDataSet.employeetoolassets[intCounter].BJCAssetID = gstrBJCAssetID;
                            TheEmployeeToolAssetDataSet.employeetoolassets[intCounter].ToolDescription = gstrToolDescription;
                            TheEmployeeToolAssetDataSet.employeetoolassets[intCounter].ToolCategory = gstrToolCategory;
                            TheEmployeeToolAssetDataSet.employeetoolassets[intCounter].SerialNumber = gstrSerialNumber;
                        }
                    }

                    dgrAssets.ItemsSource = TheEmployeeToolAssetDataSet.employeetoolassets;
                    
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Wasp Asset // Asset Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;

            ResetControls();
        }
    }
}
