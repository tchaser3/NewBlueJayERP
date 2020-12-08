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
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
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
            ClearRadioButtons();
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

            TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

            intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
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
            

            EnableRadioButtons(false);

            if((MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "ADMIN") || (MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "IT"))
            {
                EnableRadioButtons(true);
            }
            else if(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "OFFICE")
            {
                rdoOpen.IsEnabled = true;
            }
            else if(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "MANAGERS")
            {
                rdoOnHold.IsEnabled = true;
                rdoCancel.IsEnabled = true;
                rdoInProcess.IsEnabled = true;
                rdoConComplete.IsEnabled = true;
                rdoSubmitted.IsEnabled = true;
            }
        }
        private void EnableRadioButtons(bool blnValueBoolean)
        {
            rdoCancel.IsEnabled = blnValueBoolean;
            rdoClosed.IsEnabled = blnValueBoolean;
            rdoConComplete.IsEnabled = blnValueBoolean;
            rdoInProcess.IsEnabled = blnValueBoolean;
            rdoInvoiced.IsEnabled = blnValueBoolean;
            rdoOnHold.IsEnabled = blnValueBoolean;
            rdoOpen.IsEnabled = blnValueBoolean;
            rdoSubmitted.IsEnabled = blnValueBoolean;
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
            {
                gintDepartmentID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;
            }
                
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
            int intRecordsReturned;

            try
            {
                TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(gintProjectID);
                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(gintProjectID);
                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);

                intRecordsReturned = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Project Is Not Completely Entered, Please Go To Add Project");
                    return;
                }

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

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (intManagerID == TheFindProductionManagersDataSet.FindProductionManagers[intCounter].EmployeeID)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectManager.SelectedIndex = intSelectedIndex;

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (intDepartmentID == TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].DepartmentID)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectDepartment.SelectedIndex = intSelectedIndex;

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (intOfficeID == TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectOffice.SelectedIndex = intSelectedIndex;

                //setting up the buttons
                ClearRadioButtons();

                if(intStatusID == 1001)
                {
                    rdoOpen.IsChecked = true;
                }
                else if(intStatusID == 1002)
                {
                    rdoConComplete.IsChecked = true;
                }
                else if (intStatusID == 1003)
                {
                    rdoOnHold.IsChecked = true;
                }
                else if (intStatusID == 1004)
                {
                    rdoCancel.IsChecked = true;
                }
                else if (intStatusID == 1005)
                {
                    rdoInProcess.IsChecked = true;
                }
                else if (intStatusID == 1006)
                {
                    rdoClosed.IsChecked = true;
                }
                else if (intStatusID == 1007)
                {
                    rdoInvoiced.IsChecked = true;
                }
                else if (intStatusID == 1008)
                {
                    rdoSubmitted.IsChecked = true;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Projects // Fill Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }
        private void ClearRadioButtons()
        {
            rdoCancel.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoConComplete.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoSubmitted.IsChecked = false;
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
                    else if(intLength > 7)
                    {
                        TheMessagesClass.ErrorMessage("Project Not Found, Please Add the Project");
                        return;
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
            else
            {
                gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;

                FillControls();
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

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strCustomerProjectID;
            string strAssignedProjectID;
            string strProjectName;
            string strAddress;
            string strCity;
            string strState;
            DateTime datECDDate = DateTime.Now;
            string strProjectNotes;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            string strValueForValidation;
            int intTransactionID = 0;

            try
            {
                expProcess.IsExpanded = false;

                strCustomerProjectID = txtCustomerProjectID.Text;
                if(strCustomerProjectID.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Customer Project ID was not Found\n";
                }
                else
                {
                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Customer Project ID Was not Found\n";
                    }
                    else if(intRecordsReturned > 0)
                    {
                        gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                        intTransactionID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].TransactionID;
                    }
                }
                strAssignedProjectID = txtAssignedProjectID.Text;
                if(strAssignedProjectID.Length < 7)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Assigned Project ID Was not Entered\n";
                }
                strProjectName = txtProjectName.Text;
                if(strProjectName.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Name was not Long Enough\n";
                }
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                strAddress = txtAddress.Text;
                if(strAddress.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Address is not Long Enough\n";
                }
                strCity = txtCity.Text;
                if(strCity.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The City was not Entered\n";
                }
                strState = txtState.Text;
                if (strState.Length < 2)
                {
                    blnFatalError = true;
                    strErrorMessage += "The State was not Entered\n";
                }
                if (cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager Was Not Selected\n";
                }
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Was Not Selected\n";
                }
                strValueForValidation = txtECDDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The ECD Date is not a Date\n";
                }
                else
                {
                    datECDDate = Convert.ToDateTime(strValueForValidation);
                }
                strProjectNotes = txtPRojectNotes.Text;
                if(strProjectNotes.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Notes are not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheProjectMatrixClass.UpdateProjectMatrixAssignedProjectID(intTransactionID, strAssignedProjectID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProjectMatrixClass.UpdateProjectMatrixItems(intTransactionID, gintOfficeID, gintDepartmentID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProjectClass.UpdateProjectProject(gintProjectID, strCustomerProjectID, strProjectName);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);

                intRecordsReturned = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    throw new Exception();
                }

                intTransactionID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].TransactionID;

                blnFatalError = TheProductionProjectClass.UpdateProductionProject(intTransactionID, gintDepartmentID, strAddress, strCity, strState, gintManagerID, gintOfficeID, datECDDate, strProjectNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Project Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Projects // Proces Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void rdoOpen_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1001;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoInProcess_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1005;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoCancel_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1004;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoOnHold_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1003;
            rdoConComplete.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoConComplete_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1002;
            rdoOpen.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoSubmitted_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1008;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoOpen.IsChecked = false;
        }

        private void rdoInvoiced_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1007;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoClosed_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1006;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }
    }
}
