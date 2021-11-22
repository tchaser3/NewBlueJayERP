/* Title:           Add Search Terms
 * Date:            11-3-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add search terms for automatic server log reports */

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
    /// Interaction logic for AddSearchTerms.xaml
    /// </summary>
    public partial class AddSearchTerms : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setup data
        FindActiveServerLogSearchTermsDataSet TheFindActiveServerLogSearchTermsDataSet = new FindActiveServerLogSearchTermsDataSet();
        FindServerLogSearchTermDataSet TheFindServerLogSearchTermDataSet = new FindServerLogSearchTermDataSet();

        public AddSearchTerms()
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
            txtEnterSeachTerm.Text = "";

            TheFindActiveServerLogSearchTermsDataSet = TheEventLogClass.FindActiveServerLogSearchTerms();

            dgrResult.ItemsSource = TheFindActiveServerLogSearchTermsDataSet.FindActiveServerLogSearchTerms;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;
            bool blnFatalError = false;
            string strSearchTerm;

            try
            {
                strSearchTerm = txtEnterSeachTerm.Text;

                if(strSearchTerm.Length < 3)
                {
                    TheMessagesClass.ErrorMessage("The Search Term is not Long Enough");
                    return;
                }

                TheFindServerLogSearchTermDataSet = TheEventLogClass.FindServerLogSearchTerm(strSearchTerm);

                intRecordsReturned = TheFindServerLogSearchTermDataSet.FindServerLogSearchTerm.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Search Term already Exists");
                    return;
                }

                blnFatalError = TheEventLogClass.InsertServerLogSearchTerm(strSearchTerm);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Search Term has been Inserted");

                ResetControls();
            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Search Terms // Process Button " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }
    }
}
