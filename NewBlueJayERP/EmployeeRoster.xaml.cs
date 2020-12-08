/* Title:           Employee Roster
 * Date:            12-7-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for the employee roster */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeRoster.xaml
    /// </summary>
    public partial class EmployeeRoster : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindActiveEmployeesDataSet TheFindActiveEmployeeDataSet = new FindActiveEmployeesDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        ActiveEmployeeDataSet TheActiveEmployeeDataSet = new ActiveEmployeeDataSet();

        public EmployeeRoster()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strEmployee;
            string strPhoneNumber;
            string strEmailAddress;
            string strDepartment;
            string strLocation;
            string strManager;
            int intManagerID;

            try
            {
                TheActiveEmployeeDataSet.activeemployees.Rows.Clear();

                TheFindActiveEmployeeDataSet = TheEmployeeClass.FindActiveEmployees();

                intNumberOfRecords = TheFindActiveEmployeeDataSet.FindActiveEmployees.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    strEmailAddress = "";
                    intManagerID = TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].ManagerID;
                    strEmployee = TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].FirstName + " ";
                    strEmployee += TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].LastName;
                    strPhoneNumber = TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].PhoneNumber;
                    
                    if(TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].IsEmailAddressNull() == false)
                    {
                        strEmailAddress = TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].EmailAddress;                     
                    }

                    strDepartment = TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].Department;
                    strLocation = TheFindActiveEmployeeDataSet.FindActiveEmployees[intCounter].HomeOffice;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                    strManager = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                    strManager += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                    ActiveEmployeeDataSet.activeemployeesRow NewEmployeeRow = TheActiveEmployeeDataSet.activeemployees.NewactiveemployeesRow();

                    NewEmployeeRow.Department = strDepartment;
                    NewEmployeeRow.EmailAddress = strEmailAddress;
                    NewEmployeeRow.EmployeeName = strEmployee;
                    NewEmployeeRow.Location = strLocation;
                    NewEmployeeRow.Manager = strManager;
                    NewEmployeeRow.PhoneNumber = strPhoneNumber;

                    TheActiveEmployeeDataSet.activeemployees.Rows.Add(NewEmployeeRow);
                }

                dgrEmployees.ItemsSource = TheActiveEmployeeDataSet.activeemployees;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Employee Roster");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Roster // Reset Controls " + Ex.Message);

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
                intRowNumberOfRecords = TheActiveEmployeeDataSet.activeemployees.Rows.Count;
                intColumnNumberOfRecords = TheActiveEmployeeDataSet.activeemployees.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheActiveEmployeeDataSet.activeemployees.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheActiveEmployeeDataSet.activeemployees.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Vehicle Roster // Export To Excel " + ex.Message);

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
