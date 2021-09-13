/* Title:       Cell Phone Call Search
 * Date:        9-10-2021
 * Author:      Terry Holmes
 * 
 * Description: This is used for Looking into Cell Phone Calls */

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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using NewEmployeeDLL;
using NewEventLogDLL;
using DataValidationDLL;
using CellPhoneCallsDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CellPhoneCallSearch.xaml
    /// </summary>
    public partial class CellPhoneCallSearch : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up data sets
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindCellPhoneCallsByCallerDataSet TheFindCellPhoneCallsByCallerDataSet = new FindCellPhoneCallsByCallerDataSet();
        FindCellPhoneCallsForEmployeeDataSet TheFindCellPhoneCallsForEmployeeDataSet = new FindCellPhoneCallsForEmployeeDataSet();

        //setting up variables
        bool gblnEmployeeSearch;
        int gintEmployeeID;
        string gstrLastFour;
        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public CellPhoneCallSearch()
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
            txtEnterInfo.Text = "";

            cboReportType.Items.Clear();
            cboReportType.Items.Add("Select Report Type");
            cboReportType.Items.Add("Calls By Employees");
            cboReportType.Items.Add("Calls By Number");
            cboReportType.SelectedIndex = 0;

            cboSelectEmployee.Visibility = Visibility.Visible;
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;

            TheFindCellPhoneCallsByCallerDataSet = TheCellPhoneCallsClass.FindCellPhoneCallsByCaller(DateTime.Now, DateTime.Now, "0000");

            dgrCellCalls.ItemsSource = TheFindCellPhoneCallsByCallerDataSet.FindCellPhoneCallsByCaller;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Cell Phone Call Search");
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboReportType.SelectedIndex;
                txtEndDate.Text = "";
                txtEnterInfo.Text = "";
                txtStartDate.Text = "";

                if(intSelectedIndex == 2)
                {
                    cboSelectEmployee.Visibility = Visibility.Hidden;
                    lblSelectEmployee.Visibility = Visibility.Hidden;
                    lblEnterInfo.Content = "Enter Last Four";
                    gblnEmployeeSearch = false;
                }
                else if(intSelectedIndex == 1)
                {
                    cboSelectEmployee.Visibility = Visibility.Visible;
                    lblSelectEmployee.Visibility = Visibility.Visible;
                    lblEnterInfo.Content = "Enter Last Name";
                    gblnEmployeeSearch = true;
                }
                else
                {
                    cboSelectEmployee.Visibility = Visibility.Hidden;
                    lblSelectEmployee.Visibility = Visibility.Hidden;
                    lblEnterInfo.Content = "Enter Last Four";
                    //gstrReportType = cboReportType.SelectedItem.ToString();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Phone Call Search // Report Type CBO " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strEnterInfo;
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                strEnterInfo = txtEnterInfo.Text;

                if(gblnEmployeeSearch == true)
                {
                    if (strEnterInfo.Length > 2)
                    {
                        TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strEnterInfo);

                        intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;
                        cboSelectEmployee.Items.Clear();
                        cboSelectEmployee.Items.Add("Select Employee");

                        if(intNumberOfRecords < 1)
                        {
                            TheMessagesClass.ErrorMessage("Employee Was Not Found");

                            return;
                        }

                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboSelectEmployee.SelectedIndex = 0;
                    }
                }
                if (gblnEmployeeSearch == false)
                {
                    if(strEnterInfo.Length == 4)
                    {
                        blnFatalError = TheDataValidationClass.VerifyIntegerData(strEnterInfo);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The Number added in not Numeric");
                            return;
                        }

                        gstrLastFour = strEnterInfo;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay Employee Class // Cell pHone Call Search // Enter Info Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Phone Search Class // Select Employee Combo Box " + Ex.Message);
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will run the report
            //setting up local variables
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                if(cboReportType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Report Type Was Not Selected\n";
                }
                if (gblnEmployeeSearch == true)
                {
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                }
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage +="The Start Date is not a Date\n";
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
                        strErrorMessage = "The Start Date Is After The End Date";
                        return;
                    }
                }

                if(gblnEmployeeSearch == true)
                {
                    TheFindCellPhoneCallsForEmployeeDataSet = TheCellPhoneCallsClass.FindCellPhoneCallsForEmployee(gintEmployeeID, gdatStartDate, gdatEndDate);

                    dgrCellCalls.ItemsSource = TheFindCellPhoneCallsForEmployeeDataSet.FindCellPhoneCallsForEmployee;
                }
                else if(gblnEmployeeSearch == false)
                {
                    TheFindCellPhoneCallsByCallerDataSet = TheCellPhoneCallsClass.FindCellPhoneCallsByCaller(gdatStartDate, gdatEndDate, gstrLastFour);

                    dgrCellCalls.ItemsSource = TheFindCellPhoneCallsByCallerDataSet.FindCellPhoneCallsByCaller;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Phone Call Search // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            expExportToExcel.IsExpanded = false;

            if(gblnEmployeeSearch == true)
            {
                ExportEmployees();
            }
            else if(gblnEmployeeSearch == false)
            {
                ExportCallers();
            }
        }
        private void ExportEmployees()
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
                intRowNumberOfRecords = TheFindCellPhoneCallsForEmployeeDataSet.FindCellPhoneCallsForEmployee.Rows.Count;
                intColumnNumberOfRecords = TheFindCellPhoneCallsForEmployeeDataSet.FindCellPhoneCallsForEmployee.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindCellPhoneCallsForEmployeeDataSet.FindCellPhoneCallsForEmployee.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindCellPhoneCallsForEmployeeDataSet.FindCellPhoneCallsForEmployee.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Phone Call Search // Export Employees " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportCallers()
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
                intRowNumberOfRecords = TheFindCellPhoneCallsByCallerDataSet.FindCellPhoneCallsByCaller.Rows.Count;
                intColumnNumberOfRecords = TheFindCellPhoneCallsByCallerDataSet.FindCellPhoneCallsByCaller.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindCellPhoneCallsByCallerDataSet.FindCellPhoneCallsByCaller.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindCellPhoneCallsByCallerDataSet.FindCellPhoneCallsByCaller.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Phone Call Search // Export Callers " + ex.Message);

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
