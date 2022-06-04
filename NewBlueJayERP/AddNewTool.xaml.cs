/* Title:           Add New Tool
 * Date:            10-13-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Add a New Tool */

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
using EmployeeDateEntryDLL;
using NewToolsDLL;
using ToolCategoryDLL;
using ToolIDDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddNewTool.xaml
    /// </summary>
    public partial class AddNewTool : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolsClass = new ToolsClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        ToolIDClass TheToolIDClass = new ToolIDClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindToolIDByCategoryDataSet TheFindToolIDByCategoryDataSet = new FindToolIDByCategoryDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();

        //setting global variables
        int gintTransactionID;
        bool gblnNewToolCategory;

        public AddNewTool()
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
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add New Tool");

                if (blnFatalError == true)
                    throw new Exception();

                txtToolCost.Text = "";
                txtToolDescription.Text = "";
                txtToolID.Text = "";
                txtToolPartNumber.Text = "";

                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                cboSelectToolCategory.Items.Clear();
                cboSelectToolCategory.Items.Add("Select Tool Category");

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectToolCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboSelectToolCategory.SelectedIndex = 0;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add New Tools // Reset Controls " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectToolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strToolCategory;
            int intRecordsReturned;
            string strToolID;
            int intToolID;
            bool blnToolFound;

            try
            {
                intSelectedIndex = cboSelectToolCategory.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                    strToolCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;

                    TheFindToolIDByCategoryDataSet = TheToolIDClass.FindToolIDByCategory(strToolCategory);

                    intRecordsReturned = TheFindToolIDByCategoryDataSet.FindToolIDByCategory.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnToolFound = true;

                        while(blnToolFound == true)
                        {
                            strToolID = TheFindToolIDByCategoryDataSet.FindToolIDByCategory[0].ToolID;

                            intToolID = Convert.ToInt32(strToolID);

                            intToolID++;

                            strToolID = Convert.ToString(intToolID);

                            TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                            if(intRecordsReturned < 1)
                            {
                                blnToolFound = false;
                            }

                            txtToolID.Text = strToolID;
                            gintTransactionID = TheFindToolIDByCategoryDataSet.FindToolIDByCategory[0].TransactionID;
                            gblnNewToolCategory = false;
                        }
                        
                    }
                    else
                    {
                        txtToolID.Text = "None Found";
                        TheMessagesClass.InformationMessage("There is not a Tool ID for this Tool in the Database, Please Contact IT");
                        gblnNewToolCategory = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add New Tool // Tool Category Selection Changed " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void btnCreateTool_Click(object sender, RoutedEventArgs e)
        {
            //creating local variables
            int intSelectedIndex;
            string strValueForValidation;
            string strPartNumber;
            string strDescription;
            string strToolID;
            decimal decToolCost = 0;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                //data validation
                intSelectedIndex = cboSelectWarehouse.SelectedIndex;
                if (intSelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Warehouse Was Not Selected\n";
                }
                intSelectedIndex = cboSelectToolCategory.SelectedIndex;
                if (intSelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Category Was Not Selected\n";
                }
                strToolID = txtToolID.Text;
                if (strToolID == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Tool ID Was Not Entered\n";
                }
                else
                {
                    TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                    intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "There is Already an Active Tool With This ID\n";
                    }
                }
                strPartNumber = txtToolPartNumber.Text;
                if (strPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Part Number Was Not Added\n";
                }
                strDescription = txtToolDescription.Text;
                if (strDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Description Was Not Added\n";
                }
                strValueForValidation = txtToolCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Cost is not Numeric\n";
                }
                else
                {
                    decToolCost = Convert.ToDecimal(strValueForValidation);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if (gblnNewToolCategory == true)
                {
                    blnFatalError = TheToolIDClass.InsertNewToolIDForToolType(MainWindow.gintCategoryID, strToolID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if (gblnNewToolCategory == false)
                {
                    blnFatalError = TheToolIDClass.UpdateToolID(gintTransactionID, strToolID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheToolsClass.InsertTools(strToolID, MainWindow.gintWarehouseID, strPartNumber, MainWindow.gintCategoryID, strDescription, decToolCost, MainWindow.gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                ResetControls();

                TheMessagesClass.InformationMessage("The Tool Has Been Created");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add New Tool // Create Tool Button " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
