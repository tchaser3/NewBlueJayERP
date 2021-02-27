/* Title:           Import Codes for Sheets
 * Date:            2-23-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for importing the codes so we can use them for production sheets*/

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
using DepartmentDLL;
using WorkTaskDLL;
using Excel = Microsoft.Office.Interop.Excel;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportCodesForSheets.xaml
    /// </summary>
    public partial class ImportCodesForSheets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up data
        ProductionCodesForImportDataSet TheProductionCodesForImportDataSet = new ProductionCodesForImportDataSet();
        FindWorkTaskByTaskIDDataSet TheFindWorkTaskByTaskIDDataSet = new FindWorkTaskByTaskIDDataSet();
        FindDepartmentByNameDataSet TheFindDepartmentByNameDataSet = new FindDepartmentByNameDataSet();
        FindWorkTaskDepartmentWorkTaskMatchDataSet TheFindWorkTaskDepartmentWorkTaskMatchDataSet = new FindWorkTaskDepartmentWorkTaskMatchDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByTaskKeyWordDataSet = new FindWorkTaskByTaskKeywordDataSet();

        public ImportCodesForSheets()
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
            TheMessageClass.LaunchHelpDeskTickets();
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
            TheProductionCodesForImportDataSet.productioncodes.Rows.Clear();

            dgrProductionCodes.ItemsSource = TheProductionCodesForImportDataSet.productioncodes;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Codes for Sheets");
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
            int intWorkTaskID = 0;
            string strWorkTask = "";
            string strWorkTaskID = "";
            string strBusinessLine;
            string strBusinessLineID;
            int intBusinessLineID;
            string strDepartment;
            int intDepartmentID;
            int intDepartmentID2 = 0;
            string strDepartment2 = "";
            bool blnAll = false;
            int intRecordsReturned;


            try
            {
                expImportExcel.IsExpanded = false;
                TheProductionCodesForImportDataSet.productioncodes.Rows.Clear();

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
                    blnAll = false;
                    strWorkTaskID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strWorkTask = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    intWorkTaskID = Convert.ToInt32(strWorkTaskID);
                    strBusinessLine = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strDepartment = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();

                    TheFindWorkTaskByTaskKeyWordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                    intRecordsReturned = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        intWorkTaskID = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;
                    }

                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(strBusinessLine);

                    intBusinessLineID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    if(strDepartment == "ALL")
                    {
                        strDepartment = "AERIAL";
                        strDepartment2 = "UNDERGROUND";
                        blnAll = true;
                    }

                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(strDepartment);

                    intDepartmentID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                    TheFindWorkTaskByTaskIDDataSet = TheWorkTaskClass.FindWorkTaskByWorkTaskID(intWorkTaskID);

                    ProductionCodesForImportDataSet.productioncodesRow NewProductionCode = TheProductionCodesForImportDataSet.productioncodes.NewproductioncodesRow();

                    NewProductionCode.BusinessLine = strBusinessLine;
                    NewProductionCode.BusinessLineID = intBusinessLineID;
                    NewProductionCode.Department = strDepartment;
                    NewProductionCode.DepartmentID = intDepartmentID;
                    NewProductionCode.WorkTask = strWorkTask;
                    NewProductionCode.WorkTaskID = intWorkTaskID;

                    TheProductionCodesForImportDataSet.productioncodes.Rows.Add(NewProductionCode);

                    if(blnAll == true)
                    {
                        TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(strDepartment2);

                        intDepartmentID2 = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                        ProductionCodesForImportDataSet.productioncodesRow NewProductionCode2 = TheProductionCodesForImportDataSet.productioncodes.NewproductioncodesRow();

                        NewProductionCode2.BusinessLine = strBusinessLine;
                        NewProductionCode2.BusinessLineID = intBusinessLineID;
                        NewProductionCode2.Department = strDepartment2;
                        NewProductionCode2.DepartmentID = intDepartmentID2;
                        NewProductionCode2.WorkTask = strWorkTask;
                        NewProductionCode2.WorkTaskID = intWorkTaskID;

                        TheProductionCodesForImportDataSet.productioncodes.Rows.Add(NewProductionCode2);
                    }
                }

                dgrProductionCodes.ItemsSource = TheProductionCodesForImportDataSet.productioncodes;

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Codes for Sheets // Import Excel  " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWorkTaskID;
            int intDepartmentID;
            int intBusinessLineID;
            int intEmployeeID;
            DateTime datTransactionDate;
            bool blnFatalError = false;
            string strWorkTask;
            int intRecordsReturned;

            try
            {
                expProcessImport.IsExpanded = false;

                intNumberOfRecords = TheProductionCodesForImportDataSet.productioncodes.Rows.Count;
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    datTransactionDate = DateTime.Now;
                    intWorkTaskID = TheProductionCodesForImportDataSet.productioncodes[intCounter].WorkTaskID;
                    intDepartmentID = TheProductionCodesForImportDataSet.productioncodes[intCounter].DepartmentID;
                    intBusinessLineID = TheProductionCodesForImportDataSet.productioncodes[intCounter].BusinessLineID;
                    
                    if(intWorkTaskID < 0)
                    {
                        strWorkTask = TheProductionCodesForImportDataSet.productioncodes[intCounter].WorkTask;

                        TheFindWorkTaskByTaskKeyWordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                        intRecordsReturned = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            blnFatalError = TheWorkTaskClass.InsertWorkTask(strWorkTask, 0);

                            if (blnFatalError == true)
                                throw new Exception();

                            TheFindWorkTaskByTaskKeyWordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                            intWorkTaskID = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;
                        }
                        else if(intRecordsReturned > 0)
                        {
                            intWorkTaskID = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;
                        }                        
                    }

                    TheFindWorkTaskDepartmentWorkTaskMatchDataSet = TheWorkTaskClass.FindWorkTaskDepartmentWorkTaskMatch(intWorkTaskID, intBusinessLineID, intDepartmentID);

                    intRecordsReturned = TheFindWorkTaskDepartmentWorkTaskMatchDataSet.FindWorkTaskDepartmentWorkTaskMatch.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(intWorkTaskID, intBusinessLineID, intDepartmentID, intEmployeeID, datTransactionDate);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessageClass.InformationMessage("The Codes have been Imported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Codes For Sheets // Process Import Expanded " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
