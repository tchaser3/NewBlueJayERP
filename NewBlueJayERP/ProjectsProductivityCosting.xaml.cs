/* Title:           Projects Productivity Costing
 * Date:            8-25-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for getting all project costing over a date range */
    
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
using EmployeeProjectAssignmentDLL;
using DataValidationDLL;
using DateSearchDLL;
using Microsoft.Win32;
using DesignProductivityDLL;
using AssignedTasksDLL;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ToolProblemDLL;
using System.CodeDom;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ProjectsProductivityCosting.xaml
    /// </summary>
    public partial class ProjectsProductivityCosting : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();
        DesignProductivityClass TheDesignProductivityClass = new DesignProductivityClass();

        //setting up the data
        FindAllEmployeeProductionOverAWeekDataSet TheFindAllEmployeesProductionOverAWeekDataSet = new FindAllEmployeeProductionOverAWeekDataSet();
        FindAllDesignEmployeeProductivityOverAWeekDataSet TheFindAllDesignEmployeeProductivityOverAWeekDataSet = new FindAllDesignEmployeeProductivityOverAWeekDataSet();
        EmployeeProductivityDataSet TheEmployeeProductivityDataSet = new EmployeeProductivityDataSet();
        CompleteProjectProductivityDataSet TheCompleteProjectProductivityDataSet = new CompleteProjectProductivityDataSet();

        bool gblnStartDateSelected;
        bool gblnEndDateSelected;
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        int gintProjectCounter;
        int gintProjectNumberOfRecords;

        public ProjectsProductivityCosting()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessageClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessageClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessageClass.LaunchHelpSite();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessageClass.LaunchHelpSite();
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
            //calStartDate.SelectedDate = DateTime.Now;
            //calEndDate.SelectedDate = DateTime.Now;
            gblnStartDateSelected = false;
            gblnEndDateSelected = false;
            TheEmployeeProductivityDataSet.employeeproductivity.Rows.Clear();

            dgrResults.ItemsSource = TheEmployeeProductivityDataSet.employeeproductivity;
        }

        private void GenerateReport()
        {
            //setting up the data
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID = -1;
            decimal decTotalHours = 0;
            decimal decMultiplier = 1;
            decimal decTotalCost = 0;
            decimal decReportedHours;
            decimal decPayRate;
            decimal decDifference;
            int intProjectCounter;
            bool blnItemFound;
            int intProjectID;
            bool blnMonday;
            DateTime datTransactionDate;

            try
            {
                TheCompleteProjectProductivityDataSet.completeprojectproductivity.Rows.Clear();
                TheEmployeeProductivityDataSet.employeeproductivity.Rows.Clear();

                if (gblnStartDateSelected == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date was not Selected\n";
                }
                else
                {
                    if (gdatStartDate.DayOfWeek != DayOfWeek.Monday)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Start Date is not a Monday\n";
                    }
                }
                if (gblnEndDateSelected == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date Was Not Selected\n";
                }
                else
                {
                    if (gdatEndDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The End Date is not a Sunday\n";
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessageClass.ErrorMessage(strErrorMessage);
                    return;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheFindAllEmployeesProductionOverAWeekDataSet = TheEmployeeProjectAssignmentClass.FindAllEmployeeProductionOverAWeek(gdatStartDate, gdatEndDate);

                intNumberOfRecords = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek.Rows.Count - 1;
                blnMonday = false;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    decPayRate = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].PayRate;
                    decReportedHours = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].TotalHours;
                    datTransactionDate = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].TransactionDate;

                    if ((datTransactionDate.DayOfWeek == DayOfWeek.Monday) && (blnMonday == false))
                    {
                        decTotalHours = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].TotalHours;
                        intEmployeeID = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].EmployeeID;
                        blnMonday = true;
                        decMultiplier = 1;
                    }
                    else if(intEmployeeID != TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].EmployeeID)
                    {
                        decTotalHours = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].TotalHours;
                        intEmployeeID = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].EmployeeID;
                        decMultiplier = 1;
                    }
                    else if(intEmployeeID == TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].EmployeeID)
                    {
                        decTotalHours += TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].TotalHours;

                        if(datTransactionDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            blnMonday = false;
                        }
                        
                    }

                    if(decMultiplier == 1)
                    {
                        if(decTotalHours <= 40)
                        {
                            decTotalCost = decPayRate * decReportedHours;
                        }
                        if(decTotalHours > 40)
                        {
                            decDifference = decTotalHours - 40;
                            decMultiplier = Convert.ToDecimal(1.5);
                            decTotalCost = ((decReportedHours - decDifference) * decPayRate) + (decDifference * decPayRate * decMultiplier);
                        }
                    }
                    if(decMultiplier == Convert.ToDecimal(1.5))
                    {
                        decTotalCost = decReportedHours * decPayRate * decMultiplier;
                    }

                    EmployeeProductivityDataSet.employeeproductivityRow NewProductivityRow = TheEmployeeProductivityDataSet.employeeproductivity.NewemployeeproductivityRow();

                    NewProductivityRow.AssignedProjectID = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].AssignedProjectID;
                    NewProductivityRow.CalculatedHours = decTotalHours;
                    NewProductivityRow.EmployeeID = intEmployeeID;
                    NewProductivityRow.FirstName = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].FirstName;
                    NewProductivityRow.LastName = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].LastName;
                    NewProductivityRow.Multiplier = decMultiplier;
                    NewProductivityRow.PayRate = decPayRate;
                    NewProductivityRow.ProjectID = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].ProjectID;
                    NewProductivityRow.ProjectName = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].ProjectName;
                    NewProductivityRow.TotalCost = decTotalCost;
                    NewProductivityRow.TotalHours = decReportedHours;
                    NewProductivityRow.TransactionDate = TheFindAllEmployeesProductionOverAWeekDataSet.FindAllEmployeeProductionOverAWeek[intCounter].TransactionDate;

                    TheEmployeeProductivityDataSet.employeeproductivity.Rows.Add(NewProductivityRow);
                }

                TheFindAllDesignEmployeeProductivityOverAWeekDataSet = TheDesignProductivityClass.FindAllDesignEmployeeProductivityOverAWeek(gdatStartDate, gdatEndDate);

                intNumberOfRecords = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek.Rows.Count - 1;
                blnMonday = false;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    decPayRate = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].PayRate;
                    decReportedHours = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].TotalHours;
                    datTransactionDate = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].TransactionDate;

                    if ((datTransactionDate.DayOfWeek == DayOfWeek.Monday) && (blnMonday == false))
                    {
                        decTotalHours = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].TotalHours;
                        intEmployeeID = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].EmployeeID;
                        blnMonday = true;
                        decMultiplier = 1;
                    }
                    else if (intEmployeeID != TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].EmployeeID)
                    {
                        decTotalHours = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].TotalHours;
                        intEmployeeID = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].EmployeeID;
                        decMultiplier = 1;
                    }
                    else if (intEmployeeID == TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].EmployeeID)
                    {
                        decTotalHours += TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].TotalHours;

                        if (datTransactionDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            blnMonday = false;
                        }
                    }

                    if (decMultiplier == 1)
                    {
                        if (decTotalHours <= 40)
                        {
                            decTotalCost = decPayRate * decReportedHours;
                        }
                        if (decTotalHours > 40)
                        {
                            decDifference = decTotalHours - 40;
                            decMultiplier = Convert.ToDecimal(1.5);
                            decTotalCost = ((decReportedHours - decDifference) * decPayRate) + (decDifference * decPayRate * decMultiplier);
                        }
                    }
                    if (decMultiplier == Convert.ToDecimal(1.5))
                    {
                        decTotalCost = decReportedHours * decPayRate * decMultiplier;
                    }

                    EmployeeProductivityDataSet.employeeproductivityRow NewProductivityRow = TheEmployeeProductivityDataSet.employeeproductivity.NewemployeeproductivityRow();

                    NewProductivityRow.AssignedProjectID = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].AssignedProjectID;
                    NewProductivityRow.CalculatedHours = decTotalHours;
                    NewProductivityRow.EmployeeID = intEmployeeID;
                    NewProductivityRow.FirstName = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].FirstName;
                    NewProductivityRow.LastName = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].LastName;
                    NewProductivityRow.Multiplier = decMultiplier;
                    NewProductivityRow.PayRate = decPayRate;
                    NewProductivityRow.ProjectID = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].ProjectID;
                    NewProductivityRow.ProjectName = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].ProjectName;
                    NewProductivityRow.TotalCost = decTotalCost;
                    NewProductivityRow.TotalHours = decReportedHours;
                    NewProductivityRow.TransactionDate = TheFindAllDesignEmployeeProductivityOverAWeekDataSet.FindAllDesignEmployeeProductivityOverAWeek[intCounter].TransactionDate;

                    TheEmployeeProductivityDataSet.employeeproductivity.Rows.Add(NewProductivityRow);
                }

                intNumberOfRecords = TheEmployeeProductivityDataSet.employeeproductivity.Rows.Count - 1;
                gintProjectCounter = 0;
                gintProjectNumberOfRecords = 0;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter ++)
                {
                    blnItemFound = false;
                    intProjectID = TheEmployeeProductivityDataSet.employeeproductivity[intCounter].ProjectID;
                    decTotalHours = TheEmployeeProductivityDataSet.employeeproductivity[intCounter].TotalHours;
                    decTotalCost = TheEmployeeProductivityDataSet.employeeproductivity[intCounter].TotalCost;

                    if(gintProjectCounter > 0)
                    {
                        for(intProjectCounter = 0; intProjectCounter <= gintProjectNumberOfRecords; intProjectCounter++)
                        {
                            if(intProjectID == TheCompleteProjectProductivityDataSet.completeprojectproductivity[intProjectCounter].ProjectID)
                            {
                                TheCompleteProjectProductivityDataSet.completeprojectproductivity[intProjectCounter].TotalHours += decTotalHours;
                                TheCompleteProjectProductivityDataSet.completeprojectproductivity[intProjectCounter].TotalCosts += decTotalCost;
                                blnItemFound = true;
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        CompleteProjectProductivityDataSet.completeprojectproductivityRow NewProjectRow = TheCompleteProjectProductivityDataSet.completeprojectproductivity.NewcompleteprojectproductivityRow();

                        decTotalCost = Math.Round(decTotalCost, 2);

                        NewProjectRow.AssignedProjectID = TheEmployeeProductivityDataSet.employeeproductivity[intCounter].AssignedProjectID;
                        NewProjectRow.ProjectID = intProjectID;
                        NewProjectRow.ProjectName = TheEmployeeProductivityDataSet.employeeproductivity[intCounter].ProjectName;
                        NewProjectRow.TotalCosts = decTotalCost;
                        NewProjectRow.TotalHours = decTotalHours;

                        TheCompleteProjectProductivityDataSet.completeprojectproductivity.Rows.Add(NewProjectRow);
                        gintProjectNumberOfRecords = gintProjectCounter;
                        gintProjectCounter++;
                    }
                }

                dgrResults.ItemsSource = TheCompleteProjectProductivityDataSet.completeprojectproductivity;

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Projects Productivity Costing // Process Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
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
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheCompleteProjectProductivityDataSet.completeprojectproductivity.Rows.Count;
                intColumnNumberOfRecords = TheCompleteProjectProductivityDataSet.completeprojectproductivity.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompleteProjectProductivityDataSet.completeprojectproductivity.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompleteProjectProductivityDataSet.completeprojectproductivity.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                PleaseWait.Close();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP //Projects Productivity Costing // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void calStartDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            gdatStartDate = Convert.ToDateTime(calStartDate.SelectedDate);

            gblnStartDateSelected = true;

            calEndDate.Focus();
        }

        private void calEndDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            gdatEndDate = Convert.ToDateTime(calEndDate.SelectedDate);

            gblnEndDateSelected = true;

            GenerateReport();
        }
    }
}
