/* Title:           Project Productivity Reports
 * Date:            1-16-20
 * Author:          Terry Holmes
 * 
 * Description:     This will run the productivity reports */

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
using DataValidationDLL;
using DateSearchDLL;
using ProjectProductivityReportsDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ProjectProductivityReport.xaml
    /// </summary>
    public partial class ProjectProductivityReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        ProjectProductivityReportsClass TheProjectProductivityReportsClass = new ProjectProductivityReportsClass();

        //setting up the data
        FindDesignPrivateProjectProductivityDateRangeDataSet TheFindDesignPrivateProjectProductivityDateRangeDataSet = new FindDesignPrivateProjectProductivityDateRangeDataSet();
        FindDesignProjectProductivityByDateRangeDataSet TheFindDesignProjectProductivityByDateRangeDataSet = new FindDesignProjectProductivityByDateRangeDataSet();
        FindPrivateProjectProductivityDateRangeDataSet TheFindPrivateProjectProductivityDateRangeDataSet = new FindPrivateProjectProductivityDateRangeDataSet();
        FindProjectProductivityByDateRangeDataSet TheFindProjectProductivityByDateRangeDataSet = new FindProjectProductivityByDateRangeDataSet();
        ProjectProductivityReportDataSet TheProjectProductivityReportDataSet = new ProjectProductivityReportDataSet();

        bool gblnAllProjects;

        public ProjectProductivityReport()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
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
            txtEndDate.Text = "";
            txtFirstProject.Text = "";
            txtSecondProject.Text = "";
            txtStartDate.Text = "";
            cboSelectReport.Items.Clear();
            cboSelectReport.Items.Add("Select Report");
            cboSelectReport.Items.Add("Select All Projects");
            cboSelectReport.Items.Add("Select Project Range");
            cboSelectReport.SelectedIndex = 0;
            stpProjects.Visibility = Visibility.Hidden;
            gblnAllProjects = true;
        }

        private void cboSelectReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectReport.SelectedIndex;

            if (intSelectedIndex == 1)
            {
                stpProjects.Visibility = Visibility.Hidden;
                gblnAllProjects = true;
            }                
            else if (intSelectedIndex == 2)
            {
                stpProjects.Visibility = Visibility.Visible;
                gblnAllProjects = false;
            }
            else if(intSelectedIndex == 0)
            {
                stpProjects.Visibility = Visibility.Hidden;
                gblnAllProjects = true;
            }
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intSecondCounter;
            int intNumberForRecords;
            int intSecondNumberForRecords;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            string strFirstProject = "";
            string strSecondProject = "";
            string strErrorMessage = "";
            bool blnItemFound;
            string strProjectID;
            string strProjectName;
            decimal decTotalHours;

            try
            {
                //clearing the dataset
                TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Clear();

                //beginning data validation
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                }

                if (gblnAllProjects == false)
                {
                    strFirstProject = txtFirstProject.Text;
                    if(strFirstProject == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The First Project ID is not Entered\n";
                    }
                    strSecondProject = txtSecondProject.Text;
                    if(strSecondProject == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Second Project ID is not Entered\n";
                    }
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                if(gblnAllProjects == true)
                {
                    TheFindProjectProductivityByDateRangeDataSet = TheProjectProductivityReportsClass.FindProjectProductivityByDateRange(datStartDate, datEndDate);

                    intNumberForRecords = TheFindProjectProductivityByDateRangeDataSet.FindProjectProductivityTotalsByDateRange.Rows.Count - 1;

                    if(intNumberForRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberForRecords; intCounter++)
                        {
                            ProjectProductivityReportDataSet.projectproductivityreportRow NewProjectRow = TheProjectProductivityReportDataSet.projectproductivityreport.NewprojectproductivityreportRow();

                            NewProjectRow.AssignedProjectID = TheFindProjectProductivityByDateRangeDataSet.FindProjectProductivityTotalsByDateRange[intCounter].AssignedProjectID;
                            NewProjectRow.ProjectName = TheFindProjectProductivityByDateRangeDataSet.FindProjectProductivityTotalsByDateRange[intCounter].ProjectName;
                            NewProjectRow.TotalHours = TheFindProjectProductivityByDateRangeDataSet.FindProjectProductivityTotalsByDateRange[intCounter].TotalHours;

                            TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Add(NewProjectRow);
                        }
                    }

                    //adding design productivity
                    TheFindDesignProjectProductivityByDateRangeDataSet = TheProjectProductivityReportsClass.FindDesignProjectProductivityByDateRange(datStartDate, datEndDate);

                    intNumberForRecords = TheFindDesignProjectProductivityByDateRangeDataSet.FindDesignProjectProductivityByDateRange.Rows.Count - 1;
                    intSecondNumberForRecords = TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Count - 1;

                    if(intNumberForRecords > - 1)
                    {
                        for(intCounter = 0; intCounter <= intNumberForRecords; intCounter++)
                        {
                            blnItemFound = false;
                            strProjectID = TheFindDesignProjectProductivityByDateRangeDataSet.FindDesignProjectProductivityByDateRange[intCounter].AssignedProjectID;
                            strProjectName = TheFindDesignProjectProductivityByDateRangeDataSet.FindDesignProjectProductivityByDateRange[intCounter].ProjectName;
                            decTotalHours = TheFindDesignProjectProductivityByDateRangeDataSet.FindDesignProjectProductivityByDateRange[intCounter].TotalHours;

                            if(intSecondNumberForRecords > -1)
                            {
                                for(intSecondCounter = 0; intSecondCounter <= intSecondNumberForRecords; intSecondCounter++)
                                {
                                    if(strProjectID == TheProjectProductivityReportDataSet.projectproductivityreport[intSecondCounter].AssignedProjectID)
                                    {
                                        TheProjectProductivityReportDataSet.projectproductivityreport[intSecondCounter].TotalHours += decTotalHours;
                                        blnItemFound = true;
                                    }
                                }
                            }

                            if(blnItemFound == false)
                            {
                                ProjectProductivityReportDataSet.projectproductivityreportRow NewProjectRow = TheProjectProductivityReportDataSet.projectproductivityreport.NewprojectproductivityreportRow();

                                NewProjectRow.AssignedProjectID = strProjectID;
                                NewProjectRow.ProjectName = strProjectName;
                                NewProjectRow.TotalHours = decTotalHours;

                                TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Add(NewProjectRow);
                            }
                        }
                    }
                }
                else if(gblnAllProjects == false)
                {
                    TheFindPrivateProjectProductivityDateRangeDataSet = TheProjectProductivityReportsClass.FindPrivateProjectProductivityDateRange(strFirstProject, strSecondProject, datStartDate, datEndDate);

                    intNumberForRecords = TheFindPrivateProjectProductivityDateRangeDataSet.FindPrivateProjectProductivityDateRange.Rows.Count - 1;

                    if (intNumberForRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberForRecords; intCounter++)
                        {
                            ProjectProductivityReportDataSet.projectproductivityreportRow NewProjectRow = TheProjectProductivityReportDataSet.projectproductivityreport.NewprojectproductivityreportRow();

                            NewProjectRow.AssignedProjectID = TheFindPrivateProjectProductivityDateRangeDataSet.FindPrivateProjectProductivityDateRange[intCounter].AssignedProjectID;
                            NewProjectRow.ProjectName = TheFindPrivateProjectProductivityDateRangeDataSet.FindPrivateProjectProductivityDateRange[intCounter].ProjectName;
                            NewProjectRow.TotalHours = TheFindPrivateProjectProductivityDateRangeDataSet.FindPrivateProjectProductivityDateRange[intCounter].TotalHours;

                            TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Add(NewProjectRow);
                        }
                    }

                    //adding design productivity
                    TheFindDesignPrivateProjectProductivityDateRangeDataSet = TheProjectProductivityReportsClass.FindDesignPrivateProjectProductivityDateRange(strFirstProject, strSecondProject, datStartDate, datEndDate);

                    intNumberForRecords = TheFindDesignPrivateProjectProductivityDateRangeDataSet.FindDesignPrivateProjectProductivityDateRange.Rows.Count - 1;
                    intSecondNumberForRecords = TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Count - 1;

                    if (intNumberForRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberForRecords; intCounter++)
                        {
                            blnItemFound = false;
                            strProjectID = TheFindDesignPrivateProjectProductivityDateRangeDataSet.FindDesignPrivateProjectProductivityDateRange[intCounter].AssignedProjectID;
                            strProjectName = TheFindDesignPrivateProjectProductivityDateRangeDataSet.FindDesignPrivateProjectProductivityDateRange[intCounter].ProjectName;
                            decTotalHours = TheFindDesignPrivateProjectProductivityDateRangeDataSet.FindDesignPrivateProjectProductivityDateRange[intCounter].TotalHours;

                            if (intSecondNumberForRecords > -1)
                            {
                                for (intSecondCounter = 0; intSecondCounter <= intSecondNumberForRecords; intSecondCounter++)
                                {
                                    if (strProjectID == TheProjectProductivityReportDataSet.projectproductivityreport[intSecondCounter].AssignedProjectID)
                                    {
                                        TheProjectProductivityReportDataSet.projectproductivityreport[intSecondCounter].TotalHours += decTotalHours;
                                        blnItemFound = true;
                                    }
                                }
                            }

                            if (blnItemFound == false)
                            {
                                ProjectProductivityReportDataSet.projectproductivityreportRow NewProjectRow = TheProjectProductivityReportDataSet.projectproductivityreport.NewprojectproductivityreportRow();

                                NewProjectRow.AssignedProjectID = strProjectID;
                                NewProjectRow.ProjectName = strProjectName;
                                NewProjectRow.TotalHours = decTotalHours;

                                TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Add(NewProjectRow);
                            }
                        }
                    }
                }
                

                dgrResults.ItemsSource = TheProjectProductivityReportDataSet.projectproductivityreport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Productivity Report // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
                intRowNumberOfRecords = TheProjectProductivityReportDataSet.projectproductivityreport.Rows.Count;
                intColumnNumberOfRecords = TheProjectProductivityReportDataSet.projectproductivityreport.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectProductivityReportDataSet.projectproductivityreport.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectProductivityReportDataSet.projectproductivityreport.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Productivity Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
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
    }
}
