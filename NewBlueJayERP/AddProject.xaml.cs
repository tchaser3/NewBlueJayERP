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
using System.Runtime.Serialization;
using ProductionProjectDLL.FindProdutionProjectsByAssignedProjectIDDataSetTableAdapters;
using ProjectNumberAssignmentDLL;
using ATTProjectNumberAssignmentDLL;

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
        ProjectNumberAssignment TheProjectNumberAssignmentClass = new ProjectNumberAssignment();
        ATTProjectNumberAssignmentClass TheATTProjectNumberAssignmentClass = new ATTProjectNumberAssignmentClass();

        //setting up the data
        
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsbyAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
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
        bool gblnProjectMatrixExists;
        bool gblnOver2500;

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
            int intSelectedIndex = 0;

            txtAssignedProjectID.Text = "";
            txtCustomerProjectID.Text = "";
            ClearDateEntryControls();
            SetControlsReadOnly(false);
            expProecess.IsEnabled = true;

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

            TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

            intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
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

            cboSelectStatus.IsEnabled = true;

            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);

                if(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus == "OPEN")
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectStatus.SelectedIndex = intSelectedIndex;

            cboSelectStatus.IsEnabled = false;

            rdoOverNo.Visibility = Visibility.Hidden;
            rdoOverYes.Visibility = Visibility.Hidden;

        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strAssignedProjectID;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1) 
            {
                gintDepartmentID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;

                if((gintDepartmentID != 1009) && (gintDepartmentID != 1010))
                {
                    strAssignedProjectID = TheProjectNumberAssignmentClass.CreateProjectNumberAssignment();

                    txtAssignedProjectID.Text = strAssignedProjectID;
                }
                else
                {
                    rdoOverNo.Visibility = Visibility.Visible;
                    rdoOverYes.Visibility = Visibility.Visible;
                }
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
                    else if(gintDepartmentID == 1009)
                    {
                        if((strCustomerProjectID.Length < 6) && (strCustomerProjectID.Length > 7))
                        {
                            blnFatalError = true;
                            strErrorMesssage += "The Spectrum Project Length is not the Corret Length\n";
                        }
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

                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(DateTime.Now, datECDDate);

                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMesssage += "The ECD Date is before today\n";
                    }
                }
                strProjectNotes = txtPRojectNotes.Text;
                if(strProjectNotes.Length < 1)
                {
                    strProjectNotes = "PROJECT CREATED";
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

                    blnFatalError = TheProductionProjectClass.InsertProdutionProject(gintProjectID, gintDepartmentID, strAddress, strCity, strState, gintManagerID, gintOfficeID, datDateReceived, datECDDate, gintStatusID, strProjectNotes);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProductionProjectClass.InsertProductionProjectUpdate(gintProjectID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now, strProjectNotes);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Project Number " + strAssignedProjectID + " Has Been Added");

                    if (blnFatalError == true)
                        throw new Exception();
                }
                if (gblnProjectMatrixExists == false)
                {
                    blnFatalError = TheProjectMatrixClass.InsertProjectMatrix(gintProjectID, strAssignedProjectID, strCustomerProjectID, datTransactionDate, intEmployeeID, gintOfficeID, gintDepartmentID);

                    if (blnFatalError == true)
                        throw new Exception();

                }
                if(gblnProjectExists == true)
                {
                    TheFindProductionProjectByAssignedProjectIDDataSet = TheProductionProjectClass.FindProductionProjectsByAssignedProjectID(strCustomerProjectID);

                    if(TheFindProductionProjectByAssignedProjectIDDataSet.FindProductionProjectByAssignedProjectID.Rows.Count < 1)
                    {
                        blnFatalError = TheProductionProjectClass.InsertProdutionProject(gintProjectID, gintDepartmentID, strAddress, strCity, strState, gintManagerID, gintOfficeID, datDateReceived, datECDDate, gintStatusID, strProjectNotes);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Project Number " + strAssignedProjectID + " Has Been Added");

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else
                    {
                        TheFindProductionProjectByAssignedProjectIDDataSet = TheProductionProjectClass.FindProductionProjectsByAssignedProjectID(strAssignedProjectID);

                        if (TheFindProductionProjectByAssignedProjectIDDataSet.FindProductionProjectByAssignedProjectID.Rows.Count < 1)
                        {
                            blnFatalError = TheProductionProjectClass.InsertProdutionProject(gintProjectID, gintDepartmentID, strAddress, strCity, strState, gintManagerID, gintOfficeID, datDateReceived, datECDDate, gintStatusID, strProjectNotes);

                            if (blnFatalError == true)
                                throw new Exception();                            
                        }
                    }                   

                }

                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;

                if(TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].BusinessLineID == 1009)
                {
                    AddProductionProjectInfo AddProductionProjectInfo = new AddProductionProjectInfo();
                    AddProductionProjectInfo.ShowDialog();
                }
                       
                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Project Number " + strAssignedProjectID + " Has Been Added");

                if (blnFatalError == true)
                    throw new Exception();

                AddProjectDocumentation();

                TheMessagesClass.InformationMessage("Project Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Project // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void AddProjectDocumentation()
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheProductionProjectClass.InsertProductionProjectDocumentation(MainWindow.gintProjectID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now, strDocumentPath);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessagesClass.InformationMessage("The Documents have been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Project // Add Project Documentation Method " + Ex.Message);

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
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intManagerID;
            int intDepartmentID;
            int intOfficeID;
            int intSelectedIndex = 0;
            int intStatusID;
            string strCustomerIDProjectID;
            string strAssignedProjectID;

            try
            {
                strCustomerProjectID = txtCustomerProjectID.Text;
                intLength = strCustomerProjectID.Length;
                //ClearDateEntryControls();

                if (intLength > 3)
                {
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strCustomerProjectID);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                        intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            gblnProjectExists = false;
                        }
                        else if(intRecordsReturned > 0)
                        {
                            txtAssignedProjectID.Text = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].AssignedProjectID;
                        }
                    }
                    else
                    {
                        gblnProjectExists = true;

                        gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                        TheFindDesignProjectsbyAssignedProjectIDDataSet = TheDesignProjectsClass.FindDesignProjectsByAssignedProjectID(strCustomerProjectID);

                        intRecordsReturned = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            txtAddress.Text = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectAddress;
                            txtCity.Text = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].City;
                            txtProjectName.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                            txtDateReceived.Text = Convert.ToString(TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].DateReceived);

                            intOfficeID = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].OfficeID;

                            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                if(intOfficeID == TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                                {
                                    intSelectedIndex = intCounter + 1;
                                    cboSelectOffice.SelectedIndex = intSelectedIndex;
                                }
                            }
                        }

                        TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(gintProjectID);      

                        intRecordsReturned = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            gblnProjectMatrixExists = false;

                            cboSelectStatus.IsEnabled = true;

                            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                cboSelectStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);

                                if (TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus == "OPEN")
                                {
                                    intSelectedIndex = intCounter + 1;
                                }
                            }

                            cboSelectStatus.SelectedIndex = intSelectedIndex;

                            cboSelectStatus.IsEnabled = false;
                        }
                        else
                        {
                            gblnProjectMatrixExists = true;

                            strCustomerIDProjectID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
                            strAssignedProjectID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;
                            txtAssignedProjectID.Text = strAssignedProjectID;

                            TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);

                            intRecordsReturned = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID.Rows.Count;

                            if(intRecordsReturned < 1)
                            {
                                TheFindDesignProjectsbyAssignedProjectIDDataSet = TheDesignProjectsClass.FindDesignProjectsByAssignedProjectID(strCustomerIDProjectID);

                                intRecordsReturned = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID.Rows.Count;

                                if(intRecordsReturned > 0)
                                {
                                    txtAddress.Text = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectAddress;
                                    txtCity.Text = TheFindDesignProjectsbyAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].City;                                    
                                }

                                TheMessagesClass.InformationMessage("The Project Has Been Entered, but is Missing Some Information");

                                SetControlsReadOnly(false);
                            }
                            else if(intRecordsReturned > 0)
                            {
                                SetControlsReadOnly(true);
                                txtAddress.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].BusinessAddress;
                                txtCity.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].City;
                                txtDateReceived.Text = Convert.ToString(TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].DateReceived);
                                txtECDDate.Text = Convert.ToString(TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ECDDate);
                                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(gintProjectID);                                
                                txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
                                txtPRojectNotes.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectNotes;
                                txtState.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].BusinessState;

                                //setting the combo boxes;
                                intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;
                                intStatusID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;

                                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                                {
                                    if(intStatusID == TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].StatusID)
                                    {
                                        intSelectedIndex = intCounter + 1;
                                    }
                                }

                                cboSelectStatus.SelectedIndex = intSelectedIndex;

                                intManagerID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectManagerID;

                                intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;

                                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                                {
                                    if(intManagerID == TheFindProductionManagersDataSet.FindProductionManagers[intCounter].EmployeeID)
                                    {
                                        intSelectedIndex = intCounter + 1;
                                    }
                                }

                                cboSelectManager.SelectedIndex = intSelectedIndex;

                                intDepartmentID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].DepartmentID;

                                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count - 1;

                                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                                {
                                    if(intDepartmentID == TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].DepartmentID)
                                    {
                                        intSelectedIndex = intCounter + 1;
                                    }
                                }

                                cboSelectDepartment.SelectedIndex = intSelectedIndex;

                                intOfficeID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].WarehouseID;

                                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;
                                
                                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                                {
                                    if(intOfficeID == TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                                    {
                                        intSelectedIndex = intCounter + 1;
                                    }
                                }

                                cboSelectOffice.SelectedIndex = intSelectedIndex;

                                TheMessagesClass.InformationMessage("The Project Has Been Entered");

                            }
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Project // Customer Project ID Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
            expProecess.IsEnabled = !blnValueBoolean;
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void txtAssignedProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strAssignedProjectID;
            int intLength;
            int intRecordsReturned;
            string strCustomerID;
            int intCounter;
            int intNumberOfRecords;
            int intDepartmentID;
            int intOfficeID;
            int intStatusID;
            int intManagerID;
            int intSelectedIndex = 0;

            try
            {
                strAssignedProjectID = txtAssignedProjectID.Text;
                intLength = strAssignedProjectID.Length;
                gblnProjectExists = false;
                gblnProjectMatrixExists = false;

                if (intLength > 7)
                {
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;
                   

                    if(intRecordsReturned > 0)
                    {
                        if(strAssignedProjectID.Contains("003") == false)
                        {
                            if(strAssignedProjectID.Contains("004") == false)
                            {
                                if(strAssignedProjectID.Contains("086") == false)
                                {
                                    if(strAssignedProjectID.Contains("920") == false)
                                    {
                                        if (strAssignedProjectID.Contains("921") == false)
                                        {
                                            gblnProjectExists = true;

                                            txtProjectName.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                                            gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                                            TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(gintProjectID);

                                            intRecordsReturned = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID.Rows.Count;

                                            if (intRecordsReturned > 0)
                                            {
                                                strCustomerID = txtCustomerProjectID.Text;

                                                if (strCustomerID != "")
                                                {
                                                    if (strCustomerID != TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID)
                                                    {
                                                        TheMessagesClass.ErrorMessage("The Assigned Project ID is Assigned to Another CustomerID\nContact IT");
                                                        return;
                                                    }

                                                    gblnProjectMatrixExists = true;
                                                    intDepartmentID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].DepartmentID;
                                                    intOfficeID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].WarehouseID;

                                                    //setting up combo boxes
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

                                                    TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);

                                                    intRecordsReturned = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID.Rows.Count;

                                                    if (intRecordsReturned < 1)
                                                    {
                                                        TheMessagesClass.InformationMessage("The Project Has Been Entered, but Missing Some Information");
                                                        return;
                                                    }

                                                    SetControlsReadOnly(true);
                                                    txtAddress.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].BusinessAddress;
                                                    txtCity.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].City;
                                                    txtDateReceived.Text = Convert.ToString(TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].DateReceived);
                                                    txtECDDate.Text = Convert.ToString(TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ECDDate);
                                                    txtPRojectNotes.Text = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectNotes;

                                                    intManagerID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectManagerID;

                                                    intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;

                                                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                                                    {
                                                        if (intManagerID == TheFindProductionManagersDataSet.FindProductionManagers[intCounter].EmployeeID)
                                                        {
                                                            intSelectedIndex = intCounter + 1;
                                                        }
                                                    }

                                                    cboSelectManager.SelectedIndex = intSelectedIndex;

                                                    intStatusID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;

                                                    intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

                                                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                                                    {
                                                        if (intSelectedIndex == TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].StatusID)
                                                        {
                                                            intSelectedIndex = intCounter + 1;
                                                        }
                                                    }

                                                    cboSelectStatus.SelectedIndex = intSelectedIndex;

                                                    TheMessagesClass.InformationMessage("The Project Is Already Entered");
                                                }
                                                else if (txtCustomerProjectID.Text == "")
                                                {
                                                    txtCustomerProjectID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
                                                }
                                            }

                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Project // Assigned Project ID Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void rdoOverYes_Checked(object sender, RoutedEventArgs e)
        {
            string strAssignedProjectID = "";

            gblnOver2500 = true;

            if (gintDepartmentID == 1009)
            {
                strAssignedProjectID = TheProjectNumberAssignmentClass.CreateProjectNumberAssignment();
            }
            else if(gintDepartmentID == 1010)
            {
                strAssignedProjectID = TheATTProjectNumberAssignmentClass.CreateATTProjectNumberAssignment();
            }
            

            txtAssignedProjectID.Text = strAssignedProjectID;
        }

        private void rdoOverNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnOver2500 = false;
        }

    }
}
