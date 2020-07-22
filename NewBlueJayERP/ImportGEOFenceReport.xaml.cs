/* Title:           Import GEO Fence Report
 * Date:            2/17/20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for importing the GEO Fence Report */

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
using VehicleMainDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using GEOFenceDLL;
using DataValidationDLL;
using Excel = Microsoft.Office.Interop.Excel;
using InspectionsDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportGEOFenceReport.xaml
    /// </summary>
    public partial class ImportGEOFenceReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        GEOFenceClass TheGEOFenceClass = new GEOFenceClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        InspectionsClass TheInspectionsClass = new InspectionsClass();

        //setting up the data
        ImportGEOFencedDataSet TheImportGEOFenceDataSet = new ImportGEOFencedDataSet();
        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        FindEmployeeByLastNameDataSet TheFindEmployeeByLastDataSet = new FindEmployeeByLastNameDataSet();
        FindWarehousesDataSet TheFindWarehouseDataSet = new FindWarehousesDataSet();
        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDandDateRangeDataSet = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindGEOFenceTransactionByExactDateDataSet TheFindGEOFenceTransactionExactDateDataSet = new FindGEOFenceTransactionByExactDateDataSet();
        
        public ImportGEOFenceReport()
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
            TheImportGEOFenceDataSet.importgeofence.Rows.Clear();
            TheFindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();
            TheFindWarehouseDataSet = TheEmployeeClass.FindWarehouses();

            dgrResults.ItemsSource = TheImportGEOFenceDataSet.importgeofence;
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
            string strIsInside;
            string strDriver;
            string strVehicleNumber;
            string strEventDate;
            double douEventDate;
            DateTime datEventDate;
            bool blnIsInSide = false;
            int intVehicleCounter;
            int intVehicleNumberOfRecords;
            int intEmployeeCounter;
            int intEmployeeNumberOfRecords;
            string strAssignedOffice = "";
            int intSubstringLength;
            string strFullName;
            int intVehicleID = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                TheImportGEOFenceDataSet.importgeofence.Rows.Clear();

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

                for (intCounter = 5; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strEventDate = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strIsInside = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();
                    strDriver = Convert.ToString((range.Cells[intCounter, 10] as Excel.Range).Value2).ToUpper();
                    strVehicleNumber = Convert.ToString((range.Cells[intCounter, 11] as Excel.Range).Value2).ToUpper();

                    douEventDate = Convert.ToDouble(strEventDate);
                    datEventDate = DateTime.FromOADate(douEventDate);

                    if(strIsInside == "YES")
                    {
                        blnIsInSide = true;
                    }
                    else
                    {
                        blnIsInSide = false;
                    }

                    ImportGEOFencedDataSet.importgeofenceRow NewGEOFenceTransaction = TheImportGEOFenceDataSet.importgeofence.NewimportgeofenceRow();

                    NewGEOFenceTransaction.Driver = strDriver;
                    NewGEOFenceTransaction.EmployeeID = -1;
                    NewGEOFenceTransaction.EventTime = datEventDate;
                    NewGEOFenceTransaction.VehicleID = -1;
                    NewGEOFenceTransaction.VehicleNumber = strVehicleNumber;
                    NewGEOFenceTransaction.InSide = blnIsInSide;

                    TheImportGEOFenceDataSet.importgeofence.Rows.Add(NewGEOFenceTransaction);
                }

                dgrResults.ItemsSource = TheImportGEOFenceDataSet.importgeofence;

                TheMessagesClass.InformationMessage("Click OK to Continue");
                
                intNumberOfRecords = TheImportGEOFenceDataSet.importgeofence.Rows.Count - 1;
                intVehicleNumberOfRecords = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    for(intVehicleCounter = 0; intVehicleCounter <= intVehicleNumberOfRecords; intVehicleCounter++)
                    {
                        strVehicleNumber = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intVehicleCounter].VehicleNumber;

                        if(TheImportGEOFenceDataSet.importgeofence[intCounter].VehicleNumber.Contains(strVehicleNumber) == true)
                        {
                            TheImportGEOFenceDataSet.importgeofence[intCounter].VehicleID = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intVehicleCounter].VehicleID;
                            TheImportGEOFenceDataSet.importgeofence[intCounter].VehicleNumber = strVehicleNumber;
                            strAssignedOffice = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intVehicleCounter].AssignedOffice;
                            intVehicleID = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intVehicleCounter].VehicleID;
                            datStartDate = TheDateSearchClass.RemoveTime(TheImportGEOFenceDataSet.importgeofence[intCounter].EventTime);
                            datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);
                        }
                    }

                    strDriver = TheImportGEOFenceDataSet.importgeofence[intCounter].Driver;

                    if(strDriver == "NO DRIVER CHECKED IN")
                    {
                        intEmployeeNumberOfRecords = TheFindWarehouseDataSet.FindWarehouses.Rows.Count - 1;

                        for (intEmployeeCounter = 0; intEmployeeCounter <= intEmployeeNumberOfRecords; intEmployeeCounter++)
                        {
                            if(strAssignedOffice == TheFindWarehouseDataSet.FindWarehouses[intEmployeeCounter].FirstName)
                            {
                                TheImportGEOFenceDataSet.importgeofence[intCounter].EmployeeID = TheFindWarehouseDataSet.FindWarehouses[intEmployeeCounter].EmployeeID;                                
                            }
                        }
                    }
                    else
                    {
                        TheFindEmployeeByLastDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strDriver);

                        intEmployeeNumberOfRecords = TheFindEmployeeByLastDataSet.FindEmployeeByLastName.Rows.Count - 1;

                        if(intEmployeeNumberOfRecords == 0)
                        {
                            TheImportGEOFenceDataSet.importgeofence[intCounter].EmployeeID = TheFindEmployeeByLastDataSet.FindEmployeeByLastName[0].EmployeeID;
                        }
                        else
                        {
                            TheFindDailyVehicleInspectionByVehicleIDandDateRangeDataSet = TheInspectionsClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                            intEmployeeNumberOfRecords = TheFindDailyVehicleInspectionByVehicleIDandDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count;

                            if(intEmployeeNumberOfRecords > 0)
                            {
                                TheImportGEOFenceDataSet.importgeofence[intCounter].EmployeeID = TheFindDailyVehicleInspectionByVehicleIDandDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[0].EmployeeID;
                            }
                            else
                            {
                                intEmployeeNumberOfRecords = TheFindWarehouseDataSet.FindWarehouses.Rows.Count - 1;

                                for (intEmployeeCounter = 0; intEmployeeCounter <= intEmployeeNumberOfRecords; intEmployeeCounter++)
                                {
                                    if (strAssignedOffice == TheFindWarehouseDataSet.FindWarehouses[intEmployeeCounter].FirstName)
                                    {
                                        TheImportGEOFenceDataSet.importgeofence[intCounter].EmployeeID = TheFindWarehouseDataSet.FindWarehouses[intEmployeeCounter].EmployeeID;
                                    }
                                }
                            }
                        }
                    }
                }

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import GEO Fence Report // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            DateTime datEventDate;
            int intVehicleID;
            bool blnInSide;
            int intEmployeeID;
            string strDriver;
            int intRecordsReturned;
            bool blnFatalError;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheImportGEOFenceDataSet.importgeofence.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = TheImportGEOFenceDataSet.importgeofence[intCounter].VehicleID;
                    datEventDate = TheImportGEOFenceDataSet.importgeofence[intCounter].EventTime;
                    blnInSide = TheImportGEOFenceDataSet.importgeofence[intCounter].InSide;
                    intEmployeeID = TheImportGEOFenceDataSet.importgeofence[intCounter].EmployeeID;
                    strDriver = TheImportGEOFenceDataSet.importgeofence[intCounter].Driver;

                    if (intVehicleID > -1)
                    {
                        TheFindGEOFenceTransactionExactDateDataSet = TheGEOFenceClass.FindGEOFenceTransaction(datEventDate, intVehicleID);

                        intRecordsReturned = TheFindGEOFenceTransactionExactDateDataSet.FindGEOFenceTransactionByExactDate.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            blnFatalError = TheGEOFenceClass.InsertGEOFenceImportEntry(datEventDate, intVehicleID, blnInSide, intEmployeeID, strDriver);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Records Have Been Inserted");

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import GEO Fence Report // Process Import " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
