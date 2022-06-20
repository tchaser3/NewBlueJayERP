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
using ToolCategoryDLL;
using ToolIDDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddToolCategoryID.xaml
    /// </summary>
    public partial class AddToolCategoryID : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        ToolIDClass TheToolIDClass = new ToolIDClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data;
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindToolIDByCategoryDataSet TheFindToolIDByCategoryDataSet = new FindToolIDByCategoryDataSet();
        ToolIDDataSet TheToolIDDataSet = new ToolIDDataSet();

        public AddToolCategoryID()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ResetControls()
        {
            TheToolIDDataSet = TheToolIDClass.GetToolIDInfo();

            dgrToolIDs.ItemsSource = TheToolIDDataSet.toolid;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Add Tool Category ID");
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intCategoryID;
            int intRecordsReturned;
            string strCategory;
            bool blnFatalError = false;

            try
            {
                expProcess.IsExpanded = false;

                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].CategoryID;
                    strCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory;

                    TheFindToolIDByCategoryDataSet = TheToolIDClass.FindToolIDByCategory(strCategory);

                    intRecordsReturned = TheFindToolIDByCategoryDataSet.FindToolIDByCategory.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = TheToolIDClass.InsertNewToolIDForToolType(intCategoryID, "1000");

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Categories have been updated");

                TheToolIDDataSet = TheToolIDClass.GetToolIDInfo();

                dgrToolIDs.ItemsSource = TheToolIDDataSet.toolid;
            }
            catch(Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Add Tool Category ID // Process Button " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Tool Category ID // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
