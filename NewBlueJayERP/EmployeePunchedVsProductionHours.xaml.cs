/* Title:           Employee Punched Vs Production Hours
 * Date:            3-3-21
 * Author:          Terry Holmes
 * 
 * Description:     This is the form reporting employee pounched verus */

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
using EmployeeProjectAssignmentDLL;
using EmployeePunchedHoursDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using DataValidationDLL;
using Microsoft.Win32;
using EmployeeDateEntryDLL;
using DesignProductivityDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeePunchedVsProductionHours.xaml
    /// </summary>
    public partial class EmployeePunchedVsProductionHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DesignProductivityClass TheDesignProductivityClass = new DesignProductivityClass();

        //setting up the data
        EmployeeProductionPunchDataSet TheEmployeeProductionPunchDataSet = new EmployeeProductionPunchDataSet();
        FindActiveNonExemptEmployeesByPayDateDataSet TheFindActiveNoExemptEmployeesDataSet = new FindActiveNonExemptEmployeesByPayDateDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindEmployeeProductionHoursOverPayPeriodDataSet TheFindEmployeeProductionHoursOverPayPeriodDataSet = new FindEmployeeProductionHoursOverPayPeriodDataSet();
        FindEmployeePunchedHoursDataSet TheFindEmployeePunchedHoursDataSet = new FindEmployeePunchedHoursDataSet();
        FindDesignTotalEmployeeProductivityHoursDataSet TheFindDesignTotalEmployeeProductivityHoursDataSet = new FindDesignTotalEmployeeProductivityHoursDataSet();
        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        FindManagerEmployeesByPayDateDataSet TheFindManagerEmployeesByPayDateDataSet = new FindManagerEmployeesByPayDateDataSet();
        

        public EmployeePunchedVsProductionHours()
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
            //setting up controls
            TheEmployeeProductionPunchDataSet.employees.Rows.Clear();

            dgrResults.ItemsSource = TheEmployeeProductionPunchDataSet.employees;

            txtEnterPayPeriod.Text = "";
            rdoAllEmployees.IsChecked = false;
            rdoSelectManager.IsChecked = false;

            lblSelectManger.Visibility = Visibility.Hidden;
            cboSelectManager.Visibility = Visibility.Hidden;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Employee Punched VS Production");
        }

        private void rdoAllEmployees_Checked(object sender, RoutedEventArgs e)
        {
            //setting up variables
            string strValueForValidation;
            bool blnFatalError = false;
            DateTime datPayPeriod = DateTime.Now;
            DateTime datStartDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intManagerID;
            string strFirstName;
            string strLastName;
            string strManagerFirstName;
            string strManagerLastName;
            int intRecordReturned;
            decimal decHoursPunched;
            decimal decProductiveHours = 0;

            try
            {
                TheEmployeeProductionPunchDataSet.employees.Rows.Clear();
                lblSelectManger.Visibility = Visibility.Hidden;
                cboSelectManager.Visibility = Visibility.Hidden;

                strValueForValidation = txtEnterPayPeriod.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date is not a Date");
                    return;
                }

                datPayPeriod = Convert.ToDateTime(strValueForValidation);
                datStartDate = TheDateSearchClass.SubtractingDays(datPayPeriod, 6);

                TheFindActiveNoExemptEmployeesDataSet = TheEmployeeClass.FindActiveNonExemptEmployeesByPayDate(datPayPeriod);

                intNumberOfRecords = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //getting employee info
                    intEmployeeID = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].EmployeeID;
                    strFirstName = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].FirstName;
                    strLastName = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].LastName;
                    intManagerID = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].ManagerID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                    strManagerFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    strManagerLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                    //getting employee punched hours
                    TheFindEmployeePunchedHoursDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHours(intEmployeeID, datStartDate, datPayPeriod);

                    intRecordReturned = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours.Rows.Count;

                    if (intRecordReturned == 0)
                    {
                        decHoursPunched = 0;
                    }
                    else
                    {
                        decHoursPunched = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours[0].PunchedHours;
                    }

                    //getting production hours
                    TheFindEmployeeProductionHoursOverPayPeriodDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProductionHoursOverPayPeriodDataSet(intEmployeeID, datStartDate, datPayPeriod);

                    intRecordReturned = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod.Rows.Count;

                    if (intRecordReturned == 0)
                    {
                        decProductiveHours = 0;
                    }
                    else
                    {
                        decProductiveHours = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod[0].ProductionHours;
                    }

                    TheFindDesignTotalEmployeeProductivityHoursDataSet = TheDesignProductivityClass.FindDesignTotalEmployeeProductivityHours(intEmployeeID, datStartDate, datPayPeriod);

                    intRecordReturned = TheFindDesignTotalEmployeeProductivityHoursDataSet.FindDesignTotalEmployeeProductivityHours.Rows.Count;

                    if (intRecordReturned > 0)
                    {
                        decProductiveHours += TheFindDesignTotalEmployeeProductivityHoursDataSet.FindDesignTotalEmployeeProductivityHours[0].TotalHours;
                    }

                    //loading the dataset
                    EmployeeProductionPunchDataSet.employeesRow NewEmployeeRow = TheEmployeeProductionPunchDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.HomeOffice = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].HomeOffice;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.ManagerFirstName = strManagerFirstName;
                    NewEmployeeRow.ManagerLastName = strManagerLastName;
                    NewEmployeeRow.ProductionHours = decProductiveHours;
                    NewEmployeeRow.PunchedHours = decHoursPunched;
                    NewEmployeeRow.HourVariance = decProductiveHours - decHoursPunched;

                    TheEmployeeProductionPunchDataSet.employees.Rows.Add(NewEmployeeRow);
                }

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Employee Punched VS Production Hours // All Employees");

                if (blnFatalError == true)
                    throw new Exception();

                dgrResults.ItemsSource = TheEmployeeProductionPunchDataSet.employees;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Punched Vs Production Hours // All Employees Radio Button " + Ex.Message);

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
                intRowNumberOfRecords = TheEmployeeProductionPunchDataSet.employees.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeProductionPunchDataSet.employees.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProductionPunchDataSet.employees.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProductionPunchDataSet.employees.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Punched vs Production Hours // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void rdoSelectManager_Checked(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                lblSelectManger.Visibility = Visibility.Visible;
                cboSelectManager.Visibility = Visibility.Visible;

                TheEmployeeProductionPunchDataSet.employees.Rows.Clear();

                dgrResults.ItemsSource = TheEmployeeProductionPunchDataSet.employees;

                cboSelectManager.Items.Clear();
                cboSelectManager.Items.Add("Select Manager");

                TheFindProductionManagersDataSet = TheEmployeeClass.FindProductionManagers();

                intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectManager.Items.Add(TheFindProductionManagersDataSet.FindProductionManagers[intCounter].FullName);
                }

                cboSelectManager.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Punched Vs Production Hours // Select Manager Radio Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            DateTime datPayPeriod = DateTime.Now;
            DateTime datStartDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intManagerID;
            string strFirstName;
            string strLastName;
            string strManagerFirstName;
            string strManagerLastName;
            int intRecordReturned;
            decimal decHoursPunched;
            decimal decProductiveHours = 0;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectManager.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    TheEmployeeProductionPunchDataSet.employees.Rows.Clear();

                    strValueForValidation = txtEnterPayPeriod.Text;
                    blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Date is not a Date");
                        return;
                    }

                    datPayPeriod = Convert.ToDateTime(strValueForValidation);
                    datStartDate = TheDateSearchClass.SubtractingDays(datPayPeriod, 6);

                    intManagerID = TheFindProductionManagersDataSet.FindProductionManagers[intSelectedIndex].EmployeeID;
                    strManagerFirstName = TheFindProductionManagersDataSet.FindProductionManagers[intSelectedIndex].FirstName;
                    strManagerLastName = TheFindProductionManagersDataSet.FindProductionManagers[intSelectedIndex].LastName;

                    TheFindManagerEmployeesByPayDateDataSet = TheEmployeeClass.FindManagerEmployeesByPayDate(intManagerID, datPayPeriod);

                    intNumberOfRecords = TheFindManagerEmployeesByPayDateDataSet.FindManagerEmployeesByPayDate.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        //getting employee info
                        intEmployeeID = TheFindManagerEmployeesByPayDateDataSet.FindManagerEmployeesByPayDate[intCounter].EmployeeID;
                        strFirstName = TheFindManagerEmployeesByPayDateDataSet.FindManagerEmployeesByPayDate[intCounter].FirstName;
                        strLastName = TheFindManagerEmployeesByPayDateDataSet.FindManagerEmployeesByPayDate[intCounter].LastName;

                        //getting employee punched hours
                        TheFindEmployeePunchedHoursDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHours(intEmployeeID, datStartDate, datPayPeriod);

                        intRecordReturned = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours.Rows.Count;

                        if (intRecordReturned == 0)
                        {
                            decHoursPunched = 0;
                        }
                        else
                        {
                            decHoursPunched = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours[0].PunchedHours;
                        }

                        //getting production hours
                        TheFindEmployeeProductionHoursOverPayPeriodDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProductionHoursOverPayPeriodDataSet(intEmployeeID, datStartDate, datPayPeriod);

                        intRecordReturned = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod.Rows.Count;

                        if (intRecordReturned == 0)
                        {
                            decProductiveHours = 0;
                        }
                        else
                        {
                            decProductiveHours = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod[0].ProductionHours;
                        }

                        TheFindDesignTotalEmployeeProductivityHoursDataSet = TheDesignProductivityClass.FindDesignTotalEmployeeProductivityHours(intEmployeeID, datStartDate, datPayPeriod);

                        intRecordReturned = TheFindDesignTotalEmployeeProductivityHoursDataSet.FindDesignTotalEmployeeProductivityHours.Rows.Count;

                        if (intRecordReturned > 0)
                        {
                            decProductiveHours += TheFindDesignTotalEmployeeProductivityHoursDataSet.FindDesignTotalEmployeeProductivityHours[0].TotalHours;
                        }

                        //loading the dataset
                        EmployeeProductionPunchDataSet.employeesRow NewEmployeeRow = TheEmployeeProductionPunchDataSet.employees.NewemployeesRow();

                        NewEmployeeRow.HomeOffice = TheFindManagerEmployeesByPayDateDataSet.FindManagerEmployeesByPayDate[intCounter].HomeOffice;
                        NewEmployeeRow.FirstName = strFirstName;
                        NewEmployeeRow.LastName = strLastName;
                        NewEmployeeRow.ManagerFirstName = strManagerFirstName;
                        NewEmployeeRow.ManagerLastName = strManagerLastName;
                        NewEmployeeRow.ProductionHours = decProductiveHours;
                        NewEmployeeRow.PunchedHours = decHoursPunched;
                        NewEmployeeRow.HourVariance = decProductiveHours - decHoursPunched;

                        TheEmployeeProductionPunchDataSet.employees.Rows.Add(NewEmployeeRow);
                    }

                    blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Employee Punched VS Production Hours // Report By Manager");

                    if (blnFatalError == true)
                        throw new Exception();

                    dgrResults.ItemsSource = TheEmployeeProductionPunchDataSet.employees;



                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Punched VS Productivity Hours // Select Manager Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
    }
}
