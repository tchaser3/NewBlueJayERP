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
    /// Interaction logic for EditWOVBillingCodes.xaml
    /// </summary>
    public partial class EditWOVBillingCodes : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        FindWOVBillingCodesByBillingCodesDataSet TheFindWOVBillingCodesByBillingCodesDataSet = new FindWOVBillingCodesByBillingCodesDataSet();

        string gstrBillingCode;
        int gintBillingID;

        public EditWOVBillingCodes()
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
            txtBillingDescription.Text = "";
            txtBillingID.Text = "";
            txtEnterBillingCode.Text = "";

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit WOV Billing Codes");
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;

            gstrBillingCode = txtEnterBillingCode.Text;
            if (gstrBillingCode == "")
            {
                TheMessagesClass.ErrorMessage("The Billing Code Was Not Entered");
                return;
            }

            TheFindWOVBillingCodesByBillingCodesDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByBillingCodes(gstrBillingCode);

            intRecordsReturned = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode.Rows.Count;

            if (intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("The Billing Code Was Not Found");
                return;
            }

            txtBillingDescription.Text = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode[0].BillingDescription;
            txtBillingID.Text = Convert.ToString(TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode[0].BillingID);
            gintBillingID = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode[0].BillingID;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError;
            string strBillingDescription;

            try
            {
                strBillingDescription = txtBillingDescription.Text;
                if (strBillingDescription == "")
                {
                    TheMessagesClass.ErrorMessage("The Billing Description was not Entered");
                    return;
                }

                blnFatalError = TheWOVInvoicingClass.UpdateWOVBillingCodeDescription(gintBillingID, strBillingDescription);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The WOV Billing Code has been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit WOV Billing Code // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
