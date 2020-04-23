/* Title:       Create Rental
 * Date:        3-19-2020
 * Author:      Terry Holmes
 * 
 * Description: This is used to create a new rental unit */

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
using RentalTrackingDLL;
using VendorsDLL;
using DataValidationDLL;
using DateSearchDLL;
using ProjectsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateRental.xaml
    /// </summary>
    public partial class CreateRental : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        RentalTrackingClass TheRentalTrackingClass = new RentalTrackingClass();
        VendorsClass TheVendorClass = new VendorsClass();
        ProjectClass TheProjectClass = new ProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up data variables
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorNameDataSet = new FindVendorsSortedByVendorNameDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindRentalTrackingTransactionByRequestingDateMatchDataSet TheFindRentalTrackingTransactionByRequestingDateMatchDataSet = new FindRentalTrackingTransactionByRequestingDateMatchDataSet();

        public CreateRental()
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
            int intNumberOfRecords;
            int intCounter;

            txtEnterLastName.Text = "";
            txtExpirationDate.Text = "";
            txtPickUpDate.Text = "";
            txtPONumber.Text = "";
            txtProjectedCost.Text = "";
            txtProjectID.Text = "";
            txtRentalNotes.Text = "";
            txtRequestDate.Text = Convert.ToString(DateTime.Now);
            txtAgreementNo.Text = "";

            cboSelectEmployee.Items.Clear();
            cboSelectVendor.Items.Clear();
            cboSelectVendor.Items.Add("Select Vendor");

            TheFindVendorsSortedByVendorNameDataSet = TheVendorClass.FindVendorsSortedByVendorName();

            intNumberOfRecords = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

            for(intCounter =0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectVendor.Items.Add(TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboSelectVendor.SelectedIndex = 0;
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;                                                                                                                                                                                                                                     
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Rental // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strAssignedProjectID;
            DateTime datPickupDate = DateTime.Now;
            DateTime datExpDate = DateTime.Now;
            DateTime datRequestDate = DateTime.Now;
            decimal decProjectedCost = 0;
            string strRentalNotes;
            string strPONumber;
            int intRecordsReturned;
            DateTime datRequestingDate = DateTime.Now;
            int intNoOfDays = 0;

            try
            {
                expProcess.IsExpanded = false;

                //beginning data validation
                strPONumber = txtPONumber.Text;
                if(strPONumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "PO Number Not Entered\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Was Not Selected\n";
                }
                if(cboSelectVendor.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vendor Was Not Selected\n";
                }
                strAssignedProjectID = txtProjectID.Text;
                if(strAssignedProjectID == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project ID Was Not Entered\n";
                }
                else
                {
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Project Was Not Found\n";
                    }
                    else
                    {
                        MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                        MainWindow.gstrAssignedProjectID = strAssignedProjectID;
                    }
                }
                MainWindow.gstrAgreementNo = txtAgreementNo.Text;
                if(MainWindow.gstrAgreementNo == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Rental Agreement Number Not Entered\n";
                }
                strValueForValidation = txtPickUpDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Pickup Date is not a Date\n";
                }
                else
                {
                    datPickupDate = Convert.ToDateTime(strValueForValidation);
                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(DateTime.Now, datPickupDate);

                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Pickup Date is Before Today\n";
                    }
                    
                }
                strValueForValidation = txtExpirationDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Expiration Date is not a Date\n";
                }
                else
                {
                    datExpDate = Convert.ToDateTime(strValueForValidation);

                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(DateTime.Now, datExpDate);

                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Expiration Date is Before Today\n";
                    }
                }
                strValueForValidation = txtProjectedCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Projected Cost is not Numeric\n";
                }
                else
                {
                    decProjectedCost = Convert.ToDecimal(strValueForValidation);
                }
                strRentalNotes = txtRentalNotes.Text;
                if(strRentalNotes == "")
                {
                    strRentalNotes = "Created Rental Object";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intNoOfDays = TheDateSearchClass.DateDifference(datPickupDate, datExpDate);

                blnFatalError = TheRentalTrackingClass.InsertRentalTrackingTransaction(datRequestDate, strPONumber, MainWindow.gintEmployeeID, MainWindow.gintVendorID, datPickupDate, datExpDate, intNoOfDays, 0, MainWindow.gintProjectID, decProjectedCost);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindRentalTrackingTransactionByRequestingDateMatchDataSet = TheRentalTrackingClass.FindRentalTrackingTransactionByRequestingDateMatch(datRequestDate);

                MainWindow.gintRentalTrackingID = TheFindRentalTrackingTransactionByRequestingDateMatchDataSet.FindRentalTrackingTransactionByRequestingDateMatch[0].TransactionID;

                blnFatalError = TheRentalTrackingClass.InsertRentalTrackingUpdate(MainWindow.gintRentalTrackingID, datRequestDate, MainWindow.gintEmployeeID, "CREATED RENTAL");

                if (blnFatalError == true)
                    throw new Exception();

                RentalItems RentalItems = new RentalItems();
                RentalItems.ShowDialog();

                TheMessagesClass.InformationMessage("The Rental Has Been Added");

                ResetControls();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Rental // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
        }

        private void cboSelectVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectVendor.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                MainWindow.gintVendorID = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;
        }
    }
}
