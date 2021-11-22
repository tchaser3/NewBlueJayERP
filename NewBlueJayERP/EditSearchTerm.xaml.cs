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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditSearchTerm.xaml
    /// </summary>
    public partial class EditSearchTerm : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindSortedServerLogSearchTermsDataSet TheFindSortedServerLogSearchTermsDataSet = new FindSortedServerLogSearchTermsDataSet();
        SearchTermsEditingDataSet TheSearchTermEditingDataSet = new SearchTermsEditingDataSet();

        public EditSearchTerm()
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindSortedServerLogSearchTermsDataSet = TheEventLogClass.FindSortedServerLogSearchTerms();

                TheSearchTermEditingDataSet.searchtermediting.Rows.Clear();

                intNumberOfRecords = TheFindSortedServerLogSearchTermsDataSet.FindSortedServerLogSearchTerms.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        SearchTermsEditingDataSet.searchtermeditingRow NewTermRow = TheSearchTermEditingDataSet.searchtermediting.NewsearchtermeditingRow();

                        NewTermRow.SearchTerm = TheFindSortedServerLogSearchTermsDataSet.FindSortedServerLogSearchTerms[intCounter].SearchTerm;
                        NewTermRow.IsChanged = false;
                        NewTermRow.TermActive = TheFindSortedServerLogSearchTermsDataSet.FindSortedServerLogSearchTerms[intCounter].TermActive;
                        NewTermRow.TransactionID = TheFindSortedServerLogSearchTermsDataSet.FindSortedServerLogSearchTerms[intCounter].TransactionID;

                        TheSearchTermEditingDataSet.searchtermediting.Rows.Add(NewTermRow);
                    }
                }

                dgrSearchTerms.ItemsSource = TheSearchTermEditingDataSet.searchtermediting;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Search Terms // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());   
            }
           
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intTransactionID;
            bool blnActive;
            bool blnFatalError = false;

            try
            {
                intNumberOfRecords = TheSearchTermEditingDataSet.searchtermediting.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheSearchTermEditingDataSet.searchtermediting[intCounter].IsChanged == true)
                        {
                            intTransactionID = TheSearchTermEditingDataSet.searchtermediting[intCounter].TransactionID;

                            blnActive = TheSearchTermEditingDataSet.searchtermediting[intCounter].TermActive;

                            blnFatalError = TheEventLogClass.UpdateServerLogSearchTermActive(intTransactionID, blnActive);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Search Terms are Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Search Term // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
