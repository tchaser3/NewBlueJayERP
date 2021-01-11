/* Title:           IT Create Help Desk Ticket
 * Date:            12-24-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create a help desk ticket when they call*/

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
using HelpDeskDLL;
using PhonesDLL;
using System.IO;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ITCreateHelpDeskTicket.xaml
    /// </summary>
    public partial class ITCreateHelpDeskTicket : Window
    {
        //setting the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedHelpDeskProblemTypeDataSet TheFindSortedHelpDeskProblemTypeDataSet = new FindSortedHelpDeskProblemTypeDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindHelpDeskTicketbyTicketMatchDateDataSet TheFindHelpDeskTicketByMatchDateDataSet = new FindHelpDeskTicketbyTicketMatchDateDataSet();
        FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        int gintWarehouseID;
        int gintEmployeeID;
        int gintPhoneExtension;
        int gintProblemTypeID;
        string gstrFullName;
        int gintTicketID;
        string gstrComputerName;
        string gstrUserName;
        string gstrIPAddress;
        string gstrEmailAddress;

        public ITCreateHelpDeskTicket()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = true;
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
            bool blnFatalError = false;

            try
            {
                txtEnterLastName.Text = "";
                txtProblemNotes.Text = "";
                cboSelectEmployee.Items.Clear();
                cboProblemType.Items.Clear();
                cboSelectOffice.Items.Clear();

                //loading the warehouses
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                cboSelectOffice.Items.Add("Select Office");
                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                cboProblemType.Items.Add("Select Problem Type");
                TheFindSortedHelpDeskProblemTypeDataSet = TheHelpDeskClass.FindSortedHelpDeskProblemType();

                intNumberOfRecords = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboProblemType.Items.Add(TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intCounter].ProblemType);
                }

                cboProblemType.SelectedIndex = 0;

                gstrComputerName = System.Environment.MachineName.ToUpper();
                gstrUserName = System.Environment.UserName.ToUpper();
                gstrIPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();

                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // IT Create Help Desk Ticket");

                if (blnFatalError == true)
                    throw new Exception();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Help Desk // Main Window // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;

                if (strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if (intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Was Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // IT Create Help Desk Ticket // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;
            string strHomeOffice;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    gstrFullName = TheComboEmployeeDataSet.employees[intSelectedIndex].FullName;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(gintEmployeeID);

                    if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                    {
                        gstrEmailAddress = "NONE";
                    }
                    else
                    {
                        gstrEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                        if (gstrEmailAddress.Contains("bluejaycommunications.com") == false)
                        {
                            gstrEmailAddress = "NONE";
                        }
                    }

                    TheFindPhoneExtensionByEmployeeIDDataSet = ThePhonesClass.FindPhoneExtensionByEmployeeID(gintEmployeeID);

                    intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        gintPhoneExtension = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[0].Extension;
                    }
                    else
                    {
                        gintPhoneExtension = 0;
                    }

                    strHomeOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice;

                    intNumberOfRecords = cboSelectOffice.Items.Count;

                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectOffice.SelectedIndex = intCounter;

                        if (strHomeOffice == cboSelectOffice.SelectedItem.ToString())
                        {
                            intSelectedIndex = intCounter;
                        }
                    }

                    cboSelectOffice.SelectedIndex = intSelectedIndex;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // IT Create Help Desk // Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void cboProblemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboProblemType.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintProblemTypeID = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intSelectedIndex].ProblemTypeID;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datTicketDate = DateTime.Now;
            string strRepotedProblem;
            string strEmailAddress = "itadmin@bluejaycommunications.com";
            string strHeader;
            string strMessage;


            try
            {
                //data valication
                if (cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Was Not Selected\n";
                }
                if (cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (cboProblemType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Type Was Not Selected";
                }
                strRepotedProblem = txtProblemNotes.Text;
                if (strRepotedProblem.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Reported Problem is not Long Enough\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //inserting ticket
                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicket(datTicketDate, gstrComputerName, gstrUserName, gstrIPAddress, gintWarehouseID, gintProblemTypeID, strRepotedProblem, gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindHelpDeskTicketByMatchDateDataSet = TheHelpDeskClass.FindHelpDeskTicketByTicketDateMatch(datTicketDate);

                gintTicketID = TheFindHelpDeskTicketByMatchDateDataSet.FindHelpDeskTicketByTicketDateMatch[0].TicketID;

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(gintTicketID, gintEmployeeID, "TICKET CREATED");

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = gstrFullName + " Has Submitted a Help Desk Ticket - Do Not Reply";
                strMessage = "<h1>" + gstrFullName + " Has Submitted a Help Desk Ticket - Do Not Reply</h1>";
                strMessage += "<h3> Ticket ID " + Convert.ToString(gintTicketID) + "</h3>";
                strMessage += "<h3> They have Reported The Following Problem </h3>";
                strMessage += "<h3>" + strRepotedProblem + "</h3>";
                strMessage += "<h3> They can be reached at Extension " + Convert.ToString(gintPhoneExtension) + "</h3>";
                strMessage += "<h3> Computer Name " + gstrComputerName + " User Name " + gstrUserName + " IP Address " + gstrIPAddress + "<h3>";

                blnFatalError = TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                if (gstrEmailAddress != "NONE")
                {
                    blnFatalError = TheSendEmailClass.SendEmail(gstrEmailAddress, strHeader, strMessage);

                    if (blnFatalError == false)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("Help Desk Ticket Number " + Convert.ToString(gintTicketID) + " Has Been Created");

                const string message = "Would You Like to send a Document or Attach a File?";
                const string caption = "Please Answer";
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    AttachDocuments();
                }

                this.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // IT Create Help Desk Ticket // Submit Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void AttachDocuments()
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

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketDocumentation(gintTicketID, datTransactionDate, strNewLocation);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Document Has Been Saved");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // IT Create Help Desk Ticket // Attach Documents " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
    }
}
