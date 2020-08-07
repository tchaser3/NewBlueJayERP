/* Title:           View My Ticket
 * Date:            8-7-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is the window that is used to view ticket info */

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
using HelpDeskDLL;
using DataValidationDLL;
using NewEventLogDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewMyTicketInfo.xaml
    /// </summary>
    public partial class ViewMyTicketInfo : Window
    {
        //setting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindHelpDeskTicketUpdatesByTicketIDDataSet TheFindHelpDeskTicketUpdatesByTicketIDDataSet = new FindHelpDeskTicketUpdatesByTicketIDDataSet();
        FindHelpDeskTicketDocumentationByTicketIDDataSet TheFindHelpDeskTicketDocumentationDataSet = new FindHelpDeskTicketDocumentationByTicketIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        public ViewMyTicketInfo()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindHelpDeskTicketDocumentationDataSet = TheHelpDeskClass.FindHelpDeskTicketDocumentationByTicketID(MainWindow.gintTicketID);

            dgrTicketAttachments.ItemsSource = TheFindHelpDeskTicketDocumentationDataSet.FindHelpDeskTicketDocumentationByTicketID;

            TheFindHelpDeskTicketUpdatesByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketUpdatesByTicketID(MainWindow.gintTicketID);

            dgrTicketUpdates.ItemsSource = TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID;
        }

        private void dgrTicketAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrTicketAttachments.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrTicketAttachments;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View My Ticket Info // Ticket Attachment Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSendUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strEmailAddress = "itadmin@bluejaycommunications.com";
            string strHeader;
            string strMessage;
            int intEmployeeID;
            string strFullName;
            string strUserEmail;
            string strUpdate;

            try
            {
                strUpdate = txtEnterUpdate.Text;

                if(strUpdate.Length < 10)
                {
                    TheMessagesClass.ErrorMessage("The Text in the Update is not Long Enough");
                    return;
                }
                
              
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                strFullName = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].FirstName + " ";
                strFullName += MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].LastName;

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == true)
                {
                    strUserEmail = "NONE";
                }
                else
                {
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress.Contains("bluejaycommunications") == false)
                    {
                        strUserEmail = "NONE";
                    }
                    else
                    {
                        strUserEmail = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                    }
                }

                strUserEmail = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintTicketID, intEmployeeID, strUpdate);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = strFullName + " Has Submitted a Help Desk Ticket - Do Not Reply";
                strMessage = "<h1>" + strFullName + " Has Submitted a Help Desk Ticket - Do Not Reply</h1>";
                strMessage += "<h3> Ticket ID " + Convert.ToString(MainWindow.gintTicketID) + "</h3>";
                strMessage += "<h3> They have Reported The Following Problem </h3>";
                strMessage += "<h3>" + strUpdate + "</h3>";;

                blnFatalError = TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                blnFatalError = TheSendEmailClass.SendEmail(strUserEmail, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Update Has Been Sent");

                txtEnterUpdate.Text = "";

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Help Desk // Main Window // Submit Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
