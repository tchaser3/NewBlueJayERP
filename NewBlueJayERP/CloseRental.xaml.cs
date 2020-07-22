/* Title:           Close Rental
 * Date:            5-12-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to close the rental */

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
using NewEventLogDLL;
using RentalTrackingDLL;
using ProjectsDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CloseRental.xaml
    /// </summary>
    public partial class CloseRental : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        RentalTrackingClass TheRentalTrackingClass = new RentalTrackingClass();
        ProjectClass TheProjectClass = new ProjectClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindRentalTransactionByTransactionIDDataSet TheFindRentalTransactionByTransactionIDDataSet = new FindRentalTransactionByTransactionIDDataSet();
        FindRentalTrackingAgreementByRentalTrackingIDDataSet TheFindRentalTrackingAgreementByRentalTrackingIDDataSet = new FindRentalTrackingAgreementByRentalTrackingIDDataSet();
        FindRentalTrackingItemsByRentalTrackingIDDataSet TheFindRentalTrackingItemsByRentalTrackingIDDataSet = new FindRentalTrackingItemsByRentalTrackingIDDataSet();

        public CloseRental()
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
            txtEnterProjectPO.Text = "";
            txtEXPDate.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtProjectedCost.Text = "";
            txtProjectID.Text = "";
            txtRequestingDate.Text = "";
            txtReturnDate.Text = "";
            txtTotalCost.Text = "";
            txtViewAgreement.Text = "";

            TheFindRentalTrackingItemsByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingItemsByRentalTrackingID(-1000);

            dgrRentalItems.ItemsSource = TheFindRentalTrackingItemsByRentalTrackingIDDataSet.FindRentalTrackingItemsByRentalTrackingID;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueEntered;
            int intRecordsReturned;
            int intProjectID;

            try
            {
                strValueEntered = txtEnterProjectPO.Text;

                MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet = TheRentalTrackingClass.FindRentalTrackingTransactionByPONumber(strValueEntered);

                //getting the record count
                intRecordsReturned = MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet.FindRentalTrackingTransactionByPONumber.Rows.Count;

                if (intRecordsReturned == 1)
                {
                    MainWindow.gblnRentalPO = true;
                    MainWindow.gintRentalTrackingID = MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet.FindRentalTrackingTransactionByPONumber[0].TransactionID;
                }
                else if (intRecordsReturned > 1)
                {
                    MainWindow.gblnRentalPO = true;
                    SelectRental SelectRental = new SelectRental();
                    SelectRental.ShowDialog();
                }
                else if (intRecordsReturned < 1)
                {
                    MainWindow.gblnRentalPO = false;

                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strValueEntered);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Project Was Not Found");
                        return;
                    }

                    intProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                    MainWindow.TheFindRentalTransactionByProjectIDDataSet = TheRentalTrackingClass.FindRentalTransactionByProjectID(intProjectID);

                    //getting the record count
                    intRecordsReturned = MainWindow.TheFindRentalTransactionByProjectIDDataSet.FindRentalTransasctionByProjectID.Rows.Count;

                    if (intRecordsReturned == 1)
                    {
                        MainWindow.gintRentalTrackingID = MainWindow.TheFindRentalTransactionByProjectIDDataSet.FindRentalTransasctionByProjectID[0].TransactionID;
                    }
                    else if (intRecordsReturned > 1)
                    {
                        SelectRental SelectRental = new SelectRental();
                        SelectRental.ShowDialog();
                    }
                    else if (intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("No Rentals Were Found");
                        return;
                    }
                }

                LoadControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Rental // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadControls()
        {
            int intRecordsReturned;

            try
            {
                TheFindRentalTransactionByTransactionIDDataSet = TheRentalTrackingClass.FindRentalTransactionByTransactionID(MainWindow.gintRentalTrackingID);

                intRecordsReturned = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID.Rows.Count - 1;

                if (intRecordsReturned < 0)
                {
                    TheMessagesClass.ErrorMessage("There is a Major Problem, Call IT");

                    throw new Exception();
                }

                //loading  controls
                txtEXPDate.Text = Convert.ToString(TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].ExpirationDate);
                txtFirstName.Text = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].FirstName;
                txtLastName.Text = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].LastName;
                txtProjectedCost.Text = Convert.ToString(TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].ProjectedCost);
                txtProjectID.Text = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].AssignedProjectID;
                txtRequestingDate.Text = Convert.ToString(TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].RequestingDate);

                TheFindRentalTrackingAgreementByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingAgreementByRentalTrackingID(MainWindow.gintRentalTrackingID);

                txtViewAgreement.Text = TheFindRentalTrackingAgreementByRentalTrackingIDDataSet.FindRentalTrackingAggreementByRentalTrackingID[0].AgreementPath;

                TheFindRentalTrackingItemsByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingItemsByRentalTrackingID(MainWindow.gintRentalTrackingID);

                dgrRentalItems.ItemsSource = TheFindRentalTrackingItemsByRentalTrackingIDDataSet.FindRentalTrackingItemsByRentalTrackingID;

                //activating buttons
                btnViewAgreement.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Close Rental // Load Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnViewAgreement_Click(object sender, RoutedEventArgs e)
        {
            string strDocumentPath;

            try
            {
                strDocumentPath = txtViewAgreement.Text;

                System.Diagnostics.Process.Start(strDocumentPath);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Close Rental // View Agreement Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnCloseRental_Click(object sender, RoutedEventArgs e)
        {
            //setting up variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            DateTime datCloseDate = DateTime.Now;
            decimal decTotalCost = 0;
            int intNumberOfDays = 0;
            DateTime datRequestedDate;

            try
            {
                //data validation
                strValueForValidation = txtReturnDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Return Date is not a Date\n";
                }
                else
                {
                    datCloseDate = Convert.ToDateTime(strValueForValidation);

                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datCloseDate, DateTime.Now);

                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Return Date is After Today\n";
                    }
                }
                strValueForValidation = txtTotalCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Total Cost is not Numeric\n";
                }
                else
                {
                    decTotalCost = Convert.ToDecimal(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                datRequestedDate = Convert.ToDateTime(txtRequestingDate.Text);

                //doing the math
                intNumberOfDays = TheDateSearchClass.DateDifference(datRequestedDate, datCloseDate);

                blnFatalError = TheRentalTrackingClass.CloseRentalTrackingTransaction(MainWindow.gintRentalTrackingID, datCloseDate, intNumberOfDays, decTotalCost);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Rental Has Been Closed");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Close Rental // Close Rental Button " + Ex.Message);

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
