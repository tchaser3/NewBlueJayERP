/* Title:           Edit Employee
 * Date:            12-15-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to edit an employee*/

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
using DepartmentDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditEmployee.xaml
    /// </summary>
    public partial class EditEmployee : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindAllEmployeesByLastNameDataSet TheFindAllEmployeesByLastNameDataSet = new FindAllEmployeesByLastNameDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeID = new FindEmployeeByEmployeeIDDataSet();
        FindSortedEmployeeGroupDataSet TheFindSortedEmployeeGroupDataSet = new FindSortedEmployeeGroupDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();

        //setting global variables
        bool gblnActive;
        string gstrHomeOffice;
        string gstrEmployeeType;
        string gstrGroup;
        string gstrDepartment;
        string gstrSalaryType;
        int gintManagerID;

        public EditEmployee()
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
            //setting up local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError;

            try
            {
                //clearing the text boxes
                txtEnterLastName.Text = "";
                txtEmailAddress.Text = "";
                txtEmployeeID.Text = "";
                txtEndDate.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtPayID.Text = "";
                txtPhoneNumber.Text = "";
                txtStartDate.Text = "";

                //setting up the combo boxes
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;

                //setting up the active combo box
                cboActive.Items.Clear();
                cboActive.Items.Add("Select Active");
                cboActive.Items.Add("Yes");
                cboActive.Items.Add("No");
                cboActive.SelectedIndex = 0;

                //setting up the group combo box
                cboGroup.Items.Clear();
                cboGroup.Items.Add("Select Group");

                TheFindSortedEmployeeGroupDataSet = TheEmployeeClass.FindSortedEmpoyeeGroup();

                intNumberOfRecords = TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboGroup.Items.Add(TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup[intCounter].GroupName);
                }

                cboGroup.SelectedIndex = 0;

                //setting ho the home office combo box
                cboHomeOffice.Items.Clear();
                cboHomeOffice.Items.Add("Select Home Office");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboHomeOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboHomeOffice.SelectedIndex = 0;

                //setting employee type
                cboEmployeeType.Items.Clear();
                cboEmployeeType.Items.Add("Select Employee Type");
                cboEmployeeType.Items.Add("EMPLOYEE");
                cboEmployeeType.Items.Add("CONTRACTOR");
                cboEmployeeType.SelectedIndex = 0;

                //setting up the salary type
                cboSalaryType.Items.Clear();
                cboSalaryType.Items.Add("Select Salary Type");
                cboSalaryType.Items.Add("EXEMPT");
                cboSalaryType.Items.Add("NON-EXEMPT");
                cboSalaryType.SelectedIndex = 0;

                //setting up the combo box for departments
                cboDepartment.Items.Clear();
                cboDepartment.Items.Add("Select Department");

                TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

                intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
                }

                cboDepartment.SelectedIndex = 0;

                //setting up the managers
                cboManager.Items.Clear();
                cboManager.Items.Add("Select Managers");

                TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

                intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboManager.Items.Add(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
                }

                cboManager.SelectedIndex = 0;

                EnableControls(false);

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Employee");

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Employee // Reset Control " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void EnableControls(bool blnValueBoolean)
        {
            txtFirstName.IsEnabled = blnValueBoolean;
            txtLastName.IsEnabled = blnValueBoolean;
            txtPhoneNumber.IsEnabled = blnValueBoolean;
            txtEmailAddress.IsEnabled = blnValueBoolean;
            txtPayID.IsEnabled = blnValueBoolean;
            txtStartDate.IsEnabled = blnValueBoolean;
            txtEndDate.IsEnabled = blnValueBoolean;
            cboActive.IsEnabled = blnValueBoolean;
            cboGroup.IsEnabled = blnValueBoolean;
            cboHomeOffice.IsEnabled = blnValueBoolean;
            cboEmployeeType.IsEnabled = blnValueBoolean;
            cboSalaryType.IsEnabled = blnValueBoolean;
            cboDepartment.IsEnabled = blnValueBoolean;
            cboManager.IsEnabled = blnValueBoolean;
        }

        private void cboActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboActive.SelectedIndex == 1)
            {
                gblnActive = true;
            }
            else if(cboActive.SelectedIndex == 2)
            {
                gblnActive = false;
            }
        }

        private void cboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboGroup.SelectedIndex > 0)
            {
                gstrGroup = cboGroup.SelectedItem.ToString();
            }
        }

        private void cboHomeOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboHomeOffice.SelectedIndex > 0)
            {
                gstrHomeOffice = cboHomeOffice.SelectedItem.ToString();
            }
        }

        private void cboEmployeeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboEmployeeType.SelectedIndex > 0)
            {
                gstrEmployeeType = cboEmployeeType.SelectedItem.ToString();
            }
        }

        private void cboSalaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSalaryType.SelectedIndex > 0)
            {
                gstrSalaryType = cboSalaryType.SelectedItem.ToString();
            }
        }

        private void cboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboDepartment.SelectedIndex > 0)
            {
                gstrDepartment = cboDepartment.SelectedItem.ToString();
            }
        }

        private void cboManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboManager.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;

                if(strLastName.Length > 2)
                {
                    TheFindAllEmployeesByLastNameDataSet = TheEmployeeClass.FindAllEmployeesByLastName(strLastName);

                    intNumberOfRecords = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName.Rows.Count;

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Employee // Enter Employee Last Name " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex = 0;
            int intEmployeeID;
            int intCouter;
            int intNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].EmployeeID;

                    TheFindEmployeeByEmployeeID = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                    EnableControls(true);

                    txtEmployeeID.Text = Convert.ToString(intEmployeeID);
                    txtFirstName.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].FirstName;
                    txtLastName.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].LastName;
                    txtPhoneNumber.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].PhoneNumber;

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                    {
                        txtEmailAddress.Text = "";
                    }
                    else if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == false)
                    {
                        txtEmailAddress.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EmailAddress;
                    }

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsPayIDNull() == true)
                    {
                        txtPayID.Text = "";
                    }
                    else if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsPayIDNull() == false)
                    {
                        txtPayID.Text = Convert.ToString(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].PayID);
                    }

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsStartDateNull() == true)
                    {
                        txtStartDate.Text = "";
                    }
                    else if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsStartDateNull() == false)
                    {
                        txtStartDate.Text = Convert.ToString(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].StartDate);
                    }

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsEndDateNull() == true)
                    {
                        txtStartDate.Text = "";
                    }
                    else if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsEndDateNull() == false)
                    {
                        txtEndDate.Text = Convert.ToString(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EndDate);
                    }

                    if (TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].Active == true)
                        cboActive.SelectedIndex = 1;
                    else if (TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].Active == false)
                        cboActive.SelectedIndex = 2;

                    intNumberOfRecords = cboHomeOffice.Items.Count;

                    for(intCouter = 0; intCouter < intNumberOfRecords; intCouter++)
                    {
                        cboHomeOffice.SelectedIndex = intCouter;

                        if (TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].HomeOffice == cboHomeOffice.SelectedItem.ToString())
                        {
                            intSelectedIndex = intCouter;
                        }
                    }

                    cboHomeOffice.SelectedIndex = intSelectedIndex;

                    intNumberOfRecords = cboGroup.Items.Count;

                    for(intCouter = 0; intCouter < intNumberOfRecords; intCouter++)
                    {
                        cboGroup.SelectedIndex = intCouter;

                        if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EmployeeGroup == cboGroup.SelectedItem.ToString())
                        {
                            intSelectedIndex = intCouter;
                        }
                    }

                    cboGroup.SelectedIndex = intSelectedIndex;

                    intNumberOfRecords = cboEmployeeType.Items.Count;

                    for(intCouter = 0; intCouter < intNumberOfRecords; intCouter++)
                    {
                        cboEmployeeType.SelectedIndex = intCouter;

                        if (TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EmployeeType == cboEmployeeType.SelectedItem.ToString()) 
                        {
                            intSelectedIndex = intCouter;
                        }
                    }

                    cboEmployeeType.SelectedIndex = intSelectedIndex;

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsSalaryTypeNull() == false)
                    {
                        intNumberOfRecords = cboSalaryType.Items.Count;

                        for(intCouter = 0; intCouter < intNumberOfRecords; intCouter++)
                        {
                            cboSalaryType.SelectedIndex = intCouter;

                            if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].SalaryType == cboSalaryType.SelectedItem.ToString())
                            {
                                intSelectedIndex = intCouter;
                            }
                        }

                        cboSalaryType.SelectedIndex = intSelectedIndex;
                    }
                    else
                    {
                        cboSalaryType.SelectedIndex = 0;
                    }

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsDepartmentNull() == false)
                    {
                        intNumberOfRecords = cboDepartment.Items.Count;

                        for(intCouter = 0; intCouter < intNumberOfRecords; intCouter++)
                        {
                            cboDepartment.SelectedIndex = intCouter;

                            if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].Department == cboDepartment.SelectedItem.ToString())
                            {
                                intSelectedIndex = intCouter;
                            }
                        }

                        cboDepartment.SelectedIndex = intSelectedIndex;
                    }
                    else
                    {
                        cboDepartment.SelectedIndex = 0;
                    }

                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsManagerIDNull() == false)
                    {
                        intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count;

                        for(intCouter = 0; intCouter < intNumberOfRecords; intCouter++)
                        {
                            if(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCouter].employeeID == TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].ManagerID)
                            {
                                intSelectedIndex = intCouter + 1;
                            }
                        }

                        cboManager.SelectedIndex = intSelectedIndex;
                    }
                    else
                    {
                        cboManager.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Employee // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void btnUpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intEmployeeID = 0;
            string strFirstName;
            string strLastName;
            string strPhoneNumber;
            string strEmailAddress;
            int intPayID = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                //performing data validation
                strValueForValidation = txtEmployeeID.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee ID is not an Integer\n";
                }
                else
                {
                    intEmployeeID = Convert.ToInt32(strValueForValidation);
                }
                strFirstName = txtFirstName.Text;
                if(strFirstName.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The First Name is not Long Enough\n";
                }
                strLastName = txtLastName.Text;
                if(strLastName.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Last Name is not Long Enough\n";
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Phone Number is not the Phone Number\n";
                }
                strEmailAddress = txtEmailAddress.Text;
                if(strEmailAddress.Length > 3)
                {
                    blnThereIsAProblem = TheDataValidationClass.VerifyEmailAddress(strEmailAddress);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Email Address is not the Correct Format\n";
                    }
                }
                else if(strEmailAddress.Length < 4)
                {
                    strEmailAddress = "";
                }
                strValueForValidation = txtPayID.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerRange(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Pay ID is not an Integer\n";
                }
                else
                {
                    intPayID = Convert.ToInt32(strValueForValidation);
                }
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
                if(cboActive.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Active Was Not Selected\n";
                }
                if (cboGroup.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Group Was Not Selected\n";
                }
                if (cboHomeOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Home Office Was Not Selected\n";
                }
                if (cboEmployeeType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Type Was Not Selected\n";
                }
                if (cboSalaryType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Salary Type Was Not Selected\n";
                }
                if (cboDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                if (cboManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheEmployeeClass.UpdateEmployee(intEmployeeID, strFirstName, strLastName, strPhoneNumber, gblnActive, gstrGroup, gstrHomeOffice, gstrEmployeeType, strEmailAddress, gstrSalaryType, gstrDepartment, gintManagerID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeEndDate(intEmployeeID, datEndDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeStartDate(intEmployeeID, datStartDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeePayInformation(intEmployeeID, gstrDepartment, gstrSalaryType, gintManagerID, intPayID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Employee Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Employee // Update Employee Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
