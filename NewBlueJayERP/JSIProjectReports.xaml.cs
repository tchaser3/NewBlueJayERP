/* Title:           JSI Project Report
 * Date:            11-25-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used for the JSI Reports */

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
using ProjectMatrixDLL;
using JSIMainDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for JSIProjectReports.xaml
    /// </summary>
    public partial class JSIProjectReports : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        JSIMainClass TheJSIMainClass = new JSIMainClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindJSIMainByAssignedProjectIDDataSet TheFindJSIMainByAssignedProjectIDDataSet = new FindJSIMainByAssignedProjectIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        JSIInspectionsDataSet TheJSIInspectionDataSet = new JSIInspectionsDataSet();

        public JSIProjectReports()
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
            txtAssignedProjectID.Text = "";
            txtBusinessLine.Text = "";
            txtCustomerProjectID.Text = "";
            txtEnterProjectID.Text = "";
            txtManager.Text = "";
            txtProjectName.Text = "";

            TheJSIInspectionDataSet.jsiinspections.Rows.Clear();

            dgrInspections.ItemsSource = TheJSIInspectionDataSet.jsiinspections;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // JSI Project Reports ");
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;
            string strProjectID = "";
            string strAssignedProjectID = "";
            string strProjectName = "";
            string strBusinessLine = "";
            int intManagerID;
            int intInspectionEmployeeID;
            string strFirstName;
            string strLastName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strProjectID = txtEnterProjectID.Text;

                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {

                }
                else if(intRecordsReturned > 0)
                {
                    strAssignedProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].AssignedProjectID;
                    //strProjectName = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].n
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI Project Reports // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
