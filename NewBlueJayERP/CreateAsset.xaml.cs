/* Title:           Create Asset
 * Date:            7-7-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add an individual asset*/

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
using AssetDLL;
using DataValidationDLL;
using BulkToolHistoryDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateAsset.xaml
    /// </summary>
    public partial class CreateAsset : Window
    {
        //setting up the classes
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        AssetClass TheAssetClass = new AssetClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindSortedAssetTypesDataSet TheFindSortedAssetTypesDataSet = new FindSortedAssetTypesDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();

        //setting global variables
        int gintAssetTypeID;

        public CreateAsset()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            txtActiveDate.Text = "";
            txtAssetDescription.Text = "";
            txtAssetNotes.Text = "";
            txtAssetPartNumber.Text = "";
            txtAssetPrice.Text = "";
            btnProcess.IsEnabled = false;

            //loading up the warehouses
            cboSelectOffice.Items.Clear();
            cboSelectOffice.Items.Add("Select Office");
            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectOffice.SelectedIndex = 0;

            cboSelectAssetType.Items.Clear();
            cboSelectAssetType.Items.Add("Select Asset Type");

            TheFindSortedAssetTypesDataSet = TheAssetClass.FindSortedAssetTypes();

            intNumberOfRecords = TheFindSortedAssetTypesDataSet.FindSortedAssetTypes.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectAssetType.Items.Add(TheFindSortedAssetTypesDataSet.FindSortedAssetTypes[intCounter].AssetType);
            }

            cboSelectAssetType.SelectedIndex = 0;
        }

        private void cboSelectAssetType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectAssetType.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintAssetTypeID = TheFindSortedAssetTypesDataSet.FindSortedAssetTypes[intSelectedIndex].AssetTypeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Asset // Select Asset Type Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                    btnProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Asset // Select Office Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strAssetPartNumber;
            string strAssetDescription;
            decimal decAssetCost = 0;
            DateTime datActiveDate = DateTime.Now;
            string strAssetNotes;
            DateTime datDataEntryDate = DateTime.Now;
            int intEmployeeID;
            string strSerialNumber;

            try
            {
                strAssetPartNumber = txtAssetPartNumber.Text;
                if(strAssetPartNumber.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Part Number is not Long Enough\n";
                }
                if(cboSelectAssetType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Asset Type Was Not Selected\n";
                }
                strAssetDescription = txtAssetDescription.Text;
                if(strAssetDescription.Length < 6)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Asset Description Was Not Long Enough\n";
                }
                strValueForValidation = txtAssetPrice.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Asset Cost is not Numeric\n";
                }
                else
                {
                    decAssetCost = Convert.ToDecimal(strValueForValidation);
                }
                strSerialNumber = txtSerialNumber.Text;
                if(strSerialNumber.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "Serial Number is not Long Enough\n";
                }
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                strValueForValidation = txtActiveDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Active Date is not a Date\n";
                }
                else
                {
                    datActiveDate = Convert.ToDateTime(strValueForValidation);
                }
                strAssetNotes = txtAssetNotes.Text;
                if(strAssetNotes.Length < 6)
                {
                    strAssetNotes = "NO ASSET NOTES ENTERED";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);

                    return;
                }

                blnFatalError = TheAssetClass.InsertAssetMain(strAssetPartNumber, gintAssetTypeID, strAssetDescription, decAssetCost, MainWindow.gintWarehouseID, intEmployeeID, strAssetNotes, datDataEntryDate, strSerialNumber);

                if (blnFatalError == true)
                    throw new Exception();

                if(Convert.ToInt32(decAssetCost) >= 2500)
                {
                     
                }

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Create Asset");

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Asset Has Been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Asset // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
