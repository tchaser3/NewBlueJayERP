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
using EmployeeDateEntryDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmailServerReport.xaml
    /// </summary>
    public partial class EmailServerReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindServerEventLogSecurityAccessDataSet TheFindServerEventLogSecurityAccessDataSet = new FindServerEventLogSecurityAccessDataSet();
        FindServerEventLogSecurityAccessByKeywordDataSet TheFindServerEventLogSecurityAccessByKeywordDataSet = new FindServerEventLogSecurityAccessByKeywordDataSet();
        EventlLogSecurityDataSet TheEventLogSecurityDataSet = new EventlLogSecurityDataSet();

        int gintNumberOfRecords;

        public EmailServerReport()
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

            dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

            dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;
        }
        private void ResetControls()
        {
            //setting up the variables
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strLogonName;
            string strItemAccessed;
            string strEventNotes;
            bool blnItemFound = false;
            int intSecondCounter;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheEventLogSecurityDataSet.eventlogsecurity.Rows.Clear();

                TheFindServerEventLogSecurityAccessDataSet = TheEventLogClass.FindServerEventLogSecurityAccess();

                intNumberOfRecords = TheFindServerEventLogSecurityAccessDataSet.FindServerEventLogSecurityAccess.Rows.Count;
                gintNumberOfRecords = 0;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        datTransactionDate = TheFindServerEventLogSecurityAccessDataSet.FindServerEventLogSecurityAccess[intCounter].TransactionDate;
                        strLogonName = "Just Beginging";
                        strItemAccessed = "Date Goes Here";
                        strEventNotes = TheFindServerEventLogSecurityAccessDataSet.FindServerEventLogSecurityAccess[intCounter].EventNotes;

                        char[] delims = new[] { '\n', '\t', '\r' };
                        string[] strNewItems = strEventNotes.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        strLogonName = strNewItems[5];
                        strItemAccessed = strNewItems[16];
                        blnItemFound = false;

                        datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);

                        if(gintNumberOfRecords > 0)
                        {
                            for(intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if(datTransactionDate == TheEventLogSecurityDataSet.eventlogsecurity[intSecondCounter].TransactionDate)
                                {
                                    if(strLogonName == TheEventLogSecurityDataSet.eventlogsecurity[intSecondCounter].LogonName)
                                    {
                                        if(strItemAccessed == TheEventLogSecurityDataSet.eventlogsecurity[intSecondCounter].ItemAccessed)
                                        {
                                            blnItemFound = true;
                                        }
                                    }
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            EventlLogSecurityDataSet.eventlogsecurityRow NewEventRow = TheEventLogSecurityDataSet.eventlogsecurity.NeweventlogsecurityRow();

                            NewEventRow.TransactionDate = datTransactionDate;
                            NewEventRow.LogonName = strLogonName;
                            NewEventRow.ItemAccessed = strItemAccessed;

                            TheEventLogSecurityDataSet.eventlogsecurity.Rows.Add(NewEventRow);
                            gintNumberOfRecords++;
                        }

                       
                    }
                }

                dgrEventLog.ItemsSource = TheEventLogSecurityDataSet.eventlogsecurity;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Server Security Report");

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Event Log Security // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expEmailReport_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress = "tholmes@bluejaycommunications.com";
            string strHeader;
            string strMessage;
            DateTime datPayDate = DateTime.Now;
            bool blnFatalError = false;

            try
            {
                expEmailReport.IsExpanded = false;
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intNumberOfRecords = TheEventLogSecurityDataSet.eventlogsecurity.Rows.Count;

                strHeader = "Server File Access Report Prepared on " + Convert.ToString(datPayDate);

                strMessage = "<h1>Server File Access Report Prepared on " + Convert.ToString(datPayDate) + "</h1>";
                strMessage += "<p>               </p>";
                strMessage += "<p>               </p>";
                strMessage += "<table>";
                strMessage += "<tr>";
                strMessage += "<td><b>Transaction Date</b></td>";
                strMessage += "<td><b>Logon Name</b></td>";
                strMessage += "<td><b>Item Accessed</b></td>";
                strMessage += "</tr>";
                strMessage += "<p>               </p>";

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strMessage += "<tr>";
                        strMessage += "<td>" + Convert.ToString(TheEventLogSecurityDataSet.eventlogsecurity[intCounter].TransactionDate) + "</td>";
                        strMessage += "<td>" + TheEventLogSecurityDataSet.eventlogsecurity[intCounter].LogonName + "</td>";
                        strMessage += "<td>" + TheEventLogSecurityDataSet.eventlogsecurity[intCounter].ItemAccessed + "</td>";
                        strMessage += "</tr>";
                        strMessage += "<p>               </p>";
                    }
                }

                strMessage += "</table>";

                blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();
                
                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Email Server Report // Email Report Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expCreateReport_Expanded(object sender, RoutedEventArgs e)
        {
            expCreateReport.IsExpanded = false;

            ResetControls();
        }
    }
}
