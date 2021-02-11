/* Title:           Employee Overtime Report
 * Date:            2-10-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to run the over time report */

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
using EmployeePunchedHoursDLL;
using Microsoft.Win32;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeOvertimeReport.xaml
    /// </summary>
    public partial class EmployeeOvertimeReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        FindEmployeesOverFortyHoursDataSet TheFindEmployeeOverFortyHoursDataSet = new FindEmployeesOverFortyHoursDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        EmployeeOvertimeDataSet TheEmployeeOvertimeDataSet = new EmployeeOvertimeDataSet();

        DateTime gdatSelectedDate;

        public EmployeeOvertimeReport()
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
            //setting up the controls
            TheEmployeeOvertimeDataSet.employeeovertime.Rows.Clear();

            dgrEmployees.ItemsSource = TheEmployeeOvertimeDataSet.employeeovertime;
        }

        private void calPayPeriod_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            gdatSelectedDate = Convert.ToDateTime(calPayPeriod.SelectedDate);
        }

        private void expCreateReport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strManager;
            int intManagerID;

            try
            {
                expCreateReport.IsExpanded = false;

                TheEmployeeOvertimeDataSet.employeeovertime.Rows.Clear();

                TheFindEmployeeOverFortyHoursDataSet = TheEmployeePunchedHoursClass.FindEmployeesOverFortyHours(gdatSelectedDate);

                intNumberOfRecords = TheFindEmployeeOverFortyHoursDataSet.FindEmployeesOverFortyHours.Rows.Count;

                if(intNumberOfRecords > 1)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intManagerID = TheFindEmployeeOverFortyHoursDataSet.FindEmployeesOverFortyHours[intCounter].ManagerID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                        strManager = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                        strManager += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                        EmployeeOvertimeDataSet.employeeovertimeRow NewEmployeeRow = TheEmployeeOvertimeDataSet.employeeovertime.NewemployeeovertimeRow();

                        NewEmployeeRow.FirstName = TheFindEmployeeOverFortyHoursDataSet.FindEmployeesOverFortyHours[intCounter].FirstName;
                        NewEmployeeRow.HomeOffice = TheFindEmployeeOverFortyHoursDataSet.FindEmployeesOverFortyHours[intCounter].HomeOffice;
                        NewEmployeeRow.LastName = TheFindEmployeeOverFortyHoursDataSet.FindEmployeesOverFortyHours[intCounter].LastName;
                        NewEmployeeRow.Manager = strManager;
                        NewEmployeeRow.PunchedHours = TheFindEmployeeOverFortyHoursDataSet.FindEmployeesOverFortyHours[intCounter].PunchedHours;

                        TheEmployeeOvertimeDataSet.employeeovertime.Rows.Add(NewEmployeeRow);
                    }
                }

                dgrEmployees.ItemsSource = TheEmployeeOvertimeDataSet.employeeovertime;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Overtime Report // Create Report Expander // " + Ex.Message);

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
                intRowNumberOfRecords = TheEmployeeOvertimeDataSet.employeeovertime.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeOvertimeDataSet.employeeovertime.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeOvertimeDataSet.employeeovertime.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeOvertimeDataSet.employeeovertime.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Overtime Report // Export To Excel " + ex.Message);

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
