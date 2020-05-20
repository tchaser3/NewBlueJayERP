/* Title:           Select Rental
 * Date:            5-6-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to for selecting the rental */

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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for SelectRental.xaml
    /// </summary>
    public partial class SelectRental : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        public SelectRental()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.gblnRentalPO == true)
                dgrRentals.ItemsSource = MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet.FindRentalTrackingTransactionByPONumber;
            else if (MainWindow.gblnRentalPO == false)
                dgrRentals.ItemsSource = MainWindow.TheFindRentalTransactionByProjectIDDataSet.FindRentalTransasctionByProjectID;
        }

        private void dgrRentals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrRentals.SelectedIndex;

                if (dgrRentals.SelectedIndex > -1)
                {
                    if (MainWindow.gblnRentalPO == true)
                    {
                        MainWindow.gintRentalTrackingID = MainWindow.TheFindRentalTrackingTransactionsByPONumberDataSet.FindRentalTrackingTransactionByPONumber[intSelectedIndex].TransactionID;

                        this.Close();
                    }
                    else if (MainWindow.gblnRentalPO == false)
                    {
                        MainWindow.gintRentalTrackingID = MainWindow.TheFindRentalTransactionByProjectIDDataSet.FindRentalTransasctionByProjectID[intSelectedIndex].TransactionID;

                        this.Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Select Rental // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
