/* Title:           Project Shop Analysis
 * Date:            10-27-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show the project analysis */

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
using Microsoft.Office.Core;
using ProjectMatrixDLL;
using EmployeeProjectAssignmentDLL;
using NewEventLogDLL;
using DateSearchDLL;
using EmployeeProductivityStatsDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;


namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ProjectShopAnalysis.xaml
    /// </summary>
    public partial class ProjectShopAnalysis : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectHoursAboveLimitDataSet TheFindProjectHoursAboveLimitDataSet = new FindProjectHoursAboveLimitDataSet();
        ShopViolatorDataSet TheShopViolatorDataSet = new ShopViolatorDataSet();
        FindProjectStatsDataSet TheFindProjectStatsDataSet = new FindProjectStatsDataSet();
        NormalDistributionDataSet TheNormalDistributionDataSet = new NormalDistributionDataSet();

        //setting variables
        decimal gdecTotalHours;
        decimal gdecMean;
        decimal gdecStandDeviation;
        decimal gdecVariance;
        decimal gdecUpperBound;
        decimal gdecProjectHours;
        decimal gdecProjectCost;

        public ProjectShopAnalysis()
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
            //setting up the local variables
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            DateTime datTransactionDate = DateTime.Now;
            DateTime datStartDate = DateTime.Now;
            decimal decAveragePayRate;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheShopViolatorDataSet.violator.Rows.Clear();

                datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);

                datTransactionDate = TheDateSearchClass.SubtractingDays(datTransactionDate, 31);

                TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID("SHOP");

                intProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;

                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 31);

                TheFindProjectStatsDataSet = TheEmployeeProductivityStatsClass.FindProjectStats(intProjectID);

                gdecMean = TheFindProjectStatsDataSet.FindProjectStats[0].AveHours;
                gdecStandDeviation = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].HoursSTDev);
                gdecVariance = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].HoursVariance);
                gdecTotalHours = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].TotalHours);
                decAveragePayRate = TheFindProjectStatsDataSet.FindProjectStats[0].AveragePayRate;

                gdecMean = Math.Round(gdecMean, 4);

                txtAverageHours.Text = Convert.ToString(gdecMean);

                gdecVariance = Math.Round(gdecVariance, 4);
                gdecStandDeviation = Math.Round(gdecStandDeviation, 4);

                gdecUpperBound = gdecMean + gdecStandDeviation;

                txtUpperBound.Text = Convert.ToString(gdecUpperBound);

                TheFindProjectHoursAboveLimitDataSet = TheEmployeeProjectAssignmentClass.FindProjectHoursAboveLimit(intProjectID, datStartDate, gdecUpperBound);

                intNumberOfRecords = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        ShopViolatorDataSet.violatorRow NewViolatorRow = TheShopViolatorDataSet.violator.NewviolatorRow();

                        NewViolatorRow.FirstName = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].FirstName;
                        NewViolatorRow.HomeOffice = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].HomeOffice;
                        NewViolatorRow.Hours = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].TotalHours;
                        NewViolatorRow.LastName = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].LastName;
                        NewViolatorRow.TransactionDate = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].TransactionDate;

                        TheShopViolatorDataSet.violator.Rows.Add(NewViolatorRow);
                    }
                }

                dgrResults.ItemsSource = TheShopViolatorDataSet.violator;

                gdecProjectHours =TheFindProjectStatsDataSet.FindProjectStats[0].TotalHours;
                txtShopHours.Text = Convert.ToString(gdecProjectHours);
                gdecProjectCost = gdecProjectHours * decAveragePayRate;
                gdecProjectCost = Math.Round(gdecProjectCost, 4);
                txtProjectCost.Text = Convert.ToString(gdecProjectCost);

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Shop Hours Analysis // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expNormalDis_Expanded(object sender, RoutedEventArgs e)
        {
            decimal decNumber = -2;
            double douValue = .1;

            try
            {
                while (Convert.ToInt32(decNumber) < 6)
                {
                    NormalDistributionDataSet.normaldistributionRow NewStatRow = TheNormalDistributionDataSet.normaldistribution.NewnormaldistributionRow();

                    NewStatRow.StdDev = gdecStandDeviation;
                    NewStatRow.Mean = gdecMean;
                    NewStatRow.Hours = decNumber;

                    TheNormalDistributionDataSet.normaldistribution.Rows.Add(NewStatRow);

                    decNumber = decNumber + Convert.ToDecimal(douValue);
                }

                DownloadDistribution();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Shop Analysis // Normal Distribution Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void DownloadDistribution()
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheNormalDistributionDataSet.normaldistribution.Rows.Count;
                intColumnNumberOfRecords = TheNormalDistributionDataSet.normaldistribution.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheNormalDistributionDataSet.normaldistribution.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheNormalDistributionDataSet.normaldistribution[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Shop Analyssi // Normal Distribution Method " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
