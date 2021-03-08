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
using EmployeeCrewAssignmentDLL;

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
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();

        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectMatrixByCustomerAssignedIDForEmailDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerAssignedIDForEmailDataSet();
        FindProductivitySheetForVoidingDataSet TheFindProductivitySheetForVoidingDataSet = new FindProductivitySheetForVoidingDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        ProjectsForVoidingDataSet TheProjectsForVoidingDataSet = new ProjectsForVoidingDataSet();

        int gintCounter;
        int gintNumberOfRecords;

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
            txtEnterProjectID.Text = "";

            TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Clear();

            dgrResults.ItemsSource = TheProjectsForVoidingDataSet.projectsforvoiding;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Void Productivity Sheet");
        }

        private void expFindProject_Expanded(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strProjectID;
            int intProjectID = 0;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intSecondCounter;
            bool blnItemFound;
            int intAssignmentTransactionID;
            int intCrewTransactionID;
            int intTaskTransactionID;

            try
            {
                TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Clear();

                strProjectID = txtEnterProjectID.Text;
                if (strProjectID.Length < 6)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Is Not Long Enough\n";
                }
                strValueForValidation = txtEnterDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date is not a Date\n";
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedIDForEmail(strProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerAssignedIDForEmail.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Project was not Found");
                        return;
                    }

                    intProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                }
                else if(intRecordsReturned > 0)
                {
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                    intProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                }

                TheFindProductivitySheetForVoidingDataSet = TheEmployeeProjectAssignmentClass.FindProductivitySheetForVoiding(intProjectID, datTransactionDate);

                intNumberOfRecords = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding.Rows.Count;
                gintCounter = 0;
                gintNumberOfRecords = 0;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        intAssignmentTransactionID = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].TransactionID;

                        if(gintCounter > 0)
                        {
                            for(intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if (TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].AssignmentTransactionID == intAssignmentTransactionID)
                                {
                                    blnItemFound = true;
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            ProjectsForVoidingDataSet.projectsforvoidingRow NewProjectRow = TheProjectsForVoidingDataSet.projectsforvoiding.NewprojectsforvoidingRow();

                            NewProjectRow.AssignedProjectID = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].AssignedProjectID;
                            NewProjectRow.AssignmentTransactionID = intAssignmentTransactionID;
                            NewProjectRow.CrewTransactionID = 0;
                            NewProjectRow.CustomerProjectID = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].CustomerAssignedID;
                            NewProjectRow.FirstName = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].FirstName;
                            NewProjectRow.LastName = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].LastName;
                            NewProjectRow.TaskTransactionID = 0;
                            NewProjectRow.WorkTask = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].WorkTask;
                            NewProjectRow.Void = false;

                            TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Add(NewProjectRow);
                            gintCounter++;
                            gintNumberOfRecords = gintCounter;
                            
                        }
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        intCrewTransactionID = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].CrewID;
                        intTaskTransactionID = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].TaskPerformedID;

                        for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                        {
                            if(TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].CrewTransactionID == intCrewTransactionID)
                            {
                                blnItemFound = true;
                            }
                        }

                        if(blnItemFound == false)
                        {
                            for(intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if(TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].CrewTransactionID == 0)
                                {
                                    TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].CrewTransactionID = intCrewTransactionID;
                                    intCrewTransactionID = 0;
                                }
                            }
                        }
                    }

                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        intTaskTransactionID = TheFindProductivitySheetForVoidingDataSet.FindProductivitySheetForVoiding[intCounter].TaskPerformedID;

                        for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                        {
                            if (TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].TaskTransactionID == intTaskTransactionID)
                            {
                                blnItemFound = true;
                            }
                        }

                        if (blnItemFound == false)
                        {
                            for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if (TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].TaskTransactionID == 0)
                                {
                                    TheProjectsForVoidingDataSet.projectsforvoiding[intSecondCounter].TaskTransactionID = intTaskTransactionID;
                                    intTaskTransactionID = 0;
                                }
                            }
                        }
                    }
                }

                dgrResults.ItemsSource = TheProjectsForVoidingDataSet.projectsforvoiding;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Productivity Sheet // Find Project Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expVoidItems_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intAssignedTransactionID;
            int intCrewTransactionID;
            int intTaskTransactionID;
            bool blnFatalError;

            try
            {
                intNumberOfRecords = TheProjectsForVoidingDataSet.projectsforvoiding.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].Void == true)
                        {
                            intAssignedTransactionID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].AssignmentTransactionID;
                            intCrewTransactionID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].CrewTransactionID;
                            intTaskTransactionID = TheProjectsForVoidingDataSet.projectsforvoiding[intCounter].TaskTransactionID;

                            blnFatalError = TheEmployeeProjectAssignmentClass.UpdateEmployeeLaborHours(intAssignedTransactionID, 0);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheProjectTaskClass.UpdateProjectTaskFootage(intTaskTransactionID, 0);

                            if (blnFatalError == true)
                                throw new Exception();
                            
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Selected Transactions Have Been Voided");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Production Sheets // Void Items Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
