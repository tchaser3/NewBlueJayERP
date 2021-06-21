/* Title:           Change Wasp Asset Location
 * Date:            6-18-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to change the location of a Wasp Asset Location */

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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ChangeWaspAssetLocation.xaml
    /// </summary>
    public partial class ChangeWaspAssetLocation : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssetClass TheAssetClass = new AssetClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedWaspAssetLocationsBySiteDataSet TheFindSortedAssetLocationsBySiteDataSet = new FindSortedWaspAssetLocationsBySiteDataSet();
        FindWaspAssetByAssetIDDataSet TheFindWaspAssetByAssetIDDataSet = new FindWaspAssetByAssetIDDataSet();

        string gstrLocation;
        string gstrSite;

        public ChangeWaspAssetLocation()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;
            string strSite;
            string strLocation;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");
                cboSelectLocation.SelectedIndex = 0;

                cboSelectSite.Items.Clear();
                cboSelectSite.Items.Add("Select Site");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectSite.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectSite.SelectedIndex = 0;

                TheFindWaspAssetByAssetIDDataSet = TheAssetClass.FindWaspAssetByAssetID(MainWindow.gintAssetID);

                txtAssetID.Text = Convert.ToString(MainWindow.gintAssetID);

                strSite = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].AssetSite;
                gstrSite = strSite;

                if(strSite == "GROVEPORT")
                {
                    strSite = "CBUS-GROVEPORT";
                }

                strLocation = TheFindWaspAssetByAssetIDDataSet.FindWaspAssetByAssetID[0].AssetLocation;

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if(strSite == TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectSite.SelectedIndex = intSelectedIndex;

                TheFindSortedAssetLocationsBySiteDataSet = TheAssetClass.FindSortedAssetLocationsBySite(gstrSite);

                intNumberOfRecords = TheFindSortedAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite.Rows.Count;
                intSelectedIndex = 0;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(strLocation == TheFindSortedAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intCounter].AssetLocation)
                        {
                            intSelectedIndex = intCounter + 1;
                        }
                    }
                }

                cboSelectLocation.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Wasp Location // Window Loaded Event " + Ex.Message);

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
                    gstrLocation = cboSelectLocation.SelectedItem.ToString();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Wasp Asset Location // Location Combobox Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboSelectSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectSite.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gstrSite = cboSelectSite.SelectedItem.ToString();
                    cboSelectLocation.Items.Clear();
                    cboSelectLocation.Items.Add("Select Location");

                    if(gstrSite == "CBUS-GROVEPORT")
                    {
                        gstrSite = "GROVEPORT";
                    }

                    TheFindSortedAssetLocationsBySiteDataSet = TheAssetClass.FindSortedAssetLocationsBySite(gstrSite);

                    intNumberOfRecords = TheFindSortedAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectLocation.Items.Add(TheFindSortedAssetLocationsBySiteDataSet.FindSortedWaspAssetLoctionsBySite[intCounter].AssetLocation);
                        }
                    }

                    cboSelectLocation.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Was Asset Location // Select Site Combobox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheAssetClass.UpdateWaspAssetLocation(MainWindow.gintAssetID, gstrSite, gstrLocation);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Asset Location has been Changed");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Wasp Asset Location // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
