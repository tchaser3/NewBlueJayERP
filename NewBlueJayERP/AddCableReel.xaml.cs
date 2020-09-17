﻿/* Title:           Add Cable Reel
 * Date:            9-10-20
 * Author:          Terry Holmes
 * 
 * Description:     this is used to create a cable reel */

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
using DataValidationDLL;
using CableInventoryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddCableReel.xaml
    /// </summary>
    public partial class AddCableReel : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CableInventoryClass TheCableInventoryClass = new CableInventoryClass();

        FindPartsWarehousesDataSet TheFindPartsWarehousesDataSet = new FindPartsWarehousesDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindCableReelIDByAssignedCableIDDataSet TheFindCableReelIDByAssignedCableReelIDDataSet = new FindCableReelIDByAssignedCableIDDataSet();
        FindCableReelIDByTransactionDateDataSet TheFindCableReelIDByTransactionDateDataSet = new FindCableReelIDByTransactionDateDataSet();

        public AddCableReel()
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
            int intCounter;
            int intNumberOfRecords;
            string strFullName;

            try
            {
                //clearing text boxes
                txtAssignedCableID.Text = "";
                txtPartDescription.Text = "";
                txtPartNumber.Text = "";
                txtPONumber.Text = "";
                txtProjectID.Text = "";
                txtReelFootage.Text = "";

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Warehouse Employee");

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("WAREHOUSE");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName;
                    cboSelectEmployee.Items.Add(strFullName);
                }

                cboSelectEmployee.SelectedIndex = 0;

                TheFindPartsWarehousesDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehousesDataSet.FindPartsWarehouses.Rows.Count - 1;

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Cable Reel // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

    }
}
