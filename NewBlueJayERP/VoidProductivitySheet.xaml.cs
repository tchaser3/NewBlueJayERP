/* Title:           Void Productivity Sheet
 * Date:            3-8-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to void out a transaction*/

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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VoidProductivitySheet.xaml
    /// </summary>
    public partial class VoidProductivitySheet : Window
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
        ProjectsForVoidingDataSet TheProjectsForVoidingDataSet = new ProjectsForVoidingDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        int gintCounter;
        int gintNumberOfRecords;
        int gintEmployeeID;
        DateTime gdatTransactionDate;

        public VoidProductivitySheet()
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

            TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Clear();

            dgrResults.ItemsSource = TheProjectsForVoidingDataSet.projectsforvoiding;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Void Productivity Sheet");
        }


        private void expVoidItems_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intAssignedTransactionID;
            int intTaskTransactionID;
            bool blnFatalError;

            try
            {
                expVoidItems.IsExpanded = false;
                intNumberOfRecords = TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intAssignedTransactionID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].AssignmentTransactionID;
                        intTaskTransactionID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].TaskTransactionID;

                        blnFatalError = TheEmployeeProjectAssignmentClass.UpdateEmployeeLaborHours(intAssignedTransactionID, 0);

                        if (blnFatalError == true)
                            throw new Exception();

                        if(intTaskTransactionID > -1)
                        {
                            blnFatalError = TheProjectTaskClass.UpdateProjectTaskFootage(intTaskTransactionID, 0);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Transactions Have Been Voided");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Production Sheets // Void Items Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Was Not Found");
                        return;
                    }

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Productivity Sheet // Enter Last Name Text Box " + Ex.Message);

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

        private void expFindTransactions_Expanded(object sender, RoutedEventArgs e)
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
                expFindTransactions.IsExpanded = false;
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strValueForValidation = txtEnterDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Entered is not a Date\n";
                }
                else
                {
                    gdatTransactionDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindEmployeeProjectAssignmentForVoidingDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProjectAssignmentForVoiding(gintEmployeeID, gdatTransactionDate);

                intNumberOfRecords = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    ProjectsForVoidingDataSet.projectsforvoidingRow NewProjectRow = TheProjectsForVoidingDataSet.projectsforvoiding.NewprojectsforvoidingRow();

                    NewProjectRow.AssignedProjectID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].AssignedProjectID;
                    NewProjectRow.AssignmentTransactionID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].TransactionID;
                    NewProjectRow.CustomerProjectID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].CustomerAssignedID;
                    NewProjectRow.Footage = 0;
                    NewProjectRow.ProjectID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].ProjectID;
                    NewProjectRow.TaskTransactionID = -1;
                    NewProjectRow.TotalHours = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].TotalHours;
                    NewProjectRow.WorkTask = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].WorkTask;
                    NewProjectRow.WorkTaskID = TheFindEmployeeProjectAssignmentForVoidingDataSet.FindEmployeeProjectAssignmentForVoiding[intCounter].TaskID;

                    TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Add(NewProjectRow);
                }

                TheFindProjectTaskForVoidingDataSet = TheProjectTaskClass.FindProjectTaskForVoiding(gintEmployeeID, gdatTransactionDate);

                intNumberOfRecords = TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Count;
                intSecondNumberOfRecords = TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intTaskID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].WorkTaskID;
                        intProjectID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].ProjectID;

                        if(intSecondNumberOfRecords > 0)
                        {
                            for(intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                            {
                                if(intTaskID == TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].WorkTaskID)
                                {
                                    if(intProjectID == TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].ProjectID)
                                    {
                                        TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].TaskTransactionID = TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].TransactionID;
                                        TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].Footage = TheFindProjectTaskForVoidingDataSet.FindProjectTaskForVoiding[intSecondCounter].FootagePieces;
                                    }
                                }
                            }
                        }
                    }
                }

                dgrResults.ItemsSource = TheProjectsForVoidingDataSet.projectsforvoiding;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay Erp // Void Productivity Sheet // Find Transactions Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
