/* Title:           Employee Look Up
 * Date:            12-8-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for employee lookup */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeLookup.xaml
    /// </summary>
    public partial class EmployeeLookup : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindAllEmployeesByLastNameDataSet TheFindAllEmployeesByLastNameDataSet = new FindAllEmployeesByLastNameDataSet();

        public EmployeeLookup()
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
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            txtActive.Text = "";
            txtDepartment.Text = "";
            txtEmail.Text = "";
            txtEmployeeLastName.Text = "";
            txtFirstName.Text = "";
            txtHomeOffice.Text = "";
            txtLastName.Text = "";
            txtManager.Text = "";
            txtManagerEmail.Text = "";
            txtManagerOffice.Text = "";
            txtManagerPhone.Text = "";
            txtPhoneNumber.Text = "";
            txtStartDate.Text = "";

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Employee Lookup");
        }

        private void txtEmployeeLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intNumberOfRecords;
            int intCounter;
            string strFullName;

            try
            {
                strLastName = txtEmployeeLastName.Text;

                if(strLastName.Length > 2)
                {
                    TheFindAllEmployeesByLastNameDataSet = TheEmployeeClass.FindAllEmployeesByLastName(strLastName);

                    intNumberOfRecords = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Was Not Found");
                        return;
                    }

                    cboSelectEmployee.Items.Clear();

                    cboSelectEmployee.Items.Add("Select Employee");

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strFullName = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].FirstName + " ";
                        strFullName += TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].LastName;

                        cboSelectEmployee.Items.Add(strFullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Lookup // Employee Last Name Textbox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intEmployeeID;
            int intManagerID;
            string strManagerName;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].EmployeeID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                    txtFirstName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    txtLastName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                    txtPhoneNumber.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PhoneNumber;

                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                    {
                        txtEmail.Text = "";
                    }
                    else
                    {
                        txtEmail.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                    }

                    txtHomeOffice.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice;

                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsDepartmentNull() == true)
                    {
                        txtDepartment.Text = "";
                    }
                    else
                    {
                        txtDepartment.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].Department;
                    }
                    
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsStartDateNull() == true)
                    {
                        txtStartDate.Text = "";
                    }
                    else
                    {
                        txtStartDate.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].StartDate);
                    }
                    

                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].Active == true)
                    {
                        txtActive.Text = "YES";
                    }
                    else
                    {
                        txtActive.Text = "NO";
                    }

                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsManagerIDNull() == false)
                    {
                        intManagerID = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].ManagerID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                        strManagerName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                        strManagerName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                        txtManager.Text = strManagerName;
                        txtManagerEmail.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                        txtManagerOffice.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice;
                        txtManagerPhone.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PhoneNumber;
                    }
                    else
                    {
                        txtManager.Text = "";
                        txtManagerEmail.Text = "";
                        txtManagerOffice.Text = "";
                        txtManagerPhone.Text = "";
                    }

                    
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Lookup // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
