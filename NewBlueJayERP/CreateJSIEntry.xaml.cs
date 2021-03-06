﻿/* Title:           Create JSI Entry
 * Date:            4-27-20
 * Author:          Terry Holmes
 * 
 * Description:     This is the beginning of the process */

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
using DataValidationDLL;
using JSIMainDLL;
using VehicleMainDLL;
using DepartmentDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateJSIEntry.xaml
    /// </summary>
    public partial class CreateJSIEntry : Window
    {
        //setting up the classes
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        JSIMainClass TheJSIMainClass = new JSIMainClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //setting up the data
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        JSIEmployeeAssignedDataSet TheJSIEmployeeAssignedDataSet = new JSIEmployeeAssignedDataSet();
        FindJSIMainByDateMatchDataSet TheFindJSIMainByDateMatchDataSet = new FindJSIMainByDateMatchDataSet();
        FindEmployeesForDataEntryForLastYearDataSet TheFindEmployeesForDataEntryForLastYearDataSet = new FindEmployeesForDataEntryForLastYearDataSet();
        FindEmployeesForDataEntryForLastYearDataSet TheFindInspectorEmployeesForDataEntryForLastYearDataEntryDataSet = new FindEmployeesForDataEntryForLastYearDataSet();
        FindEmployeeManagerForLastYearDataSet TheFindEmployeeManagerForLastYearDataSet = new FindEmployeeManagerForLastYearDataSet();

        public CreateJSIEntry()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ResetControls()
        {
            //setting up variables
            int intCounter;
            int intNumberOfRecords;
            DateTime datStartDate = DateTime.Now;

            cboSelectDepartment.Items.Clear();
            cboSelectEmployee.Items.Clear();
            cboSelectInspector.Items.Clear();
            cboSelectManager.Items.Clear();
            cboSelectDepartment.Items.Add("Select Department");
            cboSelectManager.Items.Add("Select Manager");

            txtAssignedProjectID.Text = "";
            txtInspectionDate.Text = "";
            txtInspectorLTName.Text = "";
            txtLastName.Text = "";
            txtVehicleNumber.Text = "";
            TheJSIEmployeeAssignedDataSet.jsiemployeeassigned.Rows.Clear();

            datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 365);

            //loading the combos
            TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboSelectDepartment.SelectedIndex = 0;

            TheFindEmployeeManagerForLastYearDataSet = TheEmployeeClass.FindEmployeeManagerForLastYear(datStartDate);

            intNumberOfRecords = TheFindEmployeeManagerForLastYearDataSet.FindEmployeeManagersForLastYear.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(TheFindEmployeeManagerForLastYearDataSet.FindEmployeeManagersForLastYear[intCounter].FullName);
            }

            cboSelectManager.SelectedIndex = 0;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            bool blnItemFound = false;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                intEmployeeID = TheFindEmployeesForDataEntryForLastYearDataSet.FindEmployeesForDataEntryForLastYear[intSelectedIndex].EmployeeID;

                intNumberOfRecords = TheJSIEmployeeAssignedDataSet.jsiemployeeassigned.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(intEmployeeID == TheJSIEmployeeAssignedDataSet.jsiemployeeassigned[intCounter].EmployeeID)
                    {
                        blnItemFound = true;
                    }
                }

                if(blnItemFound == true)
                {
                    TheMessagesClass.InformationMessage("Employee Already Added");
                }
                else
                {
                    JSIEmployeeAssignedDataSet.jsiemployeeassignedRow NewEmployeeRow = TheJSIEmployeeAssignedDataSet.jsiemployeeassigned.NewjsiemployeeassignedRow();

                    NewEmployeeRow.EmployeeID = intEmployeeID;
                    NewEmployeeRow.FirstName = TheFindEmployeesForDataEntryForLastYearDataSet.FindEmployeesForDataEntryForLastYear[intSelectedIndex].FirstName;
                    NewEmployeeRow.LastName = TheFindEmployeesForDataEntryForLastYearDataSet.FindEmployeesForDataEntryForLastYear[intSelectedIndex].LastName; 

                    TheJSIEmployeeAssignedDataSet.jsiemployeeassigned.Rows.Add(NewEmployeeRow);

                    dgrAssignedEmployees.ItemsSource = TheJSIEmployeeAssignedDataSet.jsiemployeeassigned;
                }
            }
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up local variables
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            string strLastName;
            string strAssignedProjectID;
            int intRecordsReturned;
            DateTime datStartDate = DateTime.Now;

            try
            {
                strLastName = txtLastName.Text;
                intLength = strLastName.Length;
                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 365);

                if(intLength > 2)
                {
                    //getting the project id
                    strAssignedProjectID = txtAssignedProjectID.Text;

                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strAssignedProjectID);

                        intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            MainWindow.gintProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                        }
                        else if(intRecordsReturned < 1)
                        {
                            TheMessagesClass.ErrorMessage("The Assigned Project ID Does Not Exist");
                            return;
                        }
                            
                        
                    }
                    else
                    {
                        MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    }

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheFindEmployeesForDataEntryForLastYearDataSet = TheEmployeeClass.FindEmployeesForDataEntryForLastYear(datStartDate, strLastName);

                    intNumberOfRecords = TheFindEmployeesForDataEntryForLastYearDataSet.FindEmployeesForDataEntryForLastYear.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Was Not Found");
                        cboSelectEmployee.SelectedIndex = 0;
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheFindEmployeesForDataEntryForLastYearDataSet.FindEmployeesForDataEntryForLastYear[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create JSI Entry // Last Name Text Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //checking vehicle number
            string strVehicleNumber;
            int intLength;
            int intRecordsReturned;

            try
            {
                strVehicleNumber = txtVehicleNumber.Text;
                intLength = strVehicleNumber.Length;

                if(intLength == 4)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheMessagesClass.InformationMessage("Vehicle Found");
                    }
                }
                else if(intLength == 6)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }
                    else if(intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                }
                else if(intLength > 6)
                {
                    TheMessagesClass.ErrorMessage("This is not the Correct Format for a Vehicle");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create JSI Entry // Vehicle Number Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedID;

            intSelectedID = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedID > -1)
                MainWindow.gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedID].DepartmentID;
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedID;

            intSelectedID = cboSelectManager.SelectedIndex - 1;

            if (intSelectedID > -1)
                MainWindow.gintManagerID = TheFindEmployeeManagerForLastYearDataSet.FindEmployeeManagersForLastYear[intSelectedID].EmployeeID;
        }

        private void cboSelectInspector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectInspector.SelectedIndex - 1;

            if(intSelectedIndex > -1)
                MainWindow.gintInspectingEmployeeID = TheFindEmployeeManagerForLastYearDataSet.FindEmployeeManagersForLastYear[intSelectedIndex].EmployeeID;
        }

        private void txtInspectorLTName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            DateTime datStartDate = DateTime.Now;

            try
            {
                strLastName = txtInspectorLTName.Text;
                intLength = strLastName.Length;
                cboSelectInspector.Items.Clear();
                cboSelectInspector.Items.Add("Select Inspector");

                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 365);

                if(intLength > 2)
                {
                    TheFindInspectorEmployeesForDataEntryForLastYearDataEntryDataSet = TheEmployeeClass.FindEmployeesForDataEntryForLastYear(datStartDate, strLastName);

                    intNumberOfRecords = TheFindInspectorEmployeesForDataEntryForLastYearDataEntryDataSet.FindEmployeesForDataEntryForLastYear.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Inspector Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectInspector.Items.Add(TheFindInspectorEmployeesForDataEntryForLastYearDataEntryDataSet.FindEmployeesForDataEntryForLastYear[intCounter].FullName);
                    }

                    cboSelectInspector.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create JSI Entry // Inspector LT Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnMainProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will insert into the table
            string strValueForValidation = "";
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strAssignedProjectID;
            string strVehicleNumber;
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;
            DateTime datInspectionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;

            try
            {
                strAssignedProjectID = txtAssignedProjectID.Text;
                if(strAssignedProjectID == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Was Not Entered\n";
                }
                else
                {
                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strAssignedProjectID);

                        intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            MainWindow.gintProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                        }
                        else if(intRecordsReturned < 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Project Was Not Found\n";
                        }
                    }
                    else
                    {
                        MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    }

                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                strVehicleNumber = txtVehicleNumber.Text;
                if(strVehicleNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Number Was Not Entered\n";
                }
                else
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Vehicle Number Was Not Found\n";
                        
                    }
                    else
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                }
                strValueForValidation = txtInspectionDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Inspection Date is not a Date\n";
                }
                else
                {
                    datInspectionDate = Convert.ToDateTime(strValueForValidation);

                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datInspectionDate, DateTime.Now);

                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Inspection is after Today\n";
                    }
                }
                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager Was Not Selected\n";
                }
                if(cboSelectInspector.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Inspector Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                MainWindow.gdatInspectionDate = datInspectionDate;

                intNumberOfRecords = TheJSIEmployeeAssignedDataSet.jsiemployeeassigned.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("No Employees Were Added");
                    return;
                }

                MainWindow.gintEmployeeID = TheJSIEmployeeAssignedDataSet.jsiemployeeassigned[0].EmployeeID;

                blnFatalError = TheJSIMainClass.InsertIntoJSIMain(datTransactionDate, MainWindow.gintProjectID, MainWindow.gintEmployeeID, MainWindow.gintDepartmentID, MainWindow.gintVehicleID, datInspectionDate, MainWindow.gintManagerID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, MainWindow.gintInspectingEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindJSIMainByDateMatchDataSet = TheJSIMainClass.FindJSIMainByDateMatch(datTransactionDate);

                MainWindow.gintJSITransationID = TheFindJSIMainByDateMatchDataSet.FindJSIMainByDateMatch[0].JSITransactionID;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnFatalError = TheJSIMainClass.InsertJSIEmployee(MainWindow.gintJSITransationID, TheJSIEmployeeAssignedDataSet.jsiemployeeassigned[intCounter].EmployeeID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Create JSI Entry // JSI Created");

                if (blnFatalError == true)
                    throw new Exception();

                JSIPPEWindow JSIPPEWindow = new JSIPPEWindow();
                JSIPPEWindow.ShowDialog();

                ResetControls();
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create JSI Entry // Main Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
