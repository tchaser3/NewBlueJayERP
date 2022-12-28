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
using IncentivePayDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddIncentivePayTitles.xaml
    /// </summary>
    public partial class AddIncentivePayTitles : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        IncentivePayClass TheIncentivePayClass = new IncentivePayClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up data
        FindIncentivePayTitleByPositionDataSet TheFindIncentivePayTitlesByPositionDataSet = new FindIncentivePayTitleByPositionDataSet();
        FindSortedIncentivePayTitles2DataSet TheFindSortedIncentivePayTitles2DataSet = new FindSortedIncentivePayTitles2DataSet();
        FindIncentivePayPositionByKeywordDataSet TheFindIncentivePayPositionByKeyword = new FindIncentivePayPositionByKeywordDataSet();

        //setting up global varibles
        string gstrUser;
        bool gblnAddingNewRecord;
        int gintPositionID;
        string gstrPositionTitle;

        public AddIncentivePayTitles()
        {
            InitializeComponent();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;            

            try
            {
                gstrUser = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].FirstName;
                gstrUser = gstrUser.Substring(0, 1);
                gstrUser += MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].LastName;

                MainWindow.gintEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP // Add Incentive Pay Titles ");

                cboSelectTitle.Items.Clear();
                cboSelectTitle.Items.Add("Select Incentive Pay Title");

                TheFindSortedIncentivePayTitles2DataSet = TheIncentivePayClass.FindSortedIncentivePayTitles2();

                intNumberOfRecords = TheFindSortedIncentivePayTitles2DataSet.FindSortedIncentivePayTitles2.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectTitle.Items.Add(TheFindSortedIncentivePayTitles2DataSet.FindSortedIncentivePayTitles2[intCounter].PositionTitle);
                    }
                }

                cboSelectTitle.SelectedIndex = 0;
                txtEnterRate.Text = "";
                txtEnterTitle.Text = "";

                //disabling the controls
                txtEnterTitle.IsEnabled = false;
                txtEnterRate.IsEnabled = false;
                cboSelectTitle.IsEnabled = false;
                expAdd.IsEnabled = true;
                expEdit.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Incentive Pay Titles // Reset Controls " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

        private void expAdd_Expanded(object sender, RoutedEventArgs e)
        {
            gblnAddingNewRecord = true;
            txtEnterRate.IsEnabled = true;
            txtEnterTitle.IsEnabled = true;
            expAdd.IsExpanded = false;
            expEdit.IsEnabled = false;
        }

        private void expEdit_Expanded(object sender, RoutedEventArgs e)
        {
            gblnAddingNewRecord = false;
            txtEnterRate.IsEnabled = true;
            txtEnterTitle.IsEnabled = true;
            cboSelectTitle.IsEnabled = true;
            expEdit.IsExpanded = false;
            expAdd.IsEnabled = false;
        }

        private void expResetControls_Expanded(object sender, RoutedEventArgs e)
        {
            expResetControls.IsExpanded = false;
            ResetControls();
        }

        private void txtEnterTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strPosition;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                if (gblnAddingNewRecord == false)
                {
                    strPosition = txtEnterTitle.Text;

                    if (strPosition.Length > 3)
                    {
                        TheFindIncentivePayPositionByKeyword = TheIncentivePayClass.FindIncentivePayPositionByKeyword(strPosition);

                        intNumberOfRecords = TheFindIncentivePayPositionByKeyword.FindIncentivePayPositionByKeyword.Rows.Count;

                        if (intNumberOfRecords < 1)
                        {
                            TheMessagesClass.ErrorMessage("The Position Title does not Exist");
                            return;
                        }

                        cboSelectTitle.Items.Clear();
                        cboSelectTitle.Items.Add("Select Incentive Pay Title");

                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboSelectTitle.Items.Add(TheFindIncentivePayPositionByKeyword.FindIncentivePayPositionByKeyword[intCounter].PositionTitle);
                        }

                        cboSelectTitle.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Incentive Pay Titles // Enter Title Text Box " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strPositionTitle;
            string strValueForValidation;
            decimal decPositionRate = 0;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intRecordsReturned;

            try
            {
                expProcess.IsExpanded = false;

                if(gblnAddingNewRecord == true)
                {
                    strPositionTitle = txtEnterTitle.Text;
                    if(strPositionTitle.Length < 3)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Position Title Is Not Long Enough\n";
                    }
                    else
                    {
                        TheFindIncentivePayTitlesByPositionDataSet = TheIncentivePayClass.FindIncentivePayTitleByPosition(strPositionTitle);

                        intRecordsReturned = TheFindIncentivePayTitlesByPositionDataSet.FindIncentivePayTitleByPosition.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Position Already Exists\n";
                        }
                    }
                    strValueForValidation = txtEnterRate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError |= true;
                        strErrorMessage += "The Pay Rate Is Not Numeric\n";
                    }
                    else
                    {
                        decPositionRate = Convert.ToDecimal(strValueForValidation);
                    }
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    } 
                    
                    //inserting records
                    blnFatalError = TheIncentivePayClass.InsertIncentivePayTitle(gstrUser, strPositionTitle, decPositionRate);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP // Add Incentive Pay Titles // HAS ADDED POSITION " + strPositionTitle);

                    TheMessagesClass.InformationMessage("The Title Has Been Saved");

                    ResetControls();
                }
                else if (gblnAddingNewRecord == false)
                {
                    if(cboSelectTitle.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Position Is Not Selected\n";
                    }
                    strValueForValidation = txtEnterRate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError |= true;
                        strErrorMessage += "The Pay Rate Is Not Numeric\n";
                    }
                    else
                    {
                        decPositionRate = Convert.ToDecimal(strValueForValidation);
                    }
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    blnFatalError = TheIncentivePayClass.EditIncentivePayTitleRate(gintPositionID, gstrUser, decPositionRate);


                    if (blnFatalError == true)
                        throw new Exception();

                    TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP // Add Incentive Pay Titles // HAS EDITED POSITION " + gstrPositionTitle);

                    TheMessagesClass.InformationMessage("The Title Has Been Saved");

                    ResetControls();

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Incentive Pay Titles // Process Expander " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectTitle.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    txtEnterRate.Text = Convert.ToString(TheFindIncentivePayPositionByKeyword.FindIncentivePayPositionByKeyword[intSelectedIndex].PositionRate);
                    gintPositionID = TheFindIncentivePayPositionByKeyword.FindIncentivePayPositionByKeyword[intSelectedIndex].PositionID;
                    gstrPositionTitle = TheFindIncentivePayPositionByKeyword.FindIncentivePayPositionByKeyword[intSelectedIndex].PositionTitle;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Incentive Pay Titles // Select Title Combo Box " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
