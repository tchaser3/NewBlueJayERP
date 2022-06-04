/* Title:           Add Production Project Info'
 * Date:            2-3-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for adding the production project information*/

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
using DataValidationDLL;
using NewEventLogDLL;
using JobTypeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for AddProductionProjectInfo.xaml
    /// </summary>
    public partial class AddProductionProjectInfo : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        JobTypeClass TheJobTypeClass = new JobTypeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindSortedJobTypeDataSet TheFindSortedJobTypeDataSet = new FindSortedJobTypeDataSet();

        //setting up global variables
        int gintJobTypeID;

        public AddProductionProjectInfo()
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
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;

            //this will load up the controls    

            cboJobType.Items.Add("Select Job Type");

            TheFindSortedJobTypeDataSet = TheJobTypeClass.FindSortedJobType();

            intNumberOfRecords = TheFindSortedJobTypeDataSet.FindSortedJobType.Rows.Count;

            for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
            {
                cboJobType.Items.Add(TheFindSortedJobTypeDataSet.FindSortedJobType[intCounter].JobType);
            }

            TheFindProductionProjectInfoDataSet = TheProductionProjectClass.FindProductionProjectInfo(MainWindow.gintProjectID);

            intRecordsReturned = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo.Rows.Count;

            if(intRecordsReturned > 0)
            {
                TheMessagesClass.ErrorMessage("The Production Project Has an Entry Already");

                this.Close();
            }
        }

        private void cboJobType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboJobType.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintJobTypeID = TheFindSortedJobTypeDataSet.FindSortedJobType[intSelectedIndex].JobTypeID;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will process the info
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strValueForValidation;
            string strPOC;
            string strPONumber;
            decimal decPOAmount = 0;

            try
            {
                if(cboJobType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Job Type Was Not Selected\n";
                }
                strPOC = txtPOC.Text;
                if(strPOC.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "POC is not Long Enough\n";
                }
                strPONumber = txtPONumber.Text;
                if(strPONumber.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "The PO Number is not Long Enough\n";
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

                //inserting the record
                blnFatalError = TheProductionProjectClass.InsertProductionProjectInfo(MainWindow.gintProjectID, gintJobTypeID, strPOC, strPONumber, decPOAmount);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Production Project Info has been Inserted");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Add Production Project Info // Process Button " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Add Production Project Info // Process Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
