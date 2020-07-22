/* Title:           Expiring Rentals
 * Date:            6-5-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for expiring rentals */

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
using RentalTrackingDLL;
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.Office.Core;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ExpiringRentals.xaml
    /// </summary>
    public partial class ExpiringRentals : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        RentalTrackingClass TheRentalTrackingClass = new RentalTrackingClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data
        FindExpiringRentalTrackingDataSet TheFindExpiringRentalTrackingDataSet = new FindExpiringRentalTrackingDataSet();

        public ExpiringRentals()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            TheMessagesClass.LaunchHelpSite();
            expHelp.IsExpanded = false;
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
            DateTime datTransactionDate = DateTime.Now;

            datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 3);

            TheFindExpiringRentalTrackingDataSet = TheRentalTrackingClass.FindExpiringRentalTracking(datTransactionDate);

            dgrExpiringRentals.ItemsSource = TheFindExpiringRentalTrackingDataSet.FindExpiringRentalTracking;
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
                intRowNumberOfRecords = TheFindExpiringRentalTrackingDataSet.FindExpiringRentalTracking.Rows.Count;
                intColumnNumberOfRecords = TheFindExpiringRentalTrackingDataSet.FindExpiringRentalTracking.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindExpiringRentalTrackingDataSet.FindExpiringRentalTracking.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindExpiringRentalTrackingDataSet.FindExpiringRentalTracking.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Expiring Rentals Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
