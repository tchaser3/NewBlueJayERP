/* Title:           Add Wasp Asset
 * Date:            6-4-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add an asset to a location */

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
using AssetDLL;
using NewToolsDLL;
using ToolCategoryDLL;
using EmployeeDateEntryDLL;
using ToolIDDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddWaspAsset.xaml
    /// </summary>
    public partial class AddWaspAsset : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssetClass TheAssetClass = new AssetClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ToolIDClass TheToolIDClass = new ToolIDClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindWaspAssetLocationByLocationDataSet TheFindWaspAssetLocationByLoctionDataSet = new FindWaspAssetLocationByLocationDataSet();
        WaspAssetIDDataSet TheWaspAssetIDDataSet = new WaspAssetIDDataSet();
        FindWaspAssetByAssetIDDataSet TheFindWaspAssetByAssetIDDataSet = new FindWaspAssetByAssetIDDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindToolIDByCategoryDataSet TheFindToolIDByCategoryDataSet = new FindToolIDByCategoryDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        FindWarehouseByWarehouseNameDataSet TheFindWarehouseByWarehouseNameDataSet = new FindWarehouseByWarehouseNameDataSet();

        string gstrAssetCategory;
        int gintTransactionID;

        public AddWaspAsset()
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            DateTime datTransactionDate = DateTime.Now;
            int intNumberOfRecords;
            int intCounter;
            int intTransactionID;
            int intAssetID;
            bool blnFatalError = false;
            string strSite;

            try
            {
                TheWaspAssetIDDataSet = TheAssetClass.GetWaspAssetIDInfo();

                intAssetID = TheWaspAssetIDDataSet.waspassetid[0].CreatedAssetID;
                intTransactionID = TheWaspAssetIDDataSet.waspassetid[0].TransactionID;

                MainWindow.gintAssetID = intAssetID;
                intAssetID++;
                txtAssetID.Text = Convert.ToString(MainWindow.gintAssetID);
                txtDate.Text = Convert.ToString(datTransactionDate);

                blnFatalError = TheAssetClass.UpdateWaspAssetID(intTransactionID, intAssetID);

                txtLocation.Text = MainWindow.gstrAssetLocation;

                TheFindWaspAssetLocationByLoctionDataSet = TheAssetClass.FindWaspAssetLocationByLocation(MainWindow.gstrAssetLocation);

                strSite = TheFindWaspAssetLocationByLoctionDataSet.FindWaspAssetLocationByLocation[0].AssetSite;
                txtSite.Text = strSite;

                if(strSite == "GROVEPORT")
                {
                    strSite = "CBUS-GROVEPORT";
                }

                TheFindWarehouseByWarehouseNameDataSet = TheEmployeeClass.FindWarehouseByWarehouseName(strSite);

                MainWindow.gintWarehouseID = TheFindWarehouseByWarehouseNameDataSet.FindWarehouseByWarehouseName[0].EmployeeID;

                txtWarehouseID.Text = Convert.ToString(MainWindow.gintWarehouseID);

                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                cboAssetCategory.Items.Add("Select Category");

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboAssetCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboAssetCategory.SelectedIndex = 0;

                txtSerialNo.Text = "UNKNOWN";
                txtModel.Text = "UNKNOWN";
                txtManufacturer.Text = "UNKNOWN";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Wasp Asset // Window Loaded Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

                if ((intSelectedIndex > -1))
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

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Wasp Asset // Tool Category Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //SETTING UP LOCAL VARIABLES
            int intAssetID;
            DateTime datTransactionDate = DateTime.Now;
            string strDescription;
            string strBJCAssetID;
            string strSite;
            string strLocation;
            string strSerialNumber;
            string strManufacturer;
            string strModel;
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                intAssetID = Convert.ToInt32(txtAssetID.Text);
                strDescription = txtDescription.Text;
                if(strDescription.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Description is not Long Enough\n";
                }
                if(cboAssetCategory.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Category was not Selected\n";
                }
                strBJCAssetID = txtBJCAssetID.Text;
                if(strBJCAssetID.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The BJC Asset ID is not Long Enough\n";
                }
                else
                {
                    TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strBJCAssetID);

                    intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The BJC Asset ID Is Already in the Data Base\n";
                    }
                }
                strSite = txtSite.Text;
                strSerialNumber = txtSerialNo.Text;
                strManufacturer = txtManufacturer.Text;
                strModel = txtModel.Text;

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheAssetClass.InsertWaspAssets(intAssetID, strDescription, strBJCAssetID, gstrAssetCategory, strSite, MainWindow.gstrAssetLocation, MainWindow.gintWarehouseID, datTransactionDate, strSerialNumber, strManufacturer, strModel);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Asset Has Been Added");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Wasp Asset // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
