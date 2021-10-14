/* Title:           Cell Data Search
 * Date:            10-5-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to find how much data is being used */

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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using NewEventLogDLL;
using NewEmployeeDLL;
using CellPhoneCallsDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CellDataSearch.xaml
    /// </summary>
    public partial class CellDataSearch : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindTotalCellPhoneDataForDateRangeDataSet TheFindTotalCellPhoneDataForDateRangeDataSet = new FindTotalCellPhoneDataForDateRangeDataSet();
        FindCellPhoneDataForEmployeeDataSet TheFindCellPhoneDataForEmployeesDataSet = new FindCellPhoneDataForEmployeeDataSet();

        //setting up global variables
        int gintSelectedIndex;

        public CellDataSearch()
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
            txtEndDate.Text = "";
            txtEnterLastName.Text = "";
            txtStartDate.Text = "";

            cboReportType.Items.Clear();
            cboReportType.Items.Add("Select Report Type");
            cboReportType.Items.Add("All Data");
            cboReportType.Items.Add("Employee Data Report");
            cboReportType.SelectedIndex = 0;

            SetTotalDataControls();
        }
        private void SetTotalDataControls()
        {
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblEnterLastName.Visibility = Visibility.Hidden;
            txtEnterLastName.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            lblReportType.Margin = new Thickness(250, 1, 1, 1);

        }
        private void SetEmployeeDataControls()
        {
            cboSelectEmployee.Visibility = Visibility.Visible;
            lblEnterLastName.Visibility = Visibility.Visible;
            txtEnterLastName.Visibility = Visibility.Visible;
            lblSelectEmployee.Visibility = Visibility.Visible;
            lblReportType.Margin = new Thickness(0, 0, 0, 0);

            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gintSelectedIndex = cboReportType.SelectedIndex;

            if(gintSelectedIndex == 1)
            {
                SetTotalDataControls();
                btnFind.IsEnabled = true;
            }
            else if(gintSelectedIndex == 2)
            {
                SetEmployeeDataControls();
                btnFind.IsEnabled = true;
            }
            else
            {
                SetTotalDataControls();
                btnFind.IsEnabled = false;
            }
        }
    }
}
