/* Title:           Manager Hourly Daily Report
 * Date:            2-12-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show daily productivity, hours punched and costs for the day */

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
using DateSearchDLL;
using ProjectProductivityReportsDLL;
using Microsoft.Win32;
using EmployeeTimeClockEntriesDLL;
using NewEmployeeDLL;
using EmployeeProjectAssignmentDLL;
using EmployeeLaborRateDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ManagerHourlyDailyReport.xaml
    /// </summary>
    public partial class ManagerHourlyDailyReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        ProjectProductivityReportsClass TheProjectProductivityReportsClass = new ProjectProductivityReportsClass();
        EmployeeTimeClockEntriesClass TheEmployeeTimeClockEntriesClass = new EmployeeTimeClockEntriesClass();        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeeLaborRateClass TheEmployeeLaborRateClass = new EmployeeLaborRateClass();

        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        EmployeePunchesProductivityDataSet TheEmployeePunchesProductivityDataSet = new EmployeePunchesProductivityDataSet();
        FindSortedManagersHourlyEmployeesDataSet TheFindSortedManagerHourlyemployeesDataSet = new FindSortedManagersHourlyEmployeesDataSet();
        FindEmployeeTimeCardEntriesDataSet TheFindEmployeeTimeClockEntriesDataSet = new FindEmployeeTimeCardEntriesDataSet();
        FindEmployeeProductionHoursOverPayPeriodDataSet TheFindEmployeeProductionHoursOverPayPeriodDataSet = new FindEmployeeProductionHoursOverPayPeriodDataSet();
        FindEmployeeLaborRateDataSet TheFindEmployeeLaborRateDataSet = new FindEmployeeLaborRateDataSet();

        int gintManagerID;
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        int gintEmployeeCounter;
        int gintEmployeeNumberOfRecords;

        public ManagerHourlyDailyReport()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            txtEndDate.Text = "";
            txtStartDate.Text = "";
            TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Rows.Clear();
            dgrResults.ItemsSource = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity;

            TheFindProductionManagersDataSet = TheEmployeeClass.FindProductionManagers();

            intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;
            cboSelectManager.Items.Clear();
            cboSelectManager.Items.Add("Select Manager");

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(TheFindProductionManagersDataSet.FindProductionManagers[intCounter].FullName);
            }

            cboSelectManager.SelectedIndex = 0;
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            DateTime datTransactionDate;
            DateTime datLimitingDate;
            string strFirstName;
            string strLastName;
            decimal decProductivityHours;
            double douPunchedHours;
            decimal decLaborRate;
            decimal decPunchedHours;
            int intEmployeeID;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            decimal decStraightHours = 0;
            decimal decOverTimeHours = 0;
            decimal decStraightCost = 0;
            decimal decOverTimeCosts = 0;
            decimal decOverTimeRate = 0;
            double douTotalLaborCost = 0;
            decimal decHourVariance = 0;

            try
            {
                intSelectedIndex = cboSelectManager.SelectedIndex - 1;
                TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Rows.Clear();

                if(intSelectedIndex > -1)
                {
                    strValueForValidation = txtStartDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Start Date is not a Date\n";
                    }
                    else
                    {
                        gdatStartDate = Convert.ToDateTime(strValueForValidation);
                    }
                    strValueForValidation = txtEndDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The End Date is not a Date\n";
                    }
                    else
                    {
                        gdatEndDate = Convert.ToDateTime(strValueForValidation);
                    }
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }
                    else
                    {
                        blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);
                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                            return;
                        }
                    }

                    gintManagerID = TheFindProductionManagersDataSet.FindProductionManagers[intSelectedIndex].EmployeeID;

                    TheFindSortedManagerHourlyemployeesDataSet = TheEmployeeClass.FindSortedManagersHourlyEmployees(gintManagerID);

                    intNumberOfRecords = TheFindSortedManagerHourlyemployeesDataSet.FindSortedManagersHourlyEmployees.Rows.Count - 1;

                    datTransactionDate = gdatStartDate;
                    datLimitingDate = TheDateSearchClass.AddingDays(datTransactionDate, 1);
                    datLimitingDate = datLimitingDate.AddSeconds(-1);

                    while(datLimitingDate <= gdatEndDate)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheFindSortedManagerHourlyemployeesDataSet.FindSortedManagersHourlyEmployees[intCounter].EmployeeID;

                            TheFindEmployeeProductionHoursOverPayPeriodDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProductionHoursOverPayPeriodDataSet(intEmployeeID, datTransactionDate, datTransactionDate);

                            intRecordsReturned = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod.Rows.Count;

                            if(intRecordsReturned < 1)
                            {
                                decProductivityHours = 0;
                            }
                            else
                            {
                                decProductivityHours = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod[0].ProductionHours;
                            }

                            TheFindEmployeeTimeClockEntriesDataSet = TheEmployeeTimeClockEntriesClass.FindEmployeeTimeCardEntries(intEmployeeID, datTransactionDate, datLimitingDate);

                            douPunchedHours = ComputePunchedHours();

                            decPunchedHours = Convert.ToDecimal(Math.Round(douPunchedHours, 2));

                            if(decPunchedHours > 8)
                            {
                                decStraightHours = 8;
                                decOverTimeHours = decPunchedHours - 8;
                            }
                            else
                            {
                                decStraightHours = decPunchedHours;
                                decOverTimeHours = 0;
                            }

                            TheFindEmployeeLaborRateDataSet = TheEmployeeLaborRateClass.FindEmployeeLaborRate(intEmployeeID);

                            decLaborRate = TheFindEmployeeLaborRateDataSet.FindEmployeeLaborRate[0].PayRate;

                            decStraightCost = decStraightHours * decLaborRate;
                            decOverTimeRate = decLaborRate * Convert.ToDecimal(1.5);
                            decOverTimeCosts = decOverTimeHours * decOverTimeRate;

                            strFirstName = TheFindSortedManagerHourlyemployeesDataSet.FindSortedManagersHourlyEmployees[intCounter].FirstName;
                            strLastName = TheFindSortedManagerHourlyemployeesDataSet.FindSortedManagersHourlyEmployees[intCounter].LastName;

                            douTotalLaborCost = Math.Round(Convert.ToDouble(decStraightCost + decOverTimeCosts), 2);

                            if(decPunchedHours != 0)
                            {
                                decHourVariance = decProductivityHours - decPunchedHours;

                                EmployeePunchesProductivityDataSet.employeepunchesproductivityRow NewProductivityRow = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.NewemployeepunchesproductivityRow();

                                NewProductivityRow.Date = datTransactionDate;
                                NewProductivityRow.FirstName = strFirstName;
                                NewProductivityRow.LastName = strLastName;
                                NewProductivityRow.TotalLaborCost = Convert.ToDecimal(douTotalLaborCost);
                                NewProductivityRow.HoursProductive = decProductivityHours;
                                NewProductivityRow.HoursPunched = decPunchedHours;
                                NewProductivityRow.HourVariance = decHourVariance;

                                TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Rows.Add(NewProductivityRow);
                            }
                        }

                        datTransactionDate = datLimitingDate.AddSeconds(1);
                        datLimitingDate = TheDateSearchClass.AddingDays(datLimitingDate, 1);
                    }
                    
                    dgrResults.ItemsSource = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity;

                   
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Manager Hourly Daily Report // Manager Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private double ComputePunchedHours()
        {
            double douTotalHours = 0;
            int intCounter;
            int intNumberOfRecords;
            int intRemainder;
            TimeSpan tspTotalHours;
            int intHours;
            int intMinutes;
            decimal decTotalHours;
            DateTime datPunchDate;
            DateTime datSecondPunchDate;

            try
            {
                intNumberOfRecords = TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries.Rows.Count - 1;

                intRemainder = (intNumberOfRecords + 1) % 2;
                douTotalHours = 0;
                
                if(intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (intRemainder == 0)
                        {
                            tspTotalHours = TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter + 1].PunchTime - TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime;
                            douTotalHours = tspTotalHours.TotalMinutes;

                            if(douTotalHours > 360)
                            {
                                douTotalHours = douTotalHours - 60;
                            }

                            douTotalHours = douTotalHours / 60;

                            intCounter++;
                        }
                        else if (intRemainder == 1)
                        {
                            if(TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime.Hour < 3)
                            {
                                intHours = TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime.Hour;
                                intMinutes = TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime.Minute;

                                douTotalHours = Convert.ToDouble(intHours) + Convert.ToDouble(intMinutes) / 60;

                                intRemainder = 0;
                            }
                            else if(TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime.Hour > 21)
                            {
                                intHours = TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime.Hour;
                                intHours = 24 - intHours;
                                intMinutes = TheFindEmployeeTimeClockEntriesDataSet.FindEmployeeTimeCardEntries[intCounter].PunchTime.Minute;

                                douTotalHours = Convert.ToDouble(intHours) + Convert.ToDouble(intMinutes) / 60;

                                intRemainder = 0;
                            }
                        }
                    }
                }
                else
                {
                    douTotalHours = 0;
                }

                
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Manager Hourly Daily Report // Compute Punched Hours " + Ex.Message);
            }

            return douTotalHours;
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
                intRowNumberOfRecords = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Rows.Count;
                intColumnNumberOfRecords = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeePunchesProductivityDataSet.employeepunchesproductivity.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Manager Hourly Daily Report // Export To Excel " + ex.Message);

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
