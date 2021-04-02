/* Title:           Update Ticket Informatioin
 * Date:            4-2-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for Updating Ticket Info */

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
using HelpDeskDLL;
using PhonesDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateTicketInformation.xaml
    /// </summary>
    public partial class UpdateTicketInformation : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindHelpDeskTicketsByTicketIDDataSet TheFindHelpDeskTicketsByTicketIDDataSet = new FindHelpDeskTicketsByTicketIDDataSet();
        FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        FindHelpDeskTicketUpdatesByTicketIDDataSet TheFindHelpDeskTicketUpdatesByTicketIDDataSet = new FindHelpDeskTicketUpdatesByTicketIDDataSet();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        //setting up the variables
        string gstrUserEmailAddress;

        string gstrProblemStatus;

        public UpdateTicketInformation()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intEmployeeID;
            int intPhoneExtension = 0;
            string strProblemStatus;
            int intSelectedIndex = 0;
            string strUpdates = "";
            bool blnEmailNotCorrect = false;

            try
            {
                cboTicketStatus.Items.Clear();
                cboTicketStatus.Items.Add("Select Status");
                cboTicketStatus.Items.Add("OPEN");
                cboTicketStatus.Items.Add("IN PROCESS");
                cboTicketStatus.Items.Add("WAITING ON USER");
                cboTicketStatus.Items.Add("WAITING ON PARTS");
                cboTicketStatus.Items.Add("CLOSED");
                cboTicketStatus.SelectedIndex = 0;

                TheFindHelpDeskTicketsByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketByTicketID(MainWindow.gintTicketID);

                intEmployeeID = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].EmployeeID;

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                {
                    gstrUserEmailAddress = "techadmin@bluejaycommunications.com";
                }
                else
                {
                    gstrUserEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                    blnEmailNotCorrect = TheDataValidationClass.VerifyEmailAddress(gstrUserEmailAddress);

                    if(blnEmailNotCorrect == true)
                    {
                        gstrUserEmailAddress = "techadmin@bluejaycommunications.com";
                    }
                }

                TheFindPhoneExtensionByEmployeeIDDataSet = ThePhoneClass.FindPhoneExtensionByEmployeeID(intEmployeeID);

                intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    intPhoneExtension = 0;
                }
                else if(intRecordsReturned > 0)
                {
                    intPhoneExtension = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[0].Extension;
                }

                txtExtension.Text = Convert.ToString(intPhoneExtension);
                txtFirstName.Text = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].FirstName;
                txtLastName.Text = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].LastName;
                txtProblem.Text = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].ReportedProblem;
                txtProblemType.Text = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].ProblemType;
                txtTicketDate.Text = Convert.ToString(TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].TicketDate);
                txtTicketID.Text = Convert.ToString(MainWindow.gintTicketID);

                strProblemStatus = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].TicketStatus;

                intNumberOfRecords = cboTicketStatus.Items.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboTicketStatus.SelectedIndex = intCounter;

                    if(strProblemStatus == cboTicketStatus.SelectedItem.ToString())
                    {
                        intSelectedIndex = intCounter;
                    }
                }

                cboTicketStatus.SelectedIndex = intSelectedIndex;

                TheFindHelpDeskTicketUpdatesByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketUpdatesByTicketID(MainWindow.gintTicketID);

                intNumberOfRecords = TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strUpdates += Convert.ToString(TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID[intCounter].TransactionDate);
                        strUpdates += " - ";
                        strUpdates += TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID[intCounter].UpdateNotes;
                        strUpdates += "\n\n";
                    }
                }

                txtProblemUpdates.Text = strUpdates;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Ticket Information // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboTicketStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboTicketStatus.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrProblemStatus = cboTicketStatus.SelectedItem.ToString();
            }
        }

        private void expPrcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strUpdateNotes;
            DateTime datTransactionDate = DateTime.Now;
            int intEmployeeID;
            bool blnFatalError = false;
            string strHeader;
            string strMessage;

            try
            {
                if(cboTicketStatus.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("The Status was not Selected, Process Aborted");
                    return;
                }
                strUpdateNotes = txtProblemNotes.Text;
                if(strUpdateNotes.Length < 10)
                {
                    TheMessagesClass.ErrorMessage("The Notes Was not Long Enough");
                    return;
                }

                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketStatus(MainWindow.gintTicketID, gstrProblemStatus);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintTicketID, intEmployeeID, strUpdateNotes);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = "Ticket Number " + Convert.ToString(MainWindow.gintTicketID) + "Has Been Updated";

                strMessage = "<h1>" + strHeader + "</h1>";
                strMessage += "<p>" + strUpdateNotes + "</p>";

                blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Ticket Has Been Updated");

                if (blnFatalError == true)
                    throw new Exception();

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Ticket Info // Process  Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
