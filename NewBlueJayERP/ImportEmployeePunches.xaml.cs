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
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        ImportAholaPunchesDataSet TheImportAholaPunchesDataSet = new ImportAholaPunchesDataSet();
        FindEmployeeByPayIDDataSet TheFindEmployeeByPayIDDataSet = new FindEmployeeByPayIDDataSet();
        FindAholaClockPunchesForVerificationDataSet TheFindAholaClockPunchesForVerificationDataSet = new FindAholaClockPunchesForVerificationDataSet();
        CalculatedHoursDataSet TheCalculatedHoursDataSet = new CalculatedHoursDataSet();
        EmployeeHoursDataSet TheEmployeeHoursDataSet = new EmployeeHoursDataSet();
        FindAholaEmployeePunchForVerificationDataSet TheFindAholaEmployeePunchForVerificationDataSet = new FindAholaEmployeePunchForVerificationDataSet();
        FindEmployeePunchedHoursForValidationDataSet TheFindEmployeePunchedHoursForValidationDataSet = new FindEmployeePunchedHoursForValidationDataSet();

        //setting up global variables
        int gintPunchRecords;


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
            TheImportAholaPunchesDataSet.importaholapunches.Rows.Clear();

            dgrResults.ItemsSource = TheImportAholaPunchesDataSet.importaholapunches;

            expCalculateHours.IsEnabled = false;
            expProcessHours.IsEnabled = false;
            expInsertRecords.IsEnabled = false;
            TheCalculatedHoursDataSet.calculatedhours.Rows.Clear();
            TheEmployeeHoursDataSet.employeehours.Rows.Clear();

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Employee Punches");
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
            string strValueForValidation;
            int intEmployeeID = 0;
            int intPayID = 0;
            DateTime datCreatedDateTime = DateTime.Now;
            DateTime datPunchedDateTime = DateTime.Now;
            DateTime datActualDateTime = DateTime.Now;
            string strPayGroup = "";
            string strPunchMode = "";
            string strPunchType = "";
            string strPunchSouce = "";
            string strPunchIPAddress = "";
            DateTime datLastUpdate = DateTime.Now;
            bool blnFailedValidation = false;
            int intRecordsReturned;
            double douProcessDate;
            string strFirstName = "";
            string strLastName = "";
            string strPunchUser;

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportAholaPunchesDataSet.importaholapunches.Rows.Clear();

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

                for (intCounter = 2; intCounter < intNumberOfRecords; intCounter++)
                {
                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();

                    blnFailedValidation = TheDataValidationClass.VerifyIntegerData(strValueForValidation);

                    if (blnFailedValidation == true)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        intPayID = Convert.ToInt32(strValueForValidation);

                        TheFindEmployeeByPayIDDataSet = TheEmployeeClass.FindEmployeeByPayID(intPayID);

                        intRecordsReturned = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            intEmployeeID = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].EmployeeID;
                            strFirstName = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].FirstName;
                            strLastName = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].LastName;
                        }
                    }

                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();

                    blnFailedValidation = TheDataValidationClass.VerifyDoubleData(strValueForValidation);

                    if (blnFailedValidation == true)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        douProcessDate = Convert.ToDouble(strValueForValidation);

                        datActualDateTime = DateTime.FromOADate(douProcessDate);
                    }

                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();

                    blnFailedValidation = TheDataValidationClass.VerifyDoubleData(strValueForValidation);

                    if (blnFailedValidation == true)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        douProcessDate = Convert.ToDouble(strValueForValidation);

                        datPunchedDateTime = DateTime.FromOADate(douProcessDate);
                    }

                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 10] as Excel.Range).Value2).ToUpper();

                    blnFailedValidation = TheDataValidationClass.VerifyDateData(strValueForValidation);

                    if (blnFailedValidation == true)
                    {
                        datCreatedDateTime = DateTime.Now;
                    }
                    else
                    {
                        datCreatedDateTime = Convert.ToDateTime(strValueForValidation);
                    }

                    strPayGroup = Convert.ToString((range.Cells[intCounter, 14] as Excel.Range).Value2).ToUpper();
                    strPunchMode = Convert.ToString((range.Cells[intCounter, 15] as Excel.Range).Value2).ToUpper();
                    strPunchType = Convert.ToString((range.Cells[intCounter, 16] as Excel.Range).Value2).ToUpper();
                    strPunchSouce = Convert.ToString((range.Cells[intCounter, 17] as Excel.Range).Value2).ToUpper();
                    strPunchIPAddress = Convert.ToString((range.Cells[intCounter, 20] as Excel.Range).Value2).ToUpper();
                    strPunchUser = Convert.ToString((range.Cells[intCounter, 27] as Excel.Range).Value2).ToUpper();

                    strPunchUser = "";

                    //strValueForValidation = Convert.ToString((range.Cells[intCounter, 28] as Excel.Range).Value2).ToUpper();

                    strValueForValidation = Convert.ToString(DateTime.Now);

                    blnFailedValidation = TheDataValidationClass.VerifyDateData(strValueForValidation);

                    if (blnFailedValidation == true)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        datLastUpdate = Convert.ToDateTime(strValueForValidation);
                    }


                    ImportAholaPunchesDataSet.importaholapunchesRow NewPunchRow = TheImportAholaPunchesDataSet.importaholapunches.NewimportaholapunchesRow();

                    NewPunchRow.ActualDateTime = datActualDateTime;
                    NewPunchRow.CreatedDateTime = datCreatedDateTime;
                    NewPunchRow.EmployeeID = intEmployeeID;
                    NewPunchRow.FirstName = strFirstName;
                    NewPunchRow.LastName = strLastName;
                    NewPunchRow.LastUpdate = datLastUpdate;
                    NewPunchRow.PayGroup = strPayGroup;
                    NewPunchRow.PayID = intPayID;
                    NewPunchRow.PunchDateTime = datPunchedDateTime;
                    NewPunchRow.PunchIPAddress = strPunchIPAddress;
                    NewPunchRow.PunchMode = strPunchMode;
                    NewPunchRow.PunchSource = strPunchSouce;
                    NewPunchRow.PunchType = strPunchType;
                    NewPunchRow.PunchUser = strPunchUser;

                    TheImportAholaPunchesDataSet.importaholapunches.Rows.Add(NewPunchRow);

                }

                dgrResults.ItemsSource = TheImportAholaPunchesDataSet.importaholapunches;
                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessData_Expanded(object sender, RoutedEventArgs e)
        {
            int intNumberOfRecords;
            int intCounter;
            int intPayID;
            int intEmployeeID;
            DateTime datActualDateTime;
            DateTime datPunchDateTime;
            DateTime datCreatedDateTime;
            string strPayGroup;
            string strPunchMode;
            string strPunchType;
            string strPunchUser;
            string strPunchIPAddress;
            DateTime datLastUpdate;
            bool blnFatalError = false;
            int intRecordsReturned;
            string strPunchSource;

            try
            {
                expProcessData.IsExpanded = false;
                intNumberOfRecords = TheImportAholaPunchesDataSet.importaholapunches.Rows.Count;

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intPayID = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PayID;
                        intEmployeeID = TheImportAholaPunchesDataSet.importaholapunches[intCounter].EmployeeID;
                        datActualDateTime = TheImportAholaPunchesDataSet.importaholapunches[intCounter].ActualDateTime;
                        datPunchDateTime = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchDateTime;
                        datCreatedDateTime = TheImportAholaPunchesDataSet.importaholapunches[intCounter].CreatedDateTime;
                        strPayGroup = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PayGroup;
                        strPunchMode = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchMode;
                        strPunchType = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchType;
                        strPunchSource = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchSource;
                        strPunchUser = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchUser;
                        strPunchIPAddress = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchIPAddress;
                        datLastUpdate = TheImportAholaPunchesDataSet.importaholapunches[intCounter].LastUpdate;

                        TheFindAholaClockPunchesForVerificationDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForVerification(intPayID, datActualDateTime, datPunchDateTime, datCreatedDateTime, strPunchIPAddress);

                        intRecordsReturned = TheFindAholaClockPunchesForVerificationDataSet.FindAholaClockPunchesForVerification.Rows.Count;

                        if (intRecordsReturned < 1)
                        {
                            blnFatalError = TheEmployeePunchedHoursClass.InsertAholaClockPunches(intEmployeeID, intPayID, datActualDateTime, datPunchDateTime, datCreatedDateTime, strPayGroup, strPunchMode, strPunchType, strPunchSource, strPunchUser, strPunchIPAddress, datLastUpdate);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                PleaseWait.Close();
                TheMessagesClass.InformationMessage("All Records Have Been Inserted");

                expCalculateHours.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Process Data Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expCalculateHours_Expanded(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID = 0;
            string strFirstName = "";
            string strLastName = "";
            DateTime datStartDate;
            DateTime datEndDate = DateTime.Now;
            string strPunchType;
            string strPunchSource;
            decimal decDailyHours = 0;
            TimeSpan tspTotalHours;
            int intSecondCounter;
            bool blnRecordFound;
            int intPayID;

            try
            {
                expCalculateHours.IsExpanded = false;
                TheCalculatedHoursDataSet.calculatedhours.Rows.Clear();
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();
                gintPunchRecords = 0;

                intNumberOfRecords = TheImportAholaPunchesDataSet.importaholapunches.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnRecordFound = false;
                        intEmployeeID = TheImportAholaPunchesDataSet.importaholapunches[intCounter].EmployeeID;
                        strFirstName = TheImportAholaPunchesDataSet.importaholapunches[intCounter].FirstName;
                        strLastName = TheImportAholaPunchesDataSet.importaholapunches[intCounter].LastName;
                        datStartDate = TheImportAholaPunchesDataSet.importaholapunches[intCounter].ActualDateTime;
                        intPayID = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PayID;


                        if (TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchMode == "OUT")
                        {
                            datEndDate = TheImportAholaPunchesDataSet.importaholapunches[intCounter].ActualDateTime;
                        }

                        else
                        {
                            if (intCounter + 1 < intNumberOfRecords)
                            {
                                if (intEmployeeID == TheImportAholaPunchesDataSet.importaholapunches[intCounter + 1].EmployeeID)
                                {
                                    datEndDate = TheImportAholaPunchesDataSet.importaholapunches[intCounter + 1].ActualDateTime;

                                    intCounter++;
                                }
                                else if (intEmployeeID == TheImportAholaPunchesDataSet.importaholapunches[intCounter - 1].EmployeeID)
                                {
                                    //datStartDate = TheImportAholaPunchesDataSet.importaholapunches[intCounter - 1].ActualDateTime;
                                }

                            }
                            else
                            {
                                datEndDate = datStartDate;
                            }
                        }

                        if (TheImportAholaPunchesDataSet.importaholapunches[intCounter - 1].PunchMode == "IN")
                        {
                            datEndDate = TheImportAholaPunchesDataSet.importaholapunches[intCounter].ActualDateTime;
                            datStartDate = TheImportAholaPunchesDataSet.importaholapunches[intCounter - 1].ActualDateTime;
                        }


                        tspTotalHours = datEndDate - datStartDate;

                        decDailyHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                        if (decDailyHours < 0)
                        {
                            tspTotalHours = datStartDate - datEndDate;

                            decDailyHours = Convert.ToDecimal(tspTotalHours.TotalHours);
                        }

                        decDailyHours = Math.Round(decDailyHours, 3);

                        strPunchSource = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchSource;
                        strPunchType = TheImportAholaPunchesDataSet.importaholapunches[intCounter].PunchType;


                        if (gintPunchRecords > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter < gintPunchRecords; intSecondCounter++)
                            {
                                if (intEmployeeID == TheCalculatedHoursDataSet.calculatedhours[intSecondCounter].EmployeeID)
                                {
                                    if (datStartDate.Hour == 0)
                                    {

                                    }
                                    else if (datStartDate == TheCalculatedHoursDataSet.calculatedhours[intSecondCounter].StartTime)
                                    {
                                        blnRecordFound = true;
                                    }
                                    else if (datStartDate == TheCalculatedHoursDataSet.calculatedhours[intSecondCounter].EndTime)
                                    {
                                        if (TheImportAholaPunchesDataSet.importaholapunches[intCounter - 1].PunchMode != "IN")
                                        {
                                            blnRecordFound = true;
                                        }

                                    }
                                }
                            }
                        }


                        if (blnRecordFound == false)
                        {

                            CalculatedHoursDataSet.calculatedhoursRow NewPunchRow = TheCalculatedHoursDataSet.calculatedhours.NewcalculatedhoursRow();

                            NewPunchRow.DailyHours = decDailyHours;
                            NewPunchRow.EmployeeID = intEmployeeID;
                            NewPunchRow.EndTime = datEndDate;
                            NewPunchRow.FirstName = strFirstName;
                            NewPunchRow.LastName = strLastName;
                            NewPunchRow.StartTime = datStartDate;
                            NewPunchRow.PayID = intPayID;

                            TheCalculatedHoursDataSet.calculatedhours.Rows.Add(NewPunchRow);
                            gintPunchRecords++;
                        }
                    }
                }

                dgrResults.ItemsSource = TheCalculatedHoursDataSet.calculatedhours;
                expProcessHours.IsEnabled = true;

                PleaseWait.Close();

                TheMessagesClass.InformationMessage("Hours Have Been Calculated");


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Calculate Hours Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessHours_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            DateTime datStartDate;
            DateTime datEndDate;
            decimal decDailyHours;
            DateTime datPayDate = DateTime.Now;
            string strValueForValidation;
            bool blnFatalError = false;
            int intPayID;
            int intSecondCounter;
            bool blnItemFound;
            string strLastName;
            string strFirstName;

            try
            {
                expProcessHours.IsExpanded = false;
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheEmployeeHoursDataSet.employeehours.Rows.Clear();

                strValueForValidation = txtPayDate.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date is not a Date");
                    return;
                }
                else
                {
                    datPayDate = Convert.ToDateTime(strValueForValidation);

                    blnFatalError = TheDataValidationClass.verifyDateRange(datPayDate, DateTime.Now);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Date Entered is greater than Today");
                        return;
                    }
                }

                intNumberOfRecords = TheCalculatedHoursDataSet.calculatedhours.Rows.Count;
                gintPunchRecords = 0;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    blnItemFound = false;
                    intEmployeeID = TheCalculatedHoursDataSet.calculatedhours[intCounter].EmployeeID;
                    intPayID = TheCalculatedHoursDataSet.calculatedhours[intCounter].PayID;
                    datStartDate = TheCalculatedHoursDataSet.calculatedhours[intCounter].StartTime;
                    datEndDate = TheCalculatedHoursDataSet.calculatedhours[intCounter].EndTime;
                    decDailyHours = TheCalculatedHoursDataSet.calculatedhours[intCounter].DailyHours;
                    strFirstName = TheCalculatedHoursDataSet.calculatedhours[intCounter].FirstName;
                    strLastName = TheCalculatedHoursDataSet.calculatedhours[intCounter].LastName;

                    if (gintPunchRecords > 0)
                    {
                        for (intSecondCounter = 0; intSecondCounter < gintPunchRecords; intSecondCounter++)
                        {
                            if (intEmployeeID == TheEmployeeHoursDataSet.employeehours[intSecondCounter].EmployeeID)
                            {
                                TheEmployeeHoursDataSet.employeehours[intSecondCounter].PunchedHours += decDailyHours;
                                blnItemFound = true;
                            }
                        }
                    }

                    if (blnItemFound == false)
                    {
                        EmployeeHoursDataSet.employeehoursRow NewEmployeeRow = TheEmployeeHoursDataSet.employeehours.NewemployeehoursRow();

                        NewEmployeeRow.EmployeeID = intEmployeeID;
                        NewEmployeeRow.PayID = intPayID;
                        NewEmployeeRow.PunchedHours = decDailyHours;
                        NewEmployeeRow.TransactionDate = datPayDate;
                        NewEmployeeRow.FirstName = strFirstName;
                        NewEmployeeRow.LastName = strLastName;

                        TheEmployeeHoursDataSet.employeehours.Rows.Add(NewEmployeeRow);
                        gintPunchRecords++;
                    }
                }

                PleaseWait.Close();

                expInsertRecords.IsEnabled = true;

                dgrResults.ItemsSource = TheEmployeeHoursDataSet.employeehours;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Process Hours Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expInsertRecords_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intPayID; ;
            DateTime datStartDate;
            DateTime datEndDate;
            DateTime datPayDate;
            decimal decDailyHours; ;
            decimal decTotalHours;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                expInsertRecords.IsExpanded = false;

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                intNumberOfRecords = TheCalculatedHoursDataSet.calculatedhours.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheCalculatedHoursDataSet.calculatedhours[intCounter].EmployeeID;
                    datStartDate = TheCalculatedHoursDataSet.calculatedhours[intCounter].StartTime;
                    datEndDate = TheCalculatedHoursDataSet.calculatedhours[intCounter].EndTime;
                    decDailyHours = TheCalculatedHoursDataSet.calculatedhours[intCounter].DailyHours;

                    TheFindAholaEmployeePunchForVerificationDataSet = TheEmployeePunchedHoursClass.FindAholaEmployeePunchForVerification(intEmployeeID, datStartDate, datEndDate, decDailyHours);

                    intRecordsReturned = TheFindAholaEmployeePunchForVerificationDataSet.FindAholaEmployeePunchForVerification.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        blnFatalError = TheEmployeePunchedHoursClass.InsertIntoAholaEmployeePunch(intEmployeeID, datStartDate, datEndDate, decDailyHours);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                intNumberOfRecords = TheEmployeeHoursDataSet.employeehours.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheEmployeeHoursDataSet.employeehours[intCounter].EmployeeID;
                    intPayID = TheEmployeeHoursDataSet.employeehours[intCounter].PayID;
                    datPayDate = TheEmployeeHoursDataSet.employeehours[intCounter].TransactionDate;
                    decTotalHours = TheEmployeeHoursDataSet.employeehours[intCounter].PunchedHours;

                    TheFindEmployeePunchedHoursForValidationDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHoursForValidation(datPayDate, intEmployeeID, intEmployeeID);

                    intRecordsReturned = TheFindEmployeePunchedHoursForValidationDataSet.FindEmployeePunchedHoursForValidation.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        blnFatalError = TheEmployeePunchedHoursClass.InsertEmployeePunchedHours(datPayDate, intEmployeeID, intPayID, decTotalHours);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                PleaseWait.Close();

                TheMessagesClass.InformationMessage("All Records Have Been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Employee Punches // Insert Records " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;

            ResetControls();
        }
    }
}
