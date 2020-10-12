/* Title:           Warehouse Inventory Report
 * DAte:            10-9-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create reports for Inventory */

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
using InventoryDLL;
using Excell = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for WarehouseInventoryReport.xaml
    /// </summary>
    public partial class WarehouseInventoryReport : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        InventoryClass TheInventoryClass = new InventoryClass();

        //setting up the data
        FindWarehouseInventoryDataSet TheFindWarehouseInventoryDataSet = new FindWarehouseInventoryDataSet();
        FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        FindMasterPartListPartByPartIDDataSet TheFindMasterPartListPartByPartIDDataSet = new FindMasterPartListPartByPartIDDataSet();
        WarehouseCountDataSet TheWarehouseCountDataSet = new WarehouseCountDataSet();
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();

        public WarehouseInventoryReport()
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
            int intNumberOfRecords;

            try
            {
                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count;

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");
                TheWarehouseCountDataSet.warehousecount.Rows.Clear();
                dgrResult.ItemsSource = TheWarehouseCountDataSet.warehousecount;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Warehouse Inventory Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;
            int intPartID;
            string strPartNumber;
            string strOldPartNumber = "";
            string strJDEPartNumber;
            string strPartDescription;
            int intQuantity;
            int intSelectedIndex;
            int intRecordsReturned;
            string strCurrentCount = "";
            decimal decPartCost;
            decimal decTotalCost;

            try
            {
                intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;
                TheWarehouseCountDataSet.warehousecount.Rows.Clear();
                
                if(intSelectedIndex > -1)
                {
                    intWarehouseID = TheFindPartsWarehouseDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                    TheFindWarehouseInventoryDataSet = TheInventoryClass.FindWarehouseInventory(intWarehouseID);

                    intNumberOfRecords = TheFindWarehouseInventoryDataSet.FindWarehouseInventory.Rows.Count;

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intPartID = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].PartID;
                        strPartNumber = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].PartNumber;
                        strJDEPartNumber = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].JDEPartNumber;
                        strPartDescription = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].PartDescription;
                        intQuantity = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].Quantity;

                        TheFindPartByPartIDDataSet = ThePartNumberClass.FindPartByPartID(intPartID);

                        decPartCost = Convert.ToDecimal(TheFindPartByPartIDDataSet.FindPartByPartID[0].Price);

                        decPartCost = Math.Round(decPartCost, 2);

                        decTotalCost = decPartCost * intQuantity;

                        decTotalCost = Math.Round(decTotalCost, 2);

                        TheFindMasterPartListPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                        intRecordsReturned = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            strOldPartNumber = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                        }
                        else if (intRecordsReturned < 1)
                        {
                            strOldPartNumber = "NO PART FOUND";
                        }
                        
                        WarehouseCountDataSet.warehousecountRow NewPartRow = TheWarehouseCountDataSet.warehousecount.NewwarehousecountRow();

                        NewPartRow.Quantity = intQuantity;
                        NewPartRow.JDEPartNumber = strJDEPartNumber;
                        NewPartRow.OldPartNumber = strOldPartNumber;
                        NewPartRow.PartDescription = strPartDescription;
                        NewPartRow.PartID = intPartID;
                        NewPartRow.PartNumber = strPartNumber;
                        NewPartRow.CurrentQuantity = strCurrentCount;
                        NewPartRow.PartCost = decPartCost;
                        NewPartRow.TotalValue = decTotalCost;

                        TheWarehouseCountDataSet.warehousecount.Rows.Add(NewPartRow);
                    }

                    dgrResult.ItemsSource = TheWarehouseCountDataSet.warehousecount;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Warehouse Inventory Report // Select Warehouse Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
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
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheWarehouseCountDataSet.warehousecount.Rows.Count;
                intColumnNumberOfRecords = TheWarehouseCountDataSet.warehousecount.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWarehouseCountDataSet.warehousecount.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWarehouseCountDataSet.warehousecount.Rows[intRowCounter][intColumnCounter].ToString();

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
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Warehouse Inventory Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
