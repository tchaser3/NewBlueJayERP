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
using EmployeePunchedHoursDLL;
using DataValidationDLL;
using NewEventLogDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AholaEmployeePunches.xaml
    /// </summary>
    public partial class AholaEmployeePunches : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeePunchedHoursClass TheEmployeePUnchedHoursClass = new EmployeePunchedHoursClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindAholaEmployeePunchHoursDataSet TheFindAholaEmployeePunchHoursDataSet = new FindAholaEmployeePunchHoursDataSet();

        //setting up global variables
        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public AholaEmployeePunches()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            try
            {
                gdatStartDate = TheDateSearchClass.SubtractingDays(MainWindow.gdatPayDate, 6);
                gdatEndDate = TheDateSearchClass.AddingDays(MainWindow.gdatPayDate, 1);

                TheFindAholaEmployeePunchHoursDataSet = TheEmployeePUnchedHoursClass.FindAholaEmployeePunchHours(MainWindow.gintEmployeeID, gdatStartDate, gdatEndDate);

                dgrEmployeeHours.ItemsSource = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours;
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Ahola Employee Punches // Window Loaded " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Ahola Employee Punches // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrEmployeeHours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;

            try
            {
                if (dgrEmployeeHours.SelectedIndex > -1)
                {

                    //setting local variable
                    dataGrid = dgrEmployeeHours;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTransactionID = ((TextBlock)TransactionID.Content).Text;

                    //find the record
                    MainWindow.gintTransactionID = Convert.ToInt32(strTransactionID);

                    ChangeAlohaTimes ChangeAlohaTimes = new ChangeAlohaTimes();
                    ChangeAlohaTimes.ShowDialog();

                    TheFindAholaEmployeePunchHoursDataSet = TheEmployeePUnchedHoursClass.FindAholaEmployeePunchHours(MainWindow.gintEmployeeID, gdatStartDate, gdatEndDate);

                    dgrEmployeeHours.ItemsSource = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours;

                }

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Ahola Employee Punches // Employee Hours Grid Selection " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Ahola Employee Punches // Employee Hours Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expAdjustTotalHours_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            decimal decTotalHours = 0;
            bool blnFatalError = false;

            try
            {
                expAdjustTotalHours.IsExpanded = false;

                intNumberOfRecords = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        decTotalHours += TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].DailyHours;
                    }

                    blnFatalError = TheEmployeePUnchedHoursClass.UpdateEmployeePunchedHours(MainWindow.gintEmployeeID, MainWindow.gdatPayDate, decTotalHours);

                    if (blnFatalError == true)
                        throw new Exception();
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Ahola Employee Punches // Adjust Total Hours Expander " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Ahola Employee Punches // Adjust Total Hours Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void expAddPunch_Expanded(object sender, RoutedEventArgs e)
        {
            AddAholaPunch AddAholaPunch = new AddAholaPunch();
            AddAholaPunch.ShowDialog();

            TheFindAholaEmployeePunchHoursDataSet = TheEmployeePUnchedHoursClass.FindAholaEmployeePunchHours(MainWindow.gintEmployeeID, gdatStartDate, gdatEndDate);

            dgrEmployeeHours.ItemsSource = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours;
        }
    }
}
