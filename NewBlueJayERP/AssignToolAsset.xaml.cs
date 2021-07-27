/* Title:           Assign Tool Asset
 * Date:            6-29-21
 * Author:          Terry Holmes
 * 
 * Description:     This is how we will assign assets */


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
using ToolHistoryDLL;
using ToolCategoryDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;
using ToolIDDLL;
using AssetDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignToolAsset.xaml
    /// </summary>
    public partial class AssignToolAsset : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ToolIDClass TheToolIDClass = new ToolIDClass();
        AssetClass TheAssetClass = new AssetClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindToolIDByCategoryDataSet TheFindToolIDByCategoryDataSet = new FindToolIDByCategoryDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWaspAssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();

        bool gblnItemEntered;
        int gintTransactionID;

        public AssignToolAsset()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            //this will load up the controls
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            string strCategory;

            try
            {
                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();
                gblnItemEntered = false;

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count;
                cboSelectToolCategory.Items.Clear();
                cboSelectToolCategory.Items.Add("Select Tool Category");

                cboSelectEmployee.Items.Clear();

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectToolCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboSelectToolCategory.SelectedIndex = 0;

                txtAssetID.Text = Convert.ToString(MainWindow.gintAssetID);
                txtSite.Text = ImportToolSheets.gstrSite;
                txtLocation.Text = ImportToolSheets.gstrLocation;
                txtBJCAssetID.Text = ImportToolSheets.gstrBJCAssetID;
                txtLastName.Text = ImportToolSheets.gstrLastName;
                txtToolDescription.Text = ImportToolSheets.gstrToolDescription;

                TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(ImportToolSheets.gstrBJCAssetID);

                intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count;

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strCategory = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCategory;

                        if(strCategory == TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory)
                        {
                            cboSelectToolCategory.SelectedIndex = intCounter + 1;
                            txtPartNumber.Text = "NOT NEEDED";
                            gblnItemEntered = true;
                            txtToolNotes.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolNotes;
                            txtToolDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                            MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
                        }                        
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Tool Asset // Window Loaded Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtLastName.Text;
                if (strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if (intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Was Not Found");
                        return;
                    }

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assign Tool Asset // Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectToolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intRecordsReturned;
            string strToolID;
            bool blnToolExists;
            bool blnStringID;
            int intToolID;
            bool blnFatalError = false;
            string strBJCAssetID;

            try
            {

                intSelectedIndex = cboSelectToolCategory.SelectedIndex - 1;

                if ((intSelectedIndex > -1))
                {
                    strBJCAssetID = txtBJCAssetID.Text;

                    if (strBJCAssetID.Length < 4)
                    {
                        MainWindow.gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                        ImportToolSheets.gstrToolCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;

                        TheFindToolIDByCategoryDataSet = TheToolIDClass.FindToolIDByCategory(ImportToolSheets.gstrToolCategory);

                        intRecordsReturned = TheFindToolIDByCategoryDataSet.FindToolIDByCategory.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            blnToolExists = true;

                            strToolID = TheFindToolIDByCategoryDataSet.FindToolIDByCategory[0].ToolID;
                            gintTransactionID = TheFindToolIDByCategoryDataSet.FindToolIDByCategory[0].TransactionID;

                            blnStringID = TheDataValidationClass.VerifyIntegerData(strToolID);

                            if (blnStringID == true)
                            {
                                txtBJCAssetID.Text = strToolID;
                            }
                            else if (blnStringID == false)
                            {
                                while (blnToolExists == true)
                                {

                                    intToolID = Convert.ToInt32(strToolID);

                                    intToolID = intToolID + 1;

                                    strToolID = Convert.ToString(intToolID);

                                    blnFatalError = TheToolIDClass.UpdateToolID(gintTransactionID, strToolID);

                                    TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(strToolID);

                                    intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                                    if (intRecordsReturned < 1)
                                    {
                                        blnToolExists = false;
                                    }

                                }
                            }

                            txtBJCAssetID.Text = strToolID;
                        }
                        else
                        {
                            txtBJCAssetID.Text = "None Found";
                            TheMessagesClass.InformationMessage("There is not a Tool ID for this Tool in the Database, Please Look at Old Tool Spreadsheet");
                        }

                    }
                    else if (strBJCAssetID.Length > 3)
                    {
                        ImportToolSheets.gstrToolCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;
                        MainWindow.gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                    }

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Tool Asset // Tool Category Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strPartNumber;
            string strSerialNumber = "";
            string strToolNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intToolKey;
            int intEmployeeID;

            try
            {
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                if(cboSelectToolCategory.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Category was not Selected\n";
                }
                ImportToolSheets.gstrBJCAssetID = txtBJCAssetID.Text;
                if(ImportToolSheets.gstrBJCAssetID.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The BJC Number Was Not Assigned\n";
                }
                ImportToolSheets.gstrToolDescription = txtToolDescription.Text;
                if(ImportToolSheets.gstrToolDescription.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Description was not Long Enough\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strPartNumber = txtPartNumber.Text;
                if(strPartNumber.Length < 4)
                {
                    strPartNumber = "";
                }
                strToolNotes = txtToolNotes.Text;
                if(strToolNotes.Length < 10)
                {
                    strToolNotes = "NO NOTES ENTERED";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(ImportToolSheets.gstrBJCAssetID);

                intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    blnFatalError = TheToolClass.InsertTools(ImportToolSheets.gstrBJCAssetID, MainWindow.gintEmployeeID, strPartNumber, MainWindow.gintCategoryID, ImportToolSheets.gstrToolDescription, 0, MainWindow.gintWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(ImportToolSheets.gstrBJCAssetID);
                }

                intToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;

                blnFatalError = TheToolClass.UpdateToolSignOut(intToolKey, MainWindow.gintEmployeeID, false);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheToolHistoryClass.InsertToolHistory(intToolKey, MainWindow.gintEmployeeID, intEmployeeID, "ASSIGNED FROM TOOL SHEEET DURING WASP ASSET IMPORT");

                if (blnFatalError == true)
                    throw new Exception();

                TheFindWaspAssetByBJCAssetIDDataSet = TheAssetClass.FindWaspAssetByBJCAssetID(ImportToolSheets.gstrBJCAssetID);

                intRecordsReturned = TheFindWaspAssetByBJCAssetIDDataSet.FindWaspAssetByBJCAssetID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    blnFatalError = TheAssetClass.InsertWaspAssets(MainWindow.gintAssetID, ImportToolSheets.gstrToolDescription, ImportToolSheets.gstrBJCAssetID, ImportToolSheets.gstrToolCategory, ImportToolSheets.gstrSite, ImportToolSheets.gstrLocation, MainWindow.gintWarehouseID, DateTime.Now, strSerialNumber, " ", " ");

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Tool Has Been Updated and Imported");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Assigned Tool Asset // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
