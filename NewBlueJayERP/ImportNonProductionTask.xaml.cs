/* Title:           Import Non-Production Task
 * Date:            2-8-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import non production tasks*/

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
using Excel = Microsoft.Office.Interop.Excel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportNonProductionTask.xaml
    /// </summary>
    public partial class ImportNonProductionTask : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        NonProductionProductivityClass TheNonProductionProductivityClass = new NonProductionProductivityClass();

        //setting up the data
        FindNonProductionTaskByWorkTaskDataSet TheFindNonProductionTaskByWorkTaskDataSet = new FindNonProductionTaskByWorkTaskDataSet();
        ImportNonProductionTasksDataSet TheImportNonProductionTasksDataSet = new ImportNonProductionTasksDataSet();

        public ImportNonProductionTask()
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
            TheImportNonProductionTasksDataSet.importnonproductiontasks.Rows.Clear();

            dgrWorkTasks.ItemsSource = TheImportNonProductionTasksDataSet.importnonproductiontasks;
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
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
            string strWorkTask;
            int intRecordsReturned;

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportNonProductionTasksDataSet.importnonproductiontasks.Rows.Clear();

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

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strWorkTask = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();

                    TheFindNonProductionTaskByWorkTaskDataSet = TheNonProductionProductivityClass.FindNOnProductionTaskByWorkTask(strWorkTask);

                    intRecordsReturned = TheFindNonProductionTaskByWorkTaskDataSet.FindNonProductionTaskByWorkTask.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        ImportNonProductionTasksDataSet.importnonproductiontasksRow NewTaskRow = TheImportNonProductionTasksDataSet.importnonproductiontasks.NewimportnonproductiontasksRow();

                        NewTaskRow.WorkTask = strWorkTask;

                        TheImportNonProductionTasksDataSet.importnonproductiontasks.Rows.Add(NewTaskRow);
                    }
                   
                }

                PleaseWait.Close();

                dgrWorkTasks.ItemsSource = TheImportNonProductionTasksDataSet.importnonproductiontasks;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Non-Production Tasks // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                expProcessImport.IsExpanded = false;

                intNumberOfRecords = TheImportNonProductionTasksDataSet.importnonproductiontasks.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnFatalError = TheNonProductionProductivityClass.InsertNonProductionTask(TheImportNonProductionTasksDataSet.importnonproductiontasks[intCounter].WorkTask);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("All Records Imported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Non-Production Task // Process Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
