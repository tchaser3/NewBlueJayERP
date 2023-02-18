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
using IncentivePayDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;
using NewEmployeeDLL;
using ProjectMatrixDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditIncentivePayTransaction.xaml
    /// </summary>
    public partial class EditIncentivePayTransaction : Window
    {
        //Setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        IncentivePayClass TheIncentivePayClass = new IncentivePayClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //setting up the data
        FindIncentivePayTransactionStatusByIncentivePayTransactionIDataSet TheFindIncentivePayTransactionStatusByIncentivePayTransactionIDDataSet = new FindIncentivePayTransactionStatusByIncentivePayTransactionIDataSet();
        FindIncentivePayByTransactionIDDataSet TheFindIncentivePayByTransactionIDDataSet = new FindIncentivePayByTransactionIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindSortedIncentivePayStatusDataSet TheFindSortedIncentivePayStatusDataSet = new FindSortedIncentivePayStatusDataSet();
        FindProjectMatrixByCustomerAssignedIDDataSet TheFindProjectMatrixByCustomerAssignedIDDataSet = new FindProjectMatrixByCustomerAssignedIDDataSet();
        FindIncentivePayPositionByKeywordDataSet TheFindIncentivePayPostionByKeywordDataSet = new FindIncentivePayPositionByKeywordDataSet();

        //setting up global variables
        string gstrManagerEmailAddress;
        string gstrModifyingUser;
        string gstrManagerEmail;

        public EditIncentivePayTransaction()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Close();
        }
        private void ResetControls()
        {
            //setting up local variables
            int intManagerID;
            string strManagerName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindIncentivePayByTransactionIDDataSet = TheIncentivePayClass.FindIncentivePayByTransactionID(MainWindow.gintTransactionID);

                if(TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID.Rows.Count > 0)
                {
                    intManagerID = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].ManagerID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                    strManagerName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                    strManagerName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                    if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == false)
                    {
                        gstrManagerEmail = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                    }
                    else
                    {
                        gstrManagerEmail = "NO EMAIL";
                    }

                    txtManager.Text = strManagerName;

                    txtAssignedProjectID.Text = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].AssignedProjectID;
                    txtCurrentStatus.Text = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].CurrentStatus;
                    txtCustomerProjectID.Text = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].CustomerAssignedID;
                    txtEmployee.Text = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].Employee;
                    txtPositionTitle.Text = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].PositionTitle;
                    txtProductionDate.Text = Convert.ToString(TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].ProductionDate);
                    txtProjectName.Text = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].ProjectName;
                    txtRatePerUnit.Text = Convert.ToString(TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].RatePerUnit);
                    txtTotalIncentivePay.Text = Convert.ToString(TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].TotalIncentivePay);
                    txtTotalUnits.Text = Convert.ToString(TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].TotalUnits);
                }

                cboSelectStatus.Items.Clear();
                cboSelectStatus.Items.Add("Select Status");

                TheFindSortedIncentivePayStatusDataSet = TheIncentivePayClass.FindSortedIncentivePayStatus();

                intNumberOfRecords = TheFindSortedIncentivePayStatusDataSet.FindSortedIncentivePayStatus.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectStatus.Items.Add(TheFindSortedIncentivePayStatusDataSet.FindSortedIncentivePayStatus[intCounter].TransactionStatus);
                }

                cboSelectStatus.SelectedIndex = 0;
                gstrModifyingUser = System.Environment.UserName;

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Edit Incentive Pay Transaction // Reset Controls Method " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Incentive Pay Transaction // Reset Controls Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //This will process the update on incentive pay
            string strNotes = "";
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intProjectID;
            int intTotalUnits;
            int intPositionID;
            decimal decRateOfPay;
            decimal decTotalIncentivePay;
            string strCustomerProjectID;
            string strPosition;
            string strStatus = "";

            try
            {
                //setting up data validation
                if(cboSelectStatus.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Position Was Not Selected\n";
                }
                else
                {
                    strStatus = TheFindSortedIncentivePayStatusDataSet.FindSortedIncentivePayStatus[cboSelectStatus.SelectedIndex - 1].TransactionStatus;
                }
                strNotes = txtEnterNotes.Text;
                if(strNotes.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Notes Entered is not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strCustomerProjectID = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].CustomerAssignedID;
                strPosition = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].PositionTitle;

                TheFindIncentivePayPostionByKeywordDataSet = TheIncentivePayClass.FindIncentivePayPositionByKeyword(strPosition);
                intPositionID = TheFindIncentivePayPostionByKeywordDataSet.FindIncentivePayPositionByKeyword[0].PositionID;

                TheFindProjectMatrixByCustomerAssignedIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedID(strCustomerProjectID);

                intProjectID = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ProjectID;
                decRateOfPay = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].RatePerUnit;
                
                intTotalUnits = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].TotalUnits;
                decTotalIncentivePay = TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].TotalIncentivePay;

                blnFatalError = TheIncentivePayClass.UpdateEmployeeIncentivePayTransaction(MainWindow.gintTransactionID, gstrModifyingUser, intProjectID, intPositionID, intTotalUnits, decRateOfPay, decTotalIncentivePay);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                blnFatalError = TheIncentivePayClass.UpdateIncentivePayCurrentStatus(MainWindow.gintTransactionID, gstrModifyingUser, strStatus);

                SendNotifications(txtEmployee.Text, strCustomerProjectID, strStatus, strNotes);

                blnFatalError = TheIncentivePayClass.InsertIncentivePayUpdate(gstrModifyingUser, TheFindIncentivePayByTransactionIDDataSet.FindIncentivePayByTransactionID[0].EmployeeID, MainWindow.gintTransactionID, strNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Incentive Pay Has Been Updated");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Edit Incentive Pay Transaction // Process Button " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Incentive Pay Transaction // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SendNotifications(string strEmployeeName, string strCustomerProjectID, string strStatus, string strNotes)
        {
            string strHeader = "";
            string strMessage = "";

            try
            {
                strHeader = "Incentive Pay Has Been Changed ";

                strMessage = "<h1>" + strHeader + "</h1>";
                strMessage += "<h3>Incentive Pay Has been modified for " + strEmployeeName + "</h3>";
                strMessage += "<h3>This is for Project " + strCustomerProjectID + "</h3>";
                strMessage += "<h3>The employee incentive pay current status is " + strStatus + "</h3>";
                strMessage += "<p>" + strNotes + "</p>";

                if(gstrManagerEmail != "NO EMAIL")
                {
                    TheSendEmailClass.SendEmail(gstrManagerEmail, strHeader, strMessage);
                }
                
                TheSendEmailClass.SendEmail("adminincentivepay@bluejaycommunications.com", strHeader, strMessage);
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Edit Incentive Pay Transaction // Send Notifications Method " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Incentive Pay Transaction // Send  Notifications Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
