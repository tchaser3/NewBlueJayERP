/* Title:           Edit Vehicle Problems
 * Date:            9-8-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for locating the problem */

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
using VehicleProblemsDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditVehicleProblems.xaml
    /// </summary>
    public partial class EditVehicleProblems : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindAllVehicleMainProblemsByVehicleIDDataSet TheFindAllVehicleMainProblemsByVehicleIDDataSet = new FindAllVehicleMainProblemsByVehicleIDDataSet();

        public EditVehicleProblems()
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

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strVehicleNumber;
            int intRecordsReturned;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Not Found");
                    return;
                }

                MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                TheFindAllVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindAllVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                dgrVehicleProblems.ItemsSource = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Vehicle Problems // Find Button " + Ex.Message);
            }
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void dgrVehicleProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                if (dgrVehicleProblems.SelectedIndex > -1)
                {

                    //setting local variable
                    dataGrid = dgrVehicleProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    EditSelectedVehicleProblem EditSelectedVehicleProblem = new EditSelectedVehicleProblem();
                    EditSelectedVehicleProblem.ShowDialog();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Vehicle Problems // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

            TheFindAllVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindAllVehicleMainProblemsByVehicleID(-200);

            dgrVehicleProblems.ItemsSource = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID;
        }
    }
}
