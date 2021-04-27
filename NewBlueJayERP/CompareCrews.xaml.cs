/* Title:           Compare Crews
 * Date:            4-26-21
 * Author:          Terry Holmes
 * 
 * Description:     This is for comparing the crews */

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
using EmployeeCrewAssignmentDLL;
using DataValidationDLL;
using ProjectTaskDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CompareCrews.xaml
    /// </summary>
    public partial class CompareCrews : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();

        FindEmployeeCrewAssignmentComboBoxDataSet TheFindEmployeeCrewAssignmentComboBoxDataSet = new FindEmployeeCrewAssignmentComboBoxDataSet();
        FindDetailedEmployeeCrewAssignmentByCrewIDDataSet TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet = new FindDetailedEmployeeCrewAssignmentByCrewIDDataSet();
        CompareCrewEmployeesDataSet TheCompareCrewEmployeesDataSet = new CompareCrewEmployeesDataSet();
        FindProjectTaskForProductivityReportDataSet TheFindProjectTaskForProductivityReportDataSet = new FindProjectTaskForProductivityReportDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        int gintEmployeeCounter;
        int gintEmployeeUpperLimit;

        public CompareCrews()
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
            txtEndDate.Text = "";
            txtStartDate.Text = "";
            cboSelectCrew.Items.Clear();
            cboSelectCrew.Items.Add("Select Crew");
            cboSelectCrew.SelectedIndex = 0;

            TheCompareCrewEmployeesDataSet.employees.Rows.Clear();

            dgrCrews.ItemsSource = TheCompareCrewEmployeesDataSet.employees;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheCompareCrewEmployeesDataSet.employees.Rows.Clear();

                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                //loading the combo box
                TheFindEmployeeCrewAssignmentComboBoxDataSet = TheEmployeeCrewAssignmentClass.FindEmployeeCrewAssignmentComboBox(gdatStartDate, gdatEndDate);

                cboSelectCrew.Items.Clear();

                cboSelectCrew.Items.Add("Select Crew");

                intNumberOfRecords = TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectCrew.Items.Add(TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox[intCounter].CrewID);
                    }                    
                }

                cboSelectCrew.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Compare Crews // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectCrew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strCrewID;
            int intEmployeeCounter;
            int intEmployeeID;
            int intProjectID;
            int intCounter;
            int intNumberOfRecords;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            DateTime datTransactionDate;


            try
            {
                intSelectedIndex = cboSelectCrew.SelectedIndex - 1;
                
                if (intSelectedIndex > -1)
                {
                    strCrewID = TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox[intSelectedIndex].CrewID;

                    TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet = TheEmployeeCrewAssignmentClass.FindDetailedEmployeeCrewAssignmentByCrewID(strCrewID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID.Rows.Count - 1;
                    gintEmployeeCounter = TheCompareCrewEmployeesDataSet.employees.Rows.Count;
                    gintEmployeeUpperLimit = gintEmployeeCounter - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].EmployeeID;
                            intProjectID = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].ProjectID;
                            datTransactionDate = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].TransactionDate;

                            TheFindProjectTaskForProductivityReportDataSet = TheProjectTaskClass.FindProjectTaskForProductivityReport(intEmployeeID, intProjectID, datTransactionDate);

                            intSecondNumberOfRecords = TheFindProjectTaskForProductivityReportDataSet.FindProjectTaskForProductivityReport.Rows.Count;

                            for(intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                            {
                                CompareCrewEmployeesDataSet.employeesRow NewEmployeeRow = TheCompareCrewEmployeesDataSet.employees.NewemployeesRow();

                                NewEmployeeRow.AssignedProjectID = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].AssignedProjectID;
                                NewEmployeeRow.Date = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].TransactionDate;
                                NewEmployeeRow.FirstName = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].FirstName;
                                NewEmployeeRow.HomeOffice = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].HomeOffice;
                                NewEmployeeRow.LastName = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].LastName;
                                NewEmployeeRow.CrewID = strCrewID;
                                NewEmployeeRow.WorkTask = TheFindProjectTaskForProductivityReportDataSet.FindProjectTaskForProductivityReport[intSecondCounter].WorkTask;
                                NewEmployeeRow.TaskFootage = TheFindProjectTaskForProductivityReportDataSet.FindProjectTaskForProductivityReport[intSecondCounter].FootagePieces;

                                TheCompareCrewEmployeesDataSet.employees.Rows.Add(NewEmployeeRow);
                                gintEmployeeUpperLimit = gintEmployeeCounter;
                                gintEmployeeCounter++;
                            }
                            
                                
                            
                        }
                    }                   

                    dgrCrews.ItemsSource = TheCompareCrewEmployeesDataSet.employees;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Compare Crews // Select Crew Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;

            ResetControls();
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

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheCompareCrewEmployeesDataSet.employees.Rows.Count;
                intColumnNumberOfRecords = TheCompareCrewEmployeesDataSet.employees.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompareCrewEmployeesDataSet.employees.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompareCrewEmployeesDataSet.employees.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Compare Crews // Export Excel " + ex.Message);

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
