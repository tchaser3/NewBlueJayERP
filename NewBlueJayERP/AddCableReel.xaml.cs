/* Title:           Add Cable Reel
 * Date:            9-10-20
 * Author:          Terry Holmes
 * 
 * Description:     this is used to create a cable reel */

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
using CableInventoryDLL;
using NewPartNumbersDLL;
using ProjectMatrixDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddCableReel.xaml
    /// </summary>
    public partial class AddCableReel : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CableInventoryClass TheCableInventoryClass = new CableInventoryClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        FindPartsWarehousesDataSet TheFindPartsWarehousesDataSet = new FindPartsWarehousesDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindCableReelIDByAssignedCableIDDataSet TheFindCableReelIDByAssignedCableReelIDDataSet = new FindCableReelIDByAssignedCableIDDataSet();
        FindCableReelIDByTransactionDateDataSet TheFindCableReelIDByTransactionDateDataSet = new FindCableReelIDByTransactionDateDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindCableTotalInventoryByPartWarehouseDataSet TheFindCableTotalInventoryByPartWarehouseDataSet = new FindCableTotalInventoryByPartWarehouseDataSet();

        int gintReelID;
        bool gblnReelIDChecked;

        public AddCableReel()
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
            int intCounter;
            int intNumberOfRecords;
            string strFullName;            

            try
            {
                //clearing text boxes
                txtAssignedReelID.Text = "";
                txtPartDescription.Text = "";
                txtPartNumber.Text = "";
                txtPONumber.Text = "";
                txtProjectID.Text = "";
                txtReelFootage.Text = "";
                lblWarehouseSelected.Content = "Select Warehouse";
                gblnReelIDChecked = false;

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Warehouse Employee");

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("WAREHOUSE");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName;
                    cboSelectEmployee.Items.Add(strFullName);
                }

                cboSelectEmployee.SelectedIndex = 0;

                TheFindPartsWarehousesDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehousesDataSet.FindPartsWarehouses.Rows.Count - 1;

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;

                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintWarehouseID = TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                    lblWarehouseSelected.Content = cboSelectWarehouse.SelectedItem.ToString();
                }                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Select Warehouse Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expCheckPartNumber_Expanded(object sender, RoutedEventArgs e)
        {
            string strPartNumber;
            int intRecordsReturned;

            try
            {
                expCheckPartNumber.IsExpanded = false;
                strPartNumber = txtPartNumber.Text;

                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    txtPartDescription.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;

                    MainWindow.gintPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                }
                else if (intRecordsReturned < 1)
                {
                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        txtPartDescription.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription;

                        MainWindow.gintPartID = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("The Part Number Was Not Found");
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Check Cable Reel " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expPartLookup_Expanded(object sender, RoutedEventArgs e)
        {
            expPartLookup.IsExpanded = false;
            MainWindow.PartLookupWindow.Visibility = Visibility.Visible;
        }

        private void expGetReelID_Expanded(object sender, RoutedEventArgs e)
        {
            string strPartNumber;
            int intPartID;
            string strCableReelID = "NOT ASSIGNED";
            DateTime datTransactionDate = DateTime.Now;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                expGetReelID.IsExpanded = false;
                strPartNumber = txtPartNumber.Text;

                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Part Number was not Found");
                    return;
                }

                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("The Employee Was Not Selected");
                    return;
                }

                if(cboSelectWarehouse.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("The Warehouse Was Not Selected");
                    return;
                }

                intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;

                blnFatalError = TheCableInventoryClass.InsertCableReelID(datTransactionDate, intPartID, MainWindow.gintEmployeeID, strCableReelID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindCableReelIDByTransactionDateDataSet = TheCableInventoryClass.FindCableReelIDByTransactionDate(datTransactionDate);

                gintReelID = TheFindCableReelIDByTransactionDateDataSet.FindCableReelIDByTransactionDate[0].CableReelID;

                txtAssignedReelID.Text = Convert.ToString(gintReelID);
                gblnReelIDChecked = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Get Reel ID Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strProjectID;
            string strPartNumber;
            string strPONumber;
            string strAssignedReelID;
            int intReelFootage = 0;
            int intRecordsReturned;
            string strValueForValidation;
            DateTime datTransactionDate = DateTime.Now;
            int intPartID = 0;
            int intTransactionID;
            int intTotalFootage;

            try
            {
                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Cable Reel ");

                if (blnFatalError == true)
                    throw new Exception();

                if (cboSelectWarehouse.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Warehouse Was Not Selected\n";
                }
                strProjectID = txtProjectID.Text;
                if(strProjectID.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project ID is not Long Enough\n";
                }
                else
                {
                    TheFindProjectMatrixByCustomerIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strProjectID);

                        intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Project ID Was Not Found\n";
                        }
                    }
                    else if(intRecordsReturned > 0)
                    {
                        MainWindow.gintProjectID = TheFindProjectMatrixByCustomerIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    }
                }
                strPartNumber = txtPartNumber.Text;
                if(strPartNumber.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Part Number is not Long Enough\n";
                }
                else
                {
                    TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Part Number Was Not Found\n";
                    }
                    else if(intRecordsReturned > 0)
                    {
                        intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                    }
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strPONumber = txtPONumber.Text;
                if(strPONumber.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The MSR/PO Number is not Long Enough\n";
                }
                strAssignedReelID = txtAssignedReelID.Text;
                if(strAssignedReelID.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Assigned Reel ID is not Long Enough\n";
                }
                else
                {
                    TheFindCableReelIDByAssignedCableReelIDDataSet = TheCableInventoryClass.FindCableReelIDByAssignedCableReelID(strAssignedReelID);

                    intRecordsReturned = TheFindCableReelIDByAssignedCableReelIDDataSet.FindCableReelIDByAssignedCableReelID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Assigned Reel ID has been Used\n";
                    }
                }
                strValueForValidation = txtReelFootage.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Reel Footage is not an Integer\n";
                }
                else
                {
                    intReelFootage = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //adding cable reel
                if(gblnReelIDChecked == true)
                {
                    blnFatalError = TheCableInventoryClass.UpdateCableAssignedReelID(gintReelID, strAssignedReelID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(gblnReelIDChecked == false)
                {
                    blnFatalError = TheCableInventoryClass.InsertCableReelID(datTransactionDate, intPartID, MainWindow.gintEmployeeID, strAssignedReelID);

                    if (blnFatalError == true)
                        throw new Exception();                    
                }

                TheFindCableReelIDByAssignedCableReelIDDataSet = TheCableInventoryClass.FindCableReelIDByAssignedCableReelID(strAssignedReelID);

                gintReelID = TheFindCableReelIDByAssignedCableReelIDDataSet.FindCableReelIDByAssignedCableReelID[0].CableReelID;

                //inserting the reel into the table
                blnFatalError = TheCableInventoryClass.InsertCableReelInventory(intPartID, MainWindow.gintWarehouseID, strAssignedReelID, intReelFootage);

                if (blnFatalError == true)
                    throw new Exception();

                //updating total footage
                TheFindCableTotalInventoryByPartWarehouseDataSet = TheCableInventoryClass.FindCableTotalInventoryByPartWarehouse(MainWindow.gintWarehouseID, intPartID);

                intRecordsReturned = TheFindCableTotalInventoryByPartWarehouseDataSet.FindCableTotalInventoryByPartWarehouse.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    intTransactionID = TheFindCableTotalInventoryByPartWarehouseDataSet.FindCableTotalInventoryByPartWarehouse[0].TransactionID;
                    intTotalFootage = TheFindCableTotalInventoryByPartWarehouseDataSet.FindCableTotalInventoryByPartWarehouse[0].TotalFootage;

                    intTotalFootage = intTotalFootage + intReelFootage;

                    blnFatalError = TheCableInventoryClass.UpdateCableIventoryTotalFootage(intTransactionID, intTotalFootage);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(intRecordsReturned < 1)
                {
                    blnFatalError = TheCableInventoryClass.InsertCableTotalInventory(intPartID, MainWindow.gintWarehouseID, intReelFootage);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("Cable Reel Has Been Received");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
