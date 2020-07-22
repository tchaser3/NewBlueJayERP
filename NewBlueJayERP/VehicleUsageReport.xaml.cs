/* Title:           Vehicle Usage Report
 * Date:            2-18-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show the vehicle usage report */

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
using InspectionsDLL;
using GEOFenceDLL;
using VehicleInYardDLL;
using NewEventLogDLL;
using DateSearchDLL;
using VehicleMainDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleUsageReport.xaml
    /// </summary>
    public partial class VehicleUsageReport : Window
    {
        //setting the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InspectionsClass TheInspectionsClass = new InspectionsClass();
        GEOFenceClass TheGEOFenceClass = new GEOFenceClass();
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        //setting up the data sets
        FindGEOFenceByVehicleIDDataSet TheFindGEOFenceByVehicleIDDataSet = new FindGEOFenceByVehicleIDDataSet();
        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindVehiclesInYardByVehicleIDAndDateRangeDataSet TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = new FindVehiclesInYardByVehicleIDAndDateRangeDataSet();
        VehicleUsageDataSet TheVehicleUsageDataSet = new VehicleUsageDataSet();
        FindActiveVehicleMainSortedDataSet TheFindActiveVehicleMainSortedDataSet = new FindActiveVehicleMainSortedDataSet();

        public VehicleUsageReport()
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

        }

        private void expCreateReport_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datStartDate;
            DateTime datEndDate;
            DateTime datTransactionDate;
            DateTime datLimitingDate;
            int intCounter;
            int intNumberOfRecords;
            int intRecordReturned;
            int intVehicleID;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                TheVehicleUsageDataSet.vehicleusage.Rows.Clear();
                expCreateReport.IsExpanded = false;

                TheFindActiveVehicleMainSortedDataSet = TheVehicleMainClass.FindActiveVehicleMainSorted();

                intNumberOfRecords = TheFindActiveVehicleMainSortedDataSet.FindActiveVehicleMainSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    VehicleUsageDataSet.vehicleusageRow NewVehicleRow = TheVehicleUsageDataSet.vehicleusage.NewvehicleusageRow();

                    NewVehicleRow.VehicleID = TheFindActiveVehicleMainSortedDataSet.FindActiveVehicleMainSorted[intCounter].VehicleID;
                    NewVehicleRow.VehicleNumber = TheFindActiveVehicleMainSortedDataSet.FindActiveVehicleMainSorted[intCounter].VehicleNumber;
                    NewVehicleRow.AssignedOffice = TheFindActiveVehicleMainSortedDataSet.FindActiveVehicleMainSorted[intCounter].AssignedOffice;
                    NewVehicleRow.TimesInYard = 0;
                    NewVehicleRow.TimesUnknown = 0;
                    NewVehicleRow.TimesDriven = 0;

                    TheVehicleUsageDataSet.vehicleusage.Rows.Add(NewVehicleRow);
                }

                datEndDate = DateTime.Now;
                datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
                datStartDate = TheDateSearchClass.SubtractingDays(datEndDate, 120);

                datTransactionDate = datStartDate;
                datLimitingDate = TheDateSearchClass.AddingDays(datTransactionDate, 1);
                intNumberOfRecords = TheVehicleUsageDataSet.vehicleusage.Rows.Count - 1;

                while(datLimitingDate <= datEndDate)
                {
                    if(datTransactionDate.DayOfWeek != DayOfWeek.Saturday)
                    {
                        if(datTransactionDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                intVehicleID = TheVehicleUsageDataSet.vehicleusage[intCounter].VehicleID;

                                TheFindGEOFenceByVehicleIDDataSet = TheGEOFenceClass.FindGEOFenceByVehicleID(intVehicleID, datTransactionDate, datLimitingDate);

                                intRecordReturned = TheFindGEOFenceByVehicleIDDataSet.FindGEOFenceByVehicleID.Rows.Count;

                                if(intRecordReturned > 0)
                                {
                                    TheVehicleUsageDataSet.vehicleusage[intCounter].TimesDriven++;
                                }
                                else
                                {
                                    TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = TheInspectionsClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, datTransactionDate, datLimitingDate);

                                    intRecordReturned = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count;

                                    if(intRecordReturned > 0)
                                    {
                                        TheVehicleUsageDataSet.vehicleusage[intCounter].TimesDriven++;
                                    }
                                    else
                                    {
                                        TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = TheVehicleInYardClass.FindVehiclesInYardByVehicleIDAndDateRange(intVehicleID, datTransactionDate, datLimitingDate);

                                        intRecordReturned = TheFindVehicleInYardByVehicleIDAndDateRangeDataSet.FindVehiclesInYardByVehicleIDAndDateRange.Rows.Count;

                                        if(intRecordReturned > 0)
                                        {
                                            TheVehicleUsageDataSet.vehicleusage[intCounter].TimesInYard++;
                                        }
                                        else
                                        {
                                            TheVehicleUsageDataSet.vehicleusage[intCounter].TimesUnknown++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 1);
                    datLimitingDate = TheDateSearchClass.AddingDays(datLimitingDate, 1);
                }

                dgrResults.ItemsSource = TheVehicleUsageDataSet.vehicleusage;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Vehicle Usage Report // Create Report Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
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
                intRowNumberOfRecords = TheVehicleUsageDataSet.vehicleusage.Rows.Count;
                intColumnNumberOfRecords = TheVehicleUsageDataSet.vehicleusage.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleUsageDataSet.vehicleusage.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleUsageDataSet.vehicleusage.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Productivity Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
