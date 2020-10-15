/* Title:           Edit Tool
 * Date:            10-15-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to edit tools */

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
using NewToolsDLL;
using ToolCategoryDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditTool.xaml
    /// </summary>
    public partial class EditTool : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        FindActiveToolByToolIDDataSet TheFindToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        FindToolCategoryByCategoryIDDataSet TheFindToolCategoryByCategoryIDDataSet = new FindToolCategoryByCategoryIDDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCateoryDataSet = new FindSortedToolCategoryDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();

        public EditTool()
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
                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Tool");

                if (blnFatalError == true)
                    throw new Exception();

                txtToolCost.Text = "";
                txtToolDescription.Text = "";
                txtToolID.Text = "";
                txtToolNotes.Text = "";
                txtToolPartNumber.Text = "";

                cboSelectToolCategory.Items.Clear();
                cboSelectToolCategory.Items.Add("Select Tool Category");

                TheFindSortedToolCateoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                intNumberOfRecords = TheFindSortedToolCateoryDataSet.FindSortedToolCategory.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectToolCategory.Items.Add(TheFindSortedToolCateoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboSelectToolCategory.SelectedIndex = 0;

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Tool // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnFindTool_Click(object sender, RoutedEventArgs e)
        {
            //this will find the tool
            string strToolID;
            int intRecordsReturned;
            int intWarehouseID;
            string strCategory;

            try
            {
                strToolID = txtToolID.Text;
                if (strToolID == "")
                {
                    TheMessagesClass.ErrorMessage("Tool ID Not Entered");
                    return;
                }

                TheFindToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                intRecordsReturned = TheFindToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("No Tools Found");
                    return;
                }

                txtToolCost.Text = Convert.ToString(TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCost);
                txtToolDescription.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                txtToolPartNumber.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].PartNumber;
                txtToolNotes.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolNotes;
                MainWindow.gintToolKey = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
                strCategory = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCategory;

                intWarehouseID = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].CurrentLocation;

                cboSelectWarehouse.IsEnabled = true;

                FindToolCategory(strCategory);

                FindWarehouse(intWarehouseID);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FindToolCategory(string strToolCategory)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = TheFindSortedToolCateoryDataSet.FindSortedToolCategory.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if(TheFindSortedToolCateoryDataSet.FindSortedToolCategory[intCounter].ToolCategory == strToolCategory)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectToolCategory.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Tool // Find Tool Category " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FindWarehouse(int intWarehouseID)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                if (cboSelectWarehouse.SelectedIndex > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (intWarehouseID == TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                        {
                            intSelectedIndex = intCounter + 1;
                        }
                    }

                    cboSelectWarehouse.SelectedIndex = intSelectedIndex;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Find Warehouses " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboSelectToolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectToolCategory.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintCategoryID = TheFindSortedToolCateoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
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

        private void btnProcessTool_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strPartNumber;
            string strPartDescription;
            string strNotes;
            string strValueForValidation;
            decimal decToolCost = 0;

            try
            {
                strPartNumber = txtToolPartNumber.Text;
                if (strPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Number Was Not Entered\n";
                }
                strPartDescription = txtToolDescription.Text;
                strValueForValidation = txtToolCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Cost is not Numeric\n";
                }
                else
                {
                    decToolCost = Convert.ToDecimal(strValueForValidation);
                }
                if (strPartDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Description Was Not Entered\n";
                }
                strNotes = txtToolNotes.Text;
                if (strNotes == "")
                {
                    strNotes = "NO NOTES ENTERED";
                }

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheToolsClass.UpdateToolInfo(MainWindow.gintToolKey, strPartNumber, strPartDescription, MainWindow.gintWarehouseID, decToolCost, strNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Tool Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Tools // Process Tool Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expRetireTool_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                const string message = "Are you sure that you would like to Retire This Tool?";
                const string caption = "Are You Sure";
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    blnFatalError = TheToolsClass.UpdateToolActive(MainWindow.gintToolKey, false);

                    if (blnFatalError == true)
                        throw new Exception();

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Tools // Retire Tool Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
