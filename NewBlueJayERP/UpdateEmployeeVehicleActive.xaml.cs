/* Title:           Update Employee Vehicle Active
 * Date:            2-2-21
 * Author:          Terry Holmes
 * 
 * Description:     This used to update the vehicle active */

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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateEmployeeVehicleActive.xaml
    /// </summary>
    public partial class UpdateEmployeeVehicleActive : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        FindVehicleEmployeeActiveNotMatchDataSet TheFindVehicleEmployeeActiveNotMatchDataSet = new FindVehicleEmployeeActiveNotMatchDataSet();

        public UpdateEmployeeVehicleActive()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessagesClass.LaunchEmail();
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
            expProcess.IsEnabled = true;

            TheFindVehicleEmployeeActiveNotMatchDataSet = TheEmployeeClass.FindVehicleEmployeeActiveNoMatch();

            dgrVehicles.ItemsSource = TheFindVehicleEmployeeActiveNotMatchDataSet.FindVehicleEmployeeActiveNotMatch;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Update Employee Vehicle Active");

            if(TheFindVehicleEmployeeActiveNotMatchDataSet.FindVehicleEmployeeActiveNotMatch.Rows.Count < 1)
            {
                expProcess.IsEnabled = false;
            }                   
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intEmployeeID;            

            try
            {
                intNumberOfRecords = TheFindVehicleEmployeeActiveNotMatchDataSet.FindVehicleEmployeeActiveNotMatch.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheFindVehicleEmployeeActiveNotMatchDataSet.FindVehicleEmployeeActiveNotMatch[intCounter].EmployeeID;

                    if(TheFindVehicleEmployeeActiveNotMatchDataSet.FindVehicleEmployeeActiveNotMatch[intCounter].Active1 == false)
                    {
                        blnFatalError = TheEmployeeClass.DeactivateEmployee(intEmployeeID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("All Employees are Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Employee Vehicle Active // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
