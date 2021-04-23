/* Title:           Employee Productivity Over Date Range
 * Date:            4-22-21
 * Author:          Terry Holmes
 * 
 * Description:     This used for getting employee productivity */

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
using DataValidationDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using EmployeeDateEntryDLL;
using EmployeeProjectAssignmentDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeProductivityOverDateRange.xaml
    /// </summary>
    public partial class EmployeeProductivityOverDateRange : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();

        //setting up data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindDetailEmployeeProductivityOverDateRangeDataSet TheFindDetailEmployeeProductivityOverDateRangeDataSet = new FindDetailEmployeeProductivityOverDateRangeDataSet();
        DetailedEmployeeProductivityDataSet TheDetailedEmployeeProductivityDataSet = new DetailedEmployeeProductivityDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        public EmployeeProductivityOverDateRange()
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
            txtEndDate.Text = "";
            txtStartDate.Text = "";
            txtLastName.Text = "";

            cboSelectEmployee.Items.Clear();

            cboSelectEmployee.Items.Add("Select Employee");

            cboSelectEmployee.SelectedIndex = 0;

            TheDetailedEmployeeProductivityDataSet.employeeproductivity.Rows.Clear();

            dgrResults.ItemsSource = TheDetailedEmployeeProductivityDataSet.employeeproductivity;
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strLastName;

            try
            {
                strLastName = txtLastName.Text;
                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("The employee Was not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Productivity Over Date Range // Last Name Text Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            int intEmployeeID;
            string strValueForValidation;
            int intCounter;
            int intNumberOfRecords;
            int intManagerID;
            string strManagerName;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;
                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    TheDetailedEmployeeProductivityDataSet.employeeproductivity.Rows.Clear();

                    strValueForValidation = txtStartDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Start Date is not a Date\n";
                    }
                    else
                    {
                        datStartDate = Convert.ToDateTime(strValueForValidation);
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
                        datEndDate = Convert.ToDateTime(strValueForValidation);
                    }
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }
                    else
                    {
                        blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                            return;
                        }
                    }

                    TheFindDetailEmployeeProductivityOverDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindDetailEmployeeProductivityOverDateRange(intEmployeeID, datStartDate, datEndDate);

                    intNumberOfRecords = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            intManagerID = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].ManagerID;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                            strManagerName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                            strManagerName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                            DetailedEmployeeProductivityDataSet.employeeproductivityRow NewEmployeeRow = TheDetailedEmployeeProductivityDataSet.employeeproductivity.NewemployeeproductivityRow();

                            NewEmployeeRow.BJCProjectID = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].AssignedProjectID;
                            NewEmployeeRow.CustomerProjectID = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].CustomerAssignedID;
                            NewEmployeeRow.Manager = strManagerName;
                            NewEmployeeRow.ProjectName = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].ProjectName;
                            NewEmployeeRow.TotalHours = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].TotalHours;
                            NewEmployeeRow.TransactionDate = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].TransactionDate;
                            NewEmployeeRow.WorkTask = TheFindDetailEmployeeProductivityOverDateRangeDataSet.FindDetailEmployeeProductivityOverDateRange[intCounter].WorkTask;

                            TheDetailedEmployeeProductivityDataSet.employeeproductivity.Rows.Add(NewEmployeeRow);
                        }
                    }

                    dgrResults.ItemsSource = TheDetailedEmployeeProductivityDataSet.employeeproductivity;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Productivity By Date Range // Select Employee Combo Box " + Ex.Message);

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

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheDetailedEmployeeProductivityDataSet.employeeproductivity.Rows.Count;
                intColumnNumberOfRecords = TheDetailedEmployeeProductivityDataSet.employeeproductivity.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheDetailedEmployeeProductivityDataSet.employeeproductivity.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheDetailedEmployeeProductivityDataSet.employeeproductivity.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Productivity Over Date Range // Export Department WOV to Excel " + ex.Message);

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
