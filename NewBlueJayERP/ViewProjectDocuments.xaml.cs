/* Title:           View Project Documents
 * Date:            4-12-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show the project documents */

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
using ProductionProjectDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewProjectDocuments.xaml
    /// </summary>
    public partial class ViewProjectDocuments : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data sets
        FindProductionProjectDocumentationByProjectIDDataSet TheFindProductionProjectDocumentationByProjectIDDataSet = new FindProductionProjectDocumentationByProjectIDDataSet();
        FindProductionProjectQCByProjectIDDataSet TheFindProductionProjectQCByProjectIDDataSet = new FindProductionProjectQCByProjectIDDataSet();

        public ViewProjectDocuments()
        {
            InitializeComponent();
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
            TheFindProductionProjectDocumentationByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectDocumentationByProjectID(MainWindow.gintProjectID);

            dgrProjectDocumentation.ItemsSource = TheFindProductionProjectDocumentationByProjectIDDataSet.FindProductionProjectDocumentationByProjectID;

            TheFindProductionProjectQCByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectQCByProjectID(MainWindow.gintProjectID);

            dgrProjectQC.ItemsSource = TheFindProductionProjectQCByProjectIDDataSet.FindProductionProjectQCByProjectID;
        }

        private void dgrProjectDocumentation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrProjectDocumentation.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrProjectDocumentation;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);                    
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View Project Documentation // Project Documentation Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrProjectQC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrProjectQC.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrProjectQC;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View Project Documentation // Project QC Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
