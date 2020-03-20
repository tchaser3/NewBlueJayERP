/* Title:           Vehicle Roster
 * Date:            3-4-20
 * Author:          Terry Holmes
 * 
 * Description:     This is the vehicle roster */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using VehicleMainDLL;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleRoster.xaml
    /// </summary>
    public partial class VehicleRoster : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehouseDataSet = new FindWarehousesDataSet();
        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        FindActiveVehicleMainByLocationDataSet TheFindActiveVehicleMainByLocationDataSet = new FindActiveVehicleMainByLocationDataSet();
        ActiveVehicleDataSet TheActiveVehicleDataSet = new ActiveVehicleDataSet();

        public VehicleRoster()
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");
                cboSelectLocation.Items.Add("All Locations");

                TheFindWarehouseDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehouseDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectLocation.Items.Add(TheFindWarehouseDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectLocation.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Vehicle Roster // Reset Controls " + Ex.Message);

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
                intRowNumberOfRecords = TheActiveVehicleDataSet.activevehicles.Rows.Count;
                intColumnNumberOfRecords = TheActiveVehicleDataSet.activevehicles.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheActiveVehicleDataSet.activevehicles.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheActiveVehicleDataSet.activevehicles.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Vehicle Roster // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void cboSelectLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strAssignedOffice;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectLocation.SelectedIndex - 2;
                TheActiveVehicleDataSet.activevehicles.Rows.Clear();

                if(intSelectedIndex == -1)
                {
                    TheFindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();

                    intNumberOfRecords = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {

                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            ActiveVehicleDataSet.activevehiclesRow NewVehicleRow = TheActiveVehicleDataSet.activevehicles.NewactivevehiclesRow();

                            NewVehicleRow.AssignedOffice = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].AssignedOffice;
                            NewVehicleRow.FirstName = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].FirstName;
                            NewVehicleRow.LastName = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].LastName;
                            NewVehicleRow.LicensePlate = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].LicensePlate;
                            NewVehicleRow.VehicleID = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleID;
                            NewVehicleRow.VehicleMake = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleMake;
                            NewVehicleRow.VehicleModel = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleModel;
                            NewVehicleRow.VehicleNumber = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleNumber;
                            NewVehicleRow.VehicleYear = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleYear;
                            NewVehicleRow.VINNumber = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VINNumber;

                            TheActiveVehicleDataSet.activevehicles.Rows.Add(NewVehicleRow);
                        }
                        
                    }                    
                }
                else if(intSelectedIndex > -1)
                {
                    strAssignedOffice = TheFindWarehouseDataSet.FindWarehouses[intSelectedIndex].FirstName;

                    TheFindActiveVehicleMainByLocationDataSet = TheVehicleMainClass.FindActiveVehicleMainByLocation(strAssignedOffice);

                    intNumberOfRecords = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {

                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            ActiveVehicleDataSet.activevehiclesRow NewVehicleRow = TheActiveVehicleDataSet.activevehicles.NewactivevehiclesRow();

                            NewVehicleRow.AssignedOffice = strAssignedOffice;
                            NewVehicleRow.FirstName = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].FirstName;
                            NewVehicleRow.LastName = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].LastName;
                            NewVehicleRow.LicensePlate = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].LicensePlate;
                            NewVehicleRow.VehicleID = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].VehicleID;
                            NewVehicleRow.VehicleMake = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].VehicleMake;
                            NewVehicleRow.VehicleModel = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].VehicleModel;
                            NewVehicleRow.VehicleNumber = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].VehicleNumber;
                            NewVehicleRow.VehicleYear = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].VehicleYear;
                            NewVehicleRow.VINNumber = TheFindActiveVehicleMainByLocationDataSet.FindActiveVehicleMainByLocation[intCounter].VINNumber;

                            TheActiveVehicleDataSet.activevehicles.Rows.Add(NewVehicleRow);
                        }

                    }
                }

                dgrVehicleRoster.ItemsSource = TheActiveVehicleDataSet.activevehicles;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Vehicle Roster // Combo Box Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
