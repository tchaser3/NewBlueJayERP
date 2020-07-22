/*  Title:          Create Purchase Request
 *  Date:           1-29-20
 *  Author:         Terry Holmes
 *  
 *  Description:    This will allow the user to create a purchase request */

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
using DataValidationDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using PurchaseRequestDLL;
using PurchaserRequestProjectsDLL;
using PurchaseRequestDocumentationDLL;
using PurchaseRequestUpdateDLL;
using DepartmentDLL;
using EmployeeDateEntryDLL;
using VendorsDLL;
using ProductionProjectDLL;
using ProjectsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreatePurchaseRequest.xaml
    /// </summary>
    public partial class CreatePurchaseRequest : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PurchaseRequestClass ThePurchaseRequestClass = new PurchaseRequestClass();
        PurchaseRequestDocumentationClass ThePurchaseRequestDocumentationClass = new PurchaseRequestDocumentationClass();
        PurchaseRequestProjectsClass ThePurchaseRequestProjectsClass = new PurchaseRequestProjectsClass();
        PurchaseRequestUpdateClass ThePurchaseRequestUpdateClass = new PurchaseRequestUpdateClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        ProjectClass TheProjectsClass = new ProjectClass();

        //setting up the data
        FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindVendorsSortedByVendorNameDataSet TheFindSortedVendorsDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectsByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindPurchaseRequestProjectByProjectIDDataSet TheFindPurchseRequestPRojectByProjectIDDataSet = new FindPurchaseRequestProjectByProjectIDDataSet();
        FindPurchaseRequestByRequestDateDataSet TheFindPurchaseRequestByRequestDateDataSet = new FindPurchaseRequestByRequestDateDataSet();

        //setting global variables
        int gintManagerID;
        int gintDepartmentID;
        int gintOfficeID;
        int gintVendoID;
        int gintProjectID;
        bool gblnRequestQuote;
        bool gblnPurchaseRequest;
        string gstrAssignedProjectID;
        string gstrJobNumber;
        int gintEmployeeID;

        public CreatePurchaseRequest()
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

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtAssignedProjectID.Text = "";
                txtJobNumber.Text = "";
                txtRequestDate.Text = Convert.ToString(DateTime.Now);
                txtRequestNotes.Text = "";
                txtRequiredDate.Text = "";
                cboSelectManager.Items.Clear();
                cboSelectDepartment.Items.Clear();
                cboSelectOffice.Items.Clear();
                cboSelectVendor.Items.Clear();
                chkRequestPurchase.IsChecked = false;
                chkRequestQuote.IsChecked = false;

                gintEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                //loading up the combo boxes
                TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

                intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;
                cboSelectManager.Items.Add("Select Manager");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectManager.Items.Add(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
                }

                cboSelectManager.SelectedIndex = 0;

                cboSelectDepartment.Items.Add("Select Department");
                TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

                intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
                }

                cboSelectDepartment.SelectedIndex = 0;

                cboSelectOffice.Items.Add("Select Office");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                cboSelectVendor.Items.Add("Select Vendor");

                TheFindSortedVendorsDataSet = TheVendorsClass.FindVendorsSortedByVendorName();

                intNumberOfRecords = TheFindSortedVendorsDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectVendor.Items.Add(TheFindSortedVendorsDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
                }

                cboSelectVendor.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Purchase Request // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].DepartmentID;
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintOfficeID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
        }

        private void cboSelectVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectVendor.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintVendoID = TheFindSortedVendorsDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;
        }

        private void chkRequestQuote_Checked(object sender, RoutedEventArgs e)
        {
            gblnRequestQuote = true;
        }

        private void chkRequestQuote_Unchecked(object sender, RoutedEventArgs e)
        {
            gblnRequestQuote = false;
        }

        private void chkRequestPurchase_Checked(object sender, RoutedEventArgs e)
        {
            gblnPurchaseRequest = true;
        }

        private void chkRequestPurchase_Unchecked(object sender, RoutedEventArgs e)
        {
            gblnPurchaseRequest = false;
        }
        
        private void txtJobNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;

            try
            {
                gstrAssignedProjectID = txtAssignedProjectID.Text;

                TheFindProjectByAssignedProjectIDDataSet = TheProjectsClass.FindProjectByAssignedProjectID(gstrAssignedProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Project Does Not Exist");
                    txtAssignedProjectID.Focus();
                    return;
                }

                gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                TheFindProductionProjectsByAssignedProjectIDDataSet = TheProductionProjectClass.FindProductionProjectsByAssignedProjectID(gstrAssignedProjectID);

                intRecordsReturned = TheFindProductionProjectsByAssignedProjectIDDataSet.FindProductionProjectByAssignedProjectID.Rows.Count;

                if (intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Project Needs to be Updated");
                    txtAssignedProjectID.Focus();
                    return;
                }

                TheFindPurchseRequestPRojectByProjectIDDataSet = ThePurchaseRequestProjectsClass.FindPurchaseRequestByProjectID(gintProjectID);

                intRecordsReturned = TheFindPurchseRequestPRojectByProjectIDDataSet.FindPurchaseRequestProjectByProjectID.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("There are Currently " + Convert.ToString(intRecordsReturned) + "Purchase Requests for this Project\nThe Last Job Number Has Been Entered in the Job Number Field");

                    txtJobNumber.Text = TheFindPurchseRequestPRojectByProjectIDDataSet.FindPurchaseRequestProjectByProjectID[intRecordsReturned - 1].JobNumber;
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Purchase Request // Job Number Text Box Got Focus " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables;
            int intLength;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strValueForValidation;
            string strErrorMessage = "";
            DateTime datRequiredDate = DateTime.Now;
            DateTime datRequestDate = DateTime.Now;
            string strRequestNotes;

            try
            {
                if(TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Has Not been Entered\n";
                }
                if(txtJobNumber.Text.Length < 6)
                {
                    TheMessagesClass.InformationMessage("The Job Number/Purchase Request Number will be Assigned");
                }
                strValueForValidation = txtRequestDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Request Date is not a Date\n";
                }
                else
                {
                    datRequestDate = Convert.ToDateTime(strValueForValidation);

                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datRequestDate, DateTime.Now);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Request Date is after Right Now\n";
                    }
                }
                strValueForValidation = txtRequiredDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Required Date is not a Date\n";
                }
                else
                {
                    datRequiredDate = Convert.ToDateTime(strValueForValidation);

                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(DateTime.Now, datRequiredDate);

                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Required Date is Before Today\n";
                    }
                }
                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager Was Not Selected\n";
                }
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                if((chkRequestPurchase.IsChecked == false) && (chkRequestQuote.IsChecked == false))
                {
                    blnFatalError = true;
                    strErrorMessage += "Neither Request Quote or Purchase Request are Checked\n";
                }
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office As Not Selected\n";
                }
                if(cboSelectVendor.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Vendor Was not Selected\n";
                }
                strRequestNotes = txtRequestNotes.Text;

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = ThePurchaseRequestClass.InsertPurchaseRequest(datRequestDate, datRequiredDate, gintEmployeeID, gintManagerID, gintDepartmentID, gintOfficeID, gintVendoID, strRequestNotes, gblnRequestQuote, gblnPurchaseRequest, "OPEN");

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Purchase Request // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtRequestDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intLength;

            gstrJobNumber = txtJobNumber.Text;

            intLength = gstrJobNumber.Length;

            if (intLength < 5)
                TheMessagesClass.InformationMessage("The Job Number/Purchase Request Number will be Assigned at Process");
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
