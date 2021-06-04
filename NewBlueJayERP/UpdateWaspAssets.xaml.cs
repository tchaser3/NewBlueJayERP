/* Title:           Update Wasp Assets
 * Date:            6-2-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to set up the Wasp Inventory */

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
using NewEmployeeDLL;
using AssetDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateWaspAssets.xaml
    /// </summary>
    public partial class UpdateWaspAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssetClass TheAssetClass = new AssetClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedWaspAssetLocationsBySiteDataSet TheFindSortedWaspAssetLocationsBySiteDataSet = new FindSortedWaspAssetLocationsBySiteDataSet();
        FindWaspAssetsByLocationDataSet TheFindWaspAssetsByLocationDataSet = new FindWaspAssetsByLocationDataSet();

        public UpdateWaspAssets()
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
            //this will load the controls
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");
                expAddAsset.IsEnabled = false;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;
                cboSelectSite.Items.Clear();
                cboSelectSite.Items.Add("Select Site");

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectSite.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectSite.SelectedIndex = 0;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Update Wasp Assets");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Wasp Update // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //stting up for getting the info
            int intCounter;
            int intNumberOfRecords;
            string strSite;
            int intSelectedIndex;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");

                intSelectedIndex = cboSelectSite.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    strSite = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;

                    if(strSite == "CBUS-GROVEPORT")
                    {
                        strSite = "GROVEPORT"; 
                    }

                    TheFindSortedWaspAssetLocationsBySiteDataSet = TheAssetClass.FindSortedAssetLocationsBySite(strSite);

                    intNumberOfRecords = TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectLocation.Items.Add(TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intCounter].AssetLocation);
                        }
                    }
                }

                cboSelectLocation.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Wasp Asset // CBO Site Select " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectLocation.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gstrAssetLocation = TheFindSortedWaspAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intSelectedIndex].AssetLocation;

                    TheFindWaspAssetsByLocationDataSet = TheAssetClass.FindWaspAssetsByLocation(MainWindow.gstrAssetLocation);

                    dgrAssets.ItemsSource = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation;

                    expAddAsset.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Wasp Assets // Select Location Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrAssets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell AssetID;
            string strAssetID;

            try
            {
                if (dgrAssets.SelectedIndex > -1)
                {

                    //setting local variable
                    dataGrid = dgrAssets;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    AssetID = (DataGridCell)dataGrid.Columns[1].GetCellContent(selectedRow).Parent;
                    strAssetID = ((TextBlock)AssetID.Content).Text;

                    //find the record
                    MainWindow.gintAssetID = Convert.ToInt32(strAssetID);

                    EditWaspAsset EditWaspAsset = new EditWaspAsset();
                    EditWaspAsset.ShowDialog();

                    TheFindWaspAssetsByLocationDataSet = TheAssetClass.FindWaspAssetsByLocation(MainWindow.gstrAssetLocation);

                    dgrAssets.ItemsSource = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Vehicle Problems // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expAddAsset_Expanded(object sender, RoutedEventArgs e)
        {
            expAddAsset.IsExpanded = false;

            AddWaspAsset AddWaspAsset = new AddWaspAsset();
            AddWaspAsset.ShowDialog();

            TheFindWaspAssetsByLocationDataSet = TheAssetClass.FindWaspAssetsByLocation(MainWindow.gstrAssetLocation);

            dgrAssets.ItemsSource = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation;
        }
    }
}
