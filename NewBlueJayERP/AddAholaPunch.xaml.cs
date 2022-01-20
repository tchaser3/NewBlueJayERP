/* Title:           Add Ahola PUnch
 * Date:            12-27-2021
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add a transaction for ahola if there is a missing transaction */

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
using DataValidationDLL;
using DateSearchDLL;
using EmployeePunchedHoursDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddAholaPunch.xaml
    /// </summary>
    public partial class AddAholaPunch : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();   
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        public AddAholaPunch()
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
            try
            {
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                txtEmployeeID.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeID);
                txtFirstName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                txtLastName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                txtPayID.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PayID);

                btnProcess.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Ahola Punch // Window Loaded " + Ex.Message);

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
                if (blnThereIsAProblem == true)
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

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                tspTotalHours = datEndDate - datStartDate;

                decTotalHours = Convert.ToDecimal(tspTotalHours.TotalHours);

                decTotalHours = Math.Round(decTotalHours, 3);

                txtHours.Text = Convert.ToString(decTotalHours);

                btnProcess.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Ahola Punch // Compute Hours Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            decimal decTotalHours = 0;

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
                if(blnThereIsAProblem == true)
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
                if(blnThereIsAProblem==true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Hours is not Numberic\n";
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
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                blnFatalError = TheEmployeePunchedHoursClass.InsertIntoAholaEmployeePunch(MainWindow.gintEmployeeID, datStartDate, datEndDate, decTotalHours);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Transaction Has Been Added");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Ahola Punch // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
