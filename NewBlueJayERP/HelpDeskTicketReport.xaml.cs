/* Title:           Help Desk Ticket Reportg
 * Date:            10-5-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for Help desk ticket report */

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
using NewEmployeeDLL;
using HelpDeskDLL;
using DataValidationDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;
using System.Data;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for HelpDeskTicketReport.xaml
    /// </summary>
    public partial class HelpDeskTicketReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindHelpDeskTicketProblemsByDateRangeDataSet TheFindHelpDeskTicketProblemsByDateRangeDataSet = new FindHelpDeskTicketProblemsByDateRangeDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public HelpDeskTicketReport()
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
            txtEndDate.Text = "";
            txtStartDate.Text = "";
            TheFindHelpDeskTicketProblemsByDateRangeDataSet = TheHelpDeskClass.FindHelpDeskTicketProblemsByDateRanage(DateTime.Now, DateTime.Now);

            dgrTickets.ItemsSource = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            try
            {
                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Help Desk Ticket Report // Report Created ");

                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                TheFindHelpDeskTicketProblemsByDateRangeDataSet = TheHelpDeskClass.FindHelpDeskTicketProblemsByDateRanage(gdatStartDate, gdatEndDate);

                dgrTickets.ItemsSource = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Help Desk Ticket Report // Find Button " + Ex.Message);

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
                intRowNumberOfRecords = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange.Rows.Count;
                intColumnNumberOfRecords = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Help Desk Ticket Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void dgrTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TicketID;
            string strTicketID;
            int intEmployeeID;


            try
            {
                if (dgrTickets.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrTickets;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TicketID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTicketID = ((TextBlock)TicketID.Content).Text;
                    MainWindow.gintTicketID = Convert.ToInt32(strTicketID);

                    //find the record
                    UpdateTicketInformation UpdateTicketInformation = new UpdateTicketInformation();
                    UpdateTicketInformation.ShowDialog();

                    intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                    TheFindHelpDeskTicketProblemsByDateRangeDataSet = TheHelpDeskClass.FindHelpDeskTicketProblemsByDateRanage( gdatStartDate, gdatEndDate);

                    dgrTickets.ItemsSource = TheFindHelpDeskTicketProblemsByDateRangeDataSet.FindHelpDeskTicketProblemsByDateRange;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // My Tickets // Tickets Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
