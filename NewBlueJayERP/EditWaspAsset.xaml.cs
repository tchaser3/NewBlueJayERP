/* Title:           Edit Wasp Asset
 * Date:            6-2-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to edit a Wasp Asset */

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
using NewToolsDLL;
using ToolCategoryDLL;
using AssetDLL;
using EmployeeDateEntryDLL;
using ToolIDDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditWaspAsset.xaml
    /// </summary>
    public partial class EditWaspAsset : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        AssetClass TheAssetClass = new AssetClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ToolIDClass TheToolIDClass = new ToolIDClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindWaspAssetByAssetIDDataSet TheFindWaspAssetByAssetIDDataSet = new FindWaspAssetByAssetIDDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindToolIDByCategoryDataSet TheFindToolIDByCategoryDataSet = new FindToolIDByCategoryDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        FindWaspAssetByBJCAssetIDDataSet TheFindWaspAsssetByBJCAssetIDDataSet = new FindWaspAssetByBJCAssetIDDataSet();

        int gintTransactionID;
        string gstrAssetCategory;
        bool gblnUploaded;
        bool gblnNewCategory;

        public EditWaspAsset()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intNumberOfRecords;
            int intCounter;
            string strAsseetCategory;
            int intSelectedIndex;

            try
            {
                gblnUploaded = false;

                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                cboAssetCategory.Items.Add("Select Category");

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboAssetCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboAssetCategory.SelectedIndex = 0;

                TheFindWaspAssetByAssetIDDataSet = TheAssetClass.FindWaspAssetByAssetID(MainWindow.gintAssetID);

                txtAssetID.Text = Convert.ToString(MainWindow.gintAssetID);
                txtDescription.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].AssetDescription;
                txtBJCAssetID.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].BJCAssetID;
                strAsseetCategory = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].AssetCategory;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if(strAsseetCategory == TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory)
                    {
                        cboAssetCategory.SelectedIndex = intCounter + 1;
                    }
                }

                txtSite.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].AssetSite;
                txtLocation.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].AssetLocation;
                txtWarehouseID.Text = Convert.ToString(TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].WarehouseID);
                txtDate.Text = Convert.ToString(TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].TransactionDate);
                txtSerialNo.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].SerialNumber;
                txtManufacturer.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].Manufacturer;
                txtModel.Text = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].Model;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Wasp Asset");

                gblnUploaded = true;
                gblnNewCategory = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Wasp Asset // Window Loaded Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void cboAssetCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                
                intSelectedIndex = cboAssetCategory.SelectedIndex - 1;

                if ((intSelectedIndex > -1) && (gblnUploaded == true))
                {
                    strBJCAssetID = txtBJCAssetID.Text;

                    if (strBJCAssetID.Length < 4)
                    {
                        MainWindow.gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                        gstrAssetCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;

                        TheFindToolIDByCategoryDataSet = TheToolIDClass.FindToolIDByCategory(gstrAssetCategory);

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

                                    TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                                    intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                                    if (intRecordsReturned < 1)
                                    {
                                        TheFindWaspAsssetByBJCAssetIDDataSet = TheAssetClass.FindWaspAssetByBJCAssetID(strToolID);

                                        intRecordsReturned = TheFindWaspAsssetByBJCAssetIDDataSet.FindWaspAssetByBJCAssetID.Rows.Count;

                                        if(intRecordsReturned < 1)
                                        {
                                            blnToolExists = false;
                                        }                                        
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
                    else if(strBJCAssetID.Length > 3)
                    {
                        gstrAssetCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;
                        MainWindow.gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                    }

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Wasp Asset // Tool Category Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the local variables;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strDescription;
            string strBJCAssetID;
            string strSerialNumber;
            string strManufacturer;
            string strModel;
            int intRecordsReturned;
            bool blnItemExists = false;
            int intToolKey = 0;
            string strPartNumber = "";
            int intCurrentLocation = 0;
            decimal decPartCost = 0;
            string strToolNotes = "";

            try
            {
                strDescription = txtDescription.Text;
                if(strDescription.Length < 6)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Description is to Short\n";
                }
                if(cboAssetCategory.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Category Was Not Selected\n";
                }
                strBJCAssetID = txtBJCAssetID.Text;
                if(strBJCAssetID.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The BJC Asset ID is to Short\n";
                }
                else
                {
                    TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strBJCAssetID);

                    intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnItemExists = true;
                        intToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
                        strPartNumber = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].PartNumber;
                        intCurrentLocation = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].CurrentLocation;
                        decPartCost = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCost;
                        strToolNotes = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolNotes;
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);

                    return;
                }

                strSerialNumber = txtSerialNo.Text;
                strManufacturer = txtManufacturer.Text;
                strModel = txtModel.Text;

                blnFatalError = TheAssetClass.UpdateWaspAsset(MainWindow.gintAssetID, strDescription, gstrAssetCategory, strBJCAssetID, strSerialNumber, strManufacturer, strModel);

                if (blnFatalError == true)
                    throw new Exception();

                if(blnItemExists == false)
                {
                    blnFatalError = TheToolsClass.InsertTools(strBJCAssetID, MainWindow.gintWarehouseID, strSerialNumber, MainWindow.gintCategoryID, strDescription, 0, MainWindow.gintWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(blnItemExists == true)
                {
                    blnFatalError = TheToolsClass.UpdateToolActive(intToolKey, true);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheToolsClass.UpdateToolCategory(intToolKey, MainWindow.gintCategoryID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheToolsClass.UpdateToolInfo(intToolKey, strPartNumber, strDescription, intCurrentLocation, decPartCost, strToolNotes);

                    if (blnFatalError == true)
                        throw new Exception();

                }

                TheMessagesClass.InformationMessage("The Asset has been Updated");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Wasp Asset // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expRemoveAsset_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strToolID;
            int intToolKey;
            int intRecordsReturned;

            try
            {
                const string message = "Are You Sure You Want To Remove This Asset?";
                const string caption = "Are You Sure";
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    blnFatalError = TheAssetClass.DeleteWaspAsset(MainWindow.gintAssetID);

                    if (blnFatalError == true)
                        throw new Exception();

                    const string secmessage = "Are You Sure You Want To Remove This From The Tool Table?";
                    const string seccaption = "Are You Sure";
                    MessageBoxResult secresult = MessageBox.Show(secmessage, seccaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (secresult == MessageBoxResult.Yes)
                    {
                        strToolID = txtBJCAssetID.Text;
                        if(strToolID.Length > 3)
                        {
                            TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                            if(intRecordsReturned > 0)
                            {
                                intToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;

                                blnFatalError = TheToolsClass.DeleteTool(intToolKey);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }
                            else if(intRecordsReturned < 1)
                            {
                                TheMessagesClass.ErrorMessage("The Tool Was Not Found");
                            }
                        }
                    }

                    TheMessagesClass.InformationMessage("The Asset Has Been Deleted");

                    this.Close();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Wasp Asset // Remove Asset Expander " + Ex.Message);
            }
        }

        private void expChangeLocation_Expanded(object sender, RoutedEventArgs e)
        {
            expChangeLocation.IsExpanded = false;

            ChangeWaspAssetLocation ChangeWaspAssetLocation = new ChangeWaspAssetLocation();
            ChangeWaspAssetLocation.ShowDialog();

            this.Close();

        }
    }
}
