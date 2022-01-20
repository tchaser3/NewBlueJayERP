/* Title:           Change Aloha Times
 * Date:            12-20-21
 * Author:          Terry Holmes
 * 
 * Description:     This used to edit the times */

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
using DataValidationDLL;
using DateSearchDLL;
using NewEventLogDLL;
using EmployeePunchedHoursDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ChangeAlohaTimes.xaml
    /// </summary>
    public partial class ChangeAlohaTimes : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindAlohaTimePunchesByTransactionIDDataSet TheFindAlohaTimePunchesByTransactionIDDataSet = new FindAlohaTimePunchesByTransactionIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        public ChangeAlohaTimes()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intEmployeeID;

            try
            {
                TheFindAlohaTimePunchesByTransactionIDDataSet = TheEmployeePunchedHoursClass.FindAlohaTimePunchesByTransactionID(MainWindow.gintTransactionID);

                intEmployeeID = TheFindAlohaTimePunchesByTransactionIDDataSet.FindAlohaTimePunchesByTransactionID[0].EmployeeID;

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                txtEmployeeID.Text = Convert.ToString(intEmployeeID);
                txtEndDate.Text = Convert.ToString(TheFindAlohaTimePunchesByTransactionIDDataSet.FindAlohaTimePunchesByTransactionID[0].EndDate);
                txtFirstName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                txtHours.Text = Convert.ToString(TheFindAlohaTimePunchesByTransactionIDDataSet.FindAlohaTimePunchesByTransactionID[0].DailyHours);
                txtLastName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                txtStartDate.Text = Convert.ToString(TheFindAlohaTimePunchesByTransactionIDDataSet.FindAlohaTimePunchesByTransactionID[0].StartDate);
                txtTransactionID.Text = Convert.ToString(MainWindow.gintTransactionID);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Aloha Times // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expComputeHours_Expanded(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            TimeSpan tspTotalHours;
            decimal decTotalHours;

            try
            {
                expComputeHours.IsExpanded = false;

                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                tspTotalHours = datEndDate - datStartDate;

                decTotalHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                decTotalHours = Math.Round(decTotalHours, 3);

                txtHours.Text = Convert.ToString(decTotalHours);

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Aloha Times // Compute Hours Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            decimal decTotalHours = 0;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            try
            {
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtHours.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Hours is not Numeric\n";
                }
                else
                {
                    decTotalHours = Convert.ToDecimal(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheEmployeePunchedHoursClass.UpdateAlohaTimePunches(MainWindow.gintTransactionID, datStartDate, datEndDate, decTotalHours);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Record Has Been Updated");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Aloha Times // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString()); 
            }
        }
    }
}
