/* Title:           Update Trailer Problems
 * Date:            11-4-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to update trailer problems */

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
using TrailersDLL;
using TrailerProblemDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;
using VendorsDLL;
using TrailersDLL.FindActiveSortedTrailersDataSetTableAdapters;
using System.ComponentModel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateTrailerProblems.xaml
    /// </summary>
    public partial class UpdateTrailerProblems : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VendorsClass TheVendorsClass = new VendorsClass();

        //setting up the data
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorNameDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindTrailerByTrailerNumberDataSet TheFindTrailerByTrailerNumberDataSet = new FindTrailerByTrailerNumberDataSet();
        FindOpenTrailerProblemsByTrailerIDDataSet TheFindOpenTrailerProblemsByTrailerIDDataSet = new FindOpenTrailerProblemsByTrailerIDDataSet();
        OpenTrailerProblemsDataSet TheOpenTrailerProblemsDataSet = new OpenTrailerProblemsDataSet();

        public UpdateTrailerProblems()
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
            this.Visibility = Visibility.Hidden;
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

            //setting up initial conditions
            txtEnterTrailerNumber.Text = "";
            txtProblemUpdate.Text = "";
            txtReportedDate.Text = "";
            txtReportedProblem.Text = "";
            cboSelectVendor.Items.Clear();

            TheFindVendorsSortedByVendorNameDataSet = TheVendorsClass.FindVendorsSortedByVendorName();

            intNumberOfRecords = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName.Rows.Count;
            cboSelectVendor.Items.Add("Select Vendor");

            for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
            {
                cboSelectVendor.Items.Add(TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboSelectVendor.SelectedIndex = 0;

            TheOpenTrailerProblemsDataSet.opentrailerproblems.Rows.Clear();

            dgrProblems.ItemsSource = TheOpenTrailerProblemsDataSet.opentrailerproblems;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Update Trailer Problem");

        }

        private void cboSelectVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectVendor.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintVendorID = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID; 
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Trailer Problems // Select Vendor Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strTrailerNumber;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            string strFullName;

            try
            {
                strTrailerNumber = txtEnterTrailerNumber.Text;
                TheOpenTrailerProblemsDataSet.opentrailerproblems.Rows.Clear();

                if(strTrailerNumber.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Trailer Number is to Short");
                    return;
                }

                TheFindTrailerByTrailerNumberDataSet = TheTrailersClass.FindTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Trailer Was Not Found");
                    return;
                }

                MainWindow.gintTrailerID = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerID;

                TheFindOpenTrailerProblemsByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

                intNumberOfRecords = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strFullName = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].FirstName + " ";
                        strFullName += TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].LastName;

                        OpenTrailerProblemsDataSet.opentrailerproblemsRow NewProblemRow = TheOpenTrailerProblemsDataSet.opentrailerproblems.NewopentrailerproblemsRow();

                        NewProblemRow.Employee = strFullName;
                        NewProblemRow.ProblemID = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].ProblemID;
                        NewProblemRow.ReportedProblem = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].ProblemReported;
                        NewProblemRow.TransactionDate = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].TransactionDate;

                        TheOpenTrailerProblemsDataSet.opentrailerproblems.Rows.Add(NewProblemRow);
                    }
                }

                dgrProblems.ItemsSource = TheOpenTrailerProblemsDataSet.opentrailerproblems;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Trailer Problems // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
