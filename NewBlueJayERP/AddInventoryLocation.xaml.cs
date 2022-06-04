/* Title:       Add Inventory Location
 * Date:        10-12-20
 * Author:      Terry Holmes
 * 
 * Description: This is used to add an inventory location */

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
using MaterialSheetsDLL;
using EmployeeDateEntryDLL;
using NewPartNumbersDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddInventoryLocation.xaml
    /// </summary>
    public partial class AddInventoryLocation : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        MaterialSheetClass TheMaterialSheetClass = new MaterialSheetClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();    

        //loading up the data
        FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindInventoryLocationByLocationDataSet TheFindInventoryLocationByLocationDataSet = new FindInventoryLocationByLocationDataSet();

        //setting global variables
        bool gblnItemFound;

        public AddInventoryLocation()
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
                MainWindow.gintEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP // Add Inventory Location");

                if (blnFatalError == true)
                    throw new Exception();

                gblnItemFound = false;

                txtEnterLocation.Text = "";
                txtPartDescription.Text = "";
                txtPartNumber.Text = "";

                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count;
                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Inventory Location // Reset Controls " + Ex.Message);

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
                MainWindow.gintWarehouseID = TheFindPartsWarehouseDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void btnFindPart_Click(object sender, RoutedEventArgs e)
        {
            string strPartNumber;
            int intRecordCount;

            try
            {
                strPartNumber = txtPartNumber.Text;
                gblnItemFound = false;

                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordCount = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if(intRecordCount > 0)
                {
                    MainWindow.gintPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                    txtPartDescription.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;
                    gblnItemFound = true;
                }
                else if(intRecordCount < 1)
                {
                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                    intRecordCount = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if(intRecordCount > 0)
                    {
                        MainWindow.gintPartID = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID;
                        txtPartDescription.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription;
                        gblnItemFound = true;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("The Part Was Not Found");
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Inventory Location // Find Part Button " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAddLocation_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strLocation;
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;

            try
            {
                if(cboSelectWarehouse.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Warehouse Was Not Selected\n";
                }
                if(gblnItemFound == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Part Number Was Not Found\n";
                }
                strLocation = txtEnterLocation.Text;
                if(strLocation.Length < 2)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Location Is Not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                /*
                TheFindInventoryLocationByLocationDataSet = TheMaterialSheetClass.FindInventoryLocationByLocation(strLocation, MainWindow.gintWarehouseID);

                intRecordsReturned = TheFindInventoryLocationByLocationDataSet.FindInventoryLocationByLocation.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Location Has Been Used Already");
                    return;
                }
                */

                blnFatalError = TheMaterialSheetClass.InsertInventoryLocation(MainWindow.gintPartID, MainWindow.gintEmployeeID, datTransactionDate, strLocation, MainWindow.gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Inventory Location Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Inventory Location // Add Location Button " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expImportExcel_Expanded(object sender, RoutedEventArgs e)
        {
            expImportExcel.IsExpanded = false;

            ImportInventoryLocations ImportInventoryLocations = new ImportInventoryLocations();
            ImportInventoryLocations.ShowDialog();
        }
    }
}
