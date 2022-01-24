/* Title:           Vehicles In Yard
 * Date:            1-24-22
 * Author:          Terry Holmes
 * 
 * Description:     This is used to perform Vehicles In The Yard */

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
using VehicleInYardDLL;
using VehicleMainDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VehiclesInYard.xaml
    /// </summary>
    public partial class VehiclesInYard : Window
    {
        //setting up the classes
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        public VehiclesInYard()
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
            txtEnterVehicleNumber.Text = "";

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Vehicles In Yard");
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            DateTime datTransactionDate = DateTime.Now;
            string strVehicleNumber = "";
            bool blnFatalError = false;
            int intRecordsReturned;
            int intVehicleID;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;

                if(strVehicleNumber.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Number is not Long Enough");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Not Found");
                    return;
                }

                intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                blnFatalError = TheVehicleInYardClass.InsertVehicleInYard(datTransactionDate, intVehicleID);

                if (blnFatalError == true)
                    throw new Exception();

                txtEnterVehicleNumber.Text = "";

                txtEnterVehicleNumber.Focus();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Vehicles In Yard // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
