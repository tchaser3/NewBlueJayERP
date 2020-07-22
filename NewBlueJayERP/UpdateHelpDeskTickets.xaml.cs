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

        //setting up the data
        FindOpenHelpDeskTicketsDataSet TheFindOpenHelpDeskTicketsDataSet;
        FindHelpDeskTicketsNotAssignedByTicketIDDataSet TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet = new FindHelpDeskTicketsNotAssignedByTicketIDDataSet();
        FindHelpDeskTicketUpdatesByTicketIDDataSet TheFindTicketUpdatesByTicketIDDataSet = new FindHelpDeskTicketUpdatesByTicketIDDataSet();
        FindHelpDeskTicketCurrentAssignmentDataSet TheFindHelpDeskTicketCurrentAssignmentDataSet = new FindHelpDeskTicketCurrentAssignmentDataSet();
        FindSortedHelpDeskProblemTypeDataSet TheFindSortedHelpDeskProblemTypeDataSet = new FindSortedHelpDeskProblemTypeDataSet();
        FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

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
                txtUpdateNotes.Text = "";
                txtExtension.Text = "";

                cboSelectProblemType.Items.Clear();
                cboSelectProblemType.Items.Add("Select Problem Type");

                TheFindSortedHelpDeskProblemTypeDataSet = TheHelpDeskClass.FindSortedHelpDeskProblemType();

                intNumberOfRecords = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectProblemType.Items.Add(TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intCounter].ProblemType);
                }

                cboSelectProblemType.SelectedIndex = 0;

                cboTicketStatus.Items.Clear();
                cboTicketStatus.Items.Add("Select Status");
                cboTicketStatus.Items.Add("OPEN");
                cboTicketStatus.Items.Add("IN PROCESS");
                cboTicketStatus.Items.Add("WAITING ON USER");
                cboTicketStatus.Items.Add("WAITING ON PARTS");
                cboTicketStatus.Items.Add("CLOSED");
                cboTicketStatus.SelectedIndex = 0;

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("INFORMATION TECHNOLOGY");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.Items.Add("Unassigned");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName;

                    cboSelectEmployee.Items.Add(strFullName);
                }

                cboSelectEmployee.SelectedIndex = 0;

                TheFindOpenHelpDeskTicketsDataSet = TheHelpDeskClass.FindOpenHelpDeskTickets();

                dgrOpenTickets.ItemsSource = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void dgrOpenTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TicketID;
            string strTicketID;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intRecordsReturned;
            string strTicketStatus;
            int intSelectedIndex = 0;
            int intEmployeeRecords;

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

                    TheFindTicketUpdatesByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketUpdatesByTicketID(MainWindow.gintTicketID);

                    dgrTicketUpdates.ItemsSource = TheFindTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID;

                    TheFindHelpDeskTicketCurrentAssignmentDataSet = TheHelpDeskClass.FindHelpDeskTicketCurrentAssignment(MainWindow.gintTicketID);

                    intNumberOfRecords = TheFindHelpDeskTicketCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment.Rows.Count;

                    if(intNumberOfRecords == 0)
                    {
                        cboSelectEmployee.SelectedIndex = 0;
                    } 
                    else if(intNumberOfRecords > 0)
                    {
                        intEmployeeRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

                        intEmployeeID = TheFindHelpDeskTicketCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].EmployeeID;

                        for(intCounter = 0; intCounter <= intEmployeeRecords; intCounter++)
                        {
                            if (intEmployeeID == TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].EmployeeID)
                            {
                                cboSelectEmployee.SelectedIndex = intCounter + 2;
                            }

                        }
                    }

                    TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketsNotAssignedByTicketID(MainWindow.gintTicketID);

                    txtComputerName.Text = TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet.FindHelpDeskTicketsNotAssignedByTicketID[0].ComputerName;
                    intEmployeeID = TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet.FindHelpDeskTicketsNotAssignedByTicketID[0].EmployeeID;
                    TheFindPhoneExtensionByEmployeeIDDataSet = ThePhoneClass.FindPhoneExtensionByEmployeeID(intEmployeeID);
                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                    {
                        gblnEmailAddress = false;
                    }
                    else
                    {
                        gstrUserEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                        if(gstrUserEmailAddress.Contains("bluejaycommunications.com") == false)
                        {
                            gblnEmailAddress = false;
                        }
                        else
                        {
                            gblnEmailAddress = true;
                            gstrUserEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                        }
                    }

                    intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        txtExtension.Text = "0";
                    }
                    else
                    {
                        txtExtension.Text = Convert.ToString(TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[0].Extension);
                    }

                    MainWindow.gintProblemTypeID = TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet.FindHelpDeskTicketsNotAssignedByTicketID[0].ProblemTypeID;

                    intNumberOfRecords = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(MainWindow.gintProblemTypeID == TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intCounter].ProblemTypeID)
                        {
                            cboSelectProblemType.SelectedIndex = intCounter + 1;
                        }
                    }

                    strTicketStatus = TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet.FindHelpDeskTicketsNotAssignedByTicketID[0].TicketStatus;

                    intNumberOfRecords = cboTicketStatus.Items.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboTicketStatus.SelectedIndex = intCounter;

                        if(cboTicketStatus.SelectedItem.ToString() == strTicketStatus)
                        {
                            intSelectedIndex = intCounter;
                        }
                    }

                    cboTicketStatus.SelectedIndex = intSelectedIndex;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Vehicle Dashboard // Vehicle In Shop // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectProblemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            bool blnFatalError = false;

            try
            {
                intSelectedIndex = cboSelectProblemType.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintProblemTypeID = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intSelectedIndex].ProblemTypeID;

                    TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketsNotAssignedByTicketID(MainWindow.gintTicketID);

                    if (MainWindow.gintProblemTypeID != TheFindHelpDeskTicketsNotAssignedByTicketIDDataSet.FindHelpDeskTicketsNotAssignedByTicketID[0].ProblemTypeID)
                    {
                        blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketProblemType(MainWindow.gintTicketID, MainWindow.gintProblemTypeID);

                        if (blnFatalError == true)
                            throw new Exception();

                        TheMessagesClass.InformationMessage("The Ticket Problem Type has been Updated");

                        TheFindOpenHelpDeskTicketsDataSet = TheHelpDeskClass.FindOpenHelpDeskTickets();

                        dgrOpenTickets.ItemsSource = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Select Problem Type " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intTransactionID;
            string strFullName;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 2;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].EmployeeID;
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].LastName;

                    TheFindHelpDeskTicketCurrentAssignmentDataSet = TheHelpDeskClass.FindHelpDeskTicketCurrentAssignment(MainWindow.gintTicketID);

                    intNumberOfRecords = TheFindHelpDeskTicketCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment.Rows.Count;

                    if(intNumberOfRecords == 0)
                    {
                        blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(MainWindow.gintTicketID, MainWindow.gintEmployeeID);

                        if (blnFatalError == true)
                            throw new Exception();

                        SendEmailRegardingCurrentAssignment(MainWindow.gintTicketID, strFullName);
                    }
                    else if(intNumberOfRecords > 0)
                    {
                        intTransactionID = TheFindHelpDeskTicketCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].TransactionID;

                        if(MainWindow.gintEmployeeID != TheFindHelpDeskTicketCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].EmployeeID)
                        {
                            blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketCurrrentAssignment(intTransactionID, false);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(MainWindow.gintTicketID, MainWindow.gintEmployeeID);

                            if (blnFatalError == true)
                                throw new Exception();

                            SendEmailRegardingCurrentAssignment(MainWindow.gintTicketID, strFullName);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Select Employee Combo Box " + Ex.Message);

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

        private void cboTicketStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(cboTicketStatus.SelectedIndex > 0)
                {
                    gstrTicketStatus = cboTicketStatus.SelectedItem.ToString();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Ticket Status Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnUpdateTicket_Click(object sender, RoutedEventArgs e)
        {
            string strUpdateNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intEmployeeID;
            string strEmailAddress = "itadmin@bluejaycommunications.com";
            string strMessage;
            string strHeader;

            try
            {
                if(cboSelectProblemType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Type Was Not Selected\n";
                }
                if(cboSelectEmployee.SelectedIndex < 2)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Ticket was not Assigned\n";
                }
                if(cboTicketStatus.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Ticket Status Was not Selected\n";
                }
                strUpdateNotes = txtUpdateNotes.Text;
                if(strUpdateNotes.Length < 15)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Update Notes were not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintTicketID, intEmployeeID, strUpdateNotes);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketStatus(MainWindow.gintTicketID, gstrTicketStatus);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = "Ticket " + Convert.ToString(MainWindow.gintTicketID) + " Has Been Updated";
                strMessage = "<h1>Ticket " + Convert.ToString(MainWindow.gintTicketID) + " Has Been Updated</h1>";
                strMessage += "<h3>Current Status:  " + gstrTicketStatus + "</h3>";
                strMessage += "<h3> Update Notes: " + strUpdateNotes + "</h3>";

                blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Ticket Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Help Desk Tickets // Update Ticket Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expRefreshTickets_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
