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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddIncentivePayStatus.xaml
    /// </summary>
    public partial class AddIncentivePayStatus : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        IncentivePayClass TheIncentivePayClass = new IncentivePayClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //Setting up data
        FindIncentivePayStatusByTransactionStatusDataSet TheFindIncentivePayStatusByTransactionStatusDataSet = new FindIncentivePayStatusByTransactionStatusDataSet();

        public AddIncentivePayStatus()
        {
            InitializeComponent();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessagesClass.LaunchEmail();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.gintEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP // Add Incentive Pay Status ");
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strStatus;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                strStatus = txtEnterStatus.Text;

                if(strStatus.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Status is too Short");
                    return;
                }

                TheFindIncentivePayStatusByTransactionStatusDataSet = TheIncentivePayClass.FindIncentivePayStatusByTransactionStatus(strStatus);

                intRecordsReturned = TheFindIncentivePayStatusByTransactionStatusDataSet.FindIncentivePayStatusByTransactionStatus.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Status Is Already Entered");
                    return;
                }

                blnFatalError = TheIncentivePayClass.InsertIncentivePayStatus(strStatus);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Status Has Been Entered");

                txtEnterStatus.Text = "";
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Incentive Pay Status // Process Button " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Add Incentive Pay Status // Process Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
           
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
