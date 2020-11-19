/* Title:           Add Department
 * Date:            11-19-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Add a Department */

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
using DepartmentDLL;
using EmployeeDateEntryDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddDepartment.xaml
    /// </summary>
    public partial class AddDepartment : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindDepartmentByNameDataSet TheFindDepartmentByNameDataSet = new FindDepartmentByNameDataSet();

        //setting up global variables
        bool gblnCustomerLine;

        public AddDepartment()
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
            txtDepartment.Text = "";
            rdoNo.IsChecked = true;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string strDepartment;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                strDepartment = txtDepartment.Text;
                if(strDepartment.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Department is not Long Enough");
                    return;
                }

                TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(strDepartment);

                intRecordsReturned = TheFindDepartmentByNameDataSet.FindDepartmentByName.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Department Currently Exists");
                    return;
                }

                blnFatalError = TheDepartmentClass.InsertDepartment(strDepartment, gblnCustomerLine);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Department has been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Department // Submit Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            gblnCustomerLine = true;
        }

        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnCustomerLine = false;
        }
    }
}
