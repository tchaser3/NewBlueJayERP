/* Title:           Edit Work Task
 * Date:            2-19-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for editing a specific task*/

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
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditWorkTask.xaml
    /// </summary>
    public partial class EditWorkTask : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByKeywoardDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindWorkTaskByTaskIDDataSet TheFindWorkTaskByTaskIDDataSet = new FindWorkTaskByTaskIDDataSet();

        public EditWorkTask()
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
            //setting up the controls
            txtEnterTask.Text = "";
            txtTotalCost.Text = "";
            txtWorkTask.Text = "";
            txtWorkTaskID.Text = "";

            cboSelectActive.Items.Clear();
            cboSelectActive.Items.Add("Select Active");
            cboSelectActive.Items.Add("Yes");
            cboSelectActive.Items.Add("No");
            cboSelectActive.SelectedIndex = 0;

            cboSelectTask.Items.Clear();
            cboSelectTask.Items.Add("Select Task");
            cboSelectTask.SelectedIndex = 0;

            btnProcess.IsEnabled = false;
        }

        private void txtEnterTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strWorkTask = txtEnterTask.Text;

                if(strWorkTask.Length > 2)
                {
                    cboSelectTask.Items.Clear();
                    cboSelectTask.Items.Add("Select Task");

                    TheFindWorkTaskByKeywoardDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                    intNumberOfRecords = TheFindWorkTaskByKeywoardDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Work Task Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectTask.Items.Add(TheFindWorkTaskByKeywoardDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
                    }

                    cboSelectTask.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Work Task // Enter Task Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectTask.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintWorkTaskID = TheFindWorkTaskByKeywoardDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;

                    txtTotalCost.Text = Convert.ToString(TheFindWorkTaskByKeywoardDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].TaskCost);
                    txtWorkTask.Text = TheFindWorkTaskByKeywoardDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTask;
                    txtWorkTaskID.Text = Convert.ToString(MainWindow.gintWorkTaskID);
                    btnProcess.IsEnabled = true;

                    TheFindWorkTaskByTaskIDDataSet = TheWorkTaskClass.FindWorkTaskByWorkTaskID(MainWindow.gintWorkTaskID);

                    if (TheFindWorkTaskByTaskIDDataSet.FindWorkTaskByWorkTaskID[0].TaskActive == true)
                    {
                        cboSelectActive.SelectedIndex = 1;
                    }
                    else
                    {
                        cboSelectActive.SelectedIndex = 2;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // cbo Select Task Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMessage = "";
            bool blnFatalError = false;
            string strWorkTask;
            string strActive = "";
            string strValueForValidation;
            decimal decTotalCost = 0;
            bool blnThereIsAProblem;

            try
            {
                if(cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Was Not Selected\n";
                }
                strWorkTask = txtWorkTask.Text;
                if(strWorkTask.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task is not Long Enough\n";
                }
                strValueForValidation = txtTotalCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Total Costs is not Numeric\n";
                }
                else
                {
                    decTotalCost = Convert.ToDecimal(strValueForValidation);
                }
                if(cboSelectActive.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Active has not Been Selected\n";
                }
                else
                {
                    strActive = cboSelectActive.SelectedItem.ToString().ToUpper();
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheWorkTaskClass.UpdateWorkTask(MainWindow.gintWorkTaskID, strWorkTask, decTotalCost);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Work Task Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Work Task // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
