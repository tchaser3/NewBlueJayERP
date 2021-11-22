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
using DataValidationDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for InventoryValuationReport.xaml
    /// </summary>
    public partial class InventoryValuationReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        InventoryValuationDataSet TheInventoryValuationDataSet = new InventoryValuationDataSet();
        WarehouseInventoryValuationDataSet TheWarehouseInventoryValuationDataSet = new WarehouseInventoryValuationDataSet();

        //setting up modular variables
        int gintInventoryNumberOfRecords;
        int gintWarehouseNumberOfRecords;

        public InventoryValuationReport()
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
            TheInventoryValuationDataSet.inventoryvaluation.Rows.Clear();
            TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Rows.Clear();

            dgrInventory.ItemsSource = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation;
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
            string strValueForValidation;
            int intQuantity;
            string strItemNumber;
            string strItemDescription;
            decimal decCost;
            decimal decTotalCost;
            bool blnItemFound;
            string strSite = "";
            int intInventoryCounter;
            string strNewQuantity;
            int intCommaIndex;
            int intWarehouseCounter;

            try
            {
                expImportExcel.IsExpanded = false;
                TheInventoryValuationDataSet.inventoryvaluation.Rows.Clear();
                TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Rows.Clear();

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
                gintInventoryNumberOfRecords = 0;

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strItemNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);
                    blnItemFound = false;

                    if(strItemNumber == "Site")
                    {
                        strSite = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2);
                    }
                    else if(strItemNumber == null)
                    {

                    }
                    else if(strSite.Contains("BLUE JAY") == true)
                    {
                        if(strItemNumber.Contains("Location") == false)
                        {
                            if(strItemNumber != "Item Number")
                            {
                                strItemDescription = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);
                                strValueForValidation = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2);
                                if(strValueForValidation.Contains(",") == true)
                                {
                                    intCommaIndex = strValueForValidation.IndexOf(",");

                                    strNewQuantity = strValueForValidation.Substring(0, intCommaIndex);
                                    strNewQuantity += strValueForValidation.Substring(intCommaIndex + 1);

                                    strValueForValidation = strNewQuantity;

                                }
                                intQuantity = Convert.ToInt32(strValueForValidation);
                                strValueForValidation = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2);
                                decCost = Convert.ToDecimal(strValueForValidation);
                                strValueForValidation = Convert.ToString((range.Cells[intCounter, 12] as Excel.Range).Value2);
                                decTotalCost = Convert.ToDecimal(strValueForValidation);

                                if(gintInventoryNumberOfRecords > 0)
                                {
                                    for(intInventoryCounter = 0; intInventoryCounter < gintInventoryNumberOfRecords; intInventoryCounter++)
                                    {
                                        if(strSite == TheInventoryValuationDataSet.inventoryvaluation[intInventoryCounter].Warehouse)
                                        {
                                            if(strItemNumber == TheInventoryValuationDataSet.inventoryvaluation[intInventoryCounter].ItemNumber)
                                            {
                                                TheInventoryValuationDataSet.inventoryvaluation[intInventoryCounter].Quantity += intQuantity;
                                                TheInventoryValuationDataSet.inventoryvaluation[intInventoryCounter].TotalCost += decTotalCost;
                                                blnItemFound = true;
                                            }
                                        }
                                    }
                                }

                                if(blnItemFound == false)
                                {
                                    InventoryValuationDataSet.inventoryvaluationRow NewInventoryRow = TheInventoryValuationDataSet.inventoryvaluation.NewinventoryvaluationRow();

                                    NewInventoryRow.Cost = decCost;
                                    NewInventoryRow.ItemDescription = strItemDescription;
                                    NewInventoryRow.ItemNumber = strItemNumber;
                                    NewInventoryRow.Quantity = intQuantity;
                                    NewInventoryRow.TotalCost = decTotalCost;
                                    NewInventoryRow.Warehouse = strSite;

                                    TheInventoryValuationDataSet.inventoryvaluation.Rows.Add(NewInventoryRow);
                                    gintInventoryNumberOfRecords++;  
                                }
                            }
                        }
                    }
                }

                intNumberOfRecords = TheInventoryValuationDataSet.inventoryvaluation.Rows.Count;
                gintWarehouseNumberOfRecords = 0;

                if(intNumberOfRecords > 0)
                {
                   for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                   {
                        blnItemFound = false;
                        strSite = TheInventoryValuationDataSet.inventoryvaluation[intCounter].Warehouse;
                        decTotalCost = TheInventoryValuationDataSet.inventoryvaluation[intCounter].TotalCost;

                        if(gintWarehouseNumberOfRecords > 0)
                        {
                            for(intWarehouseCounter = 0; intWarehouseCounter < gintWarehouseNumberOfRecords; intWarehouseCounter++)
                            {
                                if(strSite == TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation[intWarehouseCounter].Warehouse)
                                {
                                    blnItemFound=true;
                                    TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation[intWarehouseCounter].TotalValuation += decTotalCost;
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            WarehouseInventoryValuationDataSet.warehouseinventoryvaluationRow NewWarehouseRow = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.NewwarehouseinventoryvaluationRow();

                            NewWarehouseRow.TotalValuation = decTotalCost;
                            NewWarehouseRow.Warehouse = strSite;

                            TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Rows.Add(NewWarehouseRow);
                            gintWarehouseNumberOfRecords++;
                        }
                   }
                }

                dgrInventory.ItemsSource = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation;
                PleaseWait.Close();
               
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Prepare Asset Report // Import Excel Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            expExportToExcel.IsExpanded = false;
            ExportInventory();
            ExportWarehouse();
        }
        private void ExportWarehouse()
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
                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Rows.Count;
                intColumnNumberOfRecords = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWarehouseInventoryValuationDataSet.warehouseinventoryvaluation.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Inventory Valuation Report // Export Warehouse " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportInventory()
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
                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheInventoryValuationDataSet.inventoryvaluation.Rows.Count;
                intColumnNumberOfRecords = TheInventoryValuationDataSet.inventoryvaluation.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheInventoryValuationDataSet.inventoryvaluation.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheInventoryValuationDataSet.inventoryvaluation.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Inventory Valuation Report // Export Inventory " + ex.Message);

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
