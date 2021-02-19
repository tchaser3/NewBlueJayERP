/* Title:           Import Edited Work Tasks
 * Date:            2-18-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to correct any code that needs to be*/

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
using Excel = Microsoft.Office.Interop.Excel;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportEditedWorkTasks.xaml
    /// </summary>
    public partial class ImportEditedWorkTasks : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        ImportEditedWorkTaskDataSet TheImportEditedWorkTaskDataSet = new ImportEditedWorkTaskDataSet();
        FindWorkTaskByTaskIDDataSet TheFindWorkTaskByTaskIDDataSet = new FindWorkTaskByTaskIDDataSet();

        public ImportEditedWorkTasks()
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
            TheImportEditedWorkTaskDataSet.worktask.Rows.Clear();

            dgrTasks.ItemsSource = TheImportEditedWorkTaskDataSet.worktask;
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


            try
            {
                expImportExcel.IsExpanded = false;
                TheImportEditedWorkTaskDataSet.worktask.Rows.Clear();

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
                    strWorkTaskID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strWorkTask = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    intWorkTaskID = Convert.ToInt32(strWorkTaskID);

                    TheFindWorkTaskByTaskIDDataSet = TheWorkTaskClass.FindWorkTaskByWorkTaskID(intWorkTaskID);

                    ImportEditedWorkTaskDataSet.worktaskRow NewWorkTask = TheImportEditedWorkTaskDataSet.worktask.NewworktaskRow();

                    NewWorkTask.WorkTask = strWorkTask;
                    NewWorkTask.WorkTaskID = intWorkTaskID;
                    NewWorkTask.CurrentWorkTask = TheFindWorkTaskByTaskIDDataSet.FindWorkTaskByWorkTaskID[0].WorkTask;
                    NewWorkTask.Replace = false;

                    TheImportEditedWorkTaskDataSet.worktask.Rows.Add(NewWorkTask);
                }

                dgrTasks.ItemsSource = TheImportEditedWorkTaskDataSet.worktask;

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Production Codes // Import Excel  " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strWorkTask;
            int intWorkTaskID;
            bool blnFatalError = false;

            try
            {
                expProcess.IsExpanded = false;

                intNumberOfRecords = TheImportEditedWorkTaskDataSet.worktask.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheImportEditedWorkTaskDataSet.worktask[intCounter].Replace == true)
                        {
                            intWorkTaskID = TheImportEditedWorkTaskDataSet.worktask[intCounter].WorkTaskID;
                            strWorkTask = TheImportEditedWorkTaskDataSet.worktask[intCounter].WorkTask;

                            blnFatalError = TheWorkTaskClass.UpdateWorkTask(intWorkTaskID, strWorkTask, 0);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Tasks are Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Edited Work Task // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
