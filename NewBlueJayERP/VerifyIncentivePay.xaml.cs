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
using IncentivePayDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VerifyIncentivePay.xaml
    /// </summary>
    public partial class VerifyIncentivePay : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        IncentivePayClass TheIncentivePayClass = new IncentivePayClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up data sets
        FindIncentivePayByStatusDataSet TheFindIncentivePayByStatusDataSet = new FindIncentivePayByStatusDataSet();
        FindSortedIncentivePayStatusDataSet TheFindSortedIncentivePayStatusDataSet = new FindSortedIncentivePayStatusDataSet();


        public VerifyIncentivePay()
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
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting up variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Verify Incentive Pay Window");

                cboSelectStatus.Items.Clear();
                cboSelectStatus.Items.Add("Select Incentive Pay Status");

                TheFindSortedIncentivePayStatusDataSet = TheIncentivePayClass.FindSortedIncentivePayStatus();

                intNumberOfRecords = TheFindSortedIncentivePayStatusDataSet.FindSortedIncentivePayStatus.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectStatus.Items.Add(TheFindSortedIncentivePayStatusDataSet.FindSortedIncentivePayStatus[intCounter].TransactionStatus);
                }

                cboSelectStatus.SelectedIndex = 0;

                TheFindIncentivePayByStatusDataSet = TheIncentivePayClass.FindIncentivePayByStatus("");

                dgrResults.ItemsSource = TheFindIncentivePayByStatusDataSet.FindIncentivePayByStatus;
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Verify Incentive Pay Window // Window Loaded Event " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Verify Incentive Pay Window // Window Loaded Event " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void cboSelectStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strTransactionStatus;

            try
            {
                intSelectedIndex = cboSelectStatus.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    strTransactionStatus = TheFindSortedIncentivePayStatusDataSet.FindSortedIncentivePayStatus[intSelectedIndex].TransactionStatus;

                    TheFindIncentivePayByStatusDataSet = TheIncentivePayClass.FindIncentivePayByStatus(strTransactionStatus);

                    dgrResults.ItemsSource = TheFindIncentivePayByStatusDataSet.FindIncentivePayByStatus;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Verify Incentive Pay Window // Select Status ComboBox " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Verify Incentive Pay Window // Select Status ComboBox " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
    }
}
