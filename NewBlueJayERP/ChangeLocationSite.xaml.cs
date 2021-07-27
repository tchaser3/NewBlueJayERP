/* Title:           Change Location Site
 * Date:            6-24-21
 * Author:          Terry Holmes
 * 
 * Description:     This is how we can change a location site */


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
using AssetDLL;
using NewEmployeeDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ChangeLocationSite.xaml
    /// </summary>
    public partial class ChangeLocationSite : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AssetClass TheAssetClass = new AssetClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindWaspAssetLocationBySiteDataSet TheFindWaspAssetLocationBySiteDataSet = new FindWaspAssetLocationBySiteDataSet();
        FindWaspAssetsByLocationDataSet TheFindWaspAssetsByLocationDataSet = new FindWaspAssetsByLocationDataSet();

        string gstrLocation;
        string gstrNewSite;
        int gintTransactionID;

        public ChangeLocationSite()
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Items.Add("Select Location");
                cboSelectLocation.SelectedIndex = 0;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                cboSelectOldSite.Items.Clear();
                cboSelectOldSite.Items.Add("Select Old Site");

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOldSite.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOldSite.SelectedIndex = 0;

                cboSelectNewSite.Items.Clear();
                cboSelectNewSite.Items.Add("Select New Site");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectNewSite.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectNewSite.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Location Site // Reset Controls " + Ex.Message);

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
                    gintTransactionID = TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite[intSelectedIndex].TransactionID;
                    gstrLocation = TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite[intSelectedIndex].AssetLocation;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Location Site // Select Location Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOldSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strSite;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectOldSite.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    strSite = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;

                    if(strSite == "CBUS-GROVEPORT")
                    {
                        strSite = "GROVEPORT";
                    }

                    TheFindWaspAssetLocationBySiteDataSet = TheAssetClass.FindWaspAssetLocationBySite(strSite);

                    intNumberOfRecords = TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite.Rows.Count;
                    cboSelectLocation.Items.Clear();
                    cboSelectLocation.Items.Add("Select Location");

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectLocation.Items.Add(TheFindWaspAssetLocationBySiteDataSet.FindWaspAssetLocationBySite[intCounter].AssetLocation);
                        }
                    }

                    cboSelectLocation.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Location Site // Select Old Site " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectNewSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectNewSite.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gstrNewSite = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;

                    if(gstrNewSite == "CBUS-GROVEPORT")
                    {
                        gstrNewSite = "GROVEPORT";
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Location Site // Select New Site Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;
            int intAssetID;

            try
            {
                if(cboSelectOldSite.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Old Site Was Not Selected\n";
                }
                if(cboSelectLocation.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Location Was Not Selected\n";
                }
                if(cboSelectNewSite.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The New Site Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheAssetClass.UpdateWaspLocationSite(gintTransactionID, gstrNewSite);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindWaspAssetsByLocationDataSet = TheAssetClass.FindWaspAssetsByLocation(gstrLocation);

                intNumberOfRecords = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intAssetID = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetID;

                        blnFatalError = TheAssetClass.UpdateWaspAssetLocation(intAssetID, gstrNewSite, gstrLocation);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Location Site Has Been Changed");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Change Location Site // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
