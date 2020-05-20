/* Title:           Update Rental 
 * Date:            5-5-20
 * Author:          Terry Holmes
 * 
 * Description:     This is the way to update a rental */

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
using RentalTrackingDLL;
using NewEventLogDLL;
using ProjectsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateRental.xaml
    /// </summary>
    public partial class UpdateRental : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        RentalTrackingClass TheRentalTrackingClass = new RentalTrackingClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();

        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindRentalTransactionByTransactionIDDataSet TheFindRentalTransactionByTransactionIDDataSet = new FindRentalTransactionByTransactionIDDataSet();
        FindRentalTrackingAgreementByRentalTrackingIDDataSet TheFindRentalTrackingAgreementByRentalTrackingIDDataSet = new FindRentalTrackingAgreementByRentalTrackingIDDataSet();
        FindRentalTrackingUpdateByRentalTrackingIDDataSet TheFindRentalTrackingUpdateByTrackingIDDataSet = new FindRentalTrackingUpdateByRentalTrackingIDDataSet();
        FindRentalTrackingItemsByRentalTrackingIDDataSet TheFindRentalTrackingItemsByRentalTrackingIDDataSet = new FindRentalTrackingItemsByRentalTrackingIDDataSet();

        public UpdateRental()
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
            //this will reset the conditions on the form.
            txtEnterPOProject.Text = "";
            txtEnterUpdate.Text = "";
            txtExpirationDate.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtProjectedCost.Text = "";
            txtProjectID.Text = "";
            txtRentalAgreement.Text = "";
            txtRequestingDate.Text = "";

            TheFindRentalTrackingItemsByRentalTrackingIDDataSet.FindRentalTrackingItemsByRentalTrackingID.Rows.Clear();

            dgrRentalItems.ItemsSource = TheFindRentalTrackingItemsByRentalTrackingIDDataSet.FindRentalTrackingItemsByRentalTrackingID;

            btnAddAgreement.IsEnabled = false;
            btnAddDocuments.IsEnabled = false;
            btnProcess.IsEnabled = false;
            btnViewAgreement.IsEnabled = false;    
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueEntered;
            int intRecordsReturned;
            int intProjectID;

            try
            {
                strValueEntered = txtEnterPOProject.Text;

                MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet = TheRentalTrackingClass.FindRentalTrackingTransactionByPONumber(strValueEntered);

                //getting the record count
                intRecordsReturned = MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet.FindRentalTrackingTransactionByPONumber.Rows.Count;

                if(intRecordsReturned == 1)
                {
                    MainWindow.gblnRentalPO = true;
                    MainWindow.gintRentalTrackingID = MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet.FindRentalTrackingTransactionByPONumber[0].TransactionID;
                }
                else if(intRecordsReturned > 1)
                {
                    MainWindow.gblnRentalPO = true;
                    SelectRental SelectRental = new SelectRental();
                    SelectRental.ShowDialog();
                }
                else if(intRecordsReturned < 1)
                {
                    MainWindow.gblnRentalPO = false;

                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strValueEntered);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Project Was Not Found");
                        return;
                    }

                    intProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                    MainWindow.TheFindRentalTransactionByProjectIDDataSet = TheRentalTrackingClass.FindRentalTransactionByProjectID(intProjectID);

                    //getting the record count
                    intRecordsReturned = MainWindow.TheFindRentalTransactionByProjectIDDataSet.FindRentalTransasctionByProjectID.Rows.Count;

                    if(intRecordsReturned == 1)
                    {
                        MainWindow.gintRentalTrackingID = MainWindow.TheFindRentalTransactionByProjectIDDataSet.FindRentalTransasctionByProjectID[0].TransactionID;
                    }
                    else if(intRecordsReturned > 1)
                    {
                        SelectRental SelectRental = new SelectRental();
                        SelectRental.ShowDialog();
                    }
                    else if(intRecordsReturned < 1)
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

                if(intRecordsReturned < 0)
                {
                    TheMessagesClass.ErrorMessage("There is a Major Problem, Call IT");

                    throw new Exception();
                }

                //loading  controls
                txtExpirationDate.Text = Convert.ToString(TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].ExpirationDate);
                txtFirstName.Text = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].FirstName;
                txtLastName.Text = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].LastName;
                txtProjectedCost.Text = Convert.ToString(TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].ProjectedCost);
                txtProjectID.Text = TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].AssignedProjectID;
                txtRequestingDate.Text = Convert.ToString(TheFindRentalTransactionByTransactionIDDataSet.FindRentalTransactionByTransactionID[0].RequestingDate);

                TheFindRentalTrackingAgreementByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingAgreementByRentalTrackingID(MainWindow.gintRentalTrackingID);

                txtRentalAgreement.Text = TheFindRentalTrackingAgreementByRentalTrackingIDDataSet.FindRentalTrackingAggreementByRentalTrackingID[0].AgreementPath;

                TheFindRentalTrackingItemsByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingItemsByRentalTrackingID(MainWindow.gintRentalTrackingID);

                dgrRentalItems.ItemsSource = TheFindRentalTrackingItemsByRentalTrackingIDDataSet.FindRentalTrackingItemsByRentalTrackingID;

                //activating buttons
                btnAddAgreement.IsEnabled = true;
                btnAddDocuments.IsEnabled = true;
                btnProcess.IsEnabled = true;
                btnViewAgreement.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Rental // Load Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());                
            }
        }

        private void btnViewAgreement_Click(object sender, RoutedEventArgs e)
        {
            string strDocumentPath;

            try
            {
                strDocumentPath = txtRentalAgreement.Text;

                System.Diagnostics.Process.Start(strDocumentPath);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Rental // View Agreement Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strUpdateNotes;
            bool blnFatalError = false;

            try
            {
                strUpdateNotes = txtEnterUpdate.Text;
                if(strUpdateNotes == "")
                {
                    TheMessagesClass.ErrorMessage("No Update Was Entered");
                    return;
                }

                blnFatalError = TheRentalTrackingClass.InsertRentalTrackingUpdate(MainWindow.gintRentalTrackingID, DateTime.Now, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strUpdateNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Update Entered");

                txtEnterUpdate.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Rental // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAddAgreement_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {

                EnterAgreementNumber EnterAgreementNumber = new EnterAgreementNumber();
                EnterAgreementNumber.ShowDialog();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheRentalTrackingClass.InsertRentalTrackingAgreement(MainWindow.gintRentalTrackingID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, MainWindow.gstrAgreementNo, strDocumentPath, "AGREEMENT ADDED");

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessagesClass.InformationMessage("The Agreement Has Been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Rental // Attach Agreement Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAddDocuments_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheRentalTrackingClass.InsertRentalTrackingDocumentation(MainWindow.gintRentalTrackingID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "RENTAL DOCUMENTS", strDocumentPath);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessagesClass.InformationMessage("The Documents Have Been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Rental // Add Documents Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expViewUpdates_Expanded(object sender, RoutedEventArgs e)
        {
            expViewUpdates.IsExpanded = false;
            ViewRentalUpdates ViewRentalUpdates = new ViewRentalUpdates();
            ViewRentalUpdates.ShowDialog();
        }
    }
}
