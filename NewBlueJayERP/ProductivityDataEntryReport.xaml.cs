/* Title:           Productivity Data Entry Report
 * Date:            4-30-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to see who is entering the productivity sheets */

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
using ProductivityDataEntryDLL;
using DataValidationDLL;
using DateSearchDLL;
using Microsoft.Win32;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ProductivityDataEntryReport.xaml
    /// </summary>
    public partial class ProductivityDataEntryReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProductivityDataEntryClass TheProductivityDataEntryClass = new ProductivityDataEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        ComboEmployeeDataSet TheComboBoxEmployeeDataSet = new ComboEmployeeDataSet();
        FindProductivityDataEntryByDateRangeDataSet TheFindProductivityDataEntryByDateRangeDataSet = new FindProductivityDataEntryByDateRangeDataSet();
        FindProductivityDataEntryByEmployeeIDDataSet TheFindProductivityDataEntryByEmployeeIDDataSet = new FindProductivityDataEntryByEmployeeIDDataSet();
        FindProductivityDataEntryByProjectIDDataSet TheFindProductivtyDataEntryByProjectIDdataSet = new FindProductivityDataEntryByProjectIDDataSet();
        ProductivityDataEntryDataSet TheProductivityDataEntryDataSet = new ProductivityDataEntryDataSet();

        string gstrReportType;
        int gintEmployeeID;

        public ProductivityDataEntryReport()
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
            //this will load the report combo box
            try
            {
                cboSelectReportType.Items.Clear();
                cboSelectReportType.Items.Add("Select Report Type");
                cboSelectReportType.Items.Add("Date Range Report");
                cboSelectReportType.Items.Add("Employee Report");
                cboSelectReportType.SelectedIndex = 0;

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;
                txtEndDate.Text = "";
                txtEnterLastName.Text = "";
                txtStartDate.Text = "";

                stpEmployees.Visibility = Visibility.Hidden;

                TheProductivityDataEntryDataSet.dataentry.Rows.Clear();

                dgrResults.ItemsSource = TheProductivityDataEntryDataSet.dataentry;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Productivity Data Entry Report");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Productivity Data Entry Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboSelectReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectReportType.SelectedIndex;

            if (intSelectedIndex == 1)
            {
                gstrReportType = "DATE RANGE";
                stpEmployees.Visibility = Visibility.Hidden;
            }
            else if (intSelectedIndex == 2)
            {
                gstrReportType = "EMPLOYEE";
                stpEmployees.Visibility = Visibility.Visible;
            }           
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up the variables
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                strLastName = txtEnterLastName.Text;

                intLength = strLastName.Length;

                if ((intLength > 2) && (gstrReportType == "EMPLOYEE"))
                {
                    TheComboBoxEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboBoxEmployeeDataSet.employees.Rows.Count;

                    if (intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboBoxEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Productivity Data Entry Reports // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsaProblem = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            string strErrorMessage = "";

            try
            {
                TheProductivityDataEntryDataSet.dataentry.Rows.Clear();

                strValueForValidation = txtStartDate.Text;
                blnThereIsaProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsaProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                    datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsaProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsaProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                    datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
                    datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
                }
                if (gstrReportType == "EMPLOYEE")
                {
                    if (cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Employee Was Not Selected\n";
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is After the End Date");
                        return;
                    }
                }

                if (gstrReportType == "DATE RANGE")
                {
                    TheFindProductivityDataEntryByDateRangeDataSet = TheProductivityDataEntryClass.FindProductivityDataEntbyDateRange(datStartDate, datEndDate);

                    intNumberOfRecords = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange.Rows.Count - 1;

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.InformationMessage("No Records Found");
                        return;
                    }
                    else
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            ProductivityDataEntryDataSet.dataentryRow NewTransactionRow = TheProductivityDataEntryDataSet.dataentry.NewdataentryRow();

                            NewTransactionRow.EntryDate = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].EntryDate;
                            NewTransactionRow.FirstName = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].FirstName;
                            NewTransactionRow.LastName = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].LastName;
                            NewTransactionRow.ProjectID = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].AssignedProjectID;
                            NewTransactionRow.ProjectName = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].ProjectName;
                            NewTransactionRow.TransactionID = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].TransactionID;

                            TheProductivityDataEntryDataSet.dataentry.Rows.Add(NewTransactionRow);
                        }
                    }
                }
                else if (gstrReportType == "EMPLOYEE")
                {
                    TheFindProductivityDataEntryByEmployeeIDDataSet = TheProductivityDataEntryClass.FindProductivityDataEntryByEmployeeID(gintEmployeeID, datStartDate, datEndDate);

                    intNumberOfRecords = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID.Rows.Count - 1;

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.InformationMessage("No Records Found");
                        return;
                    }
                    else
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            ProductivityDataEntryDataSet.dataentryRow NewTransactionRow = TheProductivityDataEntryDataSet.dataentry.NewdataentryRow();

                            NewTransactionRow.EntryDate = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].EntryDate;
                            NewTransactionRow.FirstName = TheComboBoxEmployeeDataSet.employees[cboSelectEmployee.SelectedIndex - 1].FirstName;
                            NewTransactionRow.LastName = TheComboBoxEmployeeDataSet.employees[cboSelectEmployee.SelectedIndex - 1].LastName;
                            NewTransactionRow.ProjectID = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].AssignedProjectID;
                            NewTransactionRow.ProjectName = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].ProjectName;
                            NewTransactionRow.TransactionID = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].TransactionID;

                            TheProductivityDataEntryDataSet.dataentry.Rows.Add(NewTransactionRow);
                        }
                    }
                }

                dgrResults.ItemsSource = TheProductivityDataEntryDataSet.dataentry;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Productivity Data Entry Report // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboBoxEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
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
                intRowNumberOfRecords = TheProductivityDataEntryDataSet.dataentry.Rows.Count;
                intColumnNumberOfRecords = TheProductivityDataEntryDataSet.dataentry.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProductivityDataEntryDataSet.dataentry.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProductivityDataEntryDataSet.dataentry.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Productivity Data Entry Report // Export To Excel " + ex.Message);

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
