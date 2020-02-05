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
            DateTime datSecondPunchDate;
            string strFirstName;
            string strLastName;
            TimeSpan tspTotalHours;
            int intHours;
            int intMinutes;
            decimal decTotalHours;
            
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

                            intSecondNumberOfRecords = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries.Rows.Count - 1;

                            intRemander = (intSecondNumberOfRecords + 1) % 2;

                            if(intSecondNumberOfRecords > 0)
                            {
                                for (intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                                {
                                    datPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter].PunchTime;

                                    EmployeeTimePunchesDataSet.employeetimepunchesRow NewPunchRow = TheEmployeetimePunchesDataSet.employeetimepunches.NewemployeetimepunchesRow();

                                    NewPunchRow.EmployeeID = intEmployeeID;
                                    NewPunchRow.FirstName = strFirstName;
                                    NewPunchRow.LastName = strLastName;

                                    if (intRemander > 0)
                                    {
                                        if (datPunchDate.Hour < 5)
                                        {
                                            if (datPunchDate.Minute > 0)
                                            {
                                                tspTotalHours = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter + 1].PunchTime - datPunchDate;

                                                if(tspTotalHours.Hours > 8)
                                                {
                                                    datSecondPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter + 1].PunchTime;

                                                    tspTotalHours = datSecondPunchDate - datPunchDate;

                                                    intHours = tspTotalHours.Hours;
                                                    intMinutes = tspTotalHours.Minutes;

                                                    douTotalHours = Convert.ToDouble(intHours);
                                                    douTotalHours += (Convert.ToDouble(intMinutes)) / 60;

                                                    if (douTotalHours > 6)
                                                    {
                                                        douTotalHours--;
                                                    }

                                                    decTotalHours = Convert.ToDecimal(Math.Round(douTotalHours, 2));

                                                    intSecondCounter++;

                                                    NewPunchRow.StartDate = datPunchDate;
                                                    NewPunchRow.EndDate = datSecondPunchDate;
                                                    NewPunchRow.TotalHours = decTotalHours;
                                                }
                                                else
                                                {
                                                    datSecondPunchDate = datPunchDate;
                                                    datPunchDate = datStartingDate;

                                                    tspTotalHours = datPunchDate - datSecondPunchDate;

                                                    intHours = tspTotalHours.Hours;
                                                    intMinutes = tspTotalHours.Minutes;

                                                    douTotalHours = Convert.ToDouble(intHours);
                                                    douTotalHours += (Convert.ToDouble(intMinutes)) / 60;

                                                    if(douTotalHours > 6)
                                                    {
                                                        douTotalHours--;
                                                    }

                                                    decTotalHours = Convert.ToDecimal(Math.Round(douTotalHours, 2));

                                                    intRemander = 0;

                                                    NewPunchRow.StartDate = datPunchDate;
                                                    NewPunchRow.EndDate = datSecondPunchDate;
                                                    NewPunchRow.TotalHours = decTotalHours;
                                                }
                                            }

                                        }
                                        else if (datPunchDate.Hour > 20)
                                        {
                                            datSecondPunchDate = datStartingDate;
                                            datSecondPunchDate = TheDateSearchClass.AddingDays(datSecondPunchDate, 1);

                                            tspTotalHours = datSecondPunchDate - datPunchDate;

                                            intHours = tspTotalHours.Hours;
                                            intMinutes = tspTotalHours.Minutes;

                                            douTotalHours = Convert.ToDouble(intHours);
                                            douTotalHours += (Convert.ToDouble(intMinutes)) / 60;

                                            if (douTotalHours > 6)
                                            {
                                                douTotalHours--;
                                            }

                                            decTotalHours = Convert.ToDecimal(Math.Round(douTotalHours, 2));

                                            intRemander = 0;

                                            NewPunchRow.StartDate = datPunchDate;
                                            NewPunchRow.EndDate = datSecondPunchDate;
                                            NewPunchRow.TotalHours = decTotalHours;
                                        }
                                        else
                                        {
                                            datSecondPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter + 1].PunchTime;

                                            tspTotalHours = datSecondPunchDate - datPunchDate;

                                            intHours = tspTotalHours.Hours;
                                            intMinutes = tspTotalHours.Minutes;

                                            douTotalHours = Convert.ToDouble(intHours);
                                            douTotalHours += (Convert.ToDouble(intMinutes)) / 60;

                                            if (douTotalHours > 6)
                                            {
                                                douTotalHours--;
                                            }

                                            decTotalHours = Convert.ToDecimal(Math.Round(douTotalHours, 2));

                                            intSecondCounter++;

                                            NewPunchRow.StartDate = datPunchDate;
                                            NewPunchRow.EndDate = datSecondPunchDate;
                                            NewPunchRow.TotalHours = decTotalHours;
                                        }
                                    }
                                    else
                                    {
                                        datSecondPunchDate = TheFindEmployeeTimeCardEntriesDataSet.FindEmployeeTimeCardEntries[intSecondCounter + 1].PunchTime;

                                        tspTotalHours = datSecondPunchDate - datPunchDate;

                                        intHours = tspTotalHours.Hours;
                                        intMinutes = tspTotalHours.Minutes;

                                        douTotalHours = Convert.ToDouble(intHours);
                                        douTotalHours += (Convert.ToDouble(intMinutes)) / 60;

                                        if (douTotalHours > 6)
                                        {
                                            douTotalHours--;
                                        }

                                        decTotalHours = Convert.ToDecimal(Math.Round(douTotalHours, 2));

                                        intSecondCounter++;

                                        NewPunchRow.StartDate = datPunchDate;
                                        NewPunchRow.EndDate = datSecondPunchDate;
                                        NewPunchRow.TotalHours = decTotalHours;
                                    }

                                    TheEmployeetimePunchesDataSet.employeetimepunches.Rows.Add(NewPunchRow);

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
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
        }
    }
}
