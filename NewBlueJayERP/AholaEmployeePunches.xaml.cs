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
using DataValidationDLL;
using NewEventLogDLL;
using DateSearchDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AholaEmployeePunches.xaml
    /// </summary>
    public partial class AholaEmployeePunches : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeePunchedHoursClass TheEmployeePUnchedHoursClass = new EmployeePunchedHoursClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        FindAholaEmployeePunchHoursDataSet TheFindAholaEmployeePunchHoursDataSet = new FindAholaEmployeePunchHoursDataSet();

        public AholaEmployeePunches()
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
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                datStartDate = TheDateSearchClass.SubtractingDays(MainWindow.gdatPayDate, 6);
                datEndDate = TheDateSearchClass.AddingDays(MainWindow.gdatPayDate, 1);

                TheFindAholaEmployeePunchHoursDataSet = TheEmployeePUnchedHoursClass.FindAholaEmployeePunchHours(MainWindow.gintEmployeeID, datStartDate, datEndDate);

                dgrEmployeeHours.ItemsSource = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Ahola Employee Punches // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
