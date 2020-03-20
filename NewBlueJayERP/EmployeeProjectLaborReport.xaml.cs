/* Name:            Employee Project Labor Report
 * Date:            2-20-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to run the project labor report */

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
using ProjectCostingDLL;
using ProjectsDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeProjectLaborReport.xaml
    /// </summary>
    public partial class EmployeeProjectLaborReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectCostingClass TheProjectCostingClass = new ProjectCostingClass();
        ProjectClass TheProjectClass = new ProjectClass();

        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectTaskCostsDataSet TheFindProjectTaskCostsDataSet = new FindProjectTaskCostsDataSet();

        public EmployeeProjectLaborReport()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strAssignedProjectID;
            int intRecordsReturned;
            decimal decTotalCost = 0;
            decimal decTotalHours = 0;
            int intCounter;
            int intNumberOfRecords;
            double douTotalCost;

            try
            {
                strAssignedProjectID = txtAssignedProjectID.Text;
                if(strAssignedProjectID == "")
                {
                    TheMessagesClass.ErrorMessage("The Assigned Project ID Was Not Entered");
                    return;
                }

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strAssignedProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Project Was Not Found");
                    return;
                }

                TheFindProjectTaskCostsDataSet = TheProjectCostingClass.FindProjectTasksCosts(strAssignedProjectID);

                intNumberOfRecords = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        decTotalCost += TheFindProjectTaskCostsDataSet.FindProjectTaskCosts[intCounter].LaborCost;
                        decTotalHours += TheFindProjectTaskCostsDataSet.FindProjectTaskCosts[intCounter].TotalHours;
                    }
                }

                dgrResults.ItemsSource = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts;

                douTotalCost = Convert.ToDouble(decTotalCost);

                decTotalCost = Convert.ToDecimal(Math.Round(douTotalCost, 2));

                txtTotalCost.Text = Convert.ToString(decTotalCost);
                txtTotalHours.Text = Convert.ToString(decTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Project Labor Report // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtAssignedProjectID.Text = "";
            txtTotalCost.Text = "";
            txtTotalHours.Text = "";
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
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
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Rows.Count;
                intColumnNumberOfRecords = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Project Labor Report // Export To Excel " + ex.Message);

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
