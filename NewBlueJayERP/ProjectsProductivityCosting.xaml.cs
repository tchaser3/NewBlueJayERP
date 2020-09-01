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

        //setting up the data
        FindAllProjectProductivityCostsOverDateRangeDataSet TheFindAllProjectProductivityCostsOverDateRangeDataSet = new FindAllProjectProductivityCostsOverDateRangeDataSet();

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
            txtEndDate.Text = "";
            txtStartDate.Text = "";

            TheFindAllProjectProductivityCostsOverDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindAllProjectProductivityCostsOverDateRange(DateTime.Now, DateTime.Now);

            dgrResults.ItemsSource = TheFindAllProjectProductivityCostsOverDateRangeDataSet.FindAllProjectProductivityCostsOverDateRange;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the data
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;

            try
            {
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatStartDate = Convert.ToDateTime(strValueForValidation);
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
                    MainWindow.gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessageClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    if(blnFatalError == true)
                    {
                        TheMessageClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheFindAllProjectProductivityCostsOverDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindAllProjectProductivityCostsOverDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                dgrResults.ItemsSource = TheFindAllProjectProductivityCostsOverDateRangeDataSet.FindAllProjectProductivityCostsOverDateRange;

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
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheFindAllProjectProductivityCostsOverDateRangeDataSet.FindAllProjectProductivityCostsOverDateRange.Rows.Count;
                intColumnNumberOfRecords = TheFindAllProjectProductivityCostsOverDateRangeDataSet.FindAllProjectProductivityCostsOverDateRange.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindAllProjectProductivityCostsOverDateRangeDataSet.FindAllProjectProductivityCostsOverDateRange.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindAllProjectProductivityCostsOverDateRangeDataSet.FindAllProjectProductivityCostsOverDateRange.Rows[intRowCounter][intColumnCounter].ToString();

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
    }
}
