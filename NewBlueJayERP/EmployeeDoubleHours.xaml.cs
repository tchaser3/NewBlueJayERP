/* Title:           Employee Double Hours
 * Date:            2-10-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for finding and correcting double hours */

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
using NewEventLogDLL;
using DataValidationDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeDoubleHours.xaml
    /// </summary>
    public partial class EmployeeDoubleHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindDuplicateEmployeePunchedHoursDataSet TheFindDuplicateEmployeePunchedHoursDataSet = new FindDuplicateEmployeePunchedHoursDataSet();
        FindEmployeePunchedHoursDataSet TheFindEmployeePunchedHoursDataSet = new FindEmployeePunchedHoursDataSet();
        DuplicateEmployeeDataSet TheDuplicateEmployeeDataSet = new DuplicateEmployeeDataSet();

        public EmployeeDoubleHours()
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
            txtEnterPayPeriod.Text = "";

            TheDuplicateEmployeeDataSet.duplicateemployees.Rows.Clear();

            dgrEmployees.ItemsSource = TheDuplicateEmployeeDataSet.duplicateemployees;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            DateTime datTransactionDate = DateTime.Now;
            bool blnFatalError = false;
            int intEmployeeID;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            DateTime datSecondDate;
            string strFirstName;
            string strLastName;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheDuplicateEmployeeDataSet.duplicateemployees.Rows.Clear();

                strValueForValidation = txtEnterPayPeriod.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is not a Date\n");
                    return;
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(strValueForValidation);
                }

                datSecondDate = TheDateSearchClass.AddingDays(datTransactionDate, 1);

                TheFindDuplicateEmployeePunchedHoursDataSet = TheEmployeePunchedHoursClass.FindDuplicateEmployeePunchedHours(datTransactionDate);

                intNumberOfRecords = TheFindDuplicateEmployeePunchedHoursDataSet.FindDuplicateEmployeePunchedHours.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheFindDuplicateEmployeePunchedHoursDataSet.FindDuplicateEmployeePunchedHours[intCounter].EmployeeID;
                        strFirstName = TheFindDuplicateEmployeePunchedHoursDataSet.FindDuplicateEmployeePunchedHours[intCounter].FirstName;
                        strLastName = TheFindDuplicateEmployeePunchedHoursDataSet.FindDuplicateEmployeePunchedHours[intCounter].LastName;

                        TheFindEmployeePunchedHoursDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHours(intEmployeeID, datTransactionDate, datSecondDate);

                        intSecondNumberOfRecords = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours.Rows.Count;

                        for (intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                        {
                            DuplicateEmployeeDataSet.duplicateemployeesRow NewEmployeeRow = TheDuplicateEmployeeDataSet.duplicateemployees.NewduplicateemployeesRow();

                            NewEmployeeRow.EmployeeID = intEmployeeID;
                            NewEmployeeRow.FirstName = strFirstName;
                            NewEmployeeRow.LastName = strLastName;
                            NewEmployeeRow.PunchedHours = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours[intSecondCounter].PunchedHours;
                            NewEmployeeRow.TransactionID = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours[intSecondCounter].TransactionID;
                            NewEmployeeRow.Remove = false;

                            TheDuplicateEmployeeDataSet.duplicateemployees.Rows.Add(NewEmployeeRow);
                        }
                    }                    
                }

                dgrEmployees.ItemsSource = TheDuplicateEmployeeDataSet.duplicateemployees;

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Double Hours // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expRemoveHours_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intTransactionID;

            try
            {
                expRemoveHours.IsExpanded = false;

                intNumberOfRecords = TheDuplicateEmployeeDataSet.duplicateemployees.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheDuplicateEmployeeDataSet.duplicateemployees[intCounter].Remove == true)
                        {
                            intTransactionID = TheDuplicateEmployeeDataSet.duplicateemployees[intCounter].TransactionID;

                            blnFatalError = TheEmployeePunchedHoursClass.RemoveEmployeePunchedHours(intTransactionID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Transactions Have Been Removed");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Double Hours // Remove Hous Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
