/* Title:           Prepare Asset Report
 * Date:            11-17-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to prepare the asset report */

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
    /// Interaction logic for PrepareAssetReport.xaml
    /// </summary>
    public partial class PrepareAssetReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        AssetsForReportDataSet TheAssetsForReportingDataSet = new AssetsForReportDataSet();
        AssetCategoryCostDataSet TheAssetCategoryCostDataSet = new AssetCategoryCostDataSet();

        int gintAssetNumberOfRecords;

        public PrepareAssetReport()
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
            TheAssetsForReportingDataSet.assetsforreport.Rows.Clear();
            TheAssetCategoryCostDataSet.assetcategorycost.Rows.Clear();

            dgrAssets.ItemsSource = TheAssetCategoryCostDataSet.assetcategorycost;
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
            int intAssetTag;
            string strAssetDescription;
            string strSerialNumber;
            string strCategoryName;
            decimal decAssetCost = 0;
            bool blnNotNumeric;
            int intQuantity = 0;
            int intAssetCounter;
            bool blnItemFound;
            string strLocation = "";
            int intCharacterIndex;

            try
            {
                expImportExcel.IsExpanded = false;
                TheAssetsForReportingDataSet.assetsforreport.Rows.Clear();
                TheAssetCategoryCostDataSet.assetcategorycost.Rows.Clear();

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
                gintAssetNumberOfRecords = 0;

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);

                    if(strValueForValidation != null)
                    {
                        blnNotNumeric = TheDataValidationClass.VerifyIntegerData(strValueForValidation);

                        if(blnNotNumeric == false)
                        {
                            blnItemFound = false;
                            intAssetTag = Convert.ToInt32(strValueForValidation);
                            strAssetDescription = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);
                            strSerialNumber = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2);
                            if(strSerialNumber == null)
                            {
                                strSerialNumber = "";
                            }
                            strCategoryName = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2);
                            if(strCategoryName == null)
                            {
                                strCategoryName = "UNKNOWN";
                            }
                            strValueForValidation = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2);
                            blnNotNumeric = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                            if(blnNotNumeric == false)
                            {
                                decAssetCost = Convert.ToDecimal(strValueForValidation);
                            }
                            else
                            {
                                decAssetCost = 0;
                            }
                            strValueForValidation = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2);
                            blnNotNumeric = TheDataValidationClass.VerifyIntegerRange(strValueForValidation);
                            if (blnNotNumeric == false)
                            {
                                intQuantity = Convert.ToInt32(strValueForValidation);
                            }
                            else
                            {
                                intQuantity = 0;
                            }

                            AssetsForReportDataSet.assetsforreportRow NewAssetRow = TheAssetsForReportingDataSet.assetsforreport.NewassetsforreportRow();

                            NewAssetRow.AssetCategory = strCategoryName;
                            NewAssetRow.AssetCost = decAssetCost;
                            NewAssetRow.AssetDecription = strAssetDescription;
                            NewAssetRow.AssetTag = intAssetTag;
                            NewAssetRow.SerialNumber = strSerialNumber;
                            NewAssetRow.Quantity = intQuantity;
                            NewAssetRow.Location = strLocation;

                            TheAssetsForReportingDataSet.assetsforreport.Rows.Add(NewAssetRow);

                            if(gintAssetNumberOfRecords > 0)
                            {
                                for(intAssetCounter = 0; intAssetCounter < gintAssetNumberOfRecords; intAssetCounter++)
                                {
                                    if(strCategoryName == TheAssetCategoryCostDataSet.assetcategorycost[intAssetCounter].AssetCategory)
                                    {
                                        TheAssetCategoryCostDataSet.assetcategorycost[intAssetCounter].CategoryCosts += decAssetCost;
                                        blnItemFound = true;
                                    }
                                }
                            }

                            if(blnItemFound == false)
                            {
                                AssetCategoryCostDataSet.assetcategorycostRow NewCategoryCost = TheAssetCategoryCostDataSet.assetcategorycost.NewassetcategorycostRow();

                                NewCategoryCost.AssetCategory = strCategoryName;
                                NewCategoryCost.CategoryCosts = decAssetCost;

                                TheAssetCategoryCostDataSet.assetcategorycost.Rows.Add(NewCategoryCost);
                                gintAssetNumberOfRecords++;
                            }

                        }
                        else if(strValueForValidation.Contains("Location"))
                        {
                            if(strValueForValidation.Contains("Location Prefix") == false)
                            {
                                strLocation = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2);

                                if (strLocation != null)
                                {
                                    intCharacterIndex = strLocation.IndexOf(':');

                                    strLocation = strLocation.Substring(0, intCharacterIndex);
                                }
                            }

                        }
                    }
                }

                PleaseWait.Close();
                dgrAssets.ItemsSource = TheAssetsForReportingDataSet.assetsforreport;                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Prepare Asset Report // Import Excel Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void expExportReport_Expanded(object sender, RoutedEventArgs e)
        {
            expExportReport.IsExpanded = false;

            ExportAssetList();

            ExportCategoryList();
        }
        private void ExportCategoryList()
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
                intRowNumberOfRecords = TheAssetCategoryCostDataSet.assetcategorycost.Rows.Count;
                intColumnNumberOfRecords = TheAssetCategoryCostDataSet.assetcategorycost.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheAssetCategoryCostDataSet.assetcategorycost.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheAssetCategoryCostDataSet.assetcategorycost.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Prepare Asset Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportAssetList()
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
                intRowNumberOfRecords = TheAssetsForReportingDataSet.assetsforreport.Rows.Count;
                intColumnNumberOfRecords = TheAssetsForReportingDataSet.assetsforreport.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheAssetsForReportingDataSet.assetsforreport.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheAssetsForReportingDataSet.assetsforreport.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Prepare Asset Report // Export To Excel " + ex.Message);

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
