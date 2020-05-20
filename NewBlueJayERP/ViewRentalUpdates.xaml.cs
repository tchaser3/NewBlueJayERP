/* Title:           View Rental Updates
 * Date:            5-9-20
 * Author:          Terry Holmes
 * 
 * Description:     This used to view updates and documentation */

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
using RentalTrackingDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewRentalUpdates.xaml
    /// </summary>
    public partial class ViewRentalUpdates : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        RentalTrackingClass TheRentalTrackingClass = new RentalTrackingClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindRentalTrackingUpdateByRentalTrackingIDDataSet TheFindRentalTrackingUpdateByRentalTrackingIDDataSet = new FindRentalTrackingUpdateByRentalTrackingIDDataSet();
        FindRentalTrackingDocumentationByRentalTrackingIDDataSet TheFindRentalTrackingDocumentationByRentalTrackingIDDataSet = new FindRentalTrackingDocumentationByRentalTrackingIDDataSet();

        public ViewRentalUpdates()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindRentalTrackingUpdateByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingUpdateByRentalTrackingID(MainWindow.gintRentalTrackingID);

            dgrRentalUpdates.ItemsSource = TheFindRentalTrackingUpdateByRentalTrackingIDDataSet.FindRentalTrackingUpdateByRentalTrackingID;

            TheFindRentalTrackingDocumentationByRentalTrackingIDDataSet = TheRentalTrackingClass.FindRentalTrackingDocumentationByRentalTrackingID(MainWindow.gintRentalTrackingID);

            dgrRentalDocumentation.ItemsSource = TheFindRentalTrackingDocumentationByRentalTrackingIDDataSet.FindRentalTrackingDocumentationByRentalTackingID;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void dgrRentalDocumentation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrRentalDocumentation.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrRentalDocumentation;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[5].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View Rental Update // Rental Documentation Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
