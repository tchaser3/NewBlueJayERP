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
using InventoryImportDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using EmployeeDateEntryDLL;
using InventoryDLL;
using NewPartNumbersDLL;
using MaterialSheetsDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportInventory.xaml
    /// </summary>
    public partial class ImportInventory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InventoryImportClass TheInventoryImportClass = new InventoryImportClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        MaterialSheetClass TheMaterialSheetsClass = new MaterialSheetClass();

        //setting up the data
        FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        ImportInventoryDataSet TheImportInventoryDataSet = new ImportInventoryDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();
        FindInventoryLocationByPartIDDataSet TheFindInventoryLocationByPartIDDataSet = new FindInventoryLocationByPartIDDataSet();

        public ImportInventory()
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
            int intCounter;
            int intNumberOfRecord;
            bool blnFatalError = false;

            try
            {
                TheImportInventoryDataSet.importinventory.Rows.Clear();

                dgrInventory.ItemsSource = TheImportInventoryDataSet.importinventory;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import INventory ");

                if (blnFatalError == true)
                    throw new Exception();

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecord = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecord; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Inventory // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;
            string strLocation;
            int intPartID;
            string strPartNumber;
            string strJDEPartNumber;
            string strOldPartNumber;
            string strPartDescription;
            int intOldQuantity;
            int intNewQuantity;
            int intVariance;

            try
            {
                intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

                if(intSelectedIndex > 0)
                {
                    MainWindow.gintWarehouseID = TheFindPartsWarehouseDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                    TheImportInventoryDataSet.importinventory.Rows.Clear();

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
                        strLocation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                        intPartID = Convert.ToInt32((range.Cells[intCounter, 2] as Excel.Range).Value2);
                        strPartNumber = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                        strJDEPartNumber = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                        strOldPartNumber = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                        strPartDescription = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                        intOldQuantity = Convert.ToInt32((range.Cells[intCounter, 7] as Excel.Range).Value2);
                        intNewQuantity = Convert.ToInt32((range.Cells[intCounter, 8] as Excel.Range).Value2);

                        intVariance = intNewQuantity - intOldQuantity;

                        ImportInventoryDataSet.importinventoryRow NewPartRow = TheImportInventoryDataSet.importinventory.NewimportinventoryRow();

                        NewPartRow.CurrentCount = intNewQuantity;
                        NewPartRow.JDEPartNumber = strJDEPartNumber;
                        NewPartRow.OldCount = intOldQuantity;
                        NewPartRow.OldPartNumber = strOldPartNumber;
                        NewPartRow.PartDescription = strPartDescription;
                        NewPartRow.PartID = intPartID;
                        NewPartRow.PartNumber = strPartNumber;
                        NewPartRow.Variance = intVariance;
                        NewPartRow.Location = strLocation;

                        TheImportInventoryDataSet.importinventory.Rows.Add(NewPartRow);

                    }

                    dgrInventory.ItemsSource = TheImportInventoryDataSet.importinventory;

                    PleaseWait.Close();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Tow Motors // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expUpdateCounts_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intPartID;
            int intOldCount;
            int intNewCount;
            string strLocation;
            string strPartNumber;
            string strPartDescription;
            int intRecordsReturned;
            string strJDEPartNumber;
            int intTransactionID;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            bool blnItemFound;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate;

            try
            {
                blnFatalError = TheInventoryClass.ClearWarehouseInventory(MainWindow.gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                blnFatalError = TheInventoryImportClass.RemoveInventoryImport(MainWindow.gintWarehouseID, datStartDate, datEndDate);

                if (blnFatalError == true)
                    throw new Exception();

                intNumberOfRecords = TheImportInventoryDataSet.importinventory.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intPartID = TheImportInventoryDataSet.importinventory[intCounter].PartID;
                    intOldCount = TheImportInventoryDataSet.importinventory[intCounter].OldCount;
                    intNewCount = TheImportInventoryDataSet.importinventory[intCounter].CurrentCount;
                    strLocation = TheImportInventoryDataSet.importinventory[intCounter].Location;
                    strPartNumber = TheImportInventoryDataSet.importinventory[intCounter].PartNumber;
                    strPartDescription = TheImportInventoryDataSet.importinventory[intCounter].PartDescription;
                    strJDEPartNumber = TheImportInventoryDataSet.importinventory[intCounter].JDEPartNumber;

                    if(intPartID < 1)
                    {
                        TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                        intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                        }
                        else if(intRecordsReturned < 1)
                        {
                            intPartID = intCounter * -1;

                            blnFatalError = ThePartNumberClass.InsertPartIntoPartNumbers(intPartID, strPartNumber, strJDEPartNumber, strPartDescription, 0);

                            if (blnFatalError == true)
                                throw new Exception();

                            TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                            intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                        }
                    }
                    else if(intPartID > 0)
                    {
                        TheFindPartByPartIDDataSet = ThePartNumberClass.FindPartByPartID(intPartID);

                        intRecordsReturned = TheFindPartByPartIDDataSet.FindPartByPartID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            if(strPartNumber != TheFindPartByPartIDDataSet.FindPartByPartID[0].PartNumber)
                            {
                                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                                if(intRecordsReturned > 0)
                                {
                                    intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                                }
                                else if(intRecordsReturned < 1)
                                {
                                    intPartID = intCounter * -1;

                                    blnFatalError = ThePartNumberClass.InsertPartIntoPartNumbers(intPartID, strPartNumber, strJDEPartNumber, strPartDescription, 0);

                                    if (blnFatalError == true)
                                        throw new Exception();

                                    TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                                    intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                                }
                            }

                        }
                        else if(intRecordsReturned < 1)
                        {
                            TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                            intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                            if (intRecordsReturned > 0)
                            {
                                intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                            }
                            else if (intRecordsReturned < 1)
                            {
                                intPartID = intCounter * -1;

                                blnFatalError = ThePartNumberClass.InsertPartIntoPartNumbers(intPartID, strPartNumber, strJDEPartNumber, strPartDescription, 0);

                                if (blnFatalError == true)
                                    throw new Exception();

                                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                                intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                            }
                        }
                    }

                    TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, MainWindow.gintWarehouseID);

                    intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = TheInventoryClass.InsertInventoryPart(intPartID, intNewCount, MainWindow.gintWarehouseID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else if(intRecordsReturned > 0)
                    {
                        intTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;

                        blnFatalError = TheInventoryClass.UpdateInventoryPart(intTransactionID, intNewCount);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    //inserting inventory location
                    TheFindInventoryLocationByPartIDDataSet = TheMaterialSheetsClass.FindInventoryLocationByPartID(intPartID, MainWindow.gintWarehouseID);

                    intSecondNumberOfRecords = TheFindInventoryLocationByPartIDDataSet.FindInventoryLocationByPartID.Rows.Count;
                    blnItemFound = false;

                    if(intSecondNumberOfRecords > 0)
                    {
                        for(intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                        {
                            if(strLocation == TheFindInventoryLocationByPartIDDataSet.FindInventoryLocationByPartID[intSecondCounter].PartLocation)
                            {
                                blnItemFound = true;
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        blnFatalError = TheMaterialSheetsClass.InsertInventoryLocation(intPartID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now, strLocation, MainWindow.gintWarehouseID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    //adding to inventory sheet
                    blnFatalError = TheInventoryImportClass.InsertInventoryImport(MainWindow.gintWarehouseID, intPartID, strLocation, intOldCount, intNewCount, DateTime.Now);

                    if (blnFatalError == true)
                        throw new Exception();

                }

                TheMessagesClass.InformationMessage("All Parts Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Inventory // Update Counts Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
