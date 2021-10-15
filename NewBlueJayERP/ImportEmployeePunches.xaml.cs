/* Title:           Import Employee Punches
 * Date:            12-17-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import employee punches*/

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
using EmployeePunchedHoursDLL;
using DataValidationDLL;
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using EmployeeDateEntryDLL;
using EmployeeTimeClockEntriesDLL;


namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportEmployeePunches.xaml
    /// </summary>
    public partial class ImportEmployeePunches : Window
    {
        //setting up the classes
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EmployeeTimeClockEntriesClass TheEmployeeTimeCardEntriesClass = new EmployeeTimeClockEntriesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindEmployeeByPayIDDataSet TheFindEmployeeByPayIDDataSet = new FindEmployeeByPayIDDataSet();
        ImportTimePunchesDataSet TheImportTimePunchesDataSet = new ImportTimePunchesDataSet();
        TimePunchesDataSet TheTimePunchesDataSet = new TimePunchesDataSet();
        FindAholaEmployeePunchForVerificationDataSet TheFindAholoEmployeePunchForVerificationDataSet = new FindAholaEmployeePunchForVerificationDataSet();
        FindAholaEmployeeTotalHoursDataSet TheFindAholaEmployeeTotalHoursDataSet = new FindAholaEmployeeTotalHoursDataSet();
        FindEmployeePunchedHoursForValidationDataSet TheFindEmployeePunchedHoursForValidationDataSet = new FindEmployeePunchedHoursForValidationDataSet();

        int gintCounter;

        public ImportEmployeePunches()
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
            TheImportTimePunchesDataSet.importtimepunches.Rows.Clear();
            TheTimePunchesDataSet.timepunches.Rows.Clear();

            dgrResults.ItemsSource = TheImportTimePunchesDataSet.importtimepunches;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Employee Punches");
        }

        private void expImportPunches_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strPayID;
            int intPayID;
            int intEmployeeID;
            string strFirstName;
            string strLastName;
            string strTransactionDate;
            DateTime datTransactionDate;
            double douTransactionDate;
            string strSource;
            string strType;

            try
            {
                expImportPunches.IsExpanded = false;
                TheImportTimePunchesDataSet.importtimepunches.Rows.Clear();
                TheTimePunchesDataSet.timepunches.Rows.Clear();

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
                    strPayID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    intPayID = Convert.ToInt32(strPayID);

                    TheFindEmployeeByPayIDDataSet = TheEmployeeClass.FindEmployeeByPayID(intPayID);

                    intEmployeeID = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].EmployeeID;
                    strFirstName = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].FirstName;
                    strLastName = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].LastName;

                    strTransactionDate = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();
                    strSource = Convert.ToString((range.Cells[intCounter, 17] as Excel.Range).Value2).ToUpper();
                    strType = Convert.ToString((range.Cells[intCounter, 16] as Excel.Range).Value2).ToUpper();

                    douTransactionDate = Convert.ToDouble(strTransactionDate);

                    datTransactionDate = DateTime.FromOADate(douTransactionDate);



                    if (strType != "AUTO MEAL")
                    {
                        TimePunchesDataSet.timepunchesRow NewPunchRow = TheTimePunchesDataSet.timepunches.NewtimepunchesRow();

                        NewPunchRow.EmployeeID = intEmployeeID;
                        NewPunchRow.FirstName = strFirstName;
                        NewPunchRow.LastName = strLastName;
                        NewPunchRow.PayID = intPayID;
                        NewPunchRow.TransactionDate = datTransactionDate;
                        NewPunchRow.Source = strSource;

                        TheTimePunchesDataSet.timepunches.Rows.Add(NewPunchRow);
                    }

                }

                dgrResults.ItemsSource = TheTimePunchesDataSet.timepunches;
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessReport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intPayID;
            int intEmployeeID;
            string strFirstName;
            string strLastName;
            DateTime datStartTime;
            DateTime datEndTime;
            decimal decTotalHours = 0;
            int intSecondCounter;
            TimeSpan tspTotalHours;

            try
            {
                intNumberOfRecords = TheTimePunchesDataSet.timepunches.Rows.Count;
                gintCounter = 0;
                TheImportTimePunchesDataSet.importtimepunches.Rows.Clear();

                if (intNumberOfRecords < 1)
                {
                    TheMessagesClass.ErrorMessage("There Are No Records To Process");
                    return;
                }

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intPayID = TheTimePunchesDataSet.timepunches[intCounter].PayID;
                    intEmployeeID = TheTimePunchesDataSet.timepunches[intCounter].EmployeeID;
                    strFirstName = TheTimePunchesDataSet.timepunches[intCounter].FirstName;
                    strLastName = TheTimePunchesDataSet.timepunches[intCounter].LastName;
                    datStartTime = TheTimePunchesDataSet.timepunches[intCounter].TransactionDate;
                    datEndTime = TheTimePunchesDataSet.timepunches[intCounter].TransactionDate;
                    decTotalHours = 0;

                    intCounter++;

                    if (intEmployeeID == TheTimePunchesDataSet.timepunches[intCounter].EmployeeID)
                    {
                        datEndTime = TheTimePunchesDataSet.timepunches[intCounter].TransactionDate;

                        if (datStartTime.Day == datEndTime.Day)
                        {
                            tspTotalHours = datEndTime - datStartTime;

                            decTotalHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                            decTotalHours = Math.Round(decTotalHours, 3);
                        }
                        else if (datStartTime.Day < datEndTime.Day)
                        {
                            intCounter--;

                            datEndTime = TheTimePunchesDataSet.timepunches[intCounter].TransactionDate;

                            datStartTime = TheTimePunchesDataSet.timepunches[intCounter - 1].TransactionDate;

                            tspTotalHours = datEndTime - datStartTime;

                            decTotalHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                            decTotalHours = Math.Round(decTotalHours, 3);
                        }
                    }
                    else
                    {
                        intCounter--;

                        datEndTime = TheTimePunchesDataSet.timepunches[intCounter].TransactionDate;

                        datStartTime = TheTimePunchesDataSet.timepunches[intCounter - 1].TransactionDate;

                        tspTotalHours = datEndTime - datStartTime;

                        decTotalHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                        decTotalHours = Math.Round(decTotalHours, 3);
                    }


                    for (intSecondCounter = 0; intSecondCounter < gintCounter; intSecondCounter++)
                    {
                        if (TheImportTimePunchesDataSet.importtimepunches[intSecondCounter].EmployeeID == intEmployeeID)
                        {


                        }
                    }

                    ImportTimePunchesDataSet.importtimepunchesRow NewPunchRow = TheImportTimePunchesDataSet.importtimepunches.NewimporttimepunchesRow();

                    NewPunchRow.EmployeeID = intEmployeeID;
                    NewPunchRow.EndTime = datEndTime;
                    NewPunchRow.FirstName = strFirstName;
                    NewPunchRow.LastName = strLastName;
                    NewPunchRow.PayID = intPayID;
                    NewPunchRow.StartTime = datStartTime;
                    NewPunchRow.TotalHours = decTotalHours;

                    TheImportTimePunchesDataSet.importtimepunches.Rows.Add(NewPunchRow);
                    gintCounter++;


                    dgrResults.ItemsSource = TheImportTimePunchesDataSet.importtimepunches;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Process Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expUploadData_Expanded(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueForValidation;
            bool blnFatalError = false;
            DateTime datPayPeriod;
            int intEmployeeID;
            DateTime datStartDate;
            DateTime datEndDate;
            DateTime datStartWeek;
            DateTime datEndWeek;
            decimal decTotalHours;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeCounter;
            int intRecordCounter;
            int intPayID;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                strValueForValidation = txtPayDate.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is not a Date");
                    return;
                }
                else
                {
                    datPayPeriod = Convert.ToDateTime(strValueForValidation);
                }

                datEndWeek = TheDateSearchClass.AddingDays(datPayPeriod, 1);
                datStartWeek = TheDateSearchClass.SubtractingDays(datPayPeriod, 6);

                intNumberOfRecords = TheImportTimePunchesDataSet.importtimepunches.Rows.Count;

                if (intNumberOfRecords < 1)
                {
                    TheMessagesClass.ErrorMessage("No Records Found");
                    return;
                }

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheImportTimePunchesDataSet.importtimepunches[intCounter].EmployeeID;
                    datStartDate = TheImportTimePunchesDataSet.importtimepunches[intCounter].StartTime;
                    datEndDate = TheImportTimePunchesDataSet.importtimepunches[intCounter].EndTime;
                    decTotalHours = TheImportTimePunchesDataSet.importtimepunches[intCounter].TotalHours;

                    TheFindAholoEmployeePunchForVerificationDataSet = TheEmployeePunchedHoursClass.FindAholaEmployeePunchForVerification(intEmployeeID, datStartDate, datEndDate, decTotalHours);

                    intRecordCounter = TheFindAholoEmployeePunchForVerificationDataSet.FindAholaEmployeePunchForVerification.Rows.Count;

                    if (intRecordCounter < 1)
                    {
                        blnFatalError = TheEmployeePunchedHoursClass.InsertIntoAholaEmployeePunch(intEmployeeID, datStartDate, datEndDate, decTotalHours);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheFindAholaEmployeeTotalHoursDataSet = TheEmployeePunchedHoursClass.FindAholaEmployeeTotalHours(datStartWeek, datEndWeek);

                intNumberOfRecords = TheFindAholaEmployeeTotalHoursDataSet.FindAholaEmployeeTotalHours.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheFindAholaEmployeeTotalHoursDataSet.FindAholaEmployeeTotalHours[intCounter].EmployeeID;
                        intPayID = TheFindAholaEmployeeTotalHoursDataSet.FindAholaEmployeeTotalHours[intCounter].PayID;
                        decTotalHours = TheFindAholaEmployeeTotalHoursDataSet.FindAholaEmployeeTotalHours[intCounter].TotalHours;

                        TheFindEmployeePunchedHoursForValidationDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHoursForValidation(datPayPeriod, intEmployeeID, intPayID);

                        intRecordCounter = TheFindEmployeePunchedHoursForValidationDataSet.FindEmployeePunchedHoursForValidation.Rows.Count;

                        if (intRecordCounter < 1)
                        {

                            blnFatalError = TheEmployeePunchedHoursClass.InsertEmployeePunchedHours(datPayPeriod, intEmployeeID, intPayID, decTotalHours);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                PleaseWait.Close();

                TheMessagesClass.InformationMessage("The Information has been Loaded");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Upload Date Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
