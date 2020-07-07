/* Title:           Submit After Hours Work
 * Date:            6-10-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to submit an after hours employee */

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
using AfterHoursWorkDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using VehicleMainDLL;
using ProjectsDLL;
using DataValidationDLL;
using DepartmentDLL;
using TrailerCurrentLoadDLL;
using System.Net;
using Excel = Microsoft.Office.Interop.Excel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for SubmitAfterHoursWork.xaml
    /// </summary>
    public partial class SubmitAfterHoursWork : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AfterHoursWorkClass TheAfterHoursClass = new AfterHoursWorkClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        ProjectClass TheProjectClass = new ProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        AfterWorkEmployeesDataSet TheAfterWorkEmployeesDataSet = new AfterWorkEmployeesDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindEmployeeOverNightWorkbyDataEntryDateMatchDataSet TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet = new FindEmployeeOverNightWorkbyDataEntryDateMatchDataSet();
        FindDepartmentByDepartmentIDDataSet TheFindDepartmentByDepartmentIDDataSet = new FindDepartmentByDepartmentIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindVehicleMainByVehicleIDDataSet ThefindVehicleMainByVehicleIDDataSet = new FindVehicleMainByVehicleIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();

        //setting up global variables
        bool gblnVehicleFound;

        public SubmitAfterHoursWork()
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
            //setting variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtInETA.Text = "";
                txtLastName.Text = "";
                txtOutTime.Text = "";
                txtProjectID.Text = "";
                txtVehicleNumber.Text = "";
                txtWorkDate.Text = "";
                txtWorkLocation.Text = "";
                gblnVehicleFound = false;

                TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Clear();

                dgrAssignedEmployees.ItemsSource = TheAfterWorkEmployeesDataSet.afterhoursemployees;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");

                intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
                }

                cboSelectDepartment.SelectedIndex = 0;

                cboSelectEmployee.Items.Clear();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Select Office Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].DepartmentID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Select Office Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtLastName.Text;

                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Was Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Last Name Text Box Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intEmployeeID;
            string strFirstName;
            string strLastName;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    strFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    strLastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;

                    AfterWorkEmployeesDataSet.afterhoursemployeesRow NewEmployeeRow = TheAfterWorkEmployeesDataSet.afterhoursemployees.NewafterhoursemployeesRow();

                    NewEmployeeRow.EmployeeID = intEmployeeID;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;

                    TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Add(NewEmployeeRow);

                    dgrAssignedEmployees.ItemsSource = TheAfterWorkEmployeesDataSet.afterhoursemployees;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Select Employe Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up the variables
            string strVehicleNumber;
            int intRecordsReturned;
            int intLength;

            gblnVehicleFound = false;

            strVehicleNumber = txtVehicleNumber.Text;
            intLength = strVehicleNumber.Length;

            if(intLength == 4 )
            {
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheMessagesClass.InformationMessage("Vehicle was Found");

                    gblnVehicleFound = true;
                }
            }
            else if(intLength == 6)
            {
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheMessagesClass.InformationMessage("Vehicle was Found");

                    gblnVehicleFound = true;
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Vehicle Was Not Found");
                    return;
                }
            }
            else if (intLength > 6)
            {
                TheMessagesClass.ErrorMessage("Vehicle Was Not Found, To Many Characters");
                return;
            }
        }

        private void expResetForm_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expSubmitForm_Expanded(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datWorkDate = DateTime.Now;
            string strOutTime;
            string strProjectID;
            string strWorkLocation;
            string strInETA;
            int intRecordsReturned;
            DateTime datDataEntryDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intManagerID;

            try
            {
                expSubmitForm.IsExpanded = false;

                //beginning data validation
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office was not Selected\n";
                }
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department was not Selected\n";
                }
                if(TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Count < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employees were not Added\n";
                }
                if(gblnVehicleFound == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Vehicle was not Added\n";
                }
                strValueForValidation = txtWorkDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date is not a Date\n";
                }
                else
                {
                    datWorkDate = Convert.ToDateTime(strValueForValidation);
                }
                strOutTime = txtOutTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTime(strOutTime);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Out Time is not a Time\n";
                }
                strProjectID = txtProjectID.Text;
                if(strProjectID == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project ID was not Entered\n";
                }
                else
                {
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Project Was Not Entered\n";
                    }
                    else
                    {
                        MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                    }
                }
                strWorkLocation = txtWorkLocation.Text;
                if(strWorkLocation == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Location Was Not Entered\n";
                }
                strInETA = txtInETA.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTime(strInETA);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The ETA Time In is not a Time\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intNumberOfRecords = TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Count - 1;
                intManagerID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheAfterWorkEmployeesDataSet.afterhoursemployees[intCounter].EmployeeID;

                    blnFatalError = TheAfterHoursClass.InsertEmployeeOverNightWork(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID, intEmployeeID, intManagerID, MainWindow.gintVehicleID, MainWindow.gintProjectID, datWorkDate, strOutTime, strWorkLocation, strInETA, datDataEntryDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet = TheAfterHoursClass.FindEmployeeOverNightWorkByDataEntryDateMatch(datDataEntryDate);

                CreateMessage();

                TheMessagesClass.InformationMessage("The Report has been Sent\n");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Submit Form Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void CreateMessage()
        {
            int intCounter;
            int intNumberOfRecords;
            string strMessage;
            string strHeader = "After Hours Work Log";
            string strDepartment;
            string strVehicleNumber;
            string strProjectID;
            int intOfficeID;
            string strOffice;
            string strEmployee;
            string strWorkDate;
            string strOutTime;
            string strWorkLocation;
            string strInETA;
            bool blnFatalError;
            int intEmployeeID;
            string strEmailAddress;

            try
            {
                intNumberOfRecords = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch.Rows.Count - 1;

                strMessage = "<h1>After Hours Work Log</h1>";
                strMessage += "<table>";
                strMessage += "<tr>";
                strMessage += "<td><b>Office</b></td>";
                strMessage += "<td><b>Department</b></td>";
                strMessage += "<td><b>Employee</b></td>";                
                strMessage += "<td><b>Vehicle</b></td>";
                strMessage += "<td><b>Date</b></td>";
                strMessage += "<td><b>Out Time</b></td>";
                strMessage += "<td><b>Project</b></td>";
                strMessage += "<td><b>Work Location</b></td>";
                strMessage += "<td><b>In ETA</b></td>";
                strMessage += "</tr>";
                strMessage += "<p>      </p>";
                strMessage += "<p>      </p>";

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intOfficeID = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].OfficeID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intOfficeID);

                    strOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    strDepartment = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].Department;
                    strEmployee = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].FirstName + " ";
                    strEmployee += TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].LastName;
                    strVehicleNumber = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].VehicleNumber;
                    strProjectID = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].AssignedProjectID;
                    strWorkDate = Convert.ToString(TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].WorkDate);
                    strOutTime = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].OutTime;
                    strWorkLocation = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].WorkLocation;
                    strInETA = TheFindEmployeeOverNightWorkByDataEntryDateMatchDataSet.FindEmployeeOverNightWorkByDataEntryDateMatch[intCounter].InETA;

                    strMessage += "<tr>";
                    strMessage += "<td>" + strOffice + "</td>";
                    strMessage += "<td>" + strDepartment + "</td>";
                    strMessage += "<td>" + strEmployee + "</td>";
                    strMessage += "<td>" + strVehicleNumber + "</td>";
                    strMessage += "<td>" + strWorkDate + "</td>";
                    strMessage += "<td>" + strOutTime + "</td>";
                    strMessage += "<td>" + strProjectID + "</td>";
                    strMessage += "<td>" + strWorkLocation + "</td>";
                    strMessage += "<td>" + strInETA + "</td>";
                    strMessage += "</tr>";
                }

                strMessage += "</table>";

                blnFatalError = TheSendEmailClass.SendEmail("afterhourswork@bluejaycommunications.com", strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                strEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                blnFatalError = TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Submit After Hours Work // Create Message " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
    }
}
