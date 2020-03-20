/* Title:           Enter Inventory
 * Date:            3-2-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to enter inventory */

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
using InventoryWIPDLL;
using InventoryDLL;
using NewPartNumbersDLL;
using KeyWordDLL;
using ProjectsDLL;


namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EnterInventory.xaml
    /// </summary>
    public partial class EnterInventory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryWIPClass TheInventoryWIPClass = new InventoryWIPClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        PartNumberClass ThePartNumbersClass = new PartNumberClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        ProjectClass TheProjectsClass = new ProjectClass();

        public EnterInventory()
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

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessagesClass.LaunchEmail();
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
            //setting up the default configuration
            CheckForOpenSession();

            if (MainWindow.gblnIssueMaterial == true)
                lblTitle.Content = "Enter Issued Material";
            else if (MainWindow.gblnProcessBOM == true)
                lblTitle.Content = "Enter BOM Information";
            else if (MainWindow.gblnReceiveMaterial == true)
                lblTitle.Content = "Enter Material Received";

            lblCurrentWarehouse.Content = "This is the " + MainWindow.gstrWarehouseName + " " + "Warehouse";

        }
        private void CheckForOpenSession()
        {
            //this will check for open sessions or create open sessions
            //creating local variables
            int intEmployeeID;
            int intRecordsReturned;

            try
            {
                //getting the employee id
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                //checking for open sessions
                MainWindow.TheFindSessionByEmployeeIDDataSet = TheInventoryWIPClass.FindSessionByEmployeeID(intEmployeeID);

                //getting the record count
                intRecordsReturned = MainWindow.TheFindSessionByEmployeeIDDataSet.FindSessionByEmployeeID.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.InformationMessage("No Existing WIP Transactions Found, This is a new Session");

                    TheInventoryWIPClass.InsertNewSession(intEmployeeID);

                    MainWindow.TheFindSessionByEmployeeIDDataSet = TheInventoryWIPClass.FindSessionByEmployeeID(intEmployeeID);

                    MainWindow.gintSessionID = MainWindow.TheFindSessionByEmployeeIDDataSet.FindSessionByEmployeeID[0].SessionID;
                }
                else if (intRecordsReturned == 1)
                {
                    MainWindow.gintSessionID = MainWindow.TheFindSessionByEmployeeIDDataSet.FindSessionByEmployeeID[0].SessionID;

                    MainWindow.TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(MainWindow.gintSessionID);

                    dgrCurrentSession.ItemsSource = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID;

                    TheMessagesClass.InformationMessage("You Will Be Working in a Continued Session");
                }
                else
                {
                    TheMessagesClass.ErrorMessage("There is a Problem with your Session, Please Contact IT");
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Enter Inventory // Check For Open Session " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
