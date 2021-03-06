﻿/* Title:           Create Help Desk Ticket
 * Date:            7-21-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create a ticket within ERP*/

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
using EmployeeDateEntryDLL;
using System.IO;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateHelpDeskTicket.xaml
    /// </summary>
    public partial class CreateHelpDeskTicket : Window
    {
        //Setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedHelpDeskProblemTypeDataSet TheFindSortedHelpDeskProblemTypeDataSet = new FindSortedHelpDeskProblemTypeDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindHelpDeskTicketbyTicketMatchDateDataSet TheFindHelpDeskTicketByMatchDateDataSet = new FindHelpDeskTicketbyTicketMatchDateDataSet();
        FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        //setting global variables
        string gstrIPAddress;
        string gstrComputerName;
        string gstrUserName;
        int gintProblemTypeID;
        int gintPhoneExtension;
        int gintTicketID;

        public CreateHelpDeskTicket()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;
            string strOffice = "";

            try
            {
                //loading the warehouses
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                cboOffice.Items.Add("Select Office");
                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboOffice.SelectedIndex = 0;

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

                if (gstrIPAddress.Contains(".0."))
                    strOffice = "CLEVELAND";
                else if (gstrIPAddress.Contains(".11."))
                    strOffice = "CBUS-GROVEPORT";
                else if (gstrIPAddress.Contains(".31."))
                    strOffice = "NASHVILLE";
                else if (gstrIPAddress.Contains(".41."))
                    strOffice = "MILWUKEE";
                else if (gstrIPAddress.Contains(".51."))
                    strOffice = "TOLEDO";
                else if (gstrIPAddress.Contains(".61."))
                    strOffice = "YOUNGSTOWN";

                intNumberOfRecords = cboOffice.Items.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboOffice.SelectedIndex = intCounter;

                    if (cboOffice.SelectedItem.ToString() == strOffice)
                    {
                        intSelectedIndex = cboOffice.SelectedIndex;
                    }
                }

                cboOffice.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Help Desk // Main Window // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
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
            int intEmployeeID;
            string strFullName;
            string strUserEmail;

            try
            {
                //data valication
                if (cboOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Was Not Selected\n";
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

                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                strFullName = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].FirstName + " ";
                strFullName += MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].LastName;

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                {
                    strUserEmail = "NONE";
                }
                else
                {
                    if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress.Contains("bluejaycommunications") == false)
                    {
                        strUserEmail = "NONE";
                    }
                    else
                    {
                        strUserEmail = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                    }
                }

                strUserEmail = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                //inserting ticket
                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicket(datTicketDate, gstrComputerName, gstrUserName, gstrIPAddress, MainWindow.gintWarehouseID, gintProblemTypeID, strRepotedProblem, intEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindHelpDeskTicketByMatchDateDataSet = TheHelpDeskClass.FindHelpDeskTicketByTicketDateMatch(datTicketDate);

                gintTicketID = TheFindHelpDeskTicketByMatchDateDataSet.FindHelpDeskTicketByTicketDateMatch[0].TicketID;

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(gintTicketID, intEmployeeID, "TICKET CREATED");

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = strFullName + " Has Submitted a Help Desk Ticket - Do Not Reply";
                strMessage = "<h1>" + strFullName + " Has Submitted a Help Desk Ticket - Do Not Reply</h1>";
                strMessage += "<h3> Ticket ID " + Convert.ToString(gintTicketID) + "</h3>";
                strMessage += "<h3> They have Reported The Following Problem </h3>";
                strMessage += "<h3>" + strRepotedProblem + "</h3>";
                strMessage += "<h3> They can be reached at Extension " + Convert.ToString(gintPhoneExtension) + "</h3>";
                strMessage += "<h3> Computer Name " + gstrComputerName + " User Name " + gstrUserName + " IP Address " + gstrIPAddress + "<h3>";

                blnFatalError = TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                blnFatalError = TheSendEmailClass.SendEmail(strUserEmail, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Help Desk Ticket Number " + Convert.ToString(gintTicketID) + " Has Been Created");

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Create Help Desk Ticket");

                if (blnFatalError == true)
                    throw new Exception();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Help Desk // Main Window // Submit Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Help Desk Tickets // Attach Documents " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
