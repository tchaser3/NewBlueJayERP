/* Title:               Edit Selected Vehicle Problems
 * Date:                9-9-2020
 * Author:              Terry Holmes
 * 
 * Description:         This used to edit the selected Transaction */

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
using VehicleProblemDocumentationDLL;
using VehicleProblemsDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditSelectedVehicleProblem.xaml
    /// </summary>
    public partial class EditSelectedVehicleProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleProblemDocumentClass TheVehicleProblemDocumentClass = new VehicleProblemDocumentClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        FindVehicleMainProblemUpdateByProblemIDDataSet TheFindVehicleMainProblemUpdateByProblemIDDataSet = new FindVehicleMainProblemUpdateByProblemIDDataSet();
        FindVehicleProblemDocumentationByProblemIDDataSet TheFindVenicleProblemDocumentationByProblemIDDataSet = new FindVehicleProblemDocumentationByProblemIDDataSet();
        FindVehicleInvoiceByInvoiceIDDataSet TheFindVehicleInvoiceByInvoiceIDDataSet = new FindVehicleInvoiceByInvoiceIDDataSet();
        
        public EditSelectedVehicleProblem()
        {
            InitializeComponent();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //local variables
            int intInvoiceID;

            try
            {
                TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(MainWindow.gintProblemID);

                txtProblemID.Text = Convert.ToString(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemID);
                txtTransactionDate.Text = Convert.ToString(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].TransactionDAte);
                txtProblem.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;
                txtProblemStatus.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemStatus;

                if (TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].IsProblemResolutionNull() == false)
                {
                    txtProblemResolution.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemResolution;
                }

                if(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].IsInvoiceIDNull() == false)
                {
                    intInvoiceID = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].InvoiceID;

                    if(intInvoiceID > 999)
                    {

                        TheFindVehicleInvoiceByInvoiceIDDataSet = TheVehicleProblemDocumentClass.FindVehicleInvoiceByInvoiceID(intInvoiceID);

                        txtInvoiceAmount.Text = Convert.ToString(TheFindVehicleInvoiceByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID[0].InvoiceAmount);
                        txtInvoicePath.Text = TheFindVehicleInvoiceByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID[0].InvoicePath;
                    }

                }

                TheFindVehicleMainProblemUpdateByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemUpdateByProblemID(MainWindow.gintProblemID);

                dgrProblemUpdates.ItemsSource = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID;

                TheFindVenicleProblemDocumentationByProblemIDDataSet = TheVehicleProblemDocumentClass.FindVehicleProblemDocumentationByProblemID(MainWindow.gintProblemID);

                dgrProblemDocumentation.ItemsSource = TheFindVenicleProblemDocumentationByProblemIDDataSet.FindVehicleProblemDocumentationByProblemID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Selected Vehicle Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
