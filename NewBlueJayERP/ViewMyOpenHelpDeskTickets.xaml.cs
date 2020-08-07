/* Title:           View My Open Help Desk Tickets
 * Date:            8-6-20
 * Author:          Terry Holmes
 * 
 * Description:     this is used to view my Open Tickets */

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
using HelpDeskDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewMyOpenHelpDeskTickets.xaml
    /// </summary>
    public partial class ViewMyOpenHelpDeskTickets : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindOpenHelpDeskTicketsForEmployeeDataSet TheFindOpenHelpDeskTicketsForEmployeeDataSet = new FindOpenHelpDeskTicketsForEmployeeDataSet();

        public ViewMyOpenHelpDeskTickets()
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
            int intEmployeeID;

            intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

            TheFindOpenHelpDeskTicketsForEmployeeDataSet = TheHelpDeskClass.FindOpenHelpDeskTicketsForEmployee(intEmployeeID);

            dgrOpenTickets.ItemsSource = TheFindOpenHelpDeskTicketsForEmployeeDataSet.FindOpenHelpDeskTicketsForEmployee;
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

                worksheet.Name = "OpenTickets";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheFindOpenHelpDeskTicketsForEmployeeDataSet.FindOpenHelpDeskTicketsForEmployee.Rows.Count;
                intColumnNumberOfRecords = TheFindOpenHelpDeskTicketsForEmployeeDataSet.FindOpenHelpDeskTicketsForEmployee.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindOpenHelpDeskTicketsForEmployeeDataSet.FindOpenHelpDeskTicketsForEmployee.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindOpenHelpDeskTicketsForEmployeeDataSet.FindOpenHelpDeskTicketsForEmployee.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View My Open Help Desk Tickets // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void dgrOpenTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TicketID;
            string strTicketID;

            try
            {
                if (dgrOpenTickets.SelectedIndex > -1)
                {

                    //setting local variable
                    dataGrid = dgrOpenTickets;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TicketID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTicketID = ((TextBlock)TicketID.Content).Text;

                    //find the record
                    MainWindow.gintTicketID = Convert.ToInt32(strTicketID);

                    ViewMyTicketInfo ViewMyTicketInfo = new ViewMyTicketInfo();
                    ViewMyTicketInfo.ShowDialog();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Vehicle Dashboard // Vehicle In Shop // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
