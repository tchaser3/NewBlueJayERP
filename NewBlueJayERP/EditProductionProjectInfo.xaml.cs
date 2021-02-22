/* Title:           Edit Productioin Project Info
 * Date:P           2-22-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for Editing Production Project Info */

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
using ProductionProjectDLL;
using NewEventLogDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;
using JobTypeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditProductionProjectInfo.xaml
    /// </summary>
    public partial class EditProductionProjectInfo : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        JobTypeClass TheJobTypeClass = new JobTypeClass();

        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindSortedJobTypeDataSet TheFindSortedJobTypeDataSet = new FindSortedJobTypeDataSet();

        int gintJobTypeID;

        public EditProductionProjectInfo()
        {
            InitializeComponent();
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
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;

            try
            {
                TheFindSortedJobTypeDataSet = TheJobTypeClass.FindSortedJobType();

                cboSelectJobType.Items.Add("Select Job Type");

                intNumberOfRecords = TheFindSortedJobTypeDataSet.FindSortedJobType.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectJobType.Items.Add(TheFindSortedJobTypeDataSet.FindSortedJobType[intCounter].JobType);
                }

                cboSelectJobType.SelectedIndex = 0;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Edit Production Project Info ");

                if (blnFatalError == true)
                    throw new Exception();

                TheFindProductionProjectInfoDataSet = TheProductionProjectClass.FindProductionProjectInfo(MainWindow.gintProjectID);

                intNumberOfRecords = cboSelectJobType.Items.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectJobType.SelectedIndex = intCounter;

                    if (cboSelectJobType.SelectedItem.ToString() == TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].JobType)
                    {
                        intSelectedIndex = intCounter;
                    }
                }

                cboSelectJobType.SelectedIndex = intCounter;
                MainWindow.gintTransactionID = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].TransactionID;
                txtPOAmount.Text = Convert.ToString(TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].POAmount);
                txtPOC.Text = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].PointOfContact;
                txtPONumber.Text = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].PONumber;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Production Project Info // Window Loaded Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            decimal decPOAmount = 0;
            string strPOC;
            string strPONumber;

            try
            {
                if(cboSelectJobType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Job Type Was Not Selected\n";
                }
                strPOC = txtPOC.Text;
                if(strPOC.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Point Of Contact is to Short\n";
                    
                }
                strPONumber = txtPONumber.Text;
                if(strPONumber.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The PO Number is to Short\n";
                }
                strValueForValidation = txtPOAmount.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The PO Amount is not Numeric\n";
                }
                else
                {
                    decPOAmount = Convert.ToDecimal(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //updating Project Info
                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoPOAmount(MainWindow.gintTransactionID, decPOAmount);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoJobType(MainWindow.gintTransactionID, gintJobTypeID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoPONumber(MainWindow.gintTransactionID, strPONumber);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Production Project Info has been Updated");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Production Project Info // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectJobType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectJobType.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintJobTypeID = TheFindSortedJobTypeDataSet.FindSortedJobType[intSelectedIndex].JobTypeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Production Project Info // Job Type Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
