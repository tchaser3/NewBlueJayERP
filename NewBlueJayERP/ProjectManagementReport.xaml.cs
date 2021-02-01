/* Title:           Project Management Report
 * Date:            1-30-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for show the project management sheet */

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
using ProductionProjectDLL;
using ProductionProjectUpdatesDLL;
using DepartmentDLL;
using ProjectMatrixDLL;
using Microsoft.Win32;


namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ProjectManagementReport.xaml
    /// </summary>
    public partial class ProjectManagementReport : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //setting up the data
        FindOpenOfficeBusinessLineProjectListDataSet TheFindOpenOfficeBusinessLineProjectListDataSet = new FindOpenOfficeBusinessLineProjectListDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        ProjectManagementDataSet TheProjectManagementDataSet = new ProjectManagementDataSet();
        FindProductionProjectUpdateByProjectIDDataSet TheFindProductionProjectUdpateByProjectIDDataSet = new FindProductionProjectUpdateByProjectIDDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();

        public ProjectManagementReport()
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
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");
                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.SelectedIndex = 0;

                TheProjectManagementDataSet.projectmanagement.Rows.Clear();

                dgrProjects.ItemsSource = TheProjectManagementDataSet.projectmanagement;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Management Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                    if(MainWindow.gintDepartmentID > -1)
                    {
                        LoadDataGrid();
                    }
                }
                else
                {
                    MainWindow.gintWarehouseID = -1;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Management Report // Select Office Combobox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadDataGrid()
        {
            //this will load up the grid
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intProjectID;
            string strLastUpdate = "";
            DateTime datTransactionDate;
            DateTime datECDDate = DateTime.Now;
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ECDDate;
            string strECDDate;

            try
            {
                TheFindOpenOfficeBusinessLineProjectListDataSet = TheProjectMatrixClass.FindOpenOfficeBusinessLineProjectList(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID);

                TheProjectManagementDataSet.projectmanagement.Rows.Clear();

                intNumberOfRecords = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intProjectID = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList[intCounter].ProjectID;

                    TheFindProductionProjectUdpateByProjectIDDataSet = TheProductionProjectUpdatesClass.FindProductionProjectUpdateByProjectID(intProjectID);

                    intRecordsReturned = TheFindProductionProjectUdpateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        strLastUpdate = "NO UPDATES ENTERED";
                    }
                    else if(intRecordsReturned > 0)
                    {
                        datTransactionDate = TheFindProductionProjectUdpateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[0].TransactionDate;
                        strLastUpdate = Convert.ToString(datTransactionDate) + " - " + TheFindProductionProjectUdpateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[0].ProjectUpdate;
                    }

                    ProjectManagementDataSet.projectmanagementRow NewProjectRow = TheProjectManagementDataSet.projectmanagement.NewprojectmanagementRow();

                    NewProjectRow.BlueJayID = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList[intCounter].AssignedProjectID;
                    NewProjectRow.CustomerProjectID = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList[intCounter].CustomerAssignedID;
                    NewProjectRow.ECDDate = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList[intCounter].ECDDate;
                    NewProjectRow.LastUpdate = strLastUpdate;
                    NewProjectRow.ProjectName = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList[intCounter].ProjectName;
                    NewProjectRow.Status = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList[intCounter].WorkOrderStatus;

                    TheProjectManagementDataSet.projectmanagement.Rows.Add(NewProjectRow);
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Management Report // Load Data Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectBusinessLine.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintDepartmentID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;

                    if (MainWindow.gintWarehouseID > -1)
                    {
                        LoadDataGrid();
                    }
                }
                else
                {
                    MainWindow.gintWarehouseID = -1;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Management Report // Select Office Combobox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProjectID;
            string strProjectID;

            try
            {
                if (dgrProjects.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrProjects;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProjectID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProjectID = ((TextBlock)ProjectID.Content).Text;

                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strProjectID);

                    //find the record
                    MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;

                    UpdateSelectedProject UpdateSelectedProject = new UpdateSelectedProject();
                    UpdateSelectedProject.ShowDialog();

                    LoadDataGrid();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Office Info Dashboard // Open Projects List Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
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
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheProjectManagementDataSet.projectmanagement.Rows.Count;
                intColumnNumberOfRecords = TheProjectManagementDataSet.projectmanagement.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectManagementDataSet.projectmanagement.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectManagementDataSet.projectmanagement.Rows[intRowCounter][intColumnCounter].ToString();

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
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Project Management Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
