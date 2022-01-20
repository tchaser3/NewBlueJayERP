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
using EmployeePunchedHoursDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AholaClockPunches.xaml
    /// </summary>
    public partial class AholaClockPunches : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up data
        FindAholaClockPUnchesForEmployeeDataSet TheFindAholaClockPunchesForEmployeeDataSet = new FindAholaClockPUnchesForEmployeeDataSet();

        public AholaClockPunches()
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
            DateTime datStartDate;
            DateTime datEndDate;

            try
            {
                datStartDate = TheDateSearchClass.SubtractingDays(MainWindow.gdatPayDate, 6);
                datEndDate = TheDateSearchClass.AddingDays(MainWindow.gdatPayDate, 1);

                TheFindAholaClockPunchesForEmployeeDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForEmployee(MainWindow.gintEmployeeID, datStartDate, datEndDate);

                dgrEmployeeHours.ItemsSource = TheFindAholaClockPunchesForEmployeeDataSet.FindAholaClockPunchesForEmployee;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Ahola Clock Punches // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
