/* Title:           Detailed Non-Production Employee Productivity
 * Date:            2-12-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show a detailed account for an employees productivity */

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
using NonProductionProductivityDLL;
using NewEventLogDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for DetailedNonProductionEmployeeProductivity.xaml
    /// </summary>
    public partial class DetailedNonProductionEmployeeProductivity : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        NonProductionProductivityClass TheNonProductionProductivityClass = new NonProductionProductivityClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindTotalNonProductionProductivityOverDateRangeDataSet TheFindTotalNonProductionProductivityOverDateRangeDataSet = new FindTotalNonProductionProductivityOverDateRangeDataSet();
        FindNonProductionProductivityForEmployeeDataSet TheFindNonProductionProductivityForEmployeeDataSet = new FindNonProductionProductivityForEmployeeDataSet();

        public DetailedNonProductionEmployeeProductivity()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            //setting variables
            int intCounter;
            int intNumberOfRecords;
            string strName;

            try
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                TheFindTotalNonProductionProductivityOverDateRangeDataSet = TheNonProductionProductivityClass.FindTotalNonProductionProductivityOverDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindTotalNonProductionProductivityOverDateRangeDataSet.FindTotalNonProducductionProductivityOverDateRange.Rows.Count;

                if(intNumberOfRecords < 1)
                {
                    TheMessagesClass.InformationMessage("There Are No Transactions for this Date Range");
                    this.Close();
                }

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    strName = TheFindTotalNonProductionProductivityOverDateRangeDataSet.FindTotalNonProducductionProductivityOverDateRange[intCounter].FirstName + " ";
                    strName += TheFindTotalNonProductionProductivityOverDateRangeDataSet.FindTotalNonProducductionProductivityOverDateRange[intCounter].LastName;

                    cboSelectEmployee.Items.Add(strName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Detailed Non-Production Employee Productivity " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intEmployeeID;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheFindTotalNonProductionProductivityOverDateRangeDataSet.FindTotalNonProducductionProductivityOverDateRange[intSelectedIndex].EmployeeID;

                    TheFindNonProductionProductivityForEmployeeDataSet = TheNonProductionProductivityClass.FindNonProductionProductivityForEmployee(intEmployeeID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    dgrProductivity.ItemsSource = TheFindNonProductionProductivityForEmployeeDataSet.FindNonProductionProductivityForEmployee;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Detailed Non-Production Productivity // Select Employee Combo Box " + Ex.Message);

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
                intRowNumberOfRecords = TheFindNonProductionProductivityForEmployeeDataSet.FindNonProductionProductivityForEmployee.Rows.Count;
                intColumnNumberOfRecords = TheFindNonProductionProductivityForEmployeeDataSet.FindNonProductionProductivityForEmployee.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindNonProductionProductivityForEmployeeDataSet.FindNonProductionProductivityForEmployee.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindNonProductionProductivityForEmployeeDataSet.FindNonProductionProductivityForEmployee[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Detailed Production Employee Productivity // Export To Excel " + ex.Message);

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
