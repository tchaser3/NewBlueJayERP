/* Title:           Create Spectrum Report
 * Date:            10-7-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for Creating the report */

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
using NewPartNumbersDLL;
using InventoryDLL;
using NewEmployeeDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateSpectrumReport.xaml
    /// </summary>
    public partial class CreateSpectrumReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNUmberDataSet = new FindPartByJDEPartNumberDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryByPartDataSet = new FindWarehouseInventoryPartDataSet();
        FindPartsWarehousesDataSet TheFindPartsWarehousesDataSet = new FindPartsWarehousesDataSet();
        SpectrumCountDataSet TheSpectrumCountDataSet = new SpectrumCountDataSet();
        FindMasterPartListPartByPartIDDataSet TheFindMasterPartByPartIDDataSet = new FindMasterPartListPartByPartIDDataSet();

        //global variables
        int gintCounter;
        int gintNumberOfRecords;

        public CreateSpectrumReport()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            TheSpectrumCountDataSet.spectrumcount.Rows.Clear();

            dgrResult.ItemsSource = TheSpectrumCountDataSet.spectrumcount;

            try
            {
                TheFindPartsWarehousesDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehousesDataSet.FindPartsWarehouses.Rows.Count;

                cboSelectWarehouse.Items.Clear();

                cboSelectWarehouse.Items.Add("Select Warehouse");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
                 
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Spectrum Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedID;

            try
            {
                intSelectedID = cboSelectWarehouse.SelectedIndex - 1;

                if(intSelectedID > -1)
                {
                    MainWindow.gintWarehouseID = TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedID].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Spectrum Report // Select Warehouse Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
            string strJDEPartNumber;
            int intPartID = 0;
            string strPartDescription;
            int intQuantity = 0;
            int intRecordsReturned;
            bool blnItemFound;
            bool blnItemInTable;
            bool blnFatalError = false;
            int intSecondCounter;
            string strOldPartNumber = "";

            try
            {
                expImportExcel.IsExpanded = false;

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Create Spectrum Report // Import Spectrum Count");

                if (blnFatalError == true)
                    throw new Exception();

                if(cboSelectWarehouse.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("The Warehouse Was Not Selected");
                    return;
                }

                TheSpectrumCountDataSet.spectrumcount.Rows.Clear();
                
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
                gintCounter = 0;
                gintNumberOfRecords = 0;

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnItemFound = false;
                    strPartNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strJDEPartNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strPartDescription = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();

                    TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                        strJDEPartNumber = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].JDEPartNumber;
                        blnItemFound = true;
                    }
                    else if(intRecordsReturned < 1)
                    {
                        TheFindPartByJDEPartNUmberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strJDEPartNumber);

                        intRecordsReturned = TheFindPartByJDEPartNUmberDataSet.FindPartByJDEPartNumber.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            strPartNumber = TheFindPartByJDEPartNUmberDataSet.FindPartByJDEPartNumber[0].PartNumber;
                            intPartID = TheFindPartByJDEPartNUmberDataSet.FindPartByJDEPartNumber[0].PartID;
                            blnItemFound = true;
                        }
                        else if(intRecordsReturned < 1)
                        {
                            if(strPartNumber == "NONE")
                            {
                                SpectrumCountDataSet.spectrumcountRow NewPartCount = TheSpectrumCountDataSet.spectrumcount.NewspectrumcountRow();

                                NewPartCount.ERPQuantity = 0;
                                NewPartCount.JDEPartNumber =strPartNumber;
                                NewPartCount.PartDescription = strPartDescription;
                                NewPartCount.PartID = intCounter * -1;
                                NewPartCount.PartNumber = strPartNumber;
                                NewPartCount.OldPartNumber = "UNKNOWN";

                                TheSpectrumCountDataSet.spectrumcount.Rows.Add(NewPartCount);
                            }
                        }
                    }
                    

                    if(blnItemFound == true)
                    {
                        TheFindWarehouseInventoryByPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, MainWindow.gintWarehouseID);

                        intRecordsReturned = TheFindWarehouseInventoryByPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            blnItemInTable = false;

                            if (gintCounter > 0)
                            {
                                for (intSecondCounter = 0; intSecondCounter <= gintNumberOfRecords; intSecondCounter++)
                                {
                                    if(intPartID == TheSpectrumCountDataSet.spectrumcount[intSecondCounter].PartID)
                                    {
                                        blnItemInTable = true;
                                    }
                                }
                            }

                            if(blnItemInTable == false)
                            {
                                intQuantity = TheFindWarehouseInventoryByPartDataSet.FindWarehouseInventoryPart[0].Quantity;

                                TheFindMasterPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                                intRecordsReturned = TheFindMasterPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    strOldPartNumber = TheFindMasterPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                                }
                                else
                                {
                                    strOldPartNumber = "NOT FOUND";
                                }

                                SpectrumCountDataSet.spectrumcountRow NewPartCount = TheSpectrumCountDataSet.spectrumcount.NewspectrumcountRow();

                                NewPartCount.ERPQuantity = intQuantity;
                                NewPartCount.JDEPartNumber = strJDEPartNumber;
                                NewPartCount.PartDescription = strPartDescription;
                                NewPartCount.PartID = intPartID;
                                NewPartCount.PartNumber = strPartNumber;
                                NewPartCount.OldPartNumber = strOldPartNumber;

                                TheSpectrumCountDataSet.spectrumcount.Rows.Add(NewPartCount);
                            }
                            
                        }
                        else if(intRecordsReturned < 1)
                        {
                            if(strPartDescription.Contains("CABLE RG6") == true)
                            {
                                SpectrumCountDataSet.spectrumcountRow NewPartCount = TheSpectrumCountDataSet.spectrumcount.NewspectrumcountRow();

                                NewPartCount.ERPQuantity = 0;
                                NewPartCount.JDEPartNumber = strJDEPartNumber;
                                NewPartCount.PartDescription = strPartDescription;
                                NewPartCount.PartID = intPartID;
                                NewPartCount.PartNumber = strPartNumber;
                                NewPartCount.OldPartNumber = strOldPartNumber;

                                TheSpectrumCountDataSet.spectrumcount.Rows.Add(NewPartCount);
                            }
                            else if (strPartDescription.Contains("CABLE RG11") == true)
                            {
                                SpectrumCountDataSet.spectrumcountRow NewPartCount = TheSpectrumCountDataSet.spectrumcount.NewspectrumcountRow();

                                NewPartCount.ERPQuantity = 0;
                                NewPartCount.JDEPartNumber = strJDEPartNumber;
                                NewPartCount.PartDescription = strPartDescription;
                                NewPartCount.PartID = intPartID;
                                NewPartCount.PartNumber = strPartNumber;
                                NewPartCount.OldPartNumber = strOldPartNumber;

                                TheSpectrumCountDataSet.spectrumcount.Rows.Add(NewPartCount);
                            }
                            else if (strPartDescription.Contains("CABLE 875") == true)
                            {
                                SpectrumCountDataSet.spectrumcountRow NewPartCount = TheSpectrumCountDataSet.spectrumcount.NewspectrumcountRow();

                                NewPartCount.ERPQuantity = 0;
                                NewPartCount.JDEPartNumber = strJDEPartNumber;
                                NewPartCount.PartDescription = strPartDescription;
                                NewPartCount.PartID = intPartID;
                                NewPartCount.PartNumber = strPartNumber;
                                NewPartCount.OldPartNumber = strOldPartNumber;

                                TheSpectrumCountDataSet.spectrumcount.Rows.Add(NewPartCount);
                            }
                            else if (strPartDescription.Contains("CABLE 625") == true)
                            {
                                SpectrumCountDataSet.spectrumcountRow NewPartCount = TheSpectrumCountDataSet.spectrumcount.NewspectrumcountRow();

                                NewPartCount.ERPQuantity = 0;
                                NewPartCount.JDEPartNumber = strJDEPartNumber;
                                NewPartCount.PartDescription = strPartDescription;
                                NewPartCount.PartID = intPartID;
                                NewPartCount.PartNumber = strPartNumber;
                                NewPartCount.OldPartNumber = strOldPartNumber;

                                TheSpectrumCountDataSet.spectrumcount.Rows.Add(NewPartCount);
                            }
                        }
                    }
                   
                }

                dgrResult.ItemsSource = TheSpectrumCountDataSet.spectrumcount;

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Spectrum Report // Import Excel Expander " + Ex.Message);

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
                intRowNumberOfRecords = TheSpectrumCountDataSet.spectrumcount.Rows.Count;
                intColumnNumberOfRecords = TheSpectrumCountDataSet.spectrumcount.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheSpectrumCountDataSet.spectrumcount.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;
                
                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheSpectrumCountDataSet.spectrumcount.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Spectrum Report // Export To Excel " + ex.Message);

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
