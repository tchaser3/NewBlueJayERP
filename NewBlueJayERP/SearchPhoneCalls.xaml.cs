/* Title:           Search Phones Calls
 * Date:            9-16-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for Searching Phone Calls */

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
using EmployeeDateEntryDLL;
using CellPhoneCallsDLL;
using PhonesDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for SearchPhoneCalls.xaml
    /// </summary>
    public partial class SearchPhoneCalls : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        PhonesClass ThePhonesClass = new PhonesClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindPhoneByExtensionDataSet TheFindPhoneByExtensionDataSet = new FindPhoneByExtensionDataSet();
        FindPhoneCallsForCallerDataSet TheFindPhoneCallsForCallerDataSet = new FindPhoneCallsForCallerDataSet();
        FindPhoneCallsForExtensionDataSet TheFindPhoneCallsForExtensionDataSet = new FindPhoneCallsForExtensionDataSet();
        FindPhoneCallsForEmployeeDataSet TheFindPhoneCallsForEmployee = new FindPhoneCallsForEmployeeDataSet();

        string gstrReportType;
        string gstrPhoneNumber;
        int gintExtension;
        int gintEmployeeID;
        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public SearchPhoneCalls()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsEnabled = false;
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
            txtEnterInfo.Text = "";
            txtStartDate.Text = "";

            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;

            cboReportType.Items.Clear();
            cboReportType.Items.Add("Select Report Type");
            cboReportType.Items.Add("Phone Number");
            cboReportType.Items.Add("Extension");
            cboReportType.Items.Add("Employee");
            cboReportType.SelectedIndex = 0;

            lblEnterInfo.Content = "Enter Last Four";

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Search PHone Calls");
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearTextBoxes();

            if (cboReportType.SelectedIndex == 1)
            {
                gstrReportType = "PHONE";
                lblEnterInfo.Content = "Enter Last Four";
                cboSelectEmployee.Visibility = Visibility.Hidden;
                lblSelectEmployee.Visibility = Visibility.Hidden;
            }
            else if(cboReportType.SelectedIndex == 2)
            {
                gstrReportType = "EXT";
                lblEnterInfo.Content = "Enter Extension";
                cboSelectEmployee.Visibility = Visibility.Hidden;
                lblSelectEmployee.Visibility = Visibility.Hidden;
            }
            else if (cboReportType.SelectedIndex == 3)
            {
                gstrReportType = "EMPLOYEE";
                lblEnterInfo.Content = "Last Name";
                cboSelectEmployee.Visibility = Visibility.Visible;
                lblSelectEmployee.Visibility = Visibility.Visible;
            }
            else
            {
                gstrReportType = "PHONE";
                lblEnterInfo.Content = "Enter Last Four";
                cboSelectEmployee.Visibility = Visibility.Hidden;
                lblSelectEmployee.Visibility = Visibility.Hidden;
            }
        }

        private void txtEnterInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will load up the controls for the search
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strInfoEntered;
            bool blnFatalError;

            try
            {
                strInfoEntered = txtEnterInfo.Text;

                if(gstrReportType == "PHONE")
                {
                    if(strInfoEntered.Length == 4)
                    {
                        gstrPhoneNumber = strInfoEntered;
                    }
                    else if(strInfoEntered.Length > 4)
                    {
                        TheMessagesClass.ErrorMessage("There are Two Many Digits");
                        return;
                    }
                }
                else if(gstrReportType == "EXT")
                {
                    if (strInfoEntered.Length == 4)
                    {
                        blnFatalError = TheDataValidationClass.VerifyIntegerData(strInfoEntered);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The Extension Entered is not Numeric");
                            return;
                        }
                        else
                        {
                            gintExtension = Convert.ToInt32(strInfoEntered);

                            TheFindPhoneByExtensionDataSet = ThePhonesClass.FindPhoneByExtension(gintExtension);

                            intNumberOfRecords = TheFindPhoneByExtensionDataSet.FindPhoneByExtension.Rows.Count;

                            if(intNumberOfRecords < 1)
                            {
                                TheMessagesClass.ErrorMessage("The Extension Entered does not Exist");
                                return;
                            }
                        }
                    }
                    else if (strInfoEntered.Length > 4)
                    {
                        TheMessagesClass.ErrorMessage("There are Two Many Digits");
                        return;
                    }
                }
                else if(gstrReportType == "EMPLOYEE")
                {
                    if(strInfoEntered.Length > 2)
                    {
                        TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strInfoEntered);

                        intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                        if(intNumberOfRecords < 1)
                        {
                            TheMessagesClass.ErrorMessage("Employee Was Not Found");
                            return;
                        }

                        cboSelectEmployee.Items.Clear();
                        cboSelectEmployee.Items.Add("Select Employee");

                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboSelectEmployee.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Search Phone Calls // Enter Info Text Box " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strValueForValuation;

            try
            {
                //data validation
                if(cboReportType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Report Type Was Not Selected\n";
                }
                if(gstrReportType == "EMPLOYEE")
                {
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                }
                strValueForValuation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValuation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValuation);
                }
                strValueForValuation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValuation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValuation);
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

                if (gstrReportType == "PHONE")
                {
                    TheFindPhoneCallsForCallerDataSet = TheCellPhoneCallsClass.FindPhoneCallsForCaller(gstrPhoneNumber, gdatStartDate, gdatEndDate);

                    dgrPhoneCalls.ItemsSource = TheFindPhoneCallsForCallerDataSet.FindPhoneCallsForCaller;
                }
                else if(gstrReportType == "EXT")
                {
                    TheFindPhoneCallsForExtensionDataSet = TheCellPhoneCallsClass.FindPhoneCallsForExtension(gintExtension, gdatStartDate, gdatEndDate);

                    dgrPhoneCalls.ItemsSource = TheFindPhoneCallsForExtensionDataSet.FindPhoneCallsForExtension;
                }
                else if(gstrReportType == "EMPLOYEE")
                {
                    TheFindPhoneCallsForEmployee = TheCellPhoneCallsClass.FindPhoneCallsForEmployee(gintEmployeeID, gdatStartDate, gdatEndDate);

                    dgrPhoneCalls.ItemsSource = TheFindPhoneCallsForEmployee.FindPhoneCallByEmployee;
                }
                

            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Search Phone Calls // Process Button " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }
        private void ClearTextBoxes()
        {
            txtEndDate.Text = "";
            txtEnterInfo.Text = "";
            txtStartDate.Text = "";
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            expExportToExcel.IsExpanded = false;

            if (gstrReportType == "PHONE")
            {
                ExportPhoneExcel();
            }
            else if (gstrReportType == "EXT")
            {
                ExportExtensionExcel();
            }
            else if (gstrReportType == "EMPLOYEE")
            {
                ExportEmployeeExcel();
            }
        }
        private void ExportEmployeeExcel()
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
                intRowNumberOfRecords = TheFindPhoneCallsForEmployee.FindPhoneCallByEmployee.Rows.Count;
                intColumnNumberOfRecords = TheFindPhoneCallsForEmployee.FindPhoneCallByEmployee.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindPhoneCallsForEmployee.FindPhoneCallByEmployee.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindPhoneCallsForEmployee.FindPhoneCallByEmployee.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Phone Call Search // Export Employee Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportExtensionExcel()
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
                intRowNumberOfRecords = TheFindPhoneCallsForExtensionDataSet.FindPhoneCallsForExtension.Rows.Count;
                intColumnNumberOfRecords = TheFindPhoneCallsForExtensionDataSet.FindPhoneCallsForExtension.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindPhoneCallsForExtensionDataSet.FindPhoneCallsForExtension.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindPhoneCallsForExtensionDataSet.FindPhoneCallsForExtension.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Phone Call Search // Export Extension Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportPhoneExcel()
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
                intRowNumberOfRecords = TheFindPhoneCallsForCallerDataSet.FindPhoneCallsForCaller.Rows.Count;
                intColumnNumberOfRecords = TheFindPhoneCallsForCallerDataSet.FindPhoneCallsForCaller.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindPhoneCallsForCallerDataSet.FindPhoneCallsForCaller.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindPhoneCallsForCallerDataSet.FindPhoneCallsForCaller.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Phone Call Search // Export Phone Excel " + ex.Message);

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
