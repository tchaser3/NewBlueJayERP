/* Title:       Add WOV Billing Codes
 * Date:        3-10-21
 * Author:      Terry Holmes
 * 
 * Description: This is used to add WOV Billing Codes */

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
using WOVInvoicingDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddWOVBillingCodes.xaml
    /// </summary>
    public partial class AddWOVBillingCodes : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailCodes = new SendEmailClass();

        //setting up the data
        FindWOVBillingCodeByDescriptionDataSet TheFindWOVBillingCodeByDescriptionDataSet = new FindWOVBillingCodeByDescriptionDataSet();
        FindWOVBillingCodesByBillingCodesDataSet TheFindWOVBillingCodesByBillingCodesDataSet = new FindWOVBillingCodesByBillingCodesDataSet();

        public AddWOVBillingCodes()
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
            txtBillingCode.Text = "";
            txtBillingDescription.Text = "";

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add WOV Billing Codes");
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strBillingCode;
            string strBillingDescription;
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intRecordsReturned;

            strBillingCode = txtBillingCode.Text;
            if (strBillingCode == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Billing Code was not Entered\n";
            }
            strBillingDescription = txtBillingDescription.Text;
            if (strBillingDescription == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Billing Description was not Entered\n";
            }
            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            TheFindWOVBillingCodesByBillingCodesDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByBillingCodes(strBillingCode);

            intRecordsReturned = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode.Rows.Count;

            if (intRecordsReturned > 0)
            {
                TheMessagesClass.ErrorMessage("The Billing Code Has Already Been Entered");
                return;
            }

            TheFindWOVBillingCodeByDescriptionDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByDescription(strBillingDescription);

            intRecordsReturned = TheFindWOVBillingCodeByDescriptionDataSet.FindWOVBillingCodeByDescription.Rows.Count;

            if (intRecordsReturned > 0)
            {
                return;
            }

            try
            {
                blnFatalError = TheWOVInvoicingClass.InsertWOVBillingCodes(strBillingDescription, strBillingCode);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Billing Code has been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheSendEmailCodes.SendEventLog("New Blue Jay ERP // Add WOV Billing Codes // Process Button " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add WOV Billing Codes // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
