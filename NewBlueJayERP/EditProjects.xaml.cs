/* Title:           Edit Projects
 * Date:            10-1-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for editing a project */

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
using AssignedTasksDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditProjects.xaml
    /// </summary>
    public partial class EditProjects : Window
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

        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsbyAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatusSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrxiByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();

        //setting up variables
        int gintDepartmentID;
        int gintManagerID;
        int gintOfficeID;
        int gintStatusID;
        int gintProjectID;
        bool gblnProjectExists;
        bool gblnDoNotRun;
        int gintTransactionID;
        string gstrAssignedProjectID;

        public EditProjects()
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
            this.Visibility = Visibility.Hidden;
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

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
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
            SetControlsReadOnly(false);
            gblnDoNotRun = false;

            //loading up the combo boxes
            cboSelectManager.Items.Clear();

            TheFindProductionManagersDataSet = TheEmployeeClass.FindProductionManagers();
            cboSelectManager.Items.Add("Select Manager");
            intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(TheFindProductionManagersDataSet.FindProductionManagers[intCounter].FullName);
            }

            cboSelectManager.SelectedIndex = 0;

            cboSelectDepartment.Items.Clear();
            cboSelectDepartment.Items.Add("Select Department");

            TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboSelectDepartment.SelectedIndex = 0;

            cboSelectOffice.Items.Clear();
            cboSelectOffice.Items.Add("Select Office");

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectOffice.SelectedIndex = 0;

            cboSelectStatus.Items.Clear();
            cboSelectStatus.Items.Add("Select Status");

            TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();
            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
            }

            cboSelectStatus.SelectedIndex = 0;
        }
        private void ClearDateEntryControls()
        {
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
            gblnDoNotRun = false;
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtAddress.IsReadOnly = blnValueBoolean;
            txtAssignedProjectID.IsReadOnly = blnValueBoolean;
            txtCity.IsReadOnly = blnValueBoolean;
            txtDateReceived.IsReadOnly = blnValueBoolean;
            txtECDDate.IsReadOnly = blnValueBoolean;
            txtProjectName.IsReadOnly = blnValueBoolean;
            txtPRojectNotes.IsReadOnly = blnValueBoolean;
            txtState.IsReadOnly = blnValueBoolean;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtCustomerProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strCustomerProjectID;
            int intLength;
            int intRecordsReturned;

            try
            {
                strCustomerProjectID = txtCustomerProjectID.Text;
                gstrAssignedProjectID = strCustomerProjectID;
                intLength = strCustomerProjectID.Length;

                if(((intLength > 5) && (intLength < 12)) && gblnDoNotRun == false)
                {
                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;

                        gblnProjectExists = true;

                        FillControls();                        
                    }
                }
                else if (intLength >= 12)
                {
                    TheMessagesClass.ErrorMessage("The Project is not the Correct Format");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Project // Customer Project ID Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FillControls()
        {
            //setting up local variables
            int intCounter;
            int intNumberOfRecords;
            int intDepartmentID;
            int intManagerID;
            int intOfficeID;
            int intStatusID;
            int intSelectedIndex = 0;

            TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(gintProjectID);
            TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(gintProjectID);
            TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);
            gintTransactionID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].TransactionID;
            gblnDoNotRun = true;

            if (gblnProjectExists == true)
            {
                txtAssignedProjectID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;                
            }
                

            if (gblnProjectExists == false)
            {
                txtCustomerProjectID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
            }
                

            txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
            txtAddress.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].BusinessAddress;
            txtCity.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].City;
            txtDateReceived.Text = Convert.ToString(TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].DateReceived);
            txtECDDate.Text = Convert.ToString(TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ECDDate);
            txtPRojectNotes.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectNotes;
            txtState.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].BusinessState;

            intManagerID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectManagerID;
            intStatusID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;
            intDepartmentID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].DepartmentID;
            intOfficeID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].WarehouseID;

            //loading comboboxes
            intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if (intManagerID == TheFindProductionManagersDataSet.FindProductionManagers[intCounter].EmployeeID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectManager.SelectedIndex = intSelectedIndex;

            intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(intDepartmentID == TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].DepartmentID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectDepartment.SelectedIndex = intSelectedIndex;

            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(intOfficeID == TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectOffice.SelectedIndex = intSelectedIndex;

            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(intStatusID == TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].StatusID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectStatus.SelectedIndex = intSelectedIndex;
        }
        private void txtAssignedProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strAssignedProjectID;
            int intLength;
            int intRecordsReturned;

            try
            {
                strAssignedProjectID = txtAssignedProjectID.Text;
                gstrAssignedProjectID = strAssignedProjectID;
                intLength = strAssignedProjectID.Length;

                if (((intLength > 5) && (intLength < 12)) && gblnDoNotRun == false)
                {
                    TheFindProjectMatrxiByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectMatrxiByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        gintProjectID = TheFindProjectMatrxiByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;

                        gblnProjectExists = false;

                        FillControls();
                    }
                }
                else if (intLength >= 12)
                {
                    TheMessagesClass.ErrorMessage("The Project is not the Correct Format");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Project // Customer Project ID Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expCheckProject_Expanded(object sender, RoutedEventArgs e)
        {
            string strCustomerProjectID;
            int intRecordsReturned;

            strCustomerProjectID = txtCustomerProjectID.Text;

            TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

            intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

            if(intRecordsReturned < 1)
            {
                TheMessagesClass.ErrorMessage("Project Not Found");
                return;
            }
        }

        private void expUpdateStatus_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError;

            try
            {
                expUpdateStatus.IsExpanded = false;

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectStatus(gintTransactionID, gintStatusID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Project Status has been Updated");

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Projects // Project Status Change For Project " + gstrAssignedProjectID);

                if (blnFatalError == true)
                    throw new Exception();

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Projects // Update Status Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
