/* Title:           View Employee Production
 * Date:            8-12-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to view Employee Productioin */

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
using DataValidationDLL;
using EmployeeProjectAssignmentDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;
using ProjectsDLL;
using ProjectTaskDLL;
using NewEmployeeDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewEmployeeProduction.xaml
    /// </summary>
    public partial class ViewEmployeeProduction : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindProjectTaskForVoidingDataSet TheFindProjectTaskForVoidingDataSet = new FindProjectTaskForVoidingDataSet();
        FindEmployeeProjectAssignmentForVoidingDataSet TheFindEmployeeProjectAssignmentForVoidingDataSet = new FindEmployeeProjectAssignmentForVoidingDataSet();
        ViewEmployeeProductionDataSet TheViewEmployeeProductionDataSet = new ViewEmployeeProductionDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        int gintCounter;
        int gintNumberOfRecords;
        int gintEmployeeID;
        DateTime gdatTransactionDate;

        public ViewEmployeeProduction()
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
            txtEnterDate.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;

            TheViewEmployeeProductionDataSet.viewemployeeproduction.Rows.Clear();

            dgrResults.ItemsSource = TheViewEmployeeProductionDataSet.viewemployeeproduction;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // View Employee Production");
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up local variables
            string strLastName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;
                txtEnterDate.Text = "";

                if (strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if (intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Was Not Found");
                        return;
                    }

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Find Employee Production // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void expFindProductioin_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            int intProjectID;
            int intTaskID;
            int intSecondCounter;
            int intSecondNumberOfRecords;

            try
            {
                //data validation
                expFindProductioin.IsExpanded = false;
                TheViewEmployeeProductionDataSet.viewemployeeproduction.Rows.Clear();
                if (cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strValueForValidation = txtEnterDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Entered is not a Date\n";
                }
                else
                {
                    gdatTransactionDate = Convert.ToDateTime(strValueForValidation);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindEmployeeProjectAssignmentForVoidingDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProjectAssignmentForVoiding(gintEmployeeID, gdatTransactionDate);

                intNumberOfRecords = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    ViewEmployeeProductionDataSet.viewemployeeproductionRow NewProjectRow = TheViewEmployeeProductionDataSet.viewemployeeproduction.NewviewemployeeproductionRow();

                    NewProjectRow.AssignedProjectID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].AssignedProjectID;
                    NewProjectRow.CustomerProjectID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].CustomerAssignedID;
                    NewProjectRow.FootagePieces = 0;
                    NewProjectRow.ProjectID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].ProjectID;
                    NewProjectRow.TotalHours = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].TotalHours;
                    NewProjectRow.WorkTask = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].WorkTask;
                    NewProjectRow.WorkTaskID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].TaskID;

                    TheViewEmployeeProductionDataSet.viewemployeeproduction.Rows.Add(NewProjectRow);
                }

                TheFindProjectTaskForVoidingDataSet = TheProjectTaskClass.FindProjectTaskForVoiding(gintEmployeeID, gdatTransactionDate);

                intNumberOfRecords = TheViewEmployeeProductionDataSet.viewemployeeproduction.Rows.Count;
                intSecondNumberOfRecords = TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intTaskID = TheViewEmployeeProductionDataSet.viewemployeeproduction[intCounter].WorkTaskID;
                        intProjectID = TheViewEmployeeProductionDataSet.viewemployeeproduction[intCounter].ProjectID;

                        if (intSecondNumberOfRecords > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                            {
                                if (intTaskID == TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].WorkTaskID)
                                {
                                    if (intProjectID == TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].ProjectID)
                                    {
                                        TheViewEmployeeProductionDataSet.viewemployeeproduction[intCounter].FootagePieces = TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].FootagePieces;
                                    }
                                }
                            }
                        }
                    }
                }

                dgrResults.ItemsSource = TheViewEmployeeProductionDataSet.viewemployeeproduction;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay Erp // View Employee Production // Find Production Expander " + Ex.Message);

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
                intRowNumberOfRecords = TheViewEmployeeProductionDataSet.viewemployeeproduction.Rows.Count;
                intColumnNumberOfRecords = TheViewEmployeeProductionDataSet.viewemployeeproduction.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheViewEmployeeProductionDataSet.viewemployeeproduction.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheViewEmployeeProductionDataSet.viewemployeeproduction.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View Employee Production // Export To Excel " + ex.Message);

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
