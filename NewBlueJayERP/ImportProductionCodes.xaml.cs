/* Title:           Import Production Codes
 * Date:            1-11-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import the production codes */

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
using WorkTaskDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportProductionCodes.xaml
    /// </summary>
    public partial class ImportProductionCodes : Window
    {
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindWorkTaskImportByLaborCodeDataSet TheFindWorkTaskImportByLaborCodeDataSet = new FindWorkTaskImportByLaborCodeDataSet();
        ImportWorkTaskDataSet TheImportWorkTaskDataSet = new ImportWorkTaskDataSet();

        public ImportProductionCodes()
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
            TheImportWorkTaskDataSet.importworktask.Rows.Clear();

            dgrProductionCodes.ItemsSource = TheImportWorkTaskDataSet.importworktask;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Import Production Codes ");
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
            int intRecordsReturned;
            int intWorkTaskID = 0;
            string strWorkTask = "";
            string strLaborCode;
            string strLaborType;
            string strItemFunction;
            string strItemDescription;
            string strUnitOfMeasure;
            

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportWorkTaskDataSet.importworktask.Rows.Clear();

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
                    strLaborCode = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strLaborType = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strItemFunction = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                    strItemDescription = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                    strUnitOfMeasure = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();

                    TheFindWorkTaskImportByLaborCodeDataSet = TheWorkTaskClass.FindWorkTaskImportByLaborCode(strLaborCode);

                    intRecordsReturned = TheFindWorkTaskImportByLaborCodeDataSet.FindWorkTaskImportByLaborCode.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheFindWorkTaskByKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strLaborCode);

                        intRecordsReturned = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            intWorkTaskID = intCounter * -1;
                            strWorkTask = strLaborCode + " - " + strItemFunction;
                        }
                        else if(intRecordsReturned > 0)
                        {
                            intWorkTaskID = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;
                            strWorkTask = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[0].WorkTask;
                        }

                        ImportWorkTaskDataSet.importworktaskRow NewWorkTaskRow = TheImportWorkTaskDataSet.importworktask.NewimportworktaskRow();

                        NewWorkTaskRow.ItemDescription = strItemDescription;
                        NewWorkTaskRow.ItemFunction = strItemDescription;
                        NewWorkTaskRow.LaborCode = strLaborCode;
                        NewWorkTaskRow.LaborType = strLaborType;
                        NewWorkTaskRow.UnitOfMeasure = strUnitOfMeasure;
                        NewWorkTaskRow.WorkTask = strWorkTask;
                        NewWorkTaskRow.WorkTaskID = intWorkTaskID;

                        TheImportWorkTaskDataSet.importworktask.Rows.Add(NewWorkTaskRow);
                    }    
                }

                dgrProductionCodes.ItemsSource = TheImportWorkTaskDataSet.importworktask;
                

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Production Codes // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWorkTaskID;
            string strWorkTask;
            string strLaborCode;
            string strLaborType;
            string strItemFunction;
            string strItemDescription;
            string strUnitOfMeasure;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                intNumberOfRecords = TheImportWorkTaskDataSet.importworktask.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intWorkTaskID = TheImportWorkTaskDataSet.importworktask[intCounter].WorkTaskID;
                    strWorkTask = TheImportWorkTaskDataSet.importworktask[intCounter].WorkTask;
                    strLaborCode = TheImportWorkTaskDataSet.importworktask[intCounter].LaborCode;
                    strLaborType = TheImportWorkTaskDataSet.importworktask[intCounter].LaborType;
                    strItemFunction = TheImportWorkTaskDataSet.importworktask[intCounter].ItemFunction;
                    strItemDescription = TheImportWorkTaskDataSet.importworktask[intCounter].ItemDescription;
                    strUnitOfMeasure = TheImportWorkTaskDataSet.importworktask[intCounter].UnitOfMeasure;

                    if(intWorkTaskID < 0)
                    {
                        blnFatalError = TheWorkTaskClass.InsertWorkTask(strWorkTask, 0);

                        if (blnFatalError == true)
                            throw new Exception();

                        TheFindWorkTaskByKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strLaborCode);

                        intWorkTaskID = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;
                    }
                    if(intWorkTaskID > 0)
                    {
                        blnFatalError = TheWorkTaskClass.UpdateWorkTask(intWorkTaskID, strWorkTask, 0);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    TheFindWorkTaskImportByLaborCodeDataSet = TheWorkTaskClass.FindWorkTaskImportByLaborCode(strLaborCode);

                    intRecordsReturned = TheFindWorkTaskImportByLaborCodeDataSet.FindWorkTaskImportByLaborCode.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = TheWorkTaskClass.InsertWorkTaskImport(intWorkTaskID, strLaborCode, strLaborType, strItemFunction, strItemDescription, strUnitOfMeasure);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Codes have been Imported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Production Codes // Process Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
