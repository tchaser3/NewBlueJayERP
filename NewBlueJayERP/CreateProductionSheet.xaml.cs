/* Title:           Create Production Sheets
 * Date:            2-25-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for creating the production sheets */

using DepartmentDLL;
using EmployeeDateEntryDLL;
using NewEventLogDLL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WorkTaskDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateProductionSheet.xaml
    /// </summary>
    public partial class CreateProductionSheet : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindWorkTaskDepartmentByLOBDepartmentDataSet TheFindWorkTaskDepartmentByLOBDepartmentDataSet = new FindWorkTaskDepartmentByLOBDepartmentDataSet();
        FindDepartmentByNameDataSet TheFindDepartmentByNameDataSet = new FindDepartmentByNameDataSet();
        ProductionTasksForSheetsDataSet TheProductionTasksForSheetsDataSet = new ProductionTasksForSheetsDataSet();

        int gintBusinessLineID;
        int gintDepartmentID;
        string gstrBusinessLine;
        string gstrDepartment;

        public CreateProductionSheet()
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
                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");

                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.SelectedIndex = 0;

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");
                cboSelectDepartment.Items.Add("Aerial");
                cboSelectDepartment.Items.Add("Underground");

                cboSelectDepartment.SelectedIndex = 0;

                TheProductionTasksForSheetsDataSet.productiontasks.Rows.Clear();

                dgrProductionCodes.ItemsSource = TheProductionTasksForSheetsDataSet.productiontasks;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Production Sheet // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectBusinessLine.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;
                gstrBusinessLine = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].Department;

                if(cboSelectDepartment.SelectedIndex > 0)
                {
                    UpdateGrid();
                }
            }
        }
        private void UpdateGrid()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheProductionTasksForSheetsDataSet.productiontasks.Rows.Clear();

                TheFindWorkTaskDepartmentByLOBDepartmentDataSet = TheWorkTaskClass.FindWorkTaskDepartmentByLOBDepartment(gintBusinessLineID, gintDepartmentID);

                intNumberOfRecords = TheFindWorkTaskDepartmentByLOBDepartmentDataSet.FindWorkTaskDepartmentByLOBDepartment.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        ProductionTasksForSheetsDataSet.productiontasksRow NewProductionCode = TheProductionTasksForSheetsDataSet.productiontasks.NewproductiontasksRow();

                        NewProductionCode.WorkTask = TheFindWorkTaskDepartmentByLOBDepartmentDataSet.FindWorkTaskDepartmentByLOBDepartment[intCounter].WorkTask;
                        NewProductionCode.UseCode = true;

                        TheProductionTasksForSheetsDataSet.productiontasks.Rows.Add(NewProductionCode);
                    }
                }

                dgrProductionCodes.ItemsSource = TheProductionTasksForSheetsDataSet.productiontasks;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Production Sheet // Update Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intRecordsReturned;

            try
            {
                if(cboSelectDepartment.SelectedIndex > 0)
                {
                    gstrDepartment = cboSelectDepartment.SelectedItem.ToString().ToUpper();

                    TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(gstrDepartment);

                    intRecordsReturned = TheFindDepartmentByNameDataSet.FindDepartmentByName.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        gintDepartmentID = TheFindDepartmentByNameDataSet.FindDepartmentByName[0].DepartmentID;

                        if(cboSelectBusinessLine.SelectedIndex > 0)
                        {
                            UpdateGrid();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create Production Sheets // Select Department Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expPrintSheet_Expanded(object sender, RoutedEventArgs e)
        {
            int intNumberOfColumns;
            int intCounter;
            int intNumberOfRows;
            int intNumberOfRecords;
            string strWorkTask;
            int intRemainder;
            bool blnProcessing;

            try
            {
                PrintDialog pdProductivityReport = new PrintDialog();
               
                if (pdProductivityReport.ShowDialog().Value == true)
                {
                    double pageWidth = pdProductivityReport.PrintableAreaWidth;
                    double pageHeight = pdProductivityReport.PrintableAreaHeight;

                    FlowDocument fdAcceptLetter = new FlowDocument();
                    

                    fdAcceptLetter.PageHeight = pageHeight;
                    fdAcceptLetter.PageWidth = pageWidth;
                    Paragraph PageOfPage = new Paragraph(new Run("_____OF_____"));
                    PageOfPage.FontSize = 10;
                    PageOfPage.TextAlignment = TextAlignment.Right;
                    PageOfPage.FontFamily = new FontFamily("Century Gothic");
                    PageOfPage.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(PageOfPage);
                    Paragraph Title = new Paragraph(new Run(gstrBusinessLine + " " + gstrDepartment + " DAILY PRODUCTION SHEET"));
                    Title.FontSize = 20;
                    Title.FontFamily = new FontFamily("Century Gothic");
                    Title.FontStyle = FontStyles.Normal;
                    Title.FontWeight = FontWeights.UltraBold;
                    Title.TextAlignment = TextAlignment.Center;
                    Title.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(Title);
                    Paragraph ReportDate = new Paragraph(new Run("DATE__________"));
                    ReportDate.FontSize = 10;
                    ReportDate.TextAlignment = TextAlignment.Right;
                    ReportDate.FontFamily = new FontFamily("Century Gothic");
                    ReportDate.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(ReportDate);

                    Table ProjectTable = new Table();
                    TableColumn Testing = new TableColumn();
                    ProjectTable.CellSpacing = 5;                                        
                    ProjectTable.RowGroups.Add(new TableRowGroup());
                    ProjectTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstRow = ProjectTable.RowGroups[0].Rows[0];
                    FirstRow.Background = Brushes.LightGray;
                    FirstRow.FontSize = 10;
                    FirstRow.FontStyle = FontStyles.Normal;
                    FirstRow.FontWeight = FontWeights.UltraBold;
                    FirstRow.FontFamily = new FontFamily("Century Gothic");
                    FirstRow.Cells.Add(new TableCell(new Paragraph(new Run("BJC NUMBER/PID"))));
                    FirstRow.Cells.Add(new TableCell(new Paragraph(new Run())));
                    
                    ProjectTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow SecondRow = ProjectTable.RowGroups[0].Rows[1];
                    SecondRow.Background = Brushes.LightBlue;
                    SecondRow.FontSize = 10;
                    SecondRow.FontStyle = FontStyles.Normal;
                    SecondRow.FontWeight = FontWeights.UltraBold;
                    SecondRow.FontFamily = new FontFamily("Century Gothic");
                    SecondRow.Cells.Add(new TableCell(new Paragraph(new Run("Address"))));
                    SecondRow.Cells.Add(new TableCell(new Paragraph(new Run("City"))));
                    fdAcceptLetter.Blocks.Add(ProjectTable);

                    Table CrewTable = new Table();
                    CrewTable.CellSpacing = 5;
                    CrewTable.Columns.Add(new TableColumn());
                    CrewTable.Columns[0].Width = new GridLength(225);
                    CrewTable.RowGroups.Add(new TableRowGroup());
                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow = CrewTable.RowGroups[0].Rows[0];
                    FirstCrewRow.Background = Brushes.LightGray;
                    FirstCrewRow.FontSize = 10;
                    FirstCrewRow.FontStyle = FontStyles.Normal;
                    FirstCrewRow.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("Crew"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Start"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Arrive"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Depart"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Stop"))));
                    FirstCrewRow.Cells.Add(new TableCell(new Paragraph(new Run("|  Total HRS"))));

                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow1 = CrewTable.RowGroups[0].Rows[1];
                    FirstCrewRow1.Background = Brushes.LightBlue;
                    FirstCrewRow1.FontSize = 10;
                    FirstCrewRow1.FontStyle = FontStyles.Normal;
                    FirstCrewRow1.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow1.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow1.Cells.Add(new TableCell(new Paragraph(new Run("CREW LEAD"))));
                    FirstCrewRow1.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow1.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow1.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow1.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow1.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow2 = CrewTable.RowGroups[0].Rows[2];
                    FirstCrewRow2.Background = Brushes.LightGray;
                    FirstCrewRow2.FontSize = 10;
                    FirstCrewRow2.FontStyle = FontStyles.Normal;
                    FirstCrewRow2.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow2.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow2.Cells.Add(new TableCell(new Paragraph(new Run("NAME"))));
                    FirstCrewRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow3 = CrewTable.RowGroups[0].Rows[3];
                    FirstCrewRow3.Background = Brushes.LightBlue;
                    FirstCrewRow3.FontSize = 10;
                    FirstCrewRow3.FontStyle = FontStyles.Normal;
                    FirstCrewRow3.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow3.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow3.Cells.Add(new TableCell(new Paragraph(new Run("NAME"))));
                    FirstCrewRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow4 = CrewTable.RowGroups[0].Rows[4];
                    FirstCrewRow4.Background = Brushes.LightGray;
                    FirstCrewRow4.FontSize = 10;
                    FirstCrewRow4.FontStyle = FontStyles.Normal;
                    FirstCrewRow4.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow4.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow4.Cells.Add(new TableCell(new Paragraph(new Run("NAME"))));
                    FirstCrewRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CrewTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow FirstCrewRow5 = CrewTable.RowGroups[0].Rows[5];
                    FirstCrewRow5.Background = Brushes.LightBlue;
                    FirstCrewRow5.FontSize = 10;
                    FirstCrewRow5.FontStyle = FontStyles.Normal;
                    FirstCrewRow5.FontWeight = FontWeights.UltraBold;
                    FirstCrewRow5.FontFamily = new FontFamily("Century Gothic");
                    FirstCrewRow5.Cells.Add(new TableCell(new Paragraph(new Run("NAME"))));
                    FirstCrewRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    FirstCrewRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    fdAcceptLetter.Blocks.Add(CrewTable);


                    Table CommonTasksTable = new Table();
                    CommonTasksTable.CellSpacing = 5;
                    CommonTasksTable.Columns.Add(new TableColumn());
                    CommonTasksTable.Columns[0].Width = new GridLength(300);
                    CommonTasksTable.Columns.Add(new TableColumn());
                    CommonTasksTable.Columns[1].Width = new GridLength(40);
                    CommonTasksTable.Columns.Add(new TableColumn());
                    CommonTasksTable.Columns[2].Width = new GridLength(70);
                    CommonTasksTable.RowGroups.Add(new TableRowGroup());
                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow1 = CommonTasksTable.RowGroups[0].Rows[0];
                    CommonRow1.Background = Brushes.LightGray;
                    CommonRow1.FontSize = 10;
                    CommonRow1.FontStyle = FontStyles.Normal;
                    CommonRow1.FontWeight = FontWeights.UltraBold;
                    CommonRow1.FontFamily = new FontFamily("Century Gothic");
                    CommonRow1.Cells.Add(new TableCell(new Paragraph(new Run("Common Tasks"))));
                    CommonRow1.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow1.Cells.Add(new TableCell(new Paragraph(new Run("|  QTY"))));
                    CommonRow1.Cells.Add(new TableCell(new Paragraph(new Run("|  Notes"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow2 = CommonTasksTable.RowGroups[0].Rows[1];
                    CommonRow2.Background = Brushes.LightBlue;
                    CommonRow2.FontSize = 10;
                    CommonRow2.FontStyle = FontStyles.Normal;
                    CommonRow2.FontWeight = FontWeights.UltraBold;
                    CommonRow2.FontFamily = new FontFamily("Century Gothic");
                    CommonRow2.Cells.Add(new TableCell(new Paragraph(new Run("BJC1 - NON-PRODUCTION TIME (NOTES)"))));
                    CommonRow2.Cells.Add(new TableCell(new Paragraph(new Run("|  HR"))));
                    CommonRow2.Cells.Add(new TableCell(new Paragraph(new Run("|  "))));
                    CommonRow2.Cells.Add(new TableCell(new Paragraph(new Run("|  "))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow3 = CommonTasksTable.RowGroups[0].Rows[2];
                    CommonRow3.Background = Brushes.LightGray;
                    CommonRow3.FontSize = 10;
                    CommonRow3.FontStyle = FontStyles.Normal;
                    CommonRow3.FontWeight = FontWeights.UltraBold;
                    CommonRow3.FontFamily = new FontFamily("Century Gothic");
                    CommonRow3.Cells.Add(new TableCell(new Paragraph(new Run("ER01 - EMERGENCY CALL OUT"))));
                    CommonRow3.Cells.Add(new TableCell(new Paragraph(new Run("|  EA"))));
                    CommonRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow3.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow4 = CommonTasksTable.RowGroups[0].Rows[3];
                    CommonRow4.Background = Brushes.LightBlue;
                    CommonRow4.FontSize = 10;
                    CommonRow4.FontStyle = FontStyles.Normal;
                    CommonRow4.FontWeight = FontWeights.UltraBold;
                    CommonRow4.FontFamily = new FontFamily("Century Gothic");
                    CommonRow4.Cells.Add(new TableCell(new Paragraph(new Run("MC08 - TRAFFIC CONTROL"))));
                    CommonRow4.Cells.Add(new TableCell(new Paragraph(new Run("|  HR"))));
                    CommonRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow4.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow5 = CommonTasksTable.RowGroups[0].Rows[4];
                    CommonRow5.Background = Brushes.LightGray;
                    CommonRow5.FontSize = 10;
                    CommonRow5.FontStyle = FontStyles.Normal;
                    CommonRow5.FontWeight = FontWeights.UltraBold;
                    CommonRow5.FontFamily = new FontFamily("Century Gothic");
                    CommonRow5.Cells.Add(new TableCell(new Paragraph(new Run("MC09 - NIGHT CUTOVER"))));
                    CommonRow5.Cells.Add(new TableCell(new Paragraph(new Run("|  EA"))));
                    CommonRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow5.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow6 = CommonTasksTable.RowGroups[0].Rows[5];
                    CommonRow6.Background = Brushes.LightBlue;
                    CommonRow6.FontSize = 10;
                    CommonRow6.FontStyle = FontStyles.Normal;
                    CommonRow6.FontWeight = FontWeights.UltraBold;
                    CommonRow6.FontFamily = new FontFamily("Century Gothic");
                    CommonRow6.Cells.Add(new TableCell(new Paragraph(new Run("SOFT SURFACE RESTORATION (CREWS TOTAL HOURS)"))));
                    CommonRow6.Cells.Add(new TableCell(new Paragraph(new Run("|  HR"))));
                    CommonRow6.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow6.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow7 = CommonTasksTable.RowGroups[0].Rows[6];
                    CommonRow7.Background = Brushes.LightGray;
                    CommonRow7.FontSize = 10;
                    CommonRow7.FontStyle = FontStyles.Normal;
                    CommonRow7.FontWeight = FontWeights.UltraBold;
                    CommonRow7.FontFamily = new FontFamily("Century Gothic");
                    CommonRow7.Cells.Add(new TableCell(new Paragraph(new Run("NS002- DELIVERY CHARGE (REQUIRES SPECTRUM APPROVAL)"))));
                    CommonRow7.Cells.Add(new TableCell(new Paragraph(new Run("|  EA"))));
                    CommonRow7.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow7.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow8 = CommonTasksTable.RowGroups[0].Rows[7];
                    CommonRow8.Background = Brushes.LightBlue;
                    CommonRow8.FontSize = 10;
                    CommonRow8.FontStyle = FontStyles.Normal;
                    CommonRow8.FontWeight = FontWeights.UltraBold;
                    CommonRow8.FontFamily = new FontFamily("Century Gothic");
                    CommonRow8.Cells.Add(new TableCell(new Paragraph(new Run("MC10 - SETUP FEE (BELOW 200' AERIAL, BELOW 100' UG)"))));
                    CommonRow8.Cells.Add(new TableCell(new Paragraph(new Run("|  EA"))));
                    CommonRow8.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow8.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    CommonTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow CommonRow9 = CommonTasksTable.RowGroups[0].Rows[8];
                    CommonRow9.Background = Brushes.LightGray;
                    CommonRow9.FontSize = 10;
                    CommonRow9.FontStyle = FontStyles.Normal;
                    CommonRow9.FontWeight = FontWeights.UltraBold;
                    CommonRow9.FontFamily = new FontFamily("Century Gothic");
                    CommonRow9.Cells.Add(new TableCell(new Paragraph(new Run("MC11 - MOBILIZATION FEE (PER SPECTRUM APPROVAL)"))));
                    CommonRow9.Cells.Add(new TableCell(new Paragraph(new Run("|  EA"))));
                    CommonRow9.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                    CommonRow9.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                    fdAcceptLetter.Blocks.Add(CommonTasksTable);

                    Table WorkTasksTable = new Table();
                    intNumberOfRows = 0;
                    WorkTasksTable.CellSpacing = 5;
                    WorkTasksTable.Columns.Add(new TableColumn());
                    WorkTasksTable.Columns[0].Width = new GridLength(300);
                    WorkTasksTable.Columns.Add(new TableColumn());
                    WorkTasksTable.Columns[1].Width = new GridLength(50);
                    WorkTasksTable.Columns.Add(new TableColumn());
                    WorkTasksTable.Columns[2].Width = new GridLength(300);
                    WorkTasksTable.RowGroups.Add(new TableRowGroup());
                    WorkTasksTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow TaskRow1 = WorkTasksTable.RowGroups[0].Rows[0];
                    TaskRow1.Background = Brushes.LightGray;
                    TaskRow1.FontSize = 10;
                    TaskRow1.FontStyle = FontStyles.Normal;
                    TaskRow1.FontWeight = FontWeights.UltraBold;
                    TaskRow1.FontFamily = new FontFamily("Century Gothic");
                    TaskRow1.Cells.Add(new TableCell(new Paragraph(new Run("Work Task"))));
                    TaskRow1.Cells.Add(new TableCell(new Paragraph(new Run("|  QTY"))));
                    TaskRow1.Cells.Add(new TableCell(new Paragraph(new Run("|  Work Task"))));
                    TaskRow1.Cells.Add(new TableCell(new Paragraph(new Run("|  QTY"))));
                    intNumberOfRows++;

                    intNumberOfRecords = TheFindWorkTaskDepartmentByLOBDepartmentDataSet.FindWorkTaskDepartmentByLOBDepartment.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            if (TheProductionTasksForSheetsDataSet.productiontasks[intCounter].UseCode == true)
                            {
                                WorkTasksTable.RowGroups[0].Rows.Add(new TableRow());
                                TableRow TaskRow2 = WorkTasksTable.RowGroups[0].Rows[intNumberOfRows];

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

                                strWorkTask = TheFindWorkTaskDepartmentByLOBDepartmentDataSet.FindWorkTaskDepartmentByLOBDepartment[intCounter].WorkTask;
                                TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run(strWorkTask))));
                                TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));

                                intCounter++;
                                blnProcessing = false;

                                while (blnProcessing == false)
                                {
                                    if(intCounter < intNumberOfRecords)
                                    {
                                        if (TheProductionTasksForSheetsDataSet.productiontasks[intCounter].UseCode == true)
                                        {
                                            if (intCounter > intNumberOfRows)
                                            {
                                                strWorkTask = TheFindWorkTaskDepartmentByLOBDepartmentDataSet.FindWorkTaskDepartmentByLOBDepartment[intCounter].WorkTask;
                                            }
                                            else if (intCounter <= intNumberOfRows)
                                            {
                                                strWorkTask = "";
                                            }

                                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run("| " + strWorkTask))));
                                            TaskRow2.Cells.Add(new TableCell(new Paragraph(new Run("|"))));
                                            intNumberOfRows++;
                                            blnProcessing = true;
                                        }
                                        else
                                        {
                                            intCounter++;
                                        }
                                    }
                                    else
                                    {
                                        blnProcessing = true;
                                    }
                                }
                            }
                        }
                    }

                    fdAcceptLetter.Blocks.Add(WorkTasksTable);
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
