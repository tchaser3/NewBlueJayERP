/* Title:           Import Tow Motors
 * Date:            6-22-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import the tow motors */

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
using Excel = Microsoft.Office.Interop.Excel;
using NewEventLogDLL;
using NewEmployeeDLL;
using TowMotorDLL;
using DataValidationDLL;
using System.Data.SqlTypes;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportTowMotors.xaml
    /// </summary>
    public partial class ImportTowMotors : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        TowMotorClass TheTowMotorClass = new TowMotorClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindTowMotorMainByTowMotorNumberDataSet TheFindTowMotorMainByTowMotorNumberDataSet = new FindTowMotorMainByTowMotorNumberDataSet();
        ImportTowMotorDataSet TheImportTowMotorDataSet = new ImportTowMotorDataSet();

        public ImportTowMotors()
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
            TheImportTowMotorDataSet.importtowmotors.Rows.Clear();
            dgrTowMotors.ItemsSource = TheImportTowMotorDataSet.importtowmotors;
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strTowMotorNumber;
            int intTowMotorYear;
            string strTowMotorMake;
            string strTowMotorModel;
            decimal decTowMotorHours;
            int intWarehouseID;
            DateTime datOilChangeDate;
            bool blnFatalError = false;
            int intTowMotorWeight;
            int intTowMotorCapacity;
            int intRecordsReturned;
            string strTowMotorSerialNumber;

            try
            {
                intNumberOfRecords = TheImportTowMotorDataSet.importtowmotors.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strTowMotorNumber = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorNumber;
                    intTowMotorYear = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorYear;
                    strTowMotorMake = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorMake;
                    strTowMotorModel = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorModel;
                    decTowMotorHours = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorHours;
                    intWarehouseID = TheImportTowMotorDataSet.importtowmotors[intCounter].WarehouseID;
                    datOilChangeDate = TheImportTowMotorDataSet.importtowmotors[intCounter].OilChangeDate;
                    intTowMotorWeight = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorWeight;
                    intTowMotorCapacity = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorCapacity;
                    strTowMotorSerialNumber = TheImportTowMotorDataSet.importtowmotors[intCounter].TowMotorSerialNo;

                    TheFindTowMotorMainByTowMotorNumberDataSet = TheTowMotorClass.FindTowMotorMainByTowMotorNumber(strTowMotorNumber);

                    intRecordsReturned = TheFindTowMotorMainByTowMotorNumberDataSet.FindTowMotorMainByTowMotorNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = TheTowMotorClass.InsertTowMotorMain(strTowMotorNumber, intTowMotorYear, strTowMotorMake, strTowMotorModel, strTowMotorSerialNumber, decTowMotorHours, intWarehouseID, datOilChangeDate, true, intTowMotorWeight, intTowMotorCapacity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Tow Motors Have Been Added");

                ResetControls();
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Tow Motors // Process Import " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void expImportExcel_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strTowMotorNumber;
            string strTowMotorYear;
            int intTowMotorYear;
            string strTowMotorMake;
            string strTowMotorModel;
            string strTowMotorHours;
            decimal decTowMotorHours;
            string strWarehouse;
            int intWarehouseID = 0;
            string strOilChangeDate;
            DateTime datOilChangeDate;
            bool blnActive = true;
            int intWarehouseCounter;
            int intWarehouseNumberOfRecords;
            string strSerialNumber;
            string strWeight;
            int intWeight;
            string strCapacity;
            int intCapacity;

            try
            {
                TheImportTowMotorDataSet.importtowmotors.Rows.Clear();
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
                intWarehouseNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strTowMotorNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strTowMotorYear = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    intTowMotorYear = Convert.ToInt32(strTowMotorYear);
                    strTowMotorMake = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strTowMotorModel = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                    strSerialNumber = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                    strTowMotorHours = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                    decTowMotorHours = Convert.ToDecimal(strTowMotorHours);
                    strWarehouse = Convert.ToString((range.Cells[intCounter, 7] as Excel.Range).Value2).ToUpper();
                    strWeight = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();
                    intWeight = Convert.ToInt32(strWeight);
                    strCapacity = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();
                    intCapacity = Convert.ToInt32(strCapacity);

                    for (intWarehouseCounter = 0; intWarehouseCounter <= intWarehouseNumberOfRecords; intWarehouseCounter++)
                    {
                        if(strWarehouse == TheFindWarehousesDataSet.FindWarehouses[intWarehouseCounter].FirstName)
                        {
                            intWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intWarehouseCounter].EmployeeID;
                        }
                    }

                    strOilChangeDate = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();
                    DateTime.TryParse(strOilChangeDate, out datOilChangeDate);

                    ImportTowMotorDataSet.importtowmotorsRow NewTowMotorRow = TheImportTowMotorDataSet.importtowmotors.NewimporttowmotorsRow();

                    NewTowMotorRow.TowMotorNumber = strTowMotorNumber;
                    NewTowMotorRow.TowMotorYear = intTowMotorYear;
                    NewTowMotorRow.TowMotorMake = strTowMotorMake;
                    NewTowMotorRow.TowMotorModel = strTowMotorModel;
                    NewTowMotorRow.TowMotorHours = decTowMotorHours;
                    NewTowMotorRow.WarehouseID = intWarehouseID;
                    NewTowMotorRow.OilChangeDate = DateTime.Now;
                    NewTowMotorRow.TowMotorActive = blnActive;
                    NewTowMotorRow.TowMotorSerialNo = strSerialNumber;
                    NewTowMotorRow.TowMotorWeight = intWeight;
                    NewTowMotorRow.TowMotorCapacity = intCapacity;

                    TheImportTowMotorDataSet.importtowmotors.Rows.Add(NewTowMotorRow);
                }

                dgrTowMotors.ItemsSource = TheImportTowMotorDataSet.importtowmotors;

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Tow Motors // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
