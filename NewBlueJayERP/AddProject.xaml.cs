/* Title:           Add Project
 * Date:            1-27-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for adding a project */

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
using ProjectsDLL;
using ProductionProjectDLL;
using NewEventLogDLL;
using DataValidationDLL;
using DesignProjectsDLL;
using NewEmployeeDLL;
using DepartmentDLL;
using WorkOrderDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddProject.xaml
    /// </summary>
    public partial class AddProject : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DesignProjectsClass TheDesignProjectsClass = new DesignProjectsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsbyAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatusSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrxiByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();

        //setting up variables
        int gintDepartmentID;
        int gintManagerID;
        int gintOfficeID;
        int gintStatusID;
        int gintProjectID;
        bool gblnProjectExists;
        bool gblnProjectMatrixExists;

        public AddProject()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
            expHelp.IsExpanded = false;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting up the variabvles
            int intCounter;
            int intNumberOfRecords;

            txtAssignedProjectID.Text = "";
            txtCustomerProjectID.Text = "";
            ClearDateEntryControls();

            //loading up the combo boxes
            cboSelectManager.Items.Clear();

            TheFindProductionManagersDataSet = TheEmployeeClass.FindProductionManagers();
            cboSelectManager.Items.Add("Select Manager");
            intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(TheFindProductionManagersDataSet.FindProductionManagers[intCounter].FullName);
            }

            cboSelectManager.SelectedIndex = 0;

            cboSelectDepartment.Items.Clear();
            cboSelectDepartment.Items.Add("Select Department");

            TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboSelectDepartment.SelectedIndex = 0;

            cboSelectOffice.Items.Clear();
            cboSelectOffice.Items.Add("Select Office");

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectOffice.SelectedIndex = 0;

            cboSelectStatus.Items.Clear();
            cboSelectStatus.Items.Add("Select Status");

            TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();
            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
            }

            cboSelectStatus.SelectedIndex = 0;
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].DepartmentID;
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintManagerID = TheFindProductionManagersDataSet.FindProductionManagers[intSelectedIndex].EmployeeID;
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintOfficeID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
        }

        private void cboSelectStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintStatusID = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intSelectedIndex].StatusID;
        }
        private void ClearDateEntryControls()
        {
            txtExistingAssignedProjectID.Text = "";
            txtExistingProjectName.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtDateReceived.Text = "";
            txtECDDate.Text = "";
            txtPRojectNotes.Text = "";
            txtProjectName.Text = "";
            cboSelectDepartment.SelectedIndex = 0;
            cboSelectManager.SelectedIndex = 0;
            cboSelectOffice.SelectedIndex = 0;
            cboSelectStatus.SelectedIndex = 0;
            gblnProjectExists = false;
            gblnProjectMatrixExists = false;
        }       

        private void expProecess_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMesssage = "";
            string strAddress;
            string strCity;
            string strState;
            string strValueForValidation;
            DateTime datDateReceived = DateTime.Now;
            DateTime datECDDate = DateTime.Now;
            string strProjectNotes;
            string strAssignedProjectID = "";
            string strProjectName = "";
            string strCustomerProjectID = "";
            DateTime datTransactionDate = DateTime.Now;
            int intEmployeeID;

            try
            {
                strCustomerProjectID = txtCustomerProjectID.Text;
                strAssignedProjectID = txtAssignedProjectID.Text;
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                if(gblnProjectExists == false)
                {
                    if ((strAssignedProjectID.Length <  7) && (strAssignedProjectID.Length > 10))
                    {
                        blnFatalError = true;
                        strErrorMesssage += "The Assigned Project ID is not the Correct Format\n";
                    }
                }
                if (gblnProjectMatrixExists == false)
                { 
                    
                    if(strCustomerProjectID.Length < 5)
                    {
                        blnFatalError = true;
                        strErrorMesssage += "The Customer Project ID is not the Correct Format\n";
                    }
                    strProjectName = txtProjectName.Text;
                    if(strProjectName.Length < 10)
                    {
                        blnFatalError = true;
                        strErrorMesssage += "The Project Name is to Short\n";
                    }
                }
                expProecess.IsExpanded = false;
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The Department was not Selected\n";
                }
                strAddress = txtAddress.Text;
                if(strAddress.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The Address Was Not Entered\n";
                }
                strCity = txtCity.Text;
                if(strCity.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The City is to Short\n";
                }
                strState = txtState.Text;
                if(strState.Length != 2)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The State is not the Correct Length\n";
                }
                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The Manager Was Not Selected\n";
                }
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The Office Was Not Selected\n";
                }
                strValueForValidation = txtDateReceived.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The Date Received is not a Date\n";
                }
                else
                {
                    datDateReceived = Convert.ToDateTime(strValueForValidation);
                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datDateReceived, DateTime.Now);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMesssage += "The Date Received is after Today\n";
                    }
                }
                strValueForValidation = txtECDDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMesssage += "The ECD Date is not a Date\n";
                }
                else
                {
                    datECDDate = Convert.ToDateTime(strValueForValidation);
                }
                strProjectNotes = txtPRojectNotes.Text;
                if(strProjectNotes.Length < 1)
                {
                    strProjectNotes = "NO NOTES ENTERED";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMesssage);
                    return;
                }

                if(gblnProjectExists == false)
                {
                    blnFatalError = TheProjectClass.InsertProject(strCustomerProjectID, strProjectName);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strCustomerProjectID);

                    gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                    blnFatalError = TheProjectMatrixClass.InsertProjectMatrix(gintProjectID, strAssignedProjectID, strCustomerProjectID, datTransactionDate, intEmployeeID, gintOfficeID, gintDepartmentID);
                }

                blnFatalError = TheProductionProjectClass.InsertProdutionProject(gintProjectID, gintDepartmentID, strAddress, strCity, strState, gintManagerID, gintOfficeID, datDateReceived, datECDDate, gintStatusID, strProjectNotes);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError =TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Project Number " + strAssignedProjectID + " Has Been Added");

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Project Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Project // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void txtCustomerProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strCustomerProjectID;
            int intLength;
            int intNumberOfRecords;
            int intRecordsReturned;

            try
            {
                strCustomerProjectID = txtCustomerProjectID.Text;
                intLength = strCustomerProjectID.Length;
                ClearDateEntryControls();

                if (intLength > 3)
                {
                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        txtAssignedProjectID.Text = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].AssignedProjectID;
                        gblnProjectExists = true;
                    }
                    else
                    {
                        gblnProjectExists = false;
                    }
                    
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strCustomerProjectID);

                    intNumberOfRecords = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        txtExistingAssignedProjectID.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].AssignedProjectID;
                        txtExistingProjectName.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                        txtProjectName.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                        gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                        TheFindProductionProjectByAssignedProjectIDDataSet = TheProductionProjectClass.FindProductionProjectsByAssignedProjectID(strCustomerProjectID);

                        intNumberOfRecords = TheFindProductionProjectByAssignedProjectIDDataSet.FindProductionProjectByAssignedProjectID.Rows.Count;

                        if (intNumberOfRecords > 0)
                        {
                            TheMessagesClass.ErrorMessage("Project Already Exists");
                            ResetControls();
                            return;
                        }

                        TheFindDesignProjectsbyAssignedProjectIDDataSet = TheDesignProjectsClass.FindDesignProjectsByAssignedProjectID(strCustomerProjectID);

                        intNumberOfRecords = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID.Rows.Count;

                        if (intNumberOfRecords > 0)
                        {
                            txtAddress.Text = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectAddress;
                            txtCity.Text = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].City;
                        }

                        TheMessagesClass.InformationMessage("The Project Exists, but is Missing Information.  Please Conplete The Form");
                        gblnProjectMatrixExists = true;
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Project // Customer Project ID Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
