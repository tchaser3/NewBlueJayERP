/* Title:           Update Help Desk Tickets
 * Date:            7-14-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to update Help Desk Tickets */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using HelpDeskDLL;
using PhonesDLL;
using VehiclesInShopDLL;
using System.IO;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateHelpDeskTickets.xaml
    /// </summary>
    public partial class UpdateHelpDeskTickets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindOpenHelpDeskTicketsDataSet TheFindOpenHelpDeskTicketsDataSet = new FindOpenHelpDeskTicketsDataSet();
        FindHelpDeskTicketsByTicketIDDataSet TheFindHelpDeskTicketsByTicketIDDataSet = new FindHelpDeskTicketsByTicketIDDataSet();
        FindHelpDeskTicketUpdatesByTicketIDDataSet TheFindHelpDeskTicketUpdatesByTicketIDDataSet = new FindHelpDeskTicketUpdatesByTicketIDDataSet();
        FindSortedHelpDeskProblemTypeDataSet TheFindSortedHelpDeskProblemTypeDataSet = new FindSortedHelpDeskProblemTypeDataSet();
        FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindOpenHelpDeskTicketsAssignedDataSet TheFindOpenHelpDeskTicketsAssignedDataSet = new FindOpenHelpDeskTicketsAssignedDataSet();
        OpenHelpDeskTicketsDataSet TheOpenHelpDeskTicketsDataSet = new OpenHelpDeskTicketsDataSet();
        FindOpenHelpDeskTicketsForAssignmentDataSet TheFindOpenHelpDeskTicketsForAssignmentDataSet = new FindOpenHelpDeskTicketsForAssignmentDataSet();
        FindHelpDeskTicketCurrentAssignmentDataSet TheFindHelpDeskCurrentAssignmentDataSet = new FindHelpDeskTicketCurrentAssignmentDataSet();

        string gstrTicketStatus;
        string gstrUserEmailAddress;
        bool gblnEmailAddress;
        
        public UpdateHelpDeskTickets()
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
            //setting variable
            int intCounter;
            int intNumberOfRecords;
            string strFullName;

            try
            {
                txtComputerName.Text = "";
                txtCurrentUpdte.Text = "";
                txtExtension.Text = "";
                txtTicketUpdates.Text = "";

                TheFindOpenHelpDeskTicketsDataSet = TheHelpDeskClass.FindOpenHelpDeskTickets();

                TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Clear();

                intNumberOfRecords = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    OpenHelpDeskTicketsDataSet.openhelpdeskticketsRow NewTicketRow = TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.NewopenhelpdeskticketsRow();

                    NewTicketRow.FirstName = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].FirstName;
                    NewTicketRow.LastName = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].LastName;
                    NewTicketRow.ReportedProblem = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].ReportedProblem;
                    NewTicketRow.TicketDate = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].TicketDate;
                    NewTicketRow.TicketID = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].TicketID;

                    TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Add(NewTicketRow);
                }

                dgrOpenTickets.ItemsSource = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets;

                TheFindSortedHelpDeskProblemTypeDataSet = TheHelpDeskClass.FindSortedHelpDeskProblemType();

                intNumberOfRecords = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType.Rows.Count;
                cboProblemType.Items.Clear();
                cboProblemType.Items.Add("Select Problem Type");

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboProblemType.Items.Add(TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intCounter].ProblemType);
                }

                cboProblemType.SelectedIndex = 0;

                cboTicketStatus.Items.Clear();
                cboTicketStatus.Items.Add("Select Status");
                cboTicketStatus.Items.Add("OPEN");
                cboTicketStatus.Items.Add("PROGRAM RESEARCH");
                cboTicketStatus.Items.Add("IN PROCESS");
                cboTicketStatus.Items.Add("WAITING ON USER");
                cboTicketStatus.Items.Add("WAITING ON PARTS");
                cboTicketStatus.Items.Add("WAITING ON ERP RELEASE");
                cboTicketStatus.Items.Add("CLOSED");
                cboTicketStatus.SelectedIndex = 0;

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("INFORMATION TECHNOLOGY");

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName;

                    cboSelectEmployee.Items.Add(strFullName);
                }

                cboSelectEmployee.SelectedIndex = 0;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Update Help Desk Tickets");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void SendEmailRegardingCurrentAssignment(int intTicketID, string strFullName)
        {
            string strEmailAddress = "itadmin@bluejaycommunications.com";
            string strHeader;
            string strMessage;
            bool blnFatalError = false;

            try
            {
                strHeader = "Ticket " + Convert.ToString(intTicketID) + " Has Been Assigned To " + strFullName;

                strMessage = "<h1>" + strHeader + "</h1>";

                blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Send Email Regarding Current Assignment " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void expMyTickets_Expanded(object sender, RoutedEventArgs e)
        {
            int intEmployeeID;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                expMyTickets.IsExpanded = false;
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                TheFindOpenHelpDeskTicketsAssignedDataSet = TheHelpDeskClass.FindOpenHelpDeskTicketsAssigned(intEmployeeID);
                TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Clear();

                intNumberOfRecords = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        OpenHelpDeskTicketsDataSet.openhelpdeskticketsRow NewTicketRow = TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.NewopenhelpdeskticketsRow();

                        NewTicketRow.FirstName = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].FirstName;
                        NewTicketRow.LastName = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].LastName;
                        NewTicketRow.ReportedProblem = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].ReportedProblem;
                        NewTicketRow.TicketDate = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].TicketDate;
                        NewTicketRow.TicketID = TheFindOpenHelpDeskTicketsAssignedDataSet.FindOpenHelpDeskTicketsAssgined[intCounter].TicketID;

                        TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Add(NewTicketRow);
                    }
                }

                dgrOpenTickets.ItemsSource = TheOpenHelpDeskTicketsDataSet.openhelpdesktickets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // My Tickets Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expUnassigned_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                expUnassigned.IsExpanded = false;
                TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Clear();

                TheFindOpenHelpDeskTicketsForAssignmentDataSet = TheHelpDeskClass.FindOpenHelpDeskTicketsForAssignment();

                intNumberOfRecords = TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment[intCounter].IsCurrentAssignmentNull() == true)
                        {
                            OpenHelpDeskTicketsDataSet.openhelpdeskticketsRow NewTicketRow = TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.NewopenhelpdeskticketsRow();

                            NewTicketRow.FirstName = TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment[intCounter].FirstName;
                            NewTicketRow.LastName = TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment[intCounter].LastName;
                            NewTicketRow.TicketDate = TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment[intCounter].TicketDate;
                            NewTicketRow.TicketID = TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment[intCounter].TicketID;
                            NewTicketRow.ReportedProblem = TheFindOpenHelpDeskTicketsForAssignmentDataSet.FindOpenHelpDeskTicketsForAssignment[intCounter].ReportedProblem;

                            TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Add(NewTicketRow);
                        }         
                    }
                }

                dgrOpenTickets.ItemsSource = TheOpenHelpDeskTicketsDataSet.openhelpdesktickets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Unassigned Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;

            ResetControls();
        }

        private void cboProblemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboProblemType.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintProblemTypeID = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intSelectedIndex].ProblemTypeID;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;
            bool blnFatalError = false;
            int intTransactionID;
            string strFullName;
            string strMessage;
            string strHeader;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].EmployeeID;

                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].LastName;

                    strHeader = "Ticket " + Convert.ToString(MainWindow.gintTicketID) + " Has Been Assigned To " + strFullName;
                    strMessage = "<h1>" + strHeader + "</h1>";

                    TheFindHelpDeskCurrentAssignmentDataSet = TheHelpDeskClass.FindHelpDeskTicketCurrentAssignment(MainWindow.gintTicketID);

                    intRecordsReturned = TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        if(MainWindow.gintEmployeeID != TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].EmployeeID)
                        {
                            intTransactionID = TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].TransactionID;

                            blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketCurrrentAssignment(intTransactionID, false);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(MainWindow.gintTicketID, MainWindow.gintEmployeeID);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintTicketID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strHeader);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                    else if(intRecordsReturned < 1)
                    {
                        blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(MainWindow.gintTicketID, MainWindow.gintEmployeeID);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintTicketID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strHeader);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboTicketStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboTicketStatus.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrTicketStatus = cboTicketStatus.SelectedItem.ToString();
            }
        }

        private void dgrOpenTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TicketID;
            string strTicketID;
            int intRecordsReturned;
            int intEmployeeID;
            int intProblemTypeID;
            int intAssignedEmployeeID;
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;
            string strTicketStatus = "";
            string strTicketUpdate;
            bool blnFatalError;

            try
            {
                if (dgrOpenTickets.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrOpenTickets;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TicketID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTicketID = ((TextBlock)TicketID.Content).Text;


                    //find the record
                    MainWindow.gintTicketID = Convert.ToInt32(strTicketID);

                    TheFindHelpDeskTicketsByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketByTicketID(MainWindow.gintTicketID);

                    intEmployeeID = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].EmployeeID;
                    txtComputerName.Text = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].ComputerName;

                    strTicketUpdate = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].FirstName + " ";
                    strTicketUpdate += TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].LastName + " - ";
                    strTicketUpdate += Convert.ToString(TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].TicketDate) + " - ";
                    strTicketUpdate += TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].ReportedProblem + "\n\n";

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                    if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                    {
                        gstrUserEmailAddress = "techadmin@bluejaycommunications.com";
                    }
                    else
                    {
                        gstrUserEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                        blnFatalError = TheDataValidationClass.VerifyEmailAddress(gstrUserEmailAddress);

                        if(blnFatalError == true)
                        {
                            gstrUserEmailAddress = "techadmin@bluejaycommunications.com";
                        }
                    }

                    TheFindHelpDeskCurrentAssignmentDataSet = TheHelpDeskClass.FindHelpDeskTicketCurrentAssignment(MainWindow.gintTicketID);

                    intRecordsReturned = TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        intAssignedEmployeeID = TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].EmployeeID;
                        intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count;

                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            if(intAssignedEmployeeID == TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].EmployeeID)
                            {
                                cboSelectEmployee.SelectedIndex = intCounter + 1;
                            }
                        }
                    }
                    else
                    {
                        cboSelectEmployee.SelectedIndex = 0;
                    }

                    intProblemTypeID = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].ProblemTypeID;

                    intNumberOfRecords = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType.Rows.Count;

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(intProblemTypeID == TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intCounter].ProblemTypeID)
                        {
                            cboProblemType.SelectedIndex = intCounter + 1;
                        }
                    }

                    strTicketStatus = TheFindHelpDeskTicketsByTicketIDDataSet.FindHelpDeskTicketsByTicketID[0].TicketStatus;

                    intNumberOfRecords = cboTicketStatus.Items.Count;

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboTicketStatus.SelectedIndex = intCounter;

                        if(strTicketStatus == cboTicketStatus.SelectedItem.ToString())
                        {
                            intSelectedIndex = intCounter;
                        }
                    }

                    cboTicketStatus.SelectedIndex = intSelectedIndex;

                    TheFindPhoneExtensionByEmployeeIDDataSet = ThePhoneClass.FindPhoneExtensionByEmployeeID(intEmployeeID);

                    intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        txtExtension.Text = Convert.ToString(TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[0].Extension);
                    }
                    else
                    {
                        txtExtension.Text = "0";
                    }

                    TheFindHelpDeskTicketUpdatesByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketUpdatesByTicketID(MainWindow.gintTicketID);

                    intNumberOfRecords = TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            strTicketUpdate += Convert.ToString(TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID[intCounter].TransactionDate) + " - ";
                            strTicketUpdate += TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID[intCounter].FirstName + " ";
                            strTicketUpdate += TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID[intCounter].LastName + " - ";
                            strTicketUpdate += TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID[intCounter].UpdateNotes + "\n\n";
                        }
                    }

                    txtTicketUpdates.Text = strTicketUpdate;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // dgr Open Tickets Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expUpdateTicket_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strCurrentUpdate;
            string strHeader;
            string strMessage;

            try
            {
                expUpdateTicket.IsExpanded = false;
                if(cboProblemType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Type Was Not Selected\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if(cboTicketStatus.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Ticket Status was not Selected\n";
                }
                strCurrentUpdate = txtCurrentUpdte.Text;
                if(strCurrentUpdate.Length < 15)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Current Update is not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketStatus(MainWindow.gintTicketID, gstrTicketStatus);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintTicketID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strCurrentUpdate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketProblemType(MainWindow.gintTicketID, MainWindow.gintProblemTypeID);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = "Ticket Number " + Convert.ToString(MainWindow.gintTicketID) + "Has Been Updated";

                strMessage = "<h1>" + strHeader + "</h1>";
                strMessage += "<p>" + strCurrentUpdate + "</p>";

                blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Ticket Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Update Ticket Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnViewDocuments_Click(object sender, RoutedEventArgs e)
        {
            ViewHelpDeskAttachments ViewHelpDeskAttachments = new ViewHelpDeskAttachments();
            ViewHelpDeskAttachments.ShowDialog();
        }

        private void btnAddDocuments_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datTransactionDate = DateTime.Now;
            string strDocumentPath = "";
            long intResult;
            string strNewLocation = "";
            string strTransactionName;
            bool blnFatalError;
            string strFileExtension;

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name                

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    strDocumentPath = dlg.FileName.ToUpper();
                }
                else
                {
                    return;
                }

                FileInfo FileName = new FileInfo(strDocumentPath);

                strFileExtension = FileName.Extension;

                datTransactionDate = DateTime.Now;

                intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                strTransactionName = Convert.ToString(intResult);

                strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\HelpDeskDocuments\\" + strTransactionName + strFileExtension;

                System.IO.File.Copy(strDocumentPath, strNewLocation);

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketDocumentation(MainWindow.gintTicketID, datTransactionDate, strNewLocation);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Document Has Been Saved");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Attach Documents " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
