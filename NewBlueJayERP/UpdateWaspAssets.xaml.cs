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
using Microsoft.Win32;

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
        WaspAssetForImportDataSet TheWaspAssetForImportDataSet = new WaspAssetForImportDataSet();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Wasp Asset // Asset Grid Selection " + Ex.Message);

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

        private void expCreateImportSheet_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strAssetDecription;
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                expCreateImportSheet.IsExpanded = false;

                TheWaspAssetForImportDataSet.waspassetforimport.Rows.Clear();

                intNumberOfRecords = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strAssetDecription = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetDescription + " ID: ";
                        strAssetDecription += TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].BJCAssetID;

                        WaspAssetForImportDataSet.waspassetforimportRow NewAssetRow = TheWaspAssetForImportDataSet.waspassetforimport.NewwaspassetforimportRow();

                        NewAssetRow.AssetDescription = strAssetDecription;
                        NewAssetRow.AssetID = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetID;
                        NewAssetRow.AssetType = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetCategory;
                        NewAssetRow.Location = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetLocation;
                        NewAssetRow.Manufacturer = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].Manufacturer;
                        NewAssetRow.Model = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].Model;
                        NewAssetRow.SerialNumber = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].BJCAssetID;
                        NewAssetRow.Site = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetSite;

                        TheWaspAssetForImportDataSet.waspassetforimport.Rows.Add(NewAssetRow);
                    }

                    worksheet = workbook.ActiveSheet;

                    worksheet.Name = "OpenOrders";

                    int cellRowIndex = 1;
                    int cellColumnIndex = 1;
                    intRowNumberOfRecords = TheWaspAssetForImportDataSet.waspassetforimport.Rows.Count;
                    intColumnNumberOfRecords = TheWaspAssetForImportDataSet.waspassetforimport.Columns.Count;

                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWaspAssetForImportDataSet.waspassetforimport.Columns[intColumnCounter].ColumnName;

                        cellColumnIndex++;
                    }

                    cellRowIndex++;
                    cellColumnIndex = 1;

                    //Loop through each row and read value from each column. 
                    for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                    {
                        for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = TheWaspAssetForImportDataSet.waspassetforimport.Rows[intRowCounter][intColumnCounter].ToString();

                            cellColumnIndex++;
                        }
                        cellColumnIndex = 1;
                        cellRowIndex++;
                    }

                    //Getting the location and file name of the excel to save from user. 
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveDialog.FilterIndex = 1;

                    saveDialog.ShowDialog();

                    workbook.SaveAs(saveDialog.FileName);
                    TheMessagesClass.InformationMessage("Export Successful");

                    excel.Quit();
                }

                dgrAssets.ItemsSource = TheWaspAssetForImportDataSet.waspassetforimport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Wasp Assets // Create Import Sheet Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void expAssetReport_Expanded(object sender, RoutedEventArgs e)
        {
            int intNumberOfColumns;
            int intCounter;
            int intNumberOfRows;
            int intNumberOfRecords;
            string strWorkTask;
            int intRemainder;
            bool blnProcessing;
            int intAssetID;
            string strDescription;
            string strBJCAssetID;
            string strCategory;


            try
            {
                expAssetReport.IsExpanded = false;
                PrintDialog pdProductivityReport = new PrintDialog();

                if (pdProductivityReport.ShowDialog().Value == true)
                {
                    double pageWidth = pdProductivityReport.PrintableAreaWidth;
                    double pageHeight = pdProductivityReport.PrintableAreaHeight;

                    FlowDocument fdAcceptLetter = new FlowDocument();
                    intNumberOfRows = 0;

                    fdAcceptLetter.PageHeight = pageHeight;
                    fdAcceptLetter.PageWidth = pageWidth;
                    Paragraph PageOfPage = new Paragraph(new Run("Blue Jay Communications Asset Report"));
                    PageOfPage.FontSize = 30;
                    PageOfPage.TextAlignment = TextAlignment.Center;
                    PageOfPage.FontFamily = new FontFamily("Century Gothic");
                    PageOfPage.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(PageOfPage);
                    Paragraph Title = new Paragraph(new Run("For Asset Location " + MainWindow.gstrAssetLocation));
                    Title.FontSize = 20;
                    Title.FontFamily = new FontFamily("Century Gothic");
                    Title.FontStyle = FontStyles.Normal;
                    Title.FontWeight = FontWeights.UltraBold;
                    Title.TextAlignment = TextAlignment.Center;
                    Title.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(Title);
                    
                   
                    Table CrewTable = new Table();
                    CrewTable.CellSpacing = 5;
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[0].Width = new GridLength(75);
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[1].Width = new GridLength(250);
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[2].Width = new GridLength(100);
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[3].Width = new GridLength(100);
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[4].Width = new GridLength(100);
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[5].Width = new GridLength(100);
                    CrewTable.RowGroups.Add(new TableRowGroup());
                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow = CrewTable.RowGroups[0].Rows[0];
                    FirstCrewRow.Background = Brushes.LightGray;
                    FirstCrewRow.FontSize = 10;
                    FirstCrewRow.FontStyle = FontStyles.Normal;
                    FirstCrewRow.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("Asset ID"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Asset Description"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  BJC Asset ID"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Category"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Initials"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Date"))));
                    intNumberOfRows++;

                    intNumberOfRecords = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            CrewTable.RowGroups[0].Rows.Add(new TableRow());
                            TableRow TaskRow2 = CrewTable.RowGroups[0].Rows[intNumberOfRows];

                            intRemainder = intNumberOfRows % 2;

                            if (intRemainder > 0)
                            {
                                TaskRow2.Background = Brushes.LightGray;
                            }
                            else if (intRemainder == 0)
                            {
                                TaskRow2.Background = Brushes.LightBlue;
                            }

                            TaskRow2.FontSize = 10;
                            TaskRow2.FontStyle = FontStyles.Normal;
                            TaskRow2.FontWeight = FontWeights.UltraBold;
                            TaskRow2.FontFamily = new FontFamily("Century Gothic");

                            intAssetID = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetID;
                            strDescription = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetDescription;
                            strBJCAssetID = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].BJCAssetID;
                            strCategory = TheFindWaspAssetsByLocationDataSet.FindWaspAssetsByLocation[intCounter].AssetCategory;

                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run(Convert.ToString(intAssetID)))));
                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run(strDescription))));
                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run(strBJCAssetID))));
                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run(strCategory))));
                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                            intNumberOfRows++;
                        }
                    }

                    fdAcceptLetter.Blocks.Add(CrewTable);
                    fdAcceptLetter.ColumnWidth = pdProductivityReport.PrintableAreaWidth;
                    fdAcceptLetter.PageHeight = pageHeight;
                    fdAcceptLetter.PageWidth = pageWidth;

                    //pdProductivityReport.UserPageRangeEnabled = true;

                    pdProductivityReport.PrintDocument(((IDocumentPaginatorSource)fdAcceptLetter).DocumentPaginator, "Blue Jay Communications Acceptance");

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Productivity Sheets // Print Sheet Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
