/* Title:           Import Wasp Asset Locations
 * Date:            5-18-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Import Wasp Asset Locations */

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
using AssetDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportWaspAssetLocations.xaml
    /// </summary>
    public partial class ImportWaspAssetLocations : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        AssetClass TheAssetClass = new AssetClass();

        //setting up the data
        FindWaspAssetLocationByLocationDataSet TheFindWaspAssetLocationByLocationDataSet = new FindWaspAssetLocationByLocationDataSet();
        ImportWaspAssetLocationsDataSet TheImportWaspAssetLocationsDataSet = new ImportWaspAssetLocationsDataSet();

        public ImportWaspAssetLocations()
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
            ResetControl();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControl();
        }
        private void ResetControl()
        {
            TheImportWaspAssetLocationsDataSet.waspassetlocations.Rows.Clear();

            dgrAssetLocations.ItemsSource = TheImportWaspAssetLocationsDataSet.waspassetlocations;
        }

        private void expImportExcel_Expanded(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strAssetLocation;
            string strAssetSite;
            string strSiteDescription;
            int intRecordsReturned;

            try
            {
                expImportExcel.IsExpanded = false;
                TheImportWaspAssetLocationsDataSet.waspassetlocations.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    
                    strAssetLocation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    strAssetSite = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strSiteDescription = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();

                    TheFindWaspAssetLocationByLocationDataSet = TheAssetClass.FindWaspAssetLocationByLocation(strAssetLocation);

                    intRecordsReturned = TheFindWaspAssetLocationByLocationDataSet.FindWaspAssetLocationByLocation.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        ImportWaspAssetLocationsDataSet.waspassetlocationsRow NewLocationRow = TheImportWaspAssetLocationsDataSet.waspassetlocations.NewwaspassetlocationsRow();

                        NewLocationRow.AssetLocation = strAssetLocation;
                        NewLocationRow.AssetSite = strAssetSite;
                        NewLocationRow.SiteDescription = strSiteDescription;

                        TheImportWaspAssetLocationsDataSet.waspassetlocations.Rows.Add(NewLocationRow);
                    }
                }

                
                PleaseWait.Close();
                dgrAssetLocations.ItemsSource = TheImportWaspAssetLocationsDataSet.waspassetlocations;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp Asset Locations // Import Excel Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            string strAssetLocation;
            string strAssetSite;
            string strSiteDescription;

            try
            {
                intNumberOfRecords = TheImportWaspAssetLocationsDataSet.waspassetlocations.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strAssetLocation = TheImportWaspAssetLocationsDataSet.waspassetlocations[intCounter].AssetLocation;
                        strAssetSite = TheImportWaspAssetLocationsDataSet.waspassetlocations[intCounter].AssetSite;
                        strSiteDescription = TheImportWaspAssetLocationsDataSet.waspassetlocations[intCounter].SiteDescription;

                        blnFatalError = TheAssetClass.InsertWaspAssetLocation(strAssetLocation, strAssetSite, strSiteDescription);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Import Wasp Asset Locations // Process Import Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
