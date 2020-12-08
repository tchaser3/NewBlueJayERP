/* Title:           Update Tool Problem
 * Date:            11-24-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to update a tool problem*/

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
using NewToolsDLL;
using ToolProblemDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateToolProblem.xaml
    /// </summary>
    public partial class UpdateToolProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolProblemClass TheToolProblemClass = new ToolProblemClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        OpenToolProblemsDataSet TheOpenToolProblemsDataSet = new OpenToolProblemsDataSet();
        FindToolByToolIDDataSet TheFindToolByToolIDDataSet = new FindToolByToolIDDataSet();
        FindToolProblemByToolKeyDataSet TheFindToolProblemByTookKeyDataSet = new FindToolProblemByToolKeyDataSet();

        //setting up global variables
        bool gblnWorkComplete;
        bool gblnRepairable;

        public UpdateToolProblem()
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
            txtDateReported.Text = "";
            txtDescription.Text = "";
            txtEnterToolID.Text = "";
            txtProblem.Text = "";
            txtUpdate.Text = "";

            TheOpenToolProblemsDataSet.opentoolproblems.Rows.Clear();

            dgrpProblems.ItemsSource = TheOpenToolProblemsDataSet.opentoolproblems;

            rdoCompleteNo.IsChecked = true;
            rdoRepariableYes.IsChecked = true;

        }

        private void rdoCompleteYes_Checked(object sender, RoutedEventArgs e)
        {
            gblnWorkComplete = true;
        }

        private void rdoCompleteNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnWorkComplete = false;
        }

        private void rdoRepariableYes_Checked(object sender, RoutedEventArgs e)
        {
            gblnRepairable = true;
        }

        private void rdoRepairableNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnRepairable = false;
        }

        private void btnFindTool_Click(object sender, RoutedEventArgs e)
        {
            string strToolID;
            int intRecordsReturned;
            int intToolKey;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheOpenToolProblemsDataSet.opentoolproblems.Rows.Clear();

                strToolID = txtEnterToolID.Text;

                TheFindToolByToolIDDataSet = TheToolsClass.FindAToolByToolID(strToolID);

                intRecordsReturned = TheFindToolByToolIDDataSet.FindToolByToolID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Tool Was Not Found");
                    return;
                }

                txtDescription.Text = TheFindToolByToolIDDataSet.FindToolByToolID[0].ToolDescription;

                intToolKey = TheFindToolByToolIDDataSet.FindToolByToolID[0].ToolKey;

                TheFindToolProblemByTookKeyDataSet = TheToolProblemClass.FindToolProblemByToolKey(intToolKey);

                intNumberOfRecords = TheFindToolProblemByTookKeyDataSet.FindToolProblemByToolKey.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        OpenToolProblemsDataSet.opentoolproblemsRow NewToolProblem = TheOpenToolProblemsDataSet.opentoolproblems.NewopentoolproblemsRow();

                        NewToolProblem.ProblemID = TheFindToolProblemByTookKeyDataSet.FindToolProblemByToolKey[intCounter].ProblemID;
                        NewToolProblem.ReportedDate = TheFindToolProblemByTookKeyDataSet.FindToolProblemByToolKey[intCounter].TransactionDate;
                        NewToolProblem.ReportedProblem = TheFindToolProblemByTookKeyDataSet.FindToolProblemByToolKey[intCounter].WarehouseStatement;

                        TheOpenToolProblemsDataSet.opentoolproblems.Rows.Add(NewToolProblem);
                    }
                }

                dgrpProblems.ItemsSource = TheOpenToolProblemsDataSet.opentoolproblems;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Tool Problem // Find Tool Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrpProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;
            DataGridCell Problem;
            DataGridCell ReportedDate;
            string strReportedDate;
            DateTime datReportDate;
            string strReportedProblem;

            try
            {
                if (dgrpProblems.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrpProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    ReportedDate = (DataGridCell)dataGrid.Columns[1].GetCellContent(selectedRow).Parent;
                    strReportedDate = ((TextBlock)ReportedDate.Content).Text;
                    Problem = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    strReportedProblem = ((TextBlock)Problem.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);
                    datReportDate = Convert.ToDateTime(strReportedDate);

                    txtDateReported.Text = strReportedDate;
                    txtProblem.Text = strReportedProblem;

                    gblnRepairable = TheFindToolProblemByTookKeyDataSet.FindToolProblemByToolKey[0].IsRepairable;

                    if(gblnRepairable == true)
                    {
                        rdoRepariableYes.IsChecked = true;
                    }
                    else
                    {
                        rdoRepairableNo.IsChecked = true;
                    }

                    gblnWorkComplete = TheFindToolProblemByTookKeyDataSet.FindToolProblemByToolKey[0].IsClosed;

                    if(gblnWorkComplete == true)
                    {
                        rdoCompleteYes.IsChecked = true;
                    }
                    else
                    {
                        rdoCompleteNo.IsChecked = true;
                    }

                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Vehicle Problem // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
