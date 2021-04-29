/* Title:           Employee Hours Punched
 * Date:            2-4-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to calculate an employees time */

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
using DataValidationDLL;
using DateSearchDLL;
using EmployeeTimeClockEntriesDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeHoursPunched.xaml
    /// </summary>
    public partial class EmployeeHoursPunched : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeTimeClockEntriesClass TheEmployeeTimeClockEntriesClass = new EmployeeTimeClockEntriesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the datasets
        FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();
        FindEmployeeTimeCardEntriesDataSet TheFindEmployeeTimeCardEntriesDataSet = new FindEmployeeTimeCardEntriesDataSet();
        FindSortedManagersHourlyEmployeesDataSet TheFindSortedManagersHourlyEmployeesDataSet = new FindSortedManagersHourlyEmployeesDataSet();
        EmployeeTimePunchesDataSet TheEmployeetimePunchesDataSet = new EmployeeTimePunchesDataSet();
        EmployeeTimePunchesDataSet TheComputedEmployeetimePunchesDataSet = new EmployeeTimePunchesDataSet();

        //setting global variables
        int gintManagerID;
        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public EmployeeHoursPunched()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

        private void expProecess_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValdation;
            int intCounter;
            int intNumberOfRecords;
            DateTime datStartingDate;
            DateTime datLimitingDate;
            double douTotalHours;
            int intEmployeeID;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            int intRemander;
            DateTime datPunchDate;
            DateTime datSecondPunchDate = DateTime.Now;
            string strFirstName;
            string strLastName;
            TimeSpan tspTotalHours;
            int intHours;
            int intMinutes;
            decimal decTotalHours;
            int intRecordsReturned;
            int intCounterDifference;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();
            
            try
            {
                expProecess.IsExpanded = false;
                TheEmployeetimePunchesDataSet.employeetimepunches.Rows.Clear();

                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager was not Selected\n";
                }
                strValueForValdation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValdation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValdation);
                }
                strValueForValdation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValdation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValdation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date\0n");
                        return;
                    }
                }

                TheFindSortedManagersHourlyEmployeesDataSet = TheEmployeeClass.FindSortedManagersHourlyEmployees(gintManagerID);

                intNumberOfRecords = TheFindSortedManagersHourlyEmployeesDataSet.FindSortedManagersHourlyEmployees.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheFindSortedManagersHourlyEmployeesDataSet.FindSortedManagersHourlyEmployees[intCounter].EmployeeID;
                        strFirstName = TheFindSortedManagersHourlyEmployeesDataSet.FindSortedManagersHourlyEmployees[intCounter].FirstName;
                        strLastName = TheFindSortedManagersHourlyEmployeesDataSet.FindSortedManagersHourlyEmployees[intCounter].LastName;

                        datStartingDate = gdatStartDate;
                        datStartingDate = TheDateSearchClass.RemoveTime(datStartingDate);
                        datLimitingDate = TheDateSearchClass.AddingDays(datStartingDate, 1);

                        while(datLimitingDate <= gdatEndDate)
                        {
                            TheFindEmployeeTimeCardEntriesDataSet = TheEmployeeTimeClockEntriesClass.FindEmployeeTimeCardEntries(intEmployeeID, datStartingDate, datLimitingDate);

                            intSecondNumberOfRecords = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries.Rows.Count;

                            intRemander = (intSecondNumberOfRecords) % 2;

                            intRecordsReturned = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries.Rows.Count;

                            datPunchDate = datStartingDate;

                            if (intSecondNumberOfRecords > 0)
                            {
                                for(intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                                {
                                    datPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter].PunchTime;

                                    if (intRemander == 0)
                                    {                                        
                                        intSecondCounter++;
                                        datSecondPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter].PunchTime;
                                    }
                                    else if(intRemander > 0)
                                    {
                                        if(intRecordsReturned == 1)
                                        {
                                            if(datPunchDate.Hour < 7)
                                            {
                                                datSecondPunchDate = datPunchDate;
                                                datPunchDate = TheDateSearchClass.RemoveTime(datPunchDate);
                                            }
                                            else if(datPunchDate.Hour >= 7)
                                            {
                                                datSecondPunchDate = datPunchDate;
                                                datSecondPunchDate = TheDateSearchClass.RemoveTime(datSecondPunchDate);
                                                datSecondPunchDate = TheDateSearchClass.AddingDays(datSecondPunchDate, 1);
                                            }
                                        }
                                        else if(intRecordsReturned > 1)
                                        {
                                            intCounterDifference = (intSecondNumberOfRecords - 1) - intSecondCounter;

                                            datPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter].PunchTime;

                                            if (intCounterDifference == 0)
                                            {
                                                if (datPunchDate.Hour < 7)
                                                {
                                                    datSecondPunchDate = datPunchDate;
                                                    datPunchDate = TheDateSearchClass.RemoveTime(datPunchDate);
                                                }
                                                else if ((datPunchDate.Hour >= 7) && (datPunchDate.Hour < 20))
                                                {
                                                    datSecondPunchDate = datPunchDate;
                                                    datSecondPunchDate = TheDateSearchClass.RemoveTime(datSecondPunchDate);
                                                    datSecondPunchDate = TheDateSearchClass.AddingDays(datSecondPunchDate, 1);
                                                }
                                                else if(datPunchDate.Hour >= 20)
                                                {
                                                    datSecondPunchDate = TheDateSearchClass.RemoveTime(datPunchDate);
                                                    datSecondPunchDate = TheDateSearchClass.AddingDays(datSecondPunchDate, 1);
                                                }

                                            }
                                            else if(intCounterDifference > 1)
                                            {
                                                intSecondCounter++;
                                                datSecondPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter].PunchTime;
                                            }
                                        }

                                    }

                                    tspTotalHours = datSecondPunchDate - datPunchDate;
                                    decTotalHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                                    if(decTotalHours > 6)
                                    {
                                        decTotalHours = decTotalHours - 1;
                                    }

                                    decTotalHours = Math.Round(decTotalHours, 2);

                                    EmployeeTimePunchesDataSet.employeetimepunchesRow NewEmployeeRow = TheEmployeetimePunchesDataSet.employeetimepunches.NewemployeetimepunchesRow();

                                    NewEmployeeRow.EndDate = datSecondPunchDate;
                                    NewEmployeeRow.FirstName = strFirstName;
                                    NewEmployeeRow.LastName = strLastName;
                                    NewEmployeeRow.StartDate = datPunchDate;
                                    NewEmployeeRow.TotalHours = decTotalHours;

                                    TheEmployeetimePunchesDataSet.employeetimepunches.Rows.Add(NewEmployeeRow);
                                }
                            }     

                            datStartingDate = datLimitingDate;
                            datLimitingDate = TheDateSearchClass.AddingDays(datLimitingDate, 1);
                        }
                    }
                }

                dgrResults.ItemsSource = TheEmployeetimePunchesDataSet.employeetimepunches;
            }
            catch (Exception EX)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Hours Punched // Process Expander " + EX.Message);

                TheMessagesClass.ErrorMessage(EX.ToString());
            }

            PleaseWait.Close();
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

            txtEndDate.Text = "";
            txtStartDate.Text = "";

            TheEmployeetimePunchesDataSet.employeetimepunches.Rows.Clear();

            dgrResults.ItemsSource = TheEmployeetimePunchesDataSet.employeetimepunches;

            TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

            intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;
            cboSelectManager.Items.Clear();
            cboSelectManager.Items.Add("Select Manager");
            
            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
            }

            cboSelectManager.SelectedIndex = 0;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Employee Hours Punched Report");
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

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
                intRowNumberOfRecords = TheEmployeetimePunchesDataSet.employeetimepunches.Rows.Count;
                intColumnNumberOfRecords = TheEmployeetimePunchesDataSet.employeetimepunches.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeetimePunchesDataSet.employeetimepunches.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeetimePunchesDataSet.employeetimepunches.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Hours Punched // Export To Excel " + ex.Message);

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
