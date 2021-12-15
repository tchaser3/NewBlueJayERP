/* Title:           Edit Employee Hours
 * Date:            12-14-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for editing the hours of an employee */

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
using EmployeeDateEntryDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditEmployeeHours.xaml
    /// </summary>
    public partial class EditEmployeeHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeePunchedHoursClass TheEmployeePunchedHourClass = new EmployeePunchedHoursClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindEmployeePunchedHoursForEditingDataSet TheFindEmployeePunchedHoursForEditingDataSet = new FindEmployeePunchedHoursForEditingDataSet();

        public EditEmployeeHours()
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

            TheFindEmployeePunchedHoursForEditingDataSet = TheEmployeePunchedHourClass.FindEmployeePunchedHoursForEditing(DateTime.Now);

            dgrHours.ItemsSource = TheFindEmployeePunchedHoursForEditingDataSet.FindEmployeePunchedHoursForEditing;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Employee Hours");
        }

        private void btnFindHours_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;

            try
            {
                strValueForValidation = txtEnterPayPeriod.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date entered is not a Date");
                    return;
                }

                MainWindow.gdatPayDate = Convert.ToDateTime(strValueForValidation);

                TheFindEmployeePunchedHoursForEditingDataSet = TheEmployeePunchedHourClass.FindEmployeePunchedHoursForEditing(MainWindow.gdatPayDate);

                dgrHours.ItemsSource = TheFindEmployeePunchedHoursForEditingDataSet.FindEmployeePunchedHoursForEditing;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Employee Hours // Find Hours Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrHours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell EmployeeID;
            string strEmployeeID;

            try
            {
                if (dgrHours.SelectedIndex > -1)
                {

                    //setting local variable
                    dataGrid = dgrHours;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    EmployeeID = (DataGridCell)dataGrid.Columns[1].GetCellContent(selectedRow).Parent;
                    strEmployeeID = ((TextBlock)EmployeeID.Content).Text;

                    //find the record
                    MainWindow.gintEmployeeID = Convert.ToInt32(strEmployeeID);

                    AholaEmployeePunches AholaEmployeePunches = new AholaEmployeePunches();
                    AholaEmployeePunches.Show();


                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Vehicle Problems // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
