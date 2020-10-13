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
using NewPartNumbersDLL;
using MaterialSheetsDLL;
using EmployeeDateEntryDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportInventoryLocations.xaml
    /// </summary>
    public partial class ImportInventoryLocations : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        MaterialSheetClass TheMaterialSheetClass = new MaterialSheetClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data sets
        FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindInventoryLocationByLocationDataSet TheFindInventoryLocationByLocationDataSet = new FindInventoryLocationByLocationDataSet();
        ImportInventoryLocationsDataSet TheImportInventoryLocationdDataSet = new ImportInventoryLocationsDataSet();
        FindMasterPartListPartByPartIDDataSet TheFindMasterPartByPartIDDataSet = new FindMasterPartListPartByPartIDDataSet();
        FindPartFromMasterPartListByPartNumberDataSet TheFindPartFromMasterPartListByPartNumberDataSet = new FindPartFromMasterPartListByPartNumberDataSet();
        FindPartFromMasterPartListByJDEPartNumberDataSet TheFindPartFromMasterPartListByJDEPartNumberDataSet = new FindPartFromMasterPartListByJDEPartNumberDataSet();
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();

        public ImportInventoryLocations()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                cboSelectWarehouse.Items.Add("Select Warehouse");

                MainWindow.gintEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP // Import Inventory Locations");

                if (blnFatalError == true)
                    throw new Exception();

                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;

                TheImportInventoryLocationdDataSet.importinventorylocations.Rows.Clear();

                dgrImportedInformation.ItemsSource = TheImportInventoryLocationdDataSet.importinventorylocations;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Inventory Locations // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindPartsWarehouseDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;
            }
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
            string strPartNumber;
            string strPartLocation;
            string strJDEPartNumber;
            int intPartID;
            string strOldPartNumber;
            string strPartDescription;
            int intRecordsReturned;

            try
            {
                TheImportInventoryLocationdDataSet.importinventorylocations.Rows.Clear();

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

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strPartNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strPartLocation = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();

                    TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                        strJDEPartNumber = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].JDEPartNumber;
                        strPartDescription = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;

                        TheFindMasterPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                        intRecordsReturned = TheFindMasterPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                        strOldPartNumber = "NONE FOUND";

                        if(intRecordsReturned > 0)
                        {
                            strOldPartNumber = TheFindMasterPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                        }

                        ImportInventoryLocationsDataSet.importinventorylocationsRow NewPartRow = TheImportInventoryLocationdDataSet.importinventorylocations.NewimportinventorylocationsRow();

                        NewPartRow.JDEPartNumber = strJDEPartNumber;
                        NewPartRow.Location = strPartLocation;
                        NewPartRow.OldPartNumber = strOldPartNumber;
                        NewPartRow.PartDescription = strPartDescription;
                        NewPartRow.PartID = intPartID;
                        NewPartRow.PartNumber = strPartNumber;
                        NewPartRow.ToBeImported = true;

                        TheImportInventoryLocationdDataSet.importinventorylocations.Rows.Add(NewPartRow);
                    }
                    else if(intRecordsReturned < 1)
                    {
                        strJDEPartNumber = strPartNumber;

                        TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strJDEPartNumber);

                        intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            intPartID = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID;
                            strPartNumber = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartNumber;
                            strPartDescription = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription;

                            TheFindMasterPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                            intRecordsReturned = TheFindMasterPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                            strOldPartNumber = "NONE FOUND";

                            if (intRecordsReturned > 0)
                            {
                                strOldPartNumber = TheFindMasterPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                            }

                            ImportInventoryLocationsDataSet.importinventorylocationsRow NewPartRow = TheImportInventoryLocationdDataSet.importinventorylocations.NewimportinventorylocationsRow();

                            NewPartRow.JDEPartNumber = strJDEPartNumber;
                            NewPartRow.Location = strPartLocation;
                            NewPartRow.OldPartNumber = strOldPartNumber;
                            NewPartRow.PartDescription = strPartDescription;
                            NewPartRow.PartID = intPartID;
                            NewPartRow.PartNumber = strPartNumber;
                            NewPartRow.ToBeImported = true;

                            TheImportInventoryLocationdDataSet.importinventorylocations.Rows.Add(NewPartRow);
                        }
                    }
                }

                PleaseWait.Close();

                dgrImportedInformation.ItemsSource = TheImportInventoryLocationdDataSet.importinventorylocations;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Inventory Locations // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intPartID;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            string strLocation;

            try
            {
                intNumberOfRecords = TheImportInventoryLocationdDataSet.importinventorylocations.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intPartID = TheImportInventoryLocationdDataSet.importinventorylocations[intCounter].PartID;
                    strLocation = TheImportInventoryLocationdDataSet.importinventorylocations[intCounter].Location;

                    blnFatalError = TheMaterialSheetClass.InsertInventoryLocation(intPartID, MainWindow.gintEmployeeID, datTransactionDate, strLocation, MainWindow.gintWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("All Tools Have Been Imported");

                TheImportInventoryLocationdDataSet.importinventorylocations.Rows.Clear();

                dgrImportedInformation.ItemsSource = TheImportInventoryLocationdDataSet.importinventorylocations;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Inventory Locations // Process Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
