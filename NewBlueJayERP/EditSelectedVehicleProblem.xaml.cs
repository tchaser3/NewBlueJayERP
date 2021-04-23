/* Title:               Edit Selected Vehicle Problems
 * Date:                9-9-2020
 * Author:              Terry Holmes
 * 
 * Description:         This used to edit the selected Transaction */

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
using VehicleProblemDocumentationDLL;
using VehicleProblemsDLL;
using NewEventLogDLL;
using DataValidationDLL;
using VendorsDLL;
using VehicleMainDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditSelectedVehicleProblem.xaml
    /// </summary>
    public partial class EditSelectedVehicleProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleProblemDocumentClass TheVehicleProblemDocumentClass = new VehicleProblemDocumentClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        FindVehicleMainProblemUpdateByProblemIDDataSet TheFindVehicleMainProblemUpdateByProblemIDDataSet = new FindVehicleMainProblemUpdateByProblemIDDataSet();
        FindVehicleProblemDocumentationByProblemIDDataSet TheFindVenicleProblemDocumentationByProblemIDDataSet = new FindVehicleProblemDocumentationByProblemIDDataSet();
        FindVehicleInvoiceByInvoiceIDDataSet TheFindVehicleInvoiceByInvoiceIDDataSet = new FindVehicleInvoiceByInvoiceIDDataSet();
        FindVendorByVendorNameDataSet TheFindVendorByVendorNameDataSet = new FindVendorByVendorNameDataSet();
        FindVehicleInvoiceByInvoiceNumberDataSet TheFindVehicleInvoiceByInvoiceNumberDataSet = new FindVehicleInvoiceByInvoiceNumberDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        string gstrInvoicePath;
        bool gblnInvoiceAttached;

        public EditSelectedVehicleProblem()
        {
            InitializeComponent();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
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
            //local variables
            int intInvoiceID;
            string strProblemUpdates = "";
            int intCounter;
            int intNumberOfRecords;
            string strVendorName;
            string strVehicleNumber;

            try
            {
                TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(MainWindow.gintProblemID);

                txtProblemID.Text = Convert.ToString(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemID);
                txtTransactionDate.Text = Convert.ToString(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].TransactionDAte);
                txtProblem.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;
                txtProblemStatus.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemStatus;
                strVehicleNumber = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].VehicleNumber;

                TheFindActiveVehicleByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                MainWindow.gintVehicleID = TheFindActiveVehicleByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                
                strVendorName = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].VendorName;

                TheFindVendorByVendorNameDataSet = TheVendorsClass.FindVendorByVendorName(strVendorName);

                MainWindow.gintVendorID = TheFindVendorByVendorNameDataSet.FindVendorByVendorName[0].VendorID;

                if (TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].IsProblemResolutionNull() == false)
                {
                    txtProblemResolution.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemResolution;
                }

                if(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].IsInvoiceIDNull() == false)
                {
                    intInvoiceID = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].InvoiceID;

                    if(intInvoiceID > 999)
                    {

                        TheFindVehicleInvoiceByInvoiceIDDataSet = TheVehicleProblemDocumentClass.FindVehicleInvoiceByInvoiceID(intInvoiceID);

                        txtInvoiceAmount.Text = Convert.ToString(TheFindVehicleInvoiceByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID[0].InvoiceAmount);
                        txtInvoicePath.Text = TheFindVehicleInvoiceByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID[0].InvoicePath;
                    }

                }

                TheFindVehicleMainProblemUpdateByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemUpdateByProblemID(MainWindow.gintProblemID);

                intNumberOfRecords = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strProblemUpdates += Convert.ToString(TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].TransactionDate);
                        strProblemUpdates += " ";
                        strProblemUpdates += TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].ProblemUpdate;
                        strProblemUpdates += "\n\n";
                    }
                }

                txtCurrentUpdates.Text = strProblemUpdates;

                TheFindVenicleProblemDocumentationByProblemIDDataSet = TheVehicleProblemDocumentClass.FindVehicleProblemDocumentationByProblemID(MainWindow.gintProblemID);

                dgrProblemDocumentation.ItemsSource = TheFindVenicleProblemDocumentationByProblemIDDataSet.FindVehicleProblemDocumentationByProblemID;
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Selected Vehicle Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expAttachDocument_Expanded(object sender, RoutedEventArgs e)
        {
            expAttachDocument.IsExpanded = false;

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
                    gstrInvoicePath = dlg.FileName.ToUpper();
                }
                else
                {
                    return;
                }

                gblnInvoiceAttached = true;
                    

                txtInvoicePath.Text = gstrInvoicePath;
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Invoice Vehicle Problems // CBO Attach Invoice Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcessUpdate_Click(object sender, RoutedEventArgs e)
        {
            string strProblemStatus;
            string strProblemResolution;
            string strValueForValidation;
            string strProblemUpdate;
            decimal decInvoiceAmount = 0;
            DateTime datTransactionDate = DateTime.Now;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            long intResult;
            string strTransactionName = "";
            string strNewLocation;
            string strInvoiceNumber = "";
            int intInvoiceID;
            int intRecordsReturned;

            try
            {
                strProblemStatus = txtProblemStatus.Text;
                if(strProblemStatus.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Status Was Not Entered\n";
                }
                strProblemResolution = txtProblemResolution.Text;
                if(strProblemStatus == "CLOSED")
                {
                    if(strProblemResolution.Length < 10)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Problem Resolution Was Not Long Enough\n";
                    }
                    strValueForValidation = txtInvoiceAmount.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Invoice Amount is not Numeric\n";
                    }
                    strInvoiceNumber = txtInvoiceNumber.Text;
                    if(strInvoiceNumber.Length < 2)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Invoice Number Was Not Entered\n";
                    }
                }
                strProblemUpdate = txtNewUpdate.Text;
                if(strProblemUpdate.Length < 15)
                {
                    blnFatalError = true;
                    strErrorMessage += "The New Update was not Long Enough\n";
                }
                strValueForValidation = txtUpdateDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Update Date is not a Date\n";
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

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strProblemUpdate, datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheVehicleProblemClass.ChangeVehicleProblemStatus(MainWindow.gintProblemID, strProblemStatus);

                if (blnFatalError == true)
                    throw new Exception();

                if(strProblemStatus == "CLOSED")
                {
                    blnFatalError = TheVehicleProblemClass.UpdateVehiclePRoblemCost(MainWindow.gintProblemID, Convert.ToSingle(decInvoiceAmount));

                    if (blnFatalError == true)
                        throw new Exception();

                    if (gblnInvoiceAttached == true)
                    {

                        intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                        strTransactionName += Convert.ToString(intResult);

                        strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\VehicleProblemFiles\\" + strTransactionName + ".pdf";

                        System.IO.File.Copy(gstrInvoicePath, strNewLocation);

                    }
                    else
                    {
                        strNewLocation = "NO INVOICE ATTACHED";
                    }

                    blnFatalError = TheVehicleProblemDocumentClass.InsertVehicleInvoice(strInvoiceNumber, datTransactionDate, MainWindow.gintVehicleID, decInvoiceAmount, strNewLocation, MainWindow.gintVendorID);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindVehicleInvoiceByInvoiceNumberDataSet = TheVehicleProblemDocumentClass.FindVehicleInvoiceByInvoiceNumber(strInvoiceNumber, MainWindow.gintVendorID, MainWindow.gintVehicleID, datTransactionDate);

                    intInvoiceID = TheFindVehicleInvoiceByInvoiceNumberDataSet.FindVehicleInvoiceByInvoiceNumber[0].InvoiceID;

                    blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemResolution(MainWindow.gintProblemID, datTransactionDate, strProblemResolution, intInvoiceID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemSolved(MainWindow.gintProblemID, true);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Problem Has Been Updated");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Selected Vehicle Problem // Process Update Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
