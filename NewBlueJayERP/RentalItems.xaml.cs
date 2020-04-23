/* Title:           Rental Items
 * Date:            4-21-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add items to the rental agreement */

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
using RentalTrackingDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for RentalItems.xaml
    /// </summary>
    public partial class RentalItems : Window
    {
        //setting up the clasess
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        RentalTrackingClass TheRentalTrackingClass = new RentalTrackingClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //creating data object
        CreateRentalItemsDataSet TheCreateRentalItemsDataSet = new CreateRentalItemsDataSet();

        //setting up variable

        bool gblnAgreementAttached;

        public RentalItems()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = true;
            TheMessagesClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = true;
            TheMessagesClass.LaunchHelpSite();
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strRentalPartNumber;
            string strRentalDescription;
            bool blnAccessoriesAttached;
            string strItemNotes;
            bool blnFatalError;

            try
            {
                if(gblnAgreementAttached == false)
                {
                    TheMessagesClass.ErrorMessage("Rental Agreement Has Not Been Attached");
                    return;
                }

                intNumberOfRecords = TheCreateRentalItemsDataSet.rentalitems.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("There Were No Items Attached");
                    return;
                }

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strRentalPartNumber = TheCreateRentalItemsDataSet.rentalitems[intCounter].RentalPartNumber.ToUpper();
                    strRentalDescription = TheCreateRentalItemsDataSet.rentalitems[intCounter].RentalDescription.ToUpper();

                    if(TheCreateRentalItemsDataSet.rentalitems[intCounter].IsAccessoriesAttachedNull() == true)
                    {
                        blnAccessoriesAttached = true;
                    }
                    else
                    {
                        blnAccessoriesAttached = TheCreateRentalItemsDataSet.rentalitems[intCounter].AccessoriesAttached;
                    }
                    
                    strItemNotes = TheCreateRentalItemsDataSet.rentalitems[intCounter].ItemNotes.ToUpper();

                    blnFatalError = TheRentalTrackingClass.InsertIntoRentalTrackingItems(MainWindow.gintRentalTrackingID, strRentalPartNumber, strRentalDescription, blnAccessoriesAttached, MainWindow.gintEmployeeID, strItemNotes);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Rental Items Have Been Inserted");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Rental Items // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheCreateRentalItemsDataSet.rentalitems.Rows.Clear();

            gblnAgreementAttached = false;

            dgrItems.ItemsSource = TheCreateRentalItemsDataSet.rentalitems;
        }

        private void expAttachAgreement_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheRentalTrackingClass.InsertRentalTrackingAgreement(MainWindow.gintRentalTrackingID, MainWindow.gintEmployeeID, MainWindow.gstrAgreementNo, strDocumentPath, "AGREEMENT ADDED");

                            if (blnFatalError == true)
                                throw new Exception();

                            gblnAgreementAttached = true;
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessagesClass.InformationMessage("The Documents have been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Rental Items // Attach Agreement Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
