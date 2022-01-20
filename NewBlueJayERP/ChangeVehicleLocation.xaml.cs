/* Title:           Change Vehicle Location
 * Date:            1-14-2022
 * Author:          Terry Holmes
 * 
 * Description:     This is used to change the vehicle location */

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
using VehicleMainDLL;
using NewEventLogDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ChangeVehicleLocation.xaml
    /// </summary>
    public partial class ChangeVehicleLocation : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        public ChangeVehicleLocation()
        {
            InitializeComponent();
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

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
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

            try
            {
                txtEnterVehicleNumber.Text = "";
                txtmake.Text = "";
                txtModel.Text = "";
                txtNotes.Text = "";
                txtVinNumber.Text = "";
                txtYear.Text = "";

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Vehicle Location // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnFindVehicle_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;
            string strVehicleNumber;
            int intCounter;
            int intNumberOfRecords;
            string strWarehouse;
            int intSelectedIndex = 0;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;

                if(strVehicleNumber.Length < 2)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Number is not Correct");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Was Not Found");
                    return;
                }

                strWarehouse = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].AssignedOffice;

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if(strWarehouse == TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectWarehouse.SelectedIndex = intSelectedIndex;

                txtmake.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleMake;
                txtModel.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleModel;
                txtNotes.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].Notes;
                txtVinNumber.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VINNumber;
                txtYear.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleYear);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Vehicle Location // Find Vehicle Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
